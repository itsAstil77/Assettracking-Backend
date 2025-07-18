using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AssetTrackingAuthAPI.Models
{
    public class Floor
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = "";

        public string FloorCode { get; set; } = "";
        public string FloorName { get; set; } = "";
        public string Description { get; set; } = "";
        public bool IsActive { get; set; } = true;

        [BsonRepresentation(BsonType.ObjectId)]
        public string BuildingId { get; set; } = "";
        public string BuildingName { get; set; } = "";
    }

    public class FloorRequest
    {
        public string BuildingId { get; set; } = "";
        public string FloorCode { get; set; } = "";
        public string FloorName { get; set; } = "";
        public string Description { get; set; } = "";
        public bool IsActive { get; set; } = true;

    }
}
