using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AssetTrackingAuthAPI.Models
{
    public class Department
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public string DepartmentCode { get; set; } = null!;
        public string DepartmentName { get; set; } = null!;
        public string OtherName { get; set; } = null!;
        public string GroupName { get; set; } = null!;
        public bool IsActive { get; set; }
    }
     public class DepartmentRequest
    {
        public string DepartmentCode { get; set; } = null!;
        public string DepartmentName { get; set; } = null!;
        public string OtherName { get; set; } = null!;
        public string GroupName { get; set; } = null!;
        public bool IsActive { get; set; }
    }
}
