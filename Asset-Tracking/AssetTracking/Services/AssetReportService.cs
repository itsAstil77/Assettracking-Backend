using MongoDB.Bson;
using MongoDB.Driver;
using Microsoft.Extensions.Options;
using AssetTrackingAuthAPI.Models;
using AssetTrackingAuthAPI.Config;

namespace AssetTrackingAuthAPI.Services
{
    public class AssetReportService
    {
        private readonly IMongoCollection<Asset> _assetCollection;

        public AssetReportService(IOptions<MongoDbSettings> dbSettings)
        {
            var client = new MongoClient(dbSettings.Value.ConnectionString);
            var db = client.GetDatabase(dbSettings.Value.DatabaseName);
            _assetCollection = db.GetCollection<Asset>("Assets");
        }

        public async Task<List<Asset>> GetFilteredReportAsync(AssetReportRequest filter)
{
    var builder = Builders<Asset>.Filter;
    var filters = new List<FilterDefinition<Asset>>();

    // Location
    if (filter.GroupName != null && filter.GroupName.Any())
        filters.Add(builder.In(x => x.Group, filter.GroupName));

    if (filter.CompanyName != null && filter.CompanyName.Any())
        filters.Add(builder.In(x => x.CompanyName, filter.CompanyName));

    if (filter.SiteName != null && filter.SiteName.Any())
        filters.Add(builder.In(x => x.SiteName, filter.SiteName));

    if (filter.BuildingName != null && filter.BuildingName.Any())
        filters.Add(builder.In(x => x.BuildingName, filter.BuildingName));

    if (filter.FloorName != null && filter.FloorName.Any())
        filters.Add(builder.In(x => x.FloorName, filter.FloorName));

    if (filter.RoomName != null && filter.RoomName.Any())
        filters.Add(builder.In(x => x.Room, filter.RoomName));

    // Category
    if (filter.MainCategory != null && filter.MainCategory.Any())
        filters.Add(builder.In(x => x.MainCategory, filter.MainCategory));

    if (filter.SubCategory != null && filter.SubCategory.Any())
        filters.Add(builder.In(x => x.SubCategory, filter.SubCategory));

    if (filter.SubSubCategory != null && filter.SubSubCategory.Any())
        filters.Add(builder.In(x => x.SubSubCategory, filter.SubSubCategory));

    // Other fields
    if (filter.Department != null && filter.Department.Any())
        filters.Add(builder.In(x => x.Department, filter.Department));

    if (filter.Custodian != null && filter.Custodian.Any())
        filters.Add(builder.In(x => x.Custodian, filter.Custodian));

    if (filter.AssetCode != null && filter.AssetCode.Any())
        filters.Add(builder.In(x => x.AssetCode, filter.AssetCode));

    var combinedFilter = filters.Count > 0 ? builder.And(filters) : builder.Empty;
    return await _assetCollection.Find(combinedFilter).ToListAsync();
}

    }
}
