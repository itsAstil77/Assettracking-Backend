using AssetTrackingAuthAPI.Config;
using AssetTrackingAuthAPI.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace AssetTrackingAuthAPI.Services
{
    public class FloorService
    {
        private readonly IMongoCollection<Floor> _floorCollection;
        private readonly IMongoCollection<Building> _buildingCollection;
        private readonly IMongoCollection<Room> _room;

        public FloorService(IOptions<MongoDbSettings> dbSettings)
        {
            var mongoClient = new MongoClient(dbSettings.Value.ConnectionString);
            var database = mongoClient.GetDatabase(dbSettings.Value.DatabaseName);

            _floorCollection = database.GetCollection<Floor>("Floors");
            _buildingCollection = database.GetCollection<Building>("Buildings");
            _room = database.GetCollection<Room>("Rooms");
        }

        public async Task<List<Floor>> GetAllAsync() =>
            await _floorCollection.Find(_ => true).ToListAsync();

        public async Task<Floor?> GetByIdAsync(string id) =>
            await _floorCollection.Find(f => f.Id == id).FirstOrDefaultAsync();

        public async Task<List<Floor>> GetAllfloorbybuildingAsync(string BuildingId)=>
        
            await _floorCollection.Find(c => c.BuildingId == BuildingId).ToListAsync();

        public async Task CreateAsync(FloorRequest request)
        {
            var building = await _buildingCollection.Find(b => b.Id == request.BuildingId).FirstOrDefaultAsync();
            if (building == null)
                throw new ArgumentException("Building does not exist.");

            var floor = new Floor
            {
                FloorCode = request.FloorCode,
                FloorName = request.FloorName,
                Description = request.Description,
                IsActive = request.IsActive,
                BuildingId = request.BuildingId,
                BuildingName = building.BuildingName
            };

            await _floorCollection.InsertOneAsync(floor);
        }

        public async Task UpdateAsync(string id, FloorRequest request)
        {
            var floor = await GetByIdAsync(id);
            if (floor == null)
                throw new ArgumentException("Floor not found.");

            var building = await _buildingCollection.Find(b => b.Id == request.BuildingId).FirstOrDefaultAsync();
            if (building == null)
                throw new ArgumentException("Building does not exist.");

            floor.FloorCode = request.FloorCode;
            floor.FloorName = request.FloorName;
            floor.Description = request.Description;
            floor.IsActive = request.IsActive;
            floor.BuildingId = request.BuildingId;
            floor.BuildingName = building.BuildingName;

            await _floorCollection.ReplaceOneAsync(f => f.Id == id, floor);
        }

        public async Task<string> DeleteAsync(string id)
        {
            var room = await _room.Find(c => c.FloorId == id).AnyAsync();
            if (room)
                return "Floor has active Rooms so deletion cant be allowed";
            var result = await _floorCollection.DeleteOneAsync(f => f.Id == id);
            return "Floor deleted sucessfully";
            if (result.DeletedCount == 0)
                throw new ArgumentException("Floor not found.");
        }
    }
}
