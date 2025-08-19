using MongoDB.Bson;
using MongoDB.Driver;
using Microsoft.Extensions.Options;
using AssetTrackingAuthAPI.Models;
using AssetTrackingAuthAPI.Config;

namespace AssetTrackingAuthAPI.Services
{
    public class AssetService
    {
        private readonly IMongoCollection<Asset> _assetCollection;
        private readonly Dictionary<string, IMongoCollection<BsonDocument>> _refCollections;
        private readonly Dictionary<string, string> _fieldNameMap;

        public AssetService(IOptions<MongoDbSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            var db = client.GetDatabase(settings.Value.DatabaseName);

            _assetCollection = db.GetCollection<Asset>("Assets");

            _refCollections = new Dictionary<string, IMongoCollection<BsonDocument>>
            {
                { "CompanyName", db.GetCollection<BsonDocument>("Companies") },
                { "SiteName", db.GetCollection<BsonDocument>("Sites") },
                { "BuildingName", db.GetCollection<BsonDocument>("Buildings") },
                { "FloorName", db.GetCollection<BsonDocument>("Floors") },
                { "Room", db.GetCollection<BsonDocument>("Rooms") },
                {"Group",db.GetCollection<BsonDocument>("Groups")},
                { "Department", db.GetCollection<BsonDocument>("Departments") },
                { "Custodian", db.GetCollection<BsonDocument>("Custodians") },
                { "Sector", db.GetCollection<BsonDocument>("Sectors") },
                { "MainCategory", db.GetCollection<BsonDocument>("MainCategories") },
                { "SubCategory", db.GetCollection<BsonDocument>("SubCategories") },
                { "SubSubCategory", db.GetCollection<BsonDocument>("SubSubCategories") },
                { "Brand", db.GetCollection<BsonDocument>("Brands") },
                { "Model", db.GetCollection<BsonDocument>("Models") }
            };

            _fieldNameMap = new Dictionary<string, string>
            {
                { "CompanyName", "CompanyName" },
                { "SiteName", "SiteName" },
                { "BuildingName", "BuildingName" },
                { "FloorName", "FloorName" },
                { "Room", "RoomName" },
                { "Department", "DepartmentName" },
                { "Custodian", "CustodianName" },
                { "Sector", "SectorName" },
                { "MainCategory", "CategoryName" },
                { "SubCategory", "CategoryName" },
                { "SubSubCategory", "CategoryName" },
                {"Group","GroupName"},
                { "Brand", "CategoryName" },
                { "Model", "ModelName" }
            };
        }

        public async Task<(bool success, string error)> AddAssetAsync(Asset asset)
{
    var referencesToCheck = new Dictionary<string, string>
    {
        { "CompanyName", asset.CompanyName },
        { "SiteName", asset.SiteName },
        { "BuildingName", asset.BuildingName },
        { "FloorName", asset.FloorName },
        { "Room", asset.Room },
        { "Department", asset.Department },
        { "Custodian", asset.Custodian },
        { "Sector", asset.Sector },
        { "MainCategory", asset.MainCategory },
        { "SubCategory", asset.SubCategory },
        { "SubSubCategory", asset.SubSubCategory },
        { "Group", asset.Group },
        { "Brand", asset.Brand },
        { "Model", asset.Model }
    };

    foreach (var refCheck in referencesToCheck)
    {
        if (string.IsNullOrWhiteSpace(refCheck.Value)) continue;

        if (!_fieldNameMap.ContainsKey(refCheck.Key))
            return (false, $"Field mapping missing for {refCheck.Key}");

        if (!_refCollections.ContainsKey(refCheck.Key))
            return (false, $"Collection mapping missing for {refCheck.Key}");

        var fieldName = _fieldNameMap[refCheck.Key];
        var collection = _refCollections[refCheck.Key];

        if (!await ExistsAsync(collection, fieldName, refCheck.Value))
            return (false, $"{refCheck.Key} '{refCheck.Value}' does not exist in collection.");
    }

    int quantity = asset.Quantity > 0 ? asset.Quantity : 1;

    // 🔍 Find the last AssetCode (sort descending)
    var lastAsset = await _assetCollection
        .Find(Builders<Asset>.Filter.Empty)
        .SortByDescending(a => a.AssetCode)
        .FirstOrDefaultAsync();

    int lastNumber = 0;
    if (lastAsset != null && int.TryParse(lastAsset.AssetCode, out int parsed))
    {
        lastNumber = parsed;
    }

    // 🔑 Generate a unique purchase code per batch
    string purchaseCode = $"PUR-{DateTime.UtcNow:yyyyMMddHHmmssfff}";

    var assetList = new List<Asset>();
    for (int i = 1; i <= quantity; i++)
    {
        var newAsset = new Asset
        {
            Id = ObjectId.GenerateNewId().ToString(),
            AssetCode = $"{(lastNumber + i):D4}", // ✅ Continue from last number
            CompanyName = asset.CompanyName,
            SiteName = asset.SiteName,
            BuildingName = asset.BuildingName,
            FloorName = asset.FloorName,
            Room = asset.Room,
            Department = asset.Department,
            Custodian = asset.Custodian,
            Sector = asset.Sector,
            MainCategory = asset.MainCategory,
            SubCategory = asset.SubCategory,
            Group = asset.Group,
            SubSubCategory = asset.SubSubCategory,
            Brand = asset.Brand,
            Model = asset.Model,
            Quantity = 1,
            AssetDescription = asset.AssetDescription,
            ReferenceCode = asset.ReferenceCode,
            AssetStatus = asset.AssetStatus,
            AssetCondition = asset.AssetCondition,
            AssetType = asset.AssetType,
            PurchaseCode = purchaseCode
        };

        assetList.Add(newAsset);
    }

    await _assetCollection.InsertManyAsync(assetList);
    return (true, $"{quantity} asset(s) added successfully with PurchaseCode: {purchaseCode}");
}

        private async Task<bool> ExistsAsync(IMongoCollection<BsonDocument> collection, string fieldName, string value)
        {
            var filter = Builders<BsonDocument>.Filter.Eq(fieldName, value);
            var count = await collection.CountDocumentsAsync(filter);
            return count > 0;
        }
        public async Task<List<Asset>> GetAllAssetsAsync()
        {
            return await _assetCollection.Find(_ => true).ToListAsync();
        }

        // Update asset by Id
        public async Task<(bool success, string message)> UpdateAssetAsync(string id, Asset updatedAsset)
        {
            var existingAsset = await _assetCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
            if (existingAsset == null)
                return (false, "Asset not found.");

            updatedAsset.Id = id;
            updatedAsset.AssetCode = existingAsset.AssetCode; // Preserve asset code

            await _assetCollection.ReplaceOneAsync(x => x.Id == id, updatedAsset);
            return (true, "Asset updated successfully.");
        }

        // Delete asset by Id
        public async Task<(bool success, string message)> DeleteAssetAsync(string id)
        {
            var result = await _assetCollection.DeleteOneAsync(x => x.Id == id);
            if (result.DeletedCount == 0)
                return (false, "Asset not found or already deleted.");

            return (true, "Asset deleted successfully.");
        }

         public async Task<List<string>> PreviewNextAssetCodes(int quantity)
{
    // Get the last asset by AssetCode (sorted descending)
    var lastAsset = await _assetCollection
        .Find(Builders<Asset>.Filter.Empty)
        .SortByDescending(a => a.AssetCode)
        .FirstOrDefaultAsync();

    int lastNumber = 0;
    if (lastAsset != null && int.TryParse(lastAsset.AssetCode, out int parsed))
    {
        lastNumber = parsed;
    }

    // Generate next asset codes
    var codes = new List<string>();
    for (int i = 1; i <= quantity; i++)
    {
        codes.Add($"{(lastNumber + i):D4}");
    }

    return codes;
}

    }
}
