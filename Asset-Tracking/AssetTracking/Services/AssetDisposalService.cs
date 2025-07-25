using AssetTrackingAuthAPI.Models;
using AssetTrackingAuthAPI.Services;
using MongoDB.Driver;
using Org.BouncyCastle.Asn1;
using YourNamespace.Models;

namespace YourNamespace.Services
{
    public class AssetDisposalService
    {
        private readonly IMongoCollection<AssetDisposal> _assetMovements;
        private readonly IMongoCollection<ApprovalWorkflow> _approval;
        private readonly IMongoCollection<User> _users;

        private readonly EmailService _emailService;
        private readonly IMongoCollection<Asset> _assets;
        private readonly IMongoCollection<PurchaseInfo> _Purchase;

        public AssetDisposalService(IMongoDatabase database, EmailService email)
        {
            _assetMovements = database.GetCollection<AssetDisposal>("AssetDisposal");
            _approval = database.GetCollection<ApprovalWorkflow>("ApprovalWorkflow");
            _users = database.GetCollection<User>("Users");
            _assets = database.GetCollection<Asset>("Assets");
            _Purchase = database.GetCollection<PurchaseInfo>("PurchaseInfo");
            _emailService = email;

        }

        public async Task<List<AssetDisposal>> GetSummaryAsync(int page, int pageSize)
        {
            return await _assetMovements.Find(_ => true)
                                        .Skip((page - 1) * pageSize)
                                        .Limit(pageSize)
                                        .ToListAsync();
        }

        public async Task<AssetDisposal> GetByIdAsync(string id) =>
            await _assetMovements.Find(am => am.Id == id).FirstOrDefaultAsync();

        public async Task CreateAsync(AssetDisposal movement) =>
            await _assetMovements.InsertOneAsync(movement);

        public async Task UpdateAsync(string id, AssetDisposal updated) =>
            await _assetMovements.ReplaceOneAsync(am => am.Id == id, updated);

        public async Task DeleteAsync(string id) =>
            await _assetMovements.DeleteOneAsync(am => am.Id == id);

