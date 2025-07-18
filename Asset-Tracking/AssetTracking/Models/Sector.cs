using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AssetTrackingAuthAPI.Models
{
    public class Sector
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public string SectorCode { get; set; } = string.Empty;
        public string SectorName { get; set; } = string.Empty;
        public string OtherName { get; set; } = string.Empty;
        public string GroupName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
    public class SectorRequest
    {
        public string SectorCode { get; set; } = string.Empty;
        public string SectorName { get; set; } = string.Empty;
        public string OtherName { get; set; } = string.Empty;
        public string GroupName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
