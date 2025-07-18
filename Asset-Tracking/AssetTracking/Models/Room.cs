using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AssetTrackingAuthAPI.Models
{
    public class Room
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = "";

        public string RoomCode { get; set; } = "";
        public string RoomName { get; set; } = "";
        public string Description { get; set; } = "";
        public bool IsActive { get; set; } = true;

        [BsonRepresentation(BsonType.ObjectId)]
        public string FloorId { get; set; } = "";
        public string FloorName { get; set; } = "";
    }

    public class RoomRequest
    {
        public string FloorId { get; set; } = "";
        
        public string RoomCode { get; set; } = "";
        public string RoomName { get; set; } = "";
        public string Description { get; set; } = "";
        public bool IsActive { get; set; } = true;

    }
}
