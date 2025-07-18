using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AssetTrackingAuthAPI.Models
{
    public class Model
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = "";

        public string BrandId { get; set; } = "";
        public string BrandName { get; set; } = "";
        public string ParentTypeName { get; set; } = "Brand";

        public string ModelCode { get; set; } = "";
        public string ModelName { get; set; } = "";
        public bool IsActive { get; set; } = true;
    }

    public class ModelRequest
    {
        public string BrandId { get; set; } = "";
        public string ModelCode { get; set; } = "";
        public string ModelName { get; set; } = "";
        public bool IsActive { get; set; } = true;
    }
}
