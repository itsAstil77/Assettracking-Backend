using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AssetTrackingAuthAPI.Models
{
    public class Group
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = "";

        public string GroupCode { get; set; } = "";
        public string GroupName { get; set; } = "";
        public bool IsActive { get; set; } = true;
    }

    public class GroupRequest
    {
        public string GroupCode { get; set; } = "";
        public string GroupName { get; set; } = "";
        public bool IsActive { get; set; } = true;
    }
}
