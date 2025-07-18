using MongoDB.Bson.Serialization.Attributes;

namespace AssetTrackingAuthAPI.Models
{
    public class AssetReportRequest
    {
        // Location hierarchy (optional)
        public string? GroupName { get; set; }
        public string? CompanyName { get; set; }
        public string? SiteName { get; set; }
        public string? BuildingName { get; set; }
        public string? FloorName { get; set; }
        public string? RoomName { get; set; }

        // Category hierarchy
        public string? MainCategory { get; set; }
        public string? SubCategory { get; set; }
        public string? SubSubCategory { get; set; }

        // Other filters
        public string? Department { get; set; }
        public string? Custodian { get; set; }

        public string? AssetCode { get; set; }
    }
}