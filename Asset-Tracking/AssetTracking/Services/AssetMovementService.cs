using AssetTrackingAuthAPI.Models;
using AssetTrackingAuthAPI.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using MongoDB.Driver;
using YourNamespace.Models;

namespace YourNamespace.Services
{
    public class AssetMovementService
    {
        private readonly IMongoCollection<AssetMovement> _assetMovements;
        private readonly IMongoCollection<ApprovalWorkflow> _approval;
        private readonly IMongoCollection<User> _user;
        private readonly EmailService _emailservice;
        private readonly IMongoCollection<Asset> _assets;

        public AssetMovementService(IMongoDatabase database, EmailService email)
        {
            _assetMovements = database.GetCollection<AssetMovement>("AssetMovements");
            _approval = database.GetCollection<ApprovalWorkflow>("ApprovalWorkflow");
            _user = database.GetCollection<User>("Users");
            _assets = database.GetCollection<Asset>("Assets");
            _emailservice = email;
        }

        public async Task<List<AssetMovement>> GetSummaryAsync(int page, int pageSize)
        {
            return await _assetMovements.Find(_ => true)
                                        .Skip((page - 1) * pageSize)
                                        .Limit(pageSize)
                                        .ToListAsync();
        }

        public async Task<AssetMovement> GetByIdAsync(string id) =>
            await _assetMovements.Find(am => am.Id == id).FirstOrDefaultAsync();

        public async Task CreateAsync(AssetMovement movement) =>
            await _assetMovements.InsertOneAsync(movement);

        public async Task UpdateAsync(string id, AssetMovement updated) =>
            await _assetMovements.ReplaceOneAsync(am => am.Id == id, updated);

        public async Task DeleteAsync(string id) =>
            await _assetMovements.DeleteOneAsync(am => am.Id == id);

        public async Task<(bool success, string message)> CreateOrApproveDisposalAsync(AssetMovement? disposal, string? id)
{
    if (!string.IsNullOrEmpty(id))
    {
        // Approve logic
        var existing = await _assetMovements.Find(x => x.Id == id).FirstOrDefaultAsync();
        if (existing == null)
            return (false, "Movement not found");

string previousRole = existing.approvalworkflow; // store it before changing

        var currentWorkflow = await _approval.Find(x => x.Role == previousRole && x.Transaction == "AssetMovement").FirstOrDefaultAsync();
        if (currentWorkflow == null)
            return (false, "Current approval step not found");

        existing.lastapprovalworkflow = currentWorkflow.Role;

        // Get next workflow step
        var nextSequence = (int.Parse(currentWorkflow.Sequence) + 1).ToString();
        var nextWorkflow = await _approval.Find(x => x.Sequence == nextSequence && x.Transaction == "AssetMovement").FirstOrDefaultAsync();

        if (nextWorkflow != null)
{
    // More approvals needed
    existing.approvalworkflow = nextWorkflow.Role;
    existing.nextapprovalworkflow = nextWorkflow.Role;

    // ✅ Fetch current approver's user name
    string approvedByUser = "";
    var approverUser = await _user.Find(x => x.AssignedRoles.Contains(previousRole)).FirstOrDefaultAsync();
    if (approverUser != null)
        approvedByUser = approverUser.UserName;

    // Add approval summary entry
    existing.ApprovalSummary.Add(new ApprovalRecord
    {
        Status = "Approved",
        RequestApprovedBy = approvedByUser,
        RequestApprovedDate = DateTime.UtcNow,
        Remarks = $"Approved by {approvedByUser}"
    });

    await _assetMovements.ReplaceOneAsync(x => x.Id == id, existing);
            // Send mail to next approver
            var nextUser = await _user.Find(u => u.AssignedRoles.Contains(nextWorkflow.Role)).FirstOrDefaultAsync();
            if (nextUser != null && !string.IsNullOrWhiteSpace(nextUser.Email))
            {
                string nextUrl = $"http://172.16.100.71:5221/api/AssetMovement/process?id={existing.Id}";
                string nextBody = $"<p>Asset Code {existing.Assetcode} needs your approval.</p><p><a href='{nextUrl}'>Approve</a></p>";
                await _emailservice.SendEmailAsync(nextUser.Email, "Asset Movement Approval - Next Step", nextBody);
            }

            return (true, "Approved current step. Next approver notified.");
        }
        else
{
    // Final approval step
    existing.approvalworkflow = null;
    existing.nextapprovalworkflow = null;
    existing.status = "Approved";
    existing.MovedDate = DateTime.UtcNow;

    // ✅ Get final approver name using AssignedRoles
    string finalApprovedBy = "";
    var finalApprover = await _user.Find(x => x.AssignedRoles.Contains(currentWorkflow.Role)).FirstOrDefaultAsync();
    if (finalApprover != null)
        finalApprovedBy = finalApprover.UserName;

    // Add final approval record
    existing.ApprovalSummary.Add(new ApprovalRecord
    {
        Status = "Final Approved",
        RequestApprovedBy = finalApprovedBy,
        RequestApprovedDate = DateTime.UtcNow,
        Remarks = "Final approval completed"
    });
            await _assetMovements.ReplaceOneAsync(x => x.Id == id, existing);

            // Update asset locations
            foreach (var assetCode in existing.Assets.Select(a => a.AssetCode))
            {
                try
                {
                    var update = Builders<Asset>.Update
                        .Set(x => x.Department, existing.Department)
                        .Set(x => x.Custodian, existing.Custodian)
                        .Set(x => x.Group, existing.Group)
                        .Set(x => x.CompanyName, existing.Company)
                        .Set(x => x.SiteName, existing.Site)
                        .Set(x => x.BuildingName, existing.Building)
                        .Set(x => x.FloorName, existing.Floor)
                        .Set(x => x.Room, existing.Room);

                    var result = await _assets.UpdateOneAsync(x => x.AssetCode == assetCode, update);
                    if (result.ModifiedCount == 0)
                        Console.WriteLine($"No update done for asset code: {assetCode}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error updating asset {assetCode}: {ex.Message}");
                }
            }

            return (true, "Movement fully approved and asset locations updated.");
        }
    }
    else if (disposal != null)
    {
        // New disposal creation
        var firstWorkflow = await _approval.Find(x => x.Transaction == "AssetMovement")
                                           .SortBy(x => x.Sequence)
                                           .FirstOrDefaultAsync();
        if (firstWorkflow == null)
            return (false, "No approval workflow defined");

        disposal.approvalworkflow = firstWorkflow.Role;
        disposal.nextapprovalworkflow = firstWorkflow.Role;
        disposal.status = "Created";
        disposal.ReferenceNumber = $"MOV{DateTime.UtcNow:yyyyMMddHHmmssfff}";

        // Set approval summary
        var createdByUser = await _user.Find(x => x.AssignedRoles.Contains(firstWorkflow.Role)).FirstOrDefaultAsync();
        disposal.ApprovalSummary = new List<ApprovalRecord>
        {
            new ApprovalRecord
            {
                Status = "Created",
                RequestApprovedBy = createdByUser?.UserName ?? "System",
                RequestApprovedDate = DateTime.UtcNow,
                Remarks = "Asset movement request created"
            }
        };

        await _assetMovements.InsertOneAsync(disposal);

        // Notify first approver
        if (createdByUser != null && !string.IsNullOrWhiteSpace(createdByUser.Email))
        {
            string url = $"http://172.16.100.71:5221/api/AssetMovement/process?id={disposal.Id}";
            string body = $"<p>Asset Code {disposal.Assetcode} has been submitted for Movement.</p><p><a href='{url}'>Approve</a></p>";
            await _emailservice.SendEmailAsync(createdByUser.Email, "Asset Movement Approval Required", body);
        }

        return (true, "Movement submitted and approval email sent.");
    }

    return (false, "Invalid request. Provide disposal or approval id.");
}


        public async Task<(bool success, string message)> ProcessApprovalAsync(string id)
        {
            var disposal = await _assetMovements.Find(x => x.Id == id).FirstOrDefaultAsync();

            if (disposal == null)
                return (false, "Movement not found");

            if (disposal.status == "Approved")
                return (false, "Movement is already approved.");

            disposal.status = "Approved";
            await _assetMovements.ReplaceOneAsync(x => x.Id == id, disposal);

            return (true, "Movement approved");


        }
        public async Task<List<AssetMovementViewDto>> GetMovementDetailsByTransactionId(string movementId)
{
    var movement = await _assetMovements.Find(x => x.Id == movementId).FirstOrDefaultAsync();
    if (movement == null)
        throw new ArgumentException("AssetMovement not found");

    var result = new List<AssetMovementViewDto>();

    foreach (var asset in movement.Assets)
    {
        var dto = new AssetMovementViewDto
        {
            AssetCode = asset.AssetCode,

            FromCompany = asset.CompanyName,
            ToCompany = movement.Company,

            FromSite = asset.SiteName,
            ToSite = movement.Site,

            FromBuilding = asset.BuildingName,
            ToBuilding = movement.Building,

            FromFloor = asset.FloorName,
            ToFloor = movement.Floor,

            FromRoom = asset.Room,
            ToRoom = movement.Room,

            FromCustodian = asset.Custodian,
            ToCustodian = movement.Custodian,

            FromDepartment = asset.Department,
            ToDepartment = movement.Department
        };

        result.Add(dto);
    }

    return result;
}

