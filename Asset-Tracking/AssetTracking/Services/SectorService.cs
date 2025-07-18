using AssetTrackingAuthAPI.Config;
using AssetTrackingAuthAPI.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace AssetTrackingAuthAPI.Services
{
    public class SectorService
    {
        private readonly IMongoCollection<Sector> _sectorCollection;
        private readonly IMongoCollection<Group> _groupCollection;

        public SectorService(IOptions<MongoDbSettings> dbSettings)
        {
            var client = new MongoClient(dbSettings.Value.ConnectionString);
            var db = client.GetDatabase(dbSettings.Value.DatabaseName);

            _sectorCollection = db.GetCollection<Sector>("Sectors");
            _groupCollection = db.GetCollection<Group>("Groups");
        }

        public async Task<List<Sector>> GetAllAsync()
        {
            return await _sectorCollection.Find(_ => true).ToListAsync();
        }

        public async Task<Sector?> GetByIdAsync(string id)
        {
            return await _sectorCollection.Find(s => s.Id == id).FirstOrDefaultAsync();
        }

        public async Task CreateAsync(SectorRequest request)
        {
            var groupExists = await _groupCollection.Find(g => g.GroupName == request.GroupName).AnyAsync();
            if (!groupExists)
                throw new ArgumentException($"Group '{request.GroupName}' does not exist.");

            var sector = new Sector
            {
                SectorCode = request.SectorCode,
                SectorName = request.SectorName,
                OtherName = request.OtherName,
                GroupName = request.GroupName,
                IsActive = request.IsActive
            };

            await _sectorCollection.InsertOneAsync(sector);
        }

        public async Task UpdateAsync(string id, SectorRequest request)
        {
            var existing = await GetByIdAsync(id);
            if (existing == null)
                throw new ArgumentException("Sector not found.");

            var groupExists = await _groupCollection.Find(g => g.GroupName == request.GroupName).AnyAsync();
            if (!groupExists)
                throw new ArgumentException($"Group '{request.GroupName}' does not exist.");

            existing.SectorCode = request.SectorCode;
            existing.SectorName = request.SectorName;
            existing.OtherName = request.OtherName;
            existing.GroupName = request.GroupName;
            existing.IsActive = request.IsActive;

            await _sectorCollection.ReplaceOneAsync(s => s.Id == id, existing);
        }

        public async Task DeleteAsync(string id)
        {
            var existing = await GetByIdAsync(id);
            if (existing == null)
                throw new ArgumentException("Sector not found.");

            await _sectorCollection.DeleteOneAsync(s => s.Id == id);
        }
    }
}
