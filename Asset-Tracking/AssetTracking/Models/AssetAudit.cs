using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace YourNamespace.Models
{
    public class AssetAudit
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string AuditCode { get; set; }
        public string AuditName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool Active { get; set; }

        public List<Group> Groupname { get; set; } = new List<Group>(); // Array of location IDs or names
    }
    public class Group
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string GroupId { get; set; } = ObjectId.GenerateNewId().ToString();

    [BsonElement("GroupName")]
    public string GroupName { get; set; } = string.Empty;

    [BsonElement("Locations")]
    public List<Location> Locations { get; set; } = new List<Location>();
}

public class Location
{
    public string LocationId { get; set; } = string.Empty;
    public string LocationName { get; set; } = string.Empty;
    public List<Site> Sites { get; set; } = new List<Site>();
}

public class Site
{
    public string SiteId { get; set; } = string.Empty;
    public string SiteName { get; set; } = string.Empty;
    public List<Building> Buildings { get; set; } = new List<Building>();
}

public class Building
{
    public string BuildingId { get; set; } = string.Empty;
    public string BuildingName { get; set; } = string.Empty;
    public List<Floor> Floors { get; set; } = new List<Floor>();
}

public class Floor
{
    public string FloorId { get; set; } = string.Empty;
    public string FloorName { get; set; } = string.Empty;
    public List<Room> Rooms { get; set; } = new List<Room>();
}

public class Room
{
    public string RoomId { get; set; } = string.Empty;
    public string RoomName { get; set; } = string.Empty;
}

}
