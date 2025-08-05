using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AssetTrackingAuthAPI.Models
{
    public class MainCategory
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = "";

        public string GroupId { get; set; } = "";
        public string GroupName { get; set; } = "";  
        public string ParentTypeName { get; set; } = "Group"; 
        public string CategoryCode { get; set; } = "";
        public string CategoryName { get; set; } = "";
        public string CategoryType { get; set; } = "MainCategory";
        public bool IsActive { get; set; } = true;
    }

    public class MainCategoryRequest
    {
        public string GroupId { get; set; } = "";
        public string CategoryCode { get; set; } = "";
        public string CategoryName { get; set; } = "";
        public bool IsActive { get; set; } = true;
    }
}
