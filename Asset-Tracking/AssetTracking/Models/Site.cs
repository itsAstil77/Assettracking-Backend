using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AssetTrackingAuthAPI.Models
{
    public class Site
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public string CompanyId { get; set; } = null!;
        public string CompanyName { get; set; } = null!;

        public string SiteCode { get; set; } = null!;
        public string SiteName { get; set; } = null!;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
    }

    public class SiteRequest
    {
        public string CompanyId { get; set; } = null!;
        public string SiteCode { get; set; } = null!;
        public string SiteName { get; set; } = null!;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
    }
}
