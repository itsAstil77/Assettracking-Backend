using AssetTrackingAuthAPI.Config;
using AssetTrackingAuthAPI.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace AssetTrackingAuthAPI.Services
{
    public class MainCategoryService
    {
        private readonly IMongoCollection<MainCategory> _mainCategoryCollection;
        private readonly IMongoCollection<Group> _groupCollection;
        private readonly IMongoCollection<SubCategory> _sub;

        public MainCategoryService(IOptions<MongoDbSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            var db = client.GetDatabase(settings.Value.DatabaseName);

            _mainCategoryCollection = db.GetCollection<MainCategory>("MainCategories");
            _groupCollection = db.GetCollection<Group>("Groups");
            _sub = db.GetCollection<SubCategory>("SubCategories");
        }

        public async Task<List<MainCategory>> GetAllAsync() =>
            await _mainCategoryCollection.Find(_ => true).ToListAsync();

        public async Task<MainCategory?> GetByIdAsync(string id) =>
            await _mainCategoryCollection.Find(mc => mc.Id == id).FirstOrDefaultAsync();

        public async Task CreateAsync(MainCategoryRequest request)
        {
            var group = await _groupCollection.Find(g => g.Id == request.GroupId).FirstOrDefaultAsync();
            if (group == null)
                throw new ArgumentException("Group not found");

            var category = new MainCategory
            {
                GroupId = request.GroupId,
                GroupName = group.GroupName,
                ParentTypeName = "Group",
                CategoryCode = request.CategoryCode,
                CategoryName = request.CategoryName,
                IsActive = request.IsActive
            };

            await _mainCategoryCollection.InsertOneAsync(category);
        }

        public async Task UpdateAsync(string id, MainCategoryRequest request)
        {
            var category = await GetByIdAsync(id);
            if (category == null)
                throw new ArgumentException("Main category not found");

            var group = await _groupCollection.Find(g => g.Id == request.GroupId).FirstOrDefaultAsync();
            if (group == null)
                throw new ArgumentException("Group not found");

            category.GroupId = request.GroupId;
            category.GroupName = group.GroupName;
            category.CategoryCode = request.CategoryCode;
            category.CategoryName = request.CategoryName;
            category.IsActive = request.IsActive;

            await _mainCategoryCollection.ReplaceOneAsync(mc => mc.Id == id, category);
        }

        public async Task<string> DeleteAsync(string id)
        {
            var sub = await _sub.Find(c => c.MainCategoryId == id).AnyAsync();
            if (sub)
                return "maincategory has active subcategory so deletion cant be allowed";
            await _mainCategoryCollection.DeleteOneAsync(mc => mc.Id == id);
            return "mainCategory deleted sucessfully";
        }
        public async Task<List<MainCategory>> GetMainCategorybygroup(string id) =>
           await _mainCategoryCollection.Find(x => x.GroupId == id).ToListAsync();    
    }
}
