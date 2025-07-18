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
            if (!string.IsNullOrEmpty(filter.GroupName))
                filters.Add(builder.Eq(x => x.Group, filter.GroupName));
            if (!string.IsNullOrEmpty(filter.CompanyName))
                filters.Add(builder.Eq(x => x.CompanyName, filter.CompanyName));
            if (!string.IsNullOrEmpty(filter.SiteName))
                filters.Add(builder.Eq(x => x.SiteName, filter.SiteName));
            if (!string.IsNullOrEmpty(filter.BuildingName))
                filters.Add(builder.Eq(x => x.BuildingName, filter.BuildingName));
            if (!string.IsNullOrEmpty(filter.FloorName))
                filters.Add(builder.Eq(x => x.FloorName, filter.FloorName));
            if (!string.IsNullOrEmpty(filter.RoomName))
                filters.Add(builder.Eq(x => x.Room, filter.RoomName));

            // Category
            if (!string.IsNullOrEmpty(filter.MainCategory))
                filters.Add(builder.Eq(x => x.MainCategory, filter.MainCategory));
            if (!string.IsNullOrEmpty(filter.SubCategory))
                filters.Add(builder.Eq(x => x.SubCategory, filter.SubCategory));
            if (!string.IsNullOrEmpty(filter.SubSubCategory))
                filters.Add(builder.Eq(x => x.SubSubCategory, filter.SubSubCategory));

            // Other fields
            if (!string.IsNullOrEmpty(filter.Department))
                filters.Add(builder.Eq(x => x.Department, filter.Department));
            if (!string.IsNullOrEmpty(filter.Custodian))
                filters.Add(builder.Eq(x => x.Custodian, filter.Custodian));
            // if (!string.IsNullOrEmpty(filter.AssetCode))
            //     filters.Add(builder.Eq(x => x.AssetCode, filter.AssetCode));

            var combinedFilter = filters.Count > 0 ? builder.And(filters) : builder.Empty;

            return await _assetCollection.Find(combinedFilter).ToListAsync();
        }
    }
}
