using AssetTrackingAuthAPI.Config;
using AssetTrackingAuthAPI.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace AssetTrackingAuthAPI.Services
{
    public class DepartmentService
    {
        private readonly IMongoCollection<Department> _departmentCollection;
        private readonly IMongoCollection<Group> _groupCollection;

        public DepartmentService(IOptions<MongoDbSettings> dbSettings)
        {
            var client = new MongoClient(dbSettings.Value.ConnectionString);
            var db = client.GetDatabase(dbSettings.Value.DatabaseName);
            _departmentCollection = db.GetCollection<Department>("Departments");
            _groupCollection = db.GetCollection<Group>("Groups"); // ✅ GROUP COLLECTION
        }

        public async Task<List<Department>> GetAllAsync()
        {
            return await _departmentCollection.Find(_ => true).ToListAsync();
        }

        public async Task<Department?> GetByIdAsync(string id)
        {
            return await _departmentCollection.Find(d => d.Id == id).FirstOrDefaultAsync();
        }

        public async Task CreateAsync(DepartmentRequest request)
        {
            if (!await GroupExists(request.GroupName))
                throw new ArgumentException($"Group '{request.GroupName}' does not exist.");

            var department = new Department
            {
                DepartmentCode = request.DepartmentCode,
                DepartmentName = request.DepartmentName,
                OtherName = request.OtherName,
                GroupName = request.GroupName,
                IsActive = request.IsActive
            };

            await _departmentCollection.InsertOneAsync(department);
        }

        public async Task UpdateAsync(string id, DepartmentRequest request)
        {
            var existing = await GetByIdAsync(id);
            if (existing == null)
                throw new ArgumentException("Department not found.");

            if (!await GroupExists(request.GroupName))
                throw new ArgumentException($"Group '{request.GroupName}' does not exist.");

            var updated = new Department
            {
                Id = id,
                DepartmentCode = request.DepartmentCode,
                DepartmentName = request.DepartmentName,
                OtherName = request.OtherName,
                GroupName = request.GroupName,
                IsActive = request.IsActive
            };

            await _departmentCollection.ReplaceOneAsync(d => d.Id == id, updated);
        }

        public async Task DeleteAsync(string id)
        {
            var existing = await GetByIdAsync(id);
            if (existing == null)
                throw new ArgumentException("Department not found.");

            await _departmentCollection.DeleteOneAsync(d => d.Id == id);
        }

        private async Task<bool> GroupExists(string groupName)
        {
            return await _groupCollection.Find(g => g.GroupName == groupName).AnyAsync();
        }
    }
}
