using AssetTrackingAuthAPI.Config;
using AssetTrackingAuthAPI.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace AssetTrackingAuthAPI.Services
{
    public class GroupService
    {
        private readonly IMongoCollection<Group> _groupCollection;
        private readonly IMongoCollection<Company> _company;

        public GroupService(IOptions<MongoDbSettings> dbSettings)
        {
            var mongoClient = new MongoClient(dbSettings.Value.ConnectionString);
            var database = mongoClient.GetDatabase(dbSettings.Value.DatabaseName);
            _groupCollection = database.GetCollection<Group>("Groups");
            _company = database.GetCollection<Company>("Companies");
        }

        public async Task<List<Group>> GetAllAsync() =>
            await _groupCollection.Find(_ => true).ToListAsync();

        public async Task<Group?> GetByIdAsync(string id) =>
            await _groupCollection.Find(g => g.Id == id).FirstOrDefaultAsync();

        public async Task CreateAsync(GroupRequest request)
        {
            var group = new Group
            {
                GroupCode = request.GroupCode,
                GroupName = request.GroupName,
                IsActive = request.IsActive
            };

            await _groupCollection.InsertOneAsync(group);
        }

        public async Task UpdateAsync(string id, GroupRequest request)
        {
            var group = await GetByIdAsync(id);
            if (group == null)
                throw new ArgumentException("Group not found");

            group.GroupCode = request.GroupCode;
            group.GroupName = request.GroupName;
            group.IsActive = request.IsActive;

            await _groupCollection.ReplaceOneAsync(g => g.Id == id, group);
        }

        public async Task<string> DeleteAsync(string id)
{
    // Check if any company exists under this group
    var hasCompany = await _company.Find(x => x.GroupId == id).AnyAsync();

    if (hasCompany)
        return "Group has active company so deletion of group is not allowed";

    await _groupCollection.DeleteOneAsync(g => g.Id == id);
    return "Deleted successfully";
}

    }
}
