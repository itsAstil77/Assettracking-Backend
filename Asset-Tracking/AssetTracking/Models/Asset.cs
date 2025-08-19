using MongoDB.Bson.Serialization.Attributes;

namespace AssetTrackingAuthAPI.Models
{
    public class Asset
    {
        [BsonId]
        public required string Id { get; set; }

        public List<string> GeneratedAssetCodes { get; set; } = new List<string>();

        public required string AssetCode { get; set; }
        public required string CompanyName { get; set; }
        public required string SiteName { get; set; }
        public required string BuildingName { get; set; }
        public required string FloorName { get; set; }
        public required string Room { get; set; }
        public required string Department { get; set; }
        public required string Custodian { get; set; }
        public required string Sector { get; set; }
        //public required string Group { get; set; }

        public required string MainCategory { get; set; }
        public required string SubCategory { get; set; }
        public required string SubSubCategory { get; set; }
        public required string Group { get; set; }

        public required string Brand { get; set; }
        public required string Model { get; set; }

        public required string AssetDescription { get; set; }
        public required string ReferenceCode { get; set; }
        public int Quantity { get; set; }
        public required string AssetStatus { get; set; }
        public required string AssetCondition { get; set; }
        public required string AssetType { get; set; }
        public string PurchaseCode { get; set; }

        
        //public List<PurchaseInfo> PurchaseInfo { get; set; } = new List<PurchaseInfo>();
    }
}