     public async Task<List<AssetMovementReportDto>> GenerateAssetMovementReportAsync(AssetMovementReportRequest request)
{
    var movementFilter = Builders<AssetMovement>.Filter.Empty;

    // Filter To details from AssetMovement
    if (request.ToGroup != null && request.ToGroup.Any())
        movementFilter &= Builders<AssetMovement>.Filter.In(x => x.Group, request.ToGroup);
    if (request.ToCompany != null && request.ToCompany.Any())
        
        movementFilter &= Builders<AssetMovement>.Filter.In(x => x.Company, request.ToCompany);
   if (request.ToSite != null && request.ToSite.Any())
        movementFilter &= Builders<AssetMovement>.Filter.In(x => x.Site, request.ToSite);
    if (request.ToBuilding != null && request.ToBuilding.Any())
        movementFilter &= Builders<AssetMovement>.Filter.In(x => x.Building, request.ToBuilding);
   if (request.ToFloor != null && request.ToFloor.Any())
        movementFilter &= Builders<AssetMovement>.Filter.In(x => x.Floor, request.ToFloor);
    if (request.ToRoom != null && request.ToRoom.Any())
        movementFilter &= Builders<AssetMovement>.Filter.In(x => x.Room, request.ToRoom);
    if (request.ToDepartment != null && request.ToDepartment.Any())
        movementFilter &= Builders<AssetMovement>.Filter.In(x => x.Department, request.ToDepartment);
    if (request.ToCustodian != null && request.ToCustodian.Any())
        movementFilter &= Builders<AssetMovement>.Filter.In(x => x.Custodian, request.ToCustodian);

    // Date Range Filter (optional)
    if (request.StartDate.HasValue && request.EndDate.HasValue)
    {
        movementFilter &= Builders<AssetMovement>.Filter.And(
            Builders<AssetMovement>.Filter.Gte(x => x.MovedDate, request.StartDate.Value),
            Builders<AssetMovement>.Filter.Lte(x => x.MovedDate, request.EndDate.Value)
        );
    }

    var matchedMovements = await _assetMovements.Find(movementFilter).ToListAsync();

    var result = new List<AssetMovementReportDto>();

    foreach (var movement in matchedMovements)
    {
        foreach (var asset in movement.Assets)
        {
            // Apply From filters
            if (request.FromGroup != null && request.FromGroup.Any() && !request.FromGroup.Contains(asset.Group)) continue;
if (request.FromCompany != null && request.FromCompany.Any() && !request.FromCompany.Contains(asset.CompanyName)) continue;
if (request.FromSite != null && request.FromSite.Any() && !request.FromSite.Contains(asset.SiteName)) continue;
if (request.FromBuilding != null && request.FromBuilding.Any() && !request.FromBuilding.Contains(asset.BuildingName)) continue;
if (request.FromFloor != null && request.FromFloor.Any() && !request.FromFloor.Contains(asset.FloorName)) continue;
if (request.FromRoom != null && request.FromRoom.Any() && !request.FromRoom.Contains(asset.Room)) continue;
if (request.FromDepartment != null && request.FromDepartment.Any() && !request.FromDepartment.Contains(asset.Department)) continue;
if (request.FromCustodian != null && request.FromCustodian.Any() && !request.FromCustodian.Contains(asset.Custodian)) continue;


            result.Add(new AssetMovementReportDto
            {
                AssetCode = asset.AssetCode,
                AssetDescription = asset.AssetDescription,
                MainCategory = asset.MainCategory,
                SubCategory = asset.SubCategory,
                SubSubCategory = asset.SubSubCategory,

                FromGroup = asset.Group,
                FromCompany = asset.CompanyName,
                FromSite = asset.SiteName,
                FromBuilding = asset.BuildingName,
                FromFloor = asset.FloorName,
                FromRoom = asset.Room,
                FromDepartment = asset.Department,
                FromCustodian = asset.Custodian,

                ToGroup = movement.Group,
                ToCompany = movement.Company,
                ToSite = movement.Site,
                ToBuilding = movement.Building,
                ToFloor = movement.Floor,
                ToRoom = movement.Room,
                ToDepartment = movement.Department,
                ToCustodian = movement.Custodian,
                LastApproval=movement.lastapprovalworkflow,

                MovementDate = movement.MovedDate ,
                MovementInitiatedDate = movement.RequestDate ,
                ReferenceNumber = movement.ReferenceNumber
            });
        }
    }

    return result;
}

    }
}
