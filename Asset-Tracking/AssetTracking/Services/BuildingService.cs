using AssetTrackingAuthAPI.Config;
using AssetTrackingAuthAPI.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace AssetTrackingAuthAPI.Services
{
    public class BuildingService
    {
        private readonly IMongoCollection<Building> _buildingCollection;
        private readonly IMongoCollection<Site> _siteCollection;
        private readonly IMongoCollection<Floor> _floor;

        public BuildingService(IOptions<MongoDbSettings> dbSettings)
        {
            var mongoClient = new MongoClient(dbSettings.Value.ConnectionString);
            var database = mongoClient.GetDatabase(dbSettings.Value.DatabaseName);

            _buildingCollection = database.GetCollection<Building>("Buildings");
            _siteCollection = database.GetCollection<Site>("Sites");
            _floor = database.GetCollection<Floor>("Floors");
        }

        public async Task<List<Building>> GetAllAsync() =>
            await _buildingCollection.Find(_ => true).ToListAsync();

           
public async Task<List<Building>> GetAllbuildingsbysiteAsync(string SiteId)=>
        
            await _buildingCollection.Find(c => c.SiteId == SiteId).ToListAsync();
        public async Task<Building?> GetByIdAsync(string id) =>
            await _buildingCollection.Find(b => b.Id == id).FirstOrDefaultAsync();

        public async Task CreateAsync(BuildingRequest request)
        {
            // Check if site exists
            var site = await _siteCollection.Find(s => s.Id == request.SiteId).FirstOrDefaultAsync();
            if (site == null)
                throw new ArgumentException("Site does not exist.");

            var building = new Building
            {
                BuildingCode = request.BuildingCode,
                BuildingName = request.BuildingName,
                Description = request.Description,
                IsActive = request.IsActive,
                SiteId = request.SiteId,
                SiteName = site.SiteName
            };

            await _buildingCollection.InsertOneAsync(building);
        }

        public async Task UpdateAsync(string id, BuildingRequest request)
        {
            var building = await GetByIdAsync(id);
            if (building == null)
                throw new ArgumentException("Building not found.");

            var site = await _siteCollection.Find(s => s.Id == request.SiteId).FirstOrDefaultAsync();
            if (site == null)
                throw new ArgumentException("Site does not exist.");

            building.BuildingCode = request.BuildingCode;
            building.BuildingName = request.BuildingName;
            building.Description = request.Description;
            building.IsActive = request.IsActive;
            building.SiteId = request.SiteId;
            building.SiteName = site.SiteName;

            await _buildingCollection.ReplaceOneAsync(b => b.Id == id, building);
        }

        public async Task<string> DeleteAsync(string id)
        {
            var build = await _floor.Find(c => c.BuildingId == id).AnyAsync();
            if (build)
                return "building has active floors so deletion cant be allowed";
            await _buildingCollection.DeleteOneAsync(b => b.Id == id);
            return "Building deleted sucessfully";
        }
    }
}
