using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AssetTrackingAuthAPI.Models
{
    public class SubSubCategory
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = "";

        public string SubCategoryId { get; set; } = "";
        public string SubCategoryName { get; set; } = "";
        public string ParentTypeName { get; set; } = "SubCategory";
        public string CategoryCode { get; set; } = "";
        public string CategoryName { get; set; } = "";
        public bool IsActive { get; set; } = true;
    }

    public class SubSubCategoryRequest
    {
        public string SubCategoryId { get; set; } = "";
        public string CategoryCode { get; set; } = "";
        public string CategoryName { get; set; } = "";
        public bool IsActive { get; set; } = true;
    }
}
