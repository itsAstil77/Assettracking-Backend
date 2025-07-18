using AssetTrackingAuthAPI.Models;
using AssetTrackingAuthAPI.Config;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace AssetTrackingAuthAPI.Services
{
    public class RoleAccessService
    {
        private readonly IMongoCollection<RoleAccess> _roleAccessCollection;
        private readonly IMongoCollection<Module> _modulesCollection;

        public RoleAccessService(IOptions<MongoDbSettings> dbSettings)
        {
            var mongoClient = new MongoClient(dbSettings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(dbSettings.Value.DatabaseName);

            _roleAccessCollection = mongoDatabase.GetCollection<RoleAccess>("RoleAccess");
            _modulesCollection = mongoDatabase.GetCollection<Module>("Modules"); // Referenced
        }

        public async Task<List<RoleAccess>> GetAllAsync(int page, int size)
        {
            return await _roleAccessCollection
                .Find(_ => true)
                .Skip((page - 1) * size)
                .Limit(size)
                .ToListAsync();
        }

        public async Task<RoleAccess?> GetByIdAsync(string id) =>
            await _roleAccessCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

        public async Task<long> GetTotalCountAsync() =>
            await _roleAccessCollection.CountDocumentsAsync(_ => true);

        public async Task CreateAsync(RoleAccess roleAccess)
        {
            // ✅ Validate module names
            // var existingModules = await _modulesCollection
            //     .Find(_ => true)
            //     .Project(m => m.ModuleName)
            //     .ToListAsync();

            // var invalidModules = roleAccess.AccessList
            //     .Where(m => !existingModules.Contains(m.ModuleName))
            //     .Select(m => m.ModuleName)
            //     .ToList();

            // if (invalidModules.Any())
            //     throw new ArgumentException($"Module(s) not found: {string.Join(", ", invalidModules)}");

            await _roleAccessCollection.InsertOneAsync(roleAccess);
        }

        public async Task UpdateAsync(string id, RoleAccess updated)
        {
            updated.Id = id;
            await _roleAccessCollection.ReplaceOneAsync(x => x.Id == id, updated);
        }

        public async Task DeleteAsync(string id) =>
            await _roleAccessCollection.DeleteOneAsync(x => x.Id == id);
    }
}
