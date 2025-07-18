using AssetTrackingAuthAPI.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace YourNamespace.Models
{
    public class AssetCheckin
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string oldCustodian{ get; set; }

        // [BsonRepresentation(BsonType.ObjectId)]
        public List<Asset> Assets { get; set; } = new List<Asset>();

        public string newcustodian{ get; set; }
        public string department{ get; set; }
        public string company { get; set; }
        public string site{ get; set; }
        public string building{ get; set; }

        public string floor{ get; set; }
        public string room { get; set; }

        public DateTime Duedate { get; set; } = DateTime.UtcNow;
        
        public string remarks{ get; set; }
    }
}
