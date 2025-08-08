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
        public string companyName { get; set; }
        public string SiteName{ get; set; }
        public string buildingName{ get; set; }

        public string floorName{ get; set; }
        public string roomName { get; set; }

        public DateTime Duedate { get; set; } = DateTime.UtcNow;
        
        public string remarks{ get; set; }
    }
}
