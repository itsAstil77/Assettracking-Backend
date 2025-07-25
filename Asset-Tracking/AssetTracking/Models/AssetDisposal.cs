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

        public DateTime DisposedDate { get; set; } = DateTime.UtcNow;
        //public string MovedBy { get; set; }
        // public string Lastapprovalworkflow { get; set; }
        public string approvalworkflow { get; set; } // Current role
        public string nextapprovalworkflow { get; set; } // Next approver
        public string lastapprovalworkflow { get; set; } // Previous approver
        public string Disposedby { get; set; }
        public string Remarks { get; set; }
        public string ReferenceNumber { get; set; }
         
        public DateTime DisposalRequestDate { get; set; } = DateTime.UtcNow;




        public string DisposalReason { get; set; }

        public string RequestedBy { get; set; }

        public string status { get; set; }
        public List<Asset> Assets { get; set; } = new List<Asset>();
        public List<ApprovalRecord> ApprovalSummary { get; set; } = new();
    }
   
}
    
