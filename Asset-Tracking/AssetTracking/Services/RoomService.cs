using AssetTrackingAuthAPI.Config;
using AssetTrackingAuthAPI.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace AssetTrackingAuthAPI.Services
{
    public class RoomService
    {
        private readonly IMongoCollection<Room> _roomCollection;
        private readonly IMongoCollection<Floor> _floorCollection;

        public RoomService(IOptions<MongoDbSettings> dbSettings)
        {
            var mongoClient = new MongoClient(dbSettings.Value.ConnectionString);
            var database = mongoClient.GetDatabase(dbSettings.Value.DatabaseName);

            _roomCollection = database.GetCollection<Room>("Rooms");
            _floorCollection = database.GetCollection<Floor>("Floors");
        }

        public async Task<List<Room>> GetAllAsync() =>
            await _roomCollection.Find(_ => true).ToListAsync();

        public async Task<List<Room>> GetAllRoombyfloorAsync(string FloorId) =>

       await _roomCollection.Find(c => c.FloorId == FloorId).ToListAsync();

        public async Task<Room?> GetByIdAsync(string id) =>
            await _roomCollection.Find(r => r.Id == id).FirstOrDefaultAsync();

        public async Task CreateAsync(RoomRequest request)
        {
            var floor = await _floorCollection.Find(f => f.Id == request.FloorId).FirstOrDefaultAsync();
            if (floor == null)
                throw new ArgumentException("Floor does not exist.");

            var room = new Room
            {
                RoomCode = request.RoomCode,
                RoomName = request.RoomName,
                Description = request.Description,
                IsActive = request.IsActive,
                FloorId = request.FloorId,
                FloorName = floor.FloorName
            };

            await _roomCollection.InsertOneAsync(room);
        }

        public async Task UpdateAsync(string id, RoomRequest request)
        {
            var room = await GetByIdAsync(id);
            if (room == null)
                throw new ArgumentException("Room not found.");

            var floor = await _floorCollection.Find(f => f.Id == request.FloorId).FirstOrDefaultAsync();
            if (floor == null)
                throw new ArgumentException("Floor does not exist.");

            room.RoomCode = request.RoomCode;
            room.RoomName = request.RoomName;
            room.Description = request.Description;
            room.IsActive = request.IsActive;
            room.FloorId = request.FloorId;
            room.FloorName = floor.FloorName;

            await _roomCollection.ReplaceOneAsync(r => r.Id == id, room);
        }

        public async Task DeleteAsync(string id)
        {
            var result = await _roomCollection.DeleteOneAsync(r => r.Id == id);
            if (result.DeletedCount == 0)
                throw new ArgumentException("Room not found.");
        }
    }
}
