using AssetTrackingAuthAPI.Models;
using Microsoft.VisualBasic;
using MongoDB.Driver;
using YourNamespace.Models;

namespace YourNamespace.Services
{
    public class AssetCheckinService
    {
        private readonly IMongoCollection<AssetCheckin> _assetMovements;
        private readonly IMongoCollection<Asset> _assets;

        public AssetCheckinService(IMongoDatabase database)
        {
            _assetMovements = database.GetCollection<AssetCheckin>("AssetCheckin");
            _assets = database.GetCollection<Asset>("Assets");
        }

        public async Task<List<AssetCheckin>> GetSummaryAsync(int page, int pageSize)
        {
            return await _assetMovements.Find(_ => true)
                                        .Skip((page - 1) * pageSize)
                                        .Limit(pageSize)
                                        .ToListAsync();
        }

        public async Task<AssetCheckin> GetByIdAsync(string id) =>
            await _assetMovements.Find(am => am.Id == id).FirstOrDefaultAsync();

       public async Task CreateAsync(AssetCheckin checkin)
{
    // Save check-in record
    await _assetMovements.InsertOneAsync(checkin);

    // Update each asset in the asset collection
    foreach (var asset in checkin.Assets)
    {
        var update = Builders<Asset>.Update
            .Set(a => a.Custodian, checkin.newcustodian)
            .Set(a => a.Department, checkin.department)
            .Set(a => a.CompanyName, checkin.companyName)
            .Set(a => a.SiteName, checkin.SiteName)
            .Set(a => a.BuildingName, checkin.buildingName)
            .Set(a => a.FloorName, checkin.floorName)
            .Set(a => a.Room, checkin.roomName);

        await _assets.UpdateOneAsync(a => a.AssetCode == asset.AssetCode, update);
    }
}


        public async Task UpdateAsync(string id, AssetCheckin updated) =>
            await _assetMovements.ReplaceOneAsync(am => am.Id == id, updated);

        public async Task DeleteAsync(string id) =>
            await _assetMovements.DeleteOneAsync(am => am.Id == id);

        public async Task<List<Asset>> getassetbycustodian(string custodianname)
        {
            var hell = await _assets.Find(x => x.Custodian == custodianname).ToListAsync();
            return hell;
        }
    }
}