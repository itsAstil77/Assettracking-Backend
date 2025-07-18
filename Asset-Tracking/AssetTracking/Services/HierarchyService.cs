using AssetTrackingAuthAPI.Config;
using AssetTrackingAuthAPI.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace AssetTrackingAuthAPI.Services
{
    public class HierarchyService
    {
        private readonly IMongoCollection<Group> _groups;
        private readonly IMongoCollection<Company> _companies;
        private readonly IMongoCollection<Site> _sites;
        private readonly IMongoCollection<Building> _buildings;
        private readonly IMongoCollection<Floor> _floors;
        private readonly IMongoCollection<Room> _rooms;

        public HierarchyService(IOptions<MongoDbSettings> dbSettings)
        {
            var client = new MongoClient(dbSettings.Value.ConnectionString);
            var db = client.GetDatabase(dbSettings.Value.DatabaseName);

            _groups = db.GetCollection<Group>("Groups");
            _companies = db.GetCollection<Company>("Companies");
            _sites = db.GetCollection<Site>("Sites");
            _buildings = db.GetCollection<Building>("Buildings");
            _floors = db.GetCollection<Floor>("Floors");
            _rooms = db.GetCollection<Room>("Rooms");
        }

        public async Task<List<HierarchyItem>> GetHierarchyAsync()
        {
            var hierarchy = new List<HierarchyItem>();

            var groups = await _groups.Find(_ => true).ToListAsync();
            foreach (var group in groups)
            {
                hierarchy.Add(new HierarchyItem
                {
                    Level = "Group",
                    Name = group.GroupName,
                    Parent = null
                });

                var companies = await _companies.Find(c => c.GroupId == group.Id).ToListAsync();
                foreach (var company in companies)
                {
                    hierarchy.Add(new HierarchyItem
                    {
                        Level = "Company",
                        Name = company.CompanyName,
                        Parent = group.GroupName
                    });

                    var sites = await _sites.Find(s => s.CompanyId == company.Id).ToListAsync();
                    foreach (var site in sites)
                    {
                        hierarchy.Add(new HierarchyItem
                        {
                            Level = "Site",
                            Name = site.SiteName,
                            Parent = company.CompanyName
                        });

                        var buildings = await _buildings.Find(b => b.SiteId == site.Id).ToListAsync();
                        foreach (var building in buildings)
                        {
                            hierarchy.Add(new HierarchyItem
                            {
                                Level = "Building",
                                Name = building.BuildingName,
                                Parent = site.SiteName
                            });

                            var floors = await _floors.Find(f => f.BuildingId == building.Id).ToListAsync();
                            foreach (var floor in floors)
                            {
                                hierarchy.Add(new HierarchyItem
                                {
                                    Level = "Floor",
                                    Name = floor.FloorName,
                                    Parent = building.BuildingName
                                });

                                var rooms = await _rooms.Find(r => r.FloorId == floor.Id).ToListAsync();
                                foreach (var room in rooms)
                                {
                                    hierarchy.Add(new HierarchyItem
                                    {
                                        Level = "Room",
                                        Name = room.RoomName,
                                        Parent = floor.FloorName
                                    });
                                }
                            }
                        }
                    }
                }
            }

            return hierarchy;
        }
    }
}
