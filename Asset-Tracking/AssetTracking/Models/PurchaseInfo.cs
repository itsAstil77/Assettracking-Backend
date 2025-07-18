using System;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace AssetTrackingAuthAPI.Models
{
    public class PurchaseInfo
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string PurchaseCode{ get; set; }
        public int AssetLife { get; set; }
        public decimal UnitPrice { get; set; }
        public string PONumber { get; set; }
        public string InvoiceNumber { get; set; }
        public string GRNNumber { get; set; }
        public string ReleaseNumber { get; set; }
        public string Supplier { get; set; }

        public DateTime? DeliveryDate { get; set; }
        public DateTime? CapitalizationDate { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public DateTime? PODate { get; set; }
        public DateTime? ServiceStartDate { get; set; }
        public DateTime? ServiceEndDate { get; set; }
        public DateTime? WarrantyStartDate { get; set; }
        public DateTime? WarrantyEndDate { get; set; }
    }
}