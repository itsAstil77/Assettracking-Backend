using AssetTrackingAuthAPI.Models;
using MongoDB.Driver;
using YourNamespace.Models;

namespace YourNamespace.Services
{
    public class AssetCheckoutService
    {
        private readonly IMongoCollection<AssetCheckout> _assetMovements;
        private readonly IMongoCollection<Asset> _asset;

        public AssetCheckoutService(IMongoDatabase database)
        {
            _assetMovements = database.GetCollection<AssetCheckout>("AssetCheckout");
            _asset = database.GetCollection<Asset>("Assets");
        }

        public async Task<List<AssetCheckout>> GetSummaryAsync(int page, int pageSize)
        {
            return await _assetMovements.Find(_ => true)
                                        .Skip((page - 1) * pageSize)
                                        .Limit(pageSize)
                                        .ToListAsync();
        }

        public async Task<AssetCheckout> GetByIdAsync(string id) =>
            await _assetMovements.Find(am => am.Id == id).FirstOrDefaultAsync();

        public async Task createAsyncAssetcheckout(AssetCheckout checkout)
        {
            await _assetMovements.InsertOneAsync(checkout);
            foreach (var asset in checkout.Assets)
            {
                var update = Builders<Asset>.Update
                .Set(a => a.Custodian, checkout.Custodian)
                .Set(a => a.CompanyName, checkout.Company)
                .Set(a => a.SiteName, checkout.Site)
                .Set(a => a.BuildingName, checkout.Building)
                .Set(a => a.FloorName, checkout.Floor)
                .Set(a => a.Room, checkout.Room);
                await _asset.UpdateOneAsync(a => a.AssetCode == checkout.Assetcode, update);
                
            }
            
        }    

        public async Task CreateAsync(AssetCheckout movement) =>
            await _assetMovements.InsertOneAsync(movement);

        public async Task UpdateAsync(string id, AssetCheckout updated) =>
            await _assetMovements.ReplaceOneAsync(am => am.Id == id, updated);

        public async Task DeleteAsync(string id) =>
            await _assetMovements.DeleteOneAsync(am => am.Id == id);
    }
}
