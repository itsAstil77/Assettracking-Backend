using AssetTrackingAuthAPI.Config;
using AssetTrackingAuthAPI.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace AssetTrackingAuthAPI.Services
{
    public class SubSubCategoryService
    {
        private readonly IMongoCollection<SubSubCategory> _collection;
        private readonly IMongoCollection<SubCategory> _subCategoryCollection;
        private readonly IMongoCollection<Brand> _brand;

        public SubSubCategoryService(IOptions<MongoDbSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            var database = client.GetDatabase(settings.Value.DatabaseName);

            _collection = database.GetCollection<SubSubCategory>("SubSubCategories");
            _subCategoryCollection = database.GetCollection<SubCategory>("SubCategories");
            _brand = database.GetCollection<Brand>("Brands");
        }

        public async Task<List<SubSubCategory>> GetAllAsync() =>
            await _collection.Find(_ => true).ToListAsync();

        public async Task<SubSubCategory?> GetByIdAsync(string id) =>
            await _collection.Find(s => s.Id == id).FirstOrDefaultAsync();

        public async Task CreateAsync(SubSubCategoryRequest request)
        {
            var subCategory = await _subCategoryCollection.Find(s => s.Id == request.SubCategoryId).FirstOrDefaultAsync();
            if (subCategory == null)
                throw new ArgumentException("Sub Category not found");

            var category = new SubSubCategory
            {
                SubCategoryId = request.SubCategoryId,
                SubCategoryName = subCategory.CategoryName,
                ParentTypeName = "SubCategory",
                CategoryCode = request.CategoryCode,
                CategoryName = request.CategoryName,
                IsActive = request.IsActive
            };

            await _collection.InsertOneAsync(category);
        }

        public async Task UpdateAsync(string id, SubSubCategoryRequest request)
        {
            var existing = await GetByIdAsync(id);
            if (existing == null)
                throw new ArgumentException("SubSub Category not found");

            var subCategory = await _subCategoryCollection.Find(s => s.Id == request.SubCategoryId).FirstOrDefaultAsync();
            if (subCategory == null)
                throw new ArgumentException("Sub Category not found");

            existing.SubCategoryId = request.SubCategoryId;
            existing.SubCategoryName = subCategory.CategoryName;
            existing.CategoryCode = request.CategoryCode;
            existing.CategoryName = request.CategoryName;
            existing.IsActive = request.IsActive;

            await _collection.ReplaceOneAsync(s => s.Id == id, existing);
        }
        public async Task<List<SubSubCategory>>GetSubsubcategorybysubcategory(string id) =>
         await _collection.Find(x => x.SubCategoryId == id).ToListAsync();

        public async Task<string> DeleteAsync(string id)
        {
            var brand = await _brand.Find(c => c.SubSubCategoryId == id).AnyAsync();
            if (brand)
                return "subsubcategory has active brand so deletion cant be allowed";

            await _collection.DeleteOneAsync(s => s.Id == id);
            return "subsubcategory deleted sucessfully";
        }
    }
}
