using MongoDB.Driver;
using YourNamespace.Models;

namespace YourNamespace.Services
{
    public class AssetCheckoutService
    {
        private readonly IMongoCollection<AssetCheckout> _assetMovements;

        public AssetCheckoutService(IMongoDatabase database)
        {
            _assetMovements = database.GetCollection<AssetCheckout>("AssetDisposal");
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

        public async Task CreateAsync(AssetCheckout movement) =>
            await _assetMovements.InsertOneAsync(movement);

        public async Task UpdateAsync(string id, AssetCheckout updated) =>
            await _assetMovements.ReplaceOneAsync(am => am.Id == id, updated);

        public async Task DeleteAsync(string id) =>
            await _assetMovements.DeleteOneAsync(am => am.Id == id);
    }
}
