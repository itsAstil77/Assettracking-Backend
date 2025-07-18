using AssetTrackingAuthAPI.Config;
using AssetTrackingAuthAPI.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace AssetTrackingAuthAPI.Services
{
    public class SubCategoryService
    {
        private readonly IMongoCollection<SubCategory> _subCategoryCollection;
        private readonly IMongoCollection<MainCategory> _mainCategoryCollection;
        private readonly IMongoCollection<SubSubCategory> _subsub;

        public SubCategoryService(IOptions<MongoDbSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            var db = client.GetDatabase(settings.Value.DatabaseName);

            _subCategoryCollection = db.GetCollection<SubCategory>("SubCategories");
            _mainCategoryCollection = db.GetCollection<MainCategory>("MainCategories");
            _subsub = db.GetCollection<SubSubCategory>("SubSubCategories");
        }

        public async Task<List<SubCategory>> GetAllAsync() =>
            await _subCategoryCollection.Find(_ => true).ToListAsync();

        public async Task<SubCategory?> GetByIdAsync(string id) =>
            await _subCategoryCollection.Find(sc => sc.Id == id).FirstOrDefaultAsync();

        public async Task CreateAsync(SubCategoryRequest request)
        {
            var mainCategory = await _mainCategoryCollection.Find(mc => mc.Id == request.MainCategoryId).FirstOrDefaultAsync();
            if (mainCategory == null)
                throw new ArgumentException("Main Category not found");

            var category = new SubCategory
            {
                MainCategoryId = request.MainCategoryId,
                MainCategoryName = mainCategory.CategoryName,
                ParentTypeName = "MainCategory",
                CategoryCode = request.CategoryCode,
                CategoryName = request.CategoryName,
                IsActive = request.IsActive
            };

            await _subCategoryCollection.InsertOneAsync(category);
        }



        public async Task<List<SubCategory>> Getsubcategorybymaincategory(string id)=>
            await _subCategoryCollection.Find(x => x.MainCategoryId == id).ToListAsync();
         
        public async Task UpdateAsync(string id, SubCategoryRequest request)
        {
            var sub = await GetByIdAsync(id);
            if (sub == null)
                throw new ArgumentException("Sub Category not found");

            var mainCategory = await _mainCategoryCollection.Find(mc => mc.Id == request.MainCategoryId).FirstOrDefaultAsync();
            if (mainCategory == null)
                throw new ArgumentException("Main Category not found");

            sub.MainCategoryId = request.MainCategoryId;
            sub.MainCategoryName = mainCategory.CategoryName;
            sub.CategoryCode = request.CategoryCode;
            sub.CategoryName = request.CategoryName;
            sub.IsActive = request.IsActive;

            await _subCategoryCollection.ReplaceOneAsync(sc => sc.Id == id, sub);
        }

        public async Task<string> DeleteAsync(string id)
        {
            var sub = await _subsub.Find(c => c.SubCategoryId == id).AnyAsync();
            if (sub)
                return "subcategory has active sucsubcategory so deletion cant be allowed";
            await _subCategoryCollection.DeleteOneAsync(sc => sc.Id == id);
            return "subcategory deleted sucessfully";
        }
    }
}
