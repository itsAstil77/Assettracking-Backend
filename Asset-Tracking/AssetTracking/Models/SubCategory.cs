using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AssetTrackingAuthAPI.Models
{
    public class SubCategory
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = "";

        public string MainCategoryId { get; set; } = "";
        public string MainCategoryName { get; set; } = "";
        public string ParentTypeName { get; set; } = "MainCategory";
        public string CategoryCode { get; set; } = "";
        public string CategoryName { get; set; } = "";
        public bool IsActive { get; set; } = true;
    }

    public class SubCategoryRequest
    {
        public string MainCategoryId { get; set; } = "";
        public string CategoryCode { get; set; } = "";
        public string CategoryName { get; set; } = "";
        public bool IsActive { get; set; } = true;
    }
}
