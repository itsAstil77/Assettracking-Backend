using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AssetTrackingAuthAPI.Models
{
    public class Brand
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = "";

        public string SubSubCategoryId { get; set; } = "";
        public string SubSubCategoryName { get; set; } = "";
        public string ParentTypeName { get; set; } = "SubSubCategory";
        public string CategoryCode { get; set; } = "";
        public string CategoryName { get; set; } = "";
        public bool IsActive { get; set; } = true;
    }

    public class BrandRequest
    {
        public string SubSubCategoryId { get; set; } = "";
        public string CategoryCode { get; set; } = "";
        public string CategoryName { get; set; } = "";
        public bool IsActive { get; set; } = true;
    }
}