       public async Task<(bool success, string message)> CreateOrApproveDisposalAsync(AssetDisposal? disposal, string? id)
{
    if (!string.IsNullOrEmpty(id))
    {
        // Approve logic
        var existing = await _assetMovements.Find(x => x.Id == id).FirstOrDefaultAsync();
        if (existing == null)
            return (false, "Disposal not found");
string previousRole = existing.approvalworkflow; // store it before changing

        var currentWorkflow = await _approval.Find(x => x.Role == previousRole && x.Transaction == "AssetDisposal").FirstOrDefaultAsync();
        if (currentWorkflow == null)
            return (false, "Current approval step not found");

        existing.lastapprovalworkflow = currentWorkflow.Role;

        // Try to get next sequence
        var nextSequence = (int.Parse(currentWorkflow.Sequence) + 1).ToString();
        var nextWorkflow = await _approval.Find(x => x.Sequence == nextSequence && x.Transaction == "AssetDisposal").FirstOrDefaultAsync();

        if (nextWorkflow != null)
        {
            // More approvals needed
            existing.approvalworkflow = nextWorkflow.Role;
            existing.nextapprovalworkflow = nextWorkflow.Role;
            string approvedByUser = "";
    var approverUser = await _users.Find(x => x.AssignedRoles.Contains(previousRole)).FirstOrDefaultAsync();
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
            var nextUser = await _users.Find(u => u.AssignedRoles.Contains(nextWorkflow.Role)).FirstOrDefaultAsync();
            if (nextUser != null && !string.IsNullOrWhiteSpace(nextUser.Email))
            {
                string nextUrl = $"http://172.16.100.71:5221/api/AssetDisposal/process?id={existing.Id}";
                string nextBody = $"<p>Asset Code {existing.Assetcode} needs your approval.</p><p><a href='{nextUrl}'>Approve</a></p>";
                await _emailService.SendEmailAsync(nextUser.Email, "Asset Disposal Approval - Next Step", nextBody);
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
                    existing.DisposedDate = DateTime.UtcNow;
                     string finalApprovedBy = "";
    var finalApprover = await _users.Find(x => x.AssignedRoles.Contains(currentWorkflow.Role)).FirstOrDefaultAsync();
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

// Delete assets from Asset collection based on Assetcodes in disposal
foreach (var asset in existing.Assets)
{
    // Step 1: Delete asset
    await _assets.DeleteOneAsync(x => x.AssetCode == asset.AssetCode);

    // Step 2: Check if any other asset still uses the same PurchaseCode
    var purchaseCodeInUse = await _assets
        .Find(x => x.PurchaseCode == asset.PurchaseCode)
        .AnyAsync();

    // Step 3: If not in use, delete the purchase info
    if (!purchaseCodeInUse)
    {
        await _Purchase.DeleteOneAsync(p => p.PurchaseCode == asset.PurchaseCode);
    }
}

return (true, "Disposal fully approved and associated assets removed.");

        }
    }
    else if (disposal != null)
    {
        // New disposal creation
        var firstWorkflow = await _approval.Find(x => x.Transaction == "AssetDisposal")
                                           .SortBy(x => x.Sequence)
                                           .FirstOrDefaultAsync();
        if (firstWorkflow == null)
            return (false, "No approval workflow defined");

        disposal.approvalworkflow = firstWorkflow.Role;
        disposal.nextapprovalworkflow = firstWorkflow.Role;
         disposal.status = "Created";
        disposal.ReferenceNumber = $"DISP{DateTime.UtcNow:yyyyMMddHHmmssfff}";
var createdByUser = await _users.Find(x => x.AssignedRoles.Contains(firstWorkflow.Role)).FirstOrDefaultAsync();
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

        var firstUser = await _users.Find(u => u.AssignedRoles.Contains(firstWorkflow.Role)).FirstOrDefaultAsync();
        if (firstUser != null && !string.IsNullOrWhiteSpace(firstUser.Email))
        {
            string url = $"http://172.16.100.71:5221/api/AssetDisposal/process?id={disposal.Id}";
            string body = $"<p>Asset Code {disposal.Assetcode} has been submitted for disposal.</p><p><a href='{url}'>Approve</a></p>";
            await _emailService.SendEmailAsync(firstUser.Email, "Asset Disposal Approval Required", body);
        }

        return (true, "Disposal submitted and approval email sent.");
    }

    return (false, "Invalid request. Provide disposal or approval id.");
}

public async Task<(bool success, string message)> ProcessApprovalAsync(string id)
{
    var disposal = await _assetMovements.Find(x => x.Id == id).FirstOrDefaultAsync();

    if (disposal == null)
        return (false, "Disposal not found");

    if (disposal.status == "Approved")
        return (false, "Disposal is already approved.");

    disposal.status = "Approved";
    await _assetMovements.ReplaceOneAsync(x => x.Id == id, disposal);

    return (true, "Disposal approved");
}

public async Task<List<Asset>> GetAssetsForDisposal(string disposalId)
{
    var disposal = await _assetMovements
        .Find(x => x.Id == disposalId)
        .FirstOrDefaultAsync();

    if (disposal == null || disposal.Assets == null || disposal.Assets.Count == 0)
        return new List<Asset>();

    // Return directly from the embedded assets array
    return disposal.Assets;
}

public async Task<List<Asset>> GetDisposalReportAsync(AssetDisposalReportFilterDto filter)
{
    var disposalFilter = Builders<AssetDisposal>.Filter.Empty;

    // Filter by date range (DisposedDate)
    if (filter.StartDate.HasValue && filter.EndDate.HasValue)
    {
        disposalFilter &= Builders<AssetDisposal>.Filter.And(
            Builders<AssetDisposal>.Filter.Gte(x => x.DisposedDate, filter.StartDate.Value),
            Builders<AssetDisposal>.Filter.Lte(x => x.DisposedDate, filter.EndDate.Value)
        );
    }

    // Only approved disposals
    disposalFilter &= Builders<AssetDisposal>.Filter.Eq(x => x.status, "Approved");

    var disposalRecords = await _assetMovements.Find(disposalFilter).ToListAsync();

    // Flatten all assets from matching disposals
    var allAssets = disposalRecords.SelectMany(x => x.Assets).ToList();

    // Apply asset-level filters
    if (!string.IsNullOrEmpty(filter.Group))
        allAssets = allAssets.Where(a => a.Group == filter.Group).ToList();

    if (!string.IsNullOrEmpty(filter.CompanyName))
        allAssets = allAssets.Where(a => a.CompanyName == filter.CompanyName).ToList();

    if (!string.IsNullOrEmpty(filter.SiteName))
        allAssets = allAssets.Where(a => a.SiteName == filter.SiteName).ToList();

    if (!string.IsNullOrEmpty(filter.BuildingName))
        allAssets = allAssets.Where(a => a.BuildingName == filter.BuildingName).ToList();

    if (!string.IsNullOrEmpty(filter.FloorName))
        allAssets = allAssets.Where(a => a.FloorName == filter.FloorName).ToList();

    if (!string.IsNullOrEmpty(filter.Room))
        allAssets = allAssets.Where(a => a.Room == filter.Room).ToList();

    if (!string.IsNullOrEmpty(filter.Custodian))
        allAssets = allAssets.Where(a => a.Custodian == filter.Custodian).ToList();

    if (!string.IsNullOrEmpty(filter.Department))
        allAssets = allAssets.Where(a => a.Department == filter.Department).ToList();

    if (!string.IsNullOrEmpty(filter.MainCategory))
        allAssets = allAssets.Where(a => a.MainCategory == filter.MainCategory).ToList();

    if (!string.IsNullOrEmpty(filter.SubCategory))
        allAssets = allAssets.Where(a => a.SubCategory == filter.SubCategory).ToList();

    if (!string.IsNullOrEmpty(filter.SubSubCategory))
        allAssets = allAssets.Where(a => a.SubSubCategory == filter.SubSubCategory).ToList();

    return allAssets;
}


    }
}
