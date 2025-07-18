using AssetTrackingAuthAPI.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace YourNamespace.Models
{
    public class AssetMovement
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

        public DateTime MovedDate { get; set; } = DateTime.UtcNow;
        public string MovedBy { get; set; }
         public string approvalworkflow { get; set; } // Current role
public string nextapprovalworkflow { get; set; } // Next approver
public string lastapprovalworkflow { get; set; }
        public string status { get; set; }
        public List<Asset> Assets { get; set; } = new List<Asset>();
    }
}
