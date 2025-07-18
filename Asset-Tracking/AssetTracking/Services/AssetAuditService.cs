using MongoDB.Driver;
using YourNamespace.Models;

namespace YourNamespace.Services
{
    public class AssetAuditService
    {
        private readonly IMongoCollection<AssetAudit> _assetAudits;

        public AssetAuditService(IMongoDatabase database)
        {
            _assetAudits = database.GetCollection<AssetAudit>("AssetAudits");
        }

        public async Task<List<AssetAudit>> GetAllAsync() =>
            await _assetAudits.Find(_ => true).ToListAsync();

        public async Task<AssetAudit> GetByIdAsync(string id) =>
            await _assetAudits.Find(a => a.Id == id).FirstOrDefaultAsync();

        public async Task AddAsync(AssetAudit audit) =>
            await _assetAudits.InsertOneAsync(audit);

        public async Task<bool> UpdateAsync(string id, AssetAudit updatedAudit)
        {
            var result = await _assetAudits.ReplaceOneAsync(a => a.Id == id, updatedAudit);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var result = await _assetAudits.DeleteOneAsync(a => a.Id == id);
            return result.DeletedCount > 0;
        }
    }
}
