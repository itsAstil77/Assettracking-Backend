using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AssetTrackingAuthAPI.Models
{
    public class Company
    {
        [BsonId, BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = "";

        [BsonRepresentation(BsonType.ObjectId)]
        public string GroupId { get; set; } = "";

        public string GroupName { get; set; } = ""; 

        public string CompanyCode { get; set; } = "";
        public string CompanyName { get; set; } = "";
        public string Description { get; set; } = "";
        public bool IsActive { get; set; } = true;
    }

    public class CompanyRequest
    {
        public string GroupId { get; set; } = ""; 
        public string CompanyCode { get; set; } = "";
        public string CompanyName { get; set; } = "";
        public string Description { get; set; } = "";
        public bool IsActive { get; set; } = true;
    }
}
