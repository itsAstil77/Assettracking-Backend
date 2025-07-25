using AssetTrackingAuthAPI.Models;
using Microsoft.AspNetCore.SignalR;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace YourNamespace.Models
{
    public class AssetCheckout
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        // [BsonRepresentation(BsonType.ObjectId)]
        public string Assetcode { get; set; }

        public string Company { get; set; }

        public string Site { get; set; }

        public string Building { get; set; }

        public string Floor { get; set; }

        public string Room { get; set; }

        public string Department { get; set; }

        public string Custodian { get; set; }

        public DateTime Duedate { get; set; } = DateTime.UtcNow;

        public List<Asset> Assets { get; set; } = new List<Asset>();
        

        public string remarks { get; set; }
    }
}
