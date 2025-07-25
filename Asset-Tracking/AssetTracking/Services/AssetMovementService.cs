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

       public async Task<List<Asset>> GenerateAssetMovementReportAsync(AssetMovementReportRequest request)
        {
            var movementFilter = Builders<AssetMovement>.Filter.Empty;
            if (!string.IsNullOrEmpty(request.ToGroup))
                movementFilter &= Builders<AssetMovement>.Filter.Eq(x => x.Group, request.ToGroup);
            if (!string.IsNullOrEmpty(request.ToCompany))
                movementFilter &= Builders<AssetMovement>.Filter.Eq(x => x.Company, request.ToCompany);
            if (!string.IsNullOrEmpty(request.ToSite))
                movementFilter &= Builders<AssetMovement>.Filter.Eq(x => x.Site, request.ToSite);
            if (!string.IsNullOrEmpty(request.ToBuilding))
                movementFilter &= Builders<AssetMovement>.Filter.Eq(x => x.Building, request.ToBuilding);
            if (!string.IsNullOrEmpty(request.ToFloor))
                movementFilter &= Builders<AssetMovement>.Filter.Eq(x => x.Floor, request.ToFloor);
            if (!string.IsNullOrEmpty(request.ToRoom))
                movementFilter &= Builders<AssetMovement>.Filter.Eq(x => x.Room, request.ToRoom);
            if (!string.IsNullOrEmpty(request.ToDepartment))
                movementFilter &= Builders<AssetMovement>.Filter.Eq(x => x.Department, request.ToDepartment);
            if (!string.IsNullOrEmpty(request.ToCustodian))
                movementFilter &= Builders<AssetMovement>.Filter.Eq(x => x.Custodian, request.ToCustodian);

            var matchedMovements = await _assetMovements.Find(movementFilter).ToListAsync();

            var result = new List<Asset>();

            foreach (var movement in matchedMovements)
            {
                var filteredAssets = movement.Assets.Where(asset =>
                    (string.IsNullOrEmpty(request.FromGroup) || asset.Group == request.FromGroup) &&
                    (string.IsNullOrEmpty(request.FromCompany) || asset.CompanyName == request.FromCompany) &&
                    (string.IsNullOrEmpty(request.FromSite) || asset.SiteName == request.FromSite) &&
                    (string.IsNullOrEmpty(request.FromBuilding) || asset.BuildingName == request.FromBuilding) &&
                    (string.IsNullOrEmpty(request.FromFloor) || asset.FloorName == request.FromFloor) &&
                    (string.IsNullOrEmpty(request.FromRoom) || asset.Room == request.FromRoom) &&
                    (string.IsNullOrEmpty(request.FromDepartment) || asset.Department == request.FromDepartment) &&
                    (string.IsNullOrEmpty(request.FromCustodian) || asset.Custodian == request.FromCustodian)
                ).ToList();

                result.AddRange(filteredAssets);
            }

            return result;
        }


    }
}
