using AssetTrackingAuthAPI.Models;
using AssetTrackingAuthAPI.Services;
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
                    return (false, "Disposal not found");

                var currentWorkflow = await _approval.Find(x => x.Role == existing.approvalworkflow && x.Transaction == "AssetMovement").FirstOrDefaultAsync();
                if (currentWorkflow == null)
                    return (false, "Current approval step not found");

                existing.lastapprovalworkflow = currentWorkflow.Role;

                // Try to get next sequence
                var nextSequence = (int.Parse(currentWorkflow.Sequence) + 1).ToString();
                var nextWorkflow = await _approval.Find(x => x.Sequence == nextSequence && x.Transaction == "AssetMovement").FirstOrDefaultAsync();

                if (nextWorkflow != null)
                {
                    // More approvals needed
                    existing.approvalworkflow = nextWorkflow.Role;
                    existing.nextapprovalworkflow = nextWorkflow.Role;
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
                    // Final step
                   // Final step
existing.approvalworkflow = null;
existing.nextapprovalworkflow = null;
existing.status = "Approved";
await _assetMovements.ReplaceOneAsync(x => x.Id == id, existing);

// Update asset details based on movement
foreach (var asset in existing.Assets)
{
    var update = Builders<Asset>.Update
   .Set(x => x.Department, asset.Department)
   .Set(x => x.Custodian, asset.Custodian)
   .Set(x => x.Group, asset.Group)
   
        .Set(x => x.CompanyName, asset.CompanyName)
        .Set(x => x.SiteName, asset.SiteName)
        .Set(x => x.BuildingName, asset.BuildingName)
        .Set(x => x.FloorName, asset.FloorName)
        .Set(x => x.Room, asset.Room);

    await _assets.UpdateOneAsync(x => x.AssetCode == asset.AssetCode, update);
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
                disposal.status = "Pending";

                await _assetMovements.InsertOneAsync(disposal);

                var firstUser = await _user.Find(u => u.AssignedRoles.Contains(firstWorkflow.Role)).FirstOrDefaultAsync();
                if (firstUser != null && !string.IsNullOrWhiteSpace(firstUser.Email))
                {
                    string url = $"http://172.16.100.71:5221/api/AssetMovement/process?id={disposal.Id}";
                    string body = $"<p>Asset Code {disposal.Assetcode} has been submitted for Movement.</p><p><a href='{url}'>Approve</a></p>";
                    await _emailservice.SendEmailAsync(firstUser.Email, "Asset Movement Approval Required", body);
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

    }
}
