using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AssetTrackingAuthAPI.Models
{
    public class ModuleAccess
    {
        public string ModuleName { get; set; } = string.Empty;
        public bool IsAllowed { get; set; }
        public bool CanView { get; set; }
        public bool CanAddUser { get; set; }
    }

    public class RoleAccess
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }  // Automatically generated if not set

        public string RoleName { get; set; } = string.Empty;
        public List<ModuleAccess> AccessList { get; set; } = new();
    }
}
