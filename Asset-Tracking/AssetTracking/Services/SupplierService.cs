using AssetTrackingAuthAPI.Config;
using AssetTrackingAuthAPI.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace AssetTrackingAuthAPI.Services
{
    public class SupplierService
    {
        private readonly IMongoCollection<Supplier> _supplierCollection;
        private readonly IMongoCollection<Group> _groupCollection;

        public SupplierService(IOptions<MongoDbSettings> dbSettings)
        {
            var client = new MongoClient(dbSettings.Value.ConnectionString);
            var db = client.GetDatabase(dbSettings.Value.DatabaseName);

            _supplierCollection = db.GetCollection<Supplier>("Suppliers");
            _groupCollection = db.GetCollection<Group>("Groups");
        }

        public async Task<List<Supplier>> GetAllAsync()
        {
            return await _supplierCollection.Find(_ => true).ToListAsync();
        }

        public async Task<Supplier?> GetByIdAsync(string id)
        {
            return await _supplierCollection.Find(s => s.Id == id).FirstOrDefaultAsync();
        }

        public async Task CreateAsync(SupplierRequest request)
        {
            var groupExists = await _groupCollection.Find(g => g.GroupName == request.GroupName).AnyAsync();
            if (!groupExists)
                throw new ArgumentException($"Group '{request.GroupName}' does not exist.");

            var supplier = new Supplier
            {
                SupplierCode = request.SupplierCode,
                SupplierName = request.SupplierName,
                OtherName = request.OtherName,
                GroupName = request.GroupName,
                IsActive = request.IsActive
            };

            await _supplierCollection.InsertOneAsync(supplier);
        }

        public async Task UpdateAsync(string id, SupplierRequest request)
        {
            var existing = await GetByIdAsync(id);
            if (existing == null)
                throw new ArgumentException("Supplier not found.");

            var groupExists = await _groupCollection.Find(g => g.GroupName == request.GroupName).AnyAsync();
            if (!groupExists)
                throw new ArgumentException($"Group '{request.GroupName}' does not exist.");

            existing.SupplierCode = request.SupplierCode;
            existing.SupplierName = request.SupplierName;
            existing.OtherName = request.OtherName;
            existing.GroupName = request.GroupName;
            existing.IsActive = request.IsActive;

            await _supplierCollection.ReplaceOneAsync(s => s.Id == id, existing);
        }

        public async Task DeleteAsync(string id)
        {
            var existing = await GetByIdAsync(id);
            if (existing == null)
                throw new ArgumentException("Supplier not found.");

            await _supplierCollection.DeleteOneAsync(s => s.Id == id);
        }
    }
}
