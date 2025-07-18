using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using MongoDB.Bson;
 namespace AssetTrackingAuthAPI.Models
 {
    public class ApprovalWorkflow
{
      
        
   [BsonId]
[BsonRepresentation(BsonType.ObjectId)]
public string Id { get; set; }


    public required String Group { get; set; }

    public required String Transaction { get; set; }
    public required String Sequence { get; set; }
    public required String ApprovalDescription { get; set; }

    public required String Role { get; set; }

    public required bool Status { get; set; }

    public required bool EmailNotification { get; set; }


}
 }