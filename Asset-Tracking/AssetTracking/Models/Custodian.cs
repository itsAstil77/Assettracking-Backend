using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AssetTrackingAuthAPI.Models
{
    public class Custodian
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public string CustodianCode { get; set; } = string.Empty;
        public string CustodianName { get; set; } = string.Empty;
        public string CustodianEmail { get; set; } = string.Empty;
        public string OtherName { get; set; } = string.Empty;
        public string DepartmentName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
    public class CustodianRequest
    {
        public string CustodianCode { get; set; } = string.Empty;
        public string CustodianName { get; set; } = string.Empty;
        public string CustodianEmail { get; set; } = string.Empty;
        public string OtherName { get; set; } = string.Empty;
        public string DepartmentName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
