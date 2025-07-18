using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AssetTrackingAuthAPI.Models
{
    public class Building
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = "";
        
        
        public string BuildingCode { get; set; } = "";
        public string BuildingName { get; set; } = "";
        public string Description { get; set; } = "";
        public bool IsActive { get; set; } = true;

        [BsonRepresentation(BsonType.ObjectId)]
        public string SiteId { get; set; } = "";

        // Optional: for convenience
        public string SiteName { get; set; } = "";
    }

    public class BuildingRequest
    {
        public string SiteId { get; set; } = ""; 
        public string BuildingCode { get; set; } = "";
        public string BuildingName { get; set; } = "";
        public string Description { get; set; } = "";
        public bool IsActive { get; set; } = true;

    }
}
