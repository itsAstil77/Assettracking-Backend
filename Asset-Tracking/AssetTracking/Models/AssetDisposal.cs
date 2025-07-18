using AssetTrackingAuthAPI.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace YourNamespace.Models
{
    public class AssetDisposal
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        // [BsonRepresentation(BsonType.ObjectId)]
        public string Assetcode { get; set; }

        public DateTime MovedDate { get; set; } = DateTime.UtcNow;
        //public string MovedBy { get; set; }
        // public string Lastapprovalworkflow { get; set; }
        public string approvalworkflow { get; set; } // Current role
        public string nextapprovalworkflow { get; set; } // Next approver
        public string lastapprovalworkflow { get; set; } // Previous approver


        public string DisposalReason { get; set; }

        public string RequestedBy { get; set; }

        public string status { get; set; }
        public List<Asset> Assets { get; set; } = new List<Asset>();
    }
}
