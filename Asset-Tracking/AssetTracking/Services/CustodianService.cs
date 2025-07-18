using AssetTrackingAuthAPI.Config;
using AssetTrackingAuthAPI.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace AssetTrackingAuthAPI.Services
{
    public class CustodianService
    {
        private readonly IMongoCollection<Custodian> _custodianCollection;
        private readonly IMongoCollection<Department> _departmentCollection;

        public CustodianService(IOptions<MongoDbSettings> dbSettings)
        {
            var client = new MongoClient(dbSettings.Value.ConnectionString);
            var db = client.GetDatabase(dbSettings.Value.DatabaseName);

            _custodianCollection = db.GetCollection<Custodian>("Custodians");
            _departmentCollection = db.GetCollection<Department>("Departments");
        }

        public async Task<List<Custodian>> GetAllAsync()
        {
            return await _custodianCollection.Find(_ => true).ToListAsync();
        }

        public async Task<Custodian?> GetByIdAsync(string id)
        {
            return await _custodianCollection.Find(c => c.Id == id).FirstOrDefaultAsync();
        }

        public async Task CreateAsync(CustodianRequest request)
        {
            var departmentExists = await _departmentCollection.Find(d => d.DepartmentName == request.DepartmentName).AnyAsync();
            if (!departmentExists)
                throw new ArgumentException($"Department '{request.DepartmentName}' does not exist.");

            var custodian = new Custodian
            {
                CustodianCode = request.CustodianCode,
                CustodianName = request.CustodianName,
                CustodianEmail = request.CustodianEmail,
                OtherName = request.OtherName,
                DepartmentName = request.DepartmentName,
                IsActive = request.IsActive
            };

            await _custodianCollection.InsertOneAsync(custodian);
        }

        public async Task UpdateAsync(string id, CustodianRequest request)
        {
            var custodian = await GetByIdAsync(id);
            if (custodian == null)
                throw new ArgumentException("Custodian not found.");

            var departmentExists = await _departmentCollection.Find(d => d.DepartmentName == request.DepartmentName).AnyAsync();
            if (!departmentExists)
                throw new ArgumentException($"Department '{request.DepartmentName}' does not exist.");

            custodian.CustodianCode = request.CustodianCode;
            custodian.CustodianName = request.CustodianName;
            custodian.CustodianEmail = request.CustodianEmail;
            custodian.OtherName = request.OtherName;
            custodian.DepartmentName = request.DepartmentName;
            custodian.IsActive = request.IsActive;

            await _custodianCollection.ReplaceOneAsync(c => c.Id == id, custodian);
        }

        public async Task DeleteAsync(string id)
        {
            var custodian = await GetByIdAsync(id);
            if (custodian == null)
                throw new ArgumentException("Custodian not found.");

            await _custodianCollection.DeleteOneAsync(c => c.Id == id);
        }
    }
}
