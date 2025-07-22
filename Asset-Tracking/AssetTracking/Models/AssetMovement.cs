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
        public string Group { get; set; }

        public string Company { get; set; }

        public string Site { get; set; }

        public string Building { get; set; }

        public string Floor { get; set; }

        public string Room { get; set; }

        public string Department { get; set; }

        public string Custodian { get; set; }

        public DateTime RequestDate { get; set; } = DateTime.UtcNow;

        public string ReferenceNumber { get; set; } = string.Empty;


        public DateTime MovedDate { get; set; } = DateTime.UtcNow;
        public string MovedBy { get; set; }
        public string approvalworkflow { get; set; } // Current role
        public string nextapprovalworkflow { get; set; } // Next approver
        public string lastapprovalworkflow { get; set; }
        public string status { get; set; }
        public string? ImageBase64 { get; set; }

        public string Remarks { get; set; }

        public List<Asset> Assets { get; set; } = new List<Asset>();
         public List<ApprovalRecord> ApprovalSummary { get; set; } = new();
    }
    public class ApprovalRecord
{
    public string Status { get; set; } = string.Empty; // e.g., Created, Approved
    public string? RequestApprovedBy { get; set; } // Approver name or ID
    public DateTime? RequestApprovedDate { get; set; }
    public string? Remarks { get; set; }
}
}
