using AssetTrackingAuthAPI.Config;
using AssetTrackingAuthAPI.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace AssetTrackingAuthAPI.Services
{
    public class BrandService
    {
        private readonly IMongoCollection<Brand> _collection;
        private readonly IMongoCollection<SubSubCategory> _subSubCategoryCollection;
        private readonly IMongoCollection<Model> _model;

        public BrandService(IOptions<MongoDbSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            var database = client.GetDatabase(settings.Value.DatabaseName);

            _collection = database.GetCollection<Brand>("Brands");
            _subSubCategoryCollection = database.GetCollection<SubSubCategory>("SubSubCategories");
            _model = database.GetCollection<Model>("Models");
        }

        public async Task<List<Brand>> GetAllAsync() =>
            await _collection.Find(_ => true).ToListAsync();

        public async Task<Brand?> GetByIdAsync(string id) =>
            await _collection.Find(s => s.Id == id).FirstOrDefaultAsync();

        public async Task CreateAsync(BrandRequest request)
        {
            var subSub = await _subSubCategoryCollection.Find(s => s.Id == request.SubSubCategoryId).FirstOrDefaultAsync();
            if (subSub == null)
                throw new ArgumentException("SubSubCategory not found");

            var brand = new Brand
            {
                SubSubCategoryId = request.SubSubCategoryId,
                SubSubCategoryName = subSub.CategoryName,
                CategoryCode = request.CategoryCode,
                CategoryName = request.CategoryName,
                IsActive = request.IsActive
            };

            await _collection.InsertOneAsync(brand);
        }

        public async Task<List<Brand>> getbrandbysubsubcategory(string id) =>
         await _collection.Find(x => x.SubSubCategoryId == id).ToListAsync();


        public async Task UpdateAsync(string id, BrandRequest request)
        {
            var existing = await GetByIdAsync(id);
            if (existing == null)
                throw new ArgumentException("Brand not found");

            var subSub = await _subSubCategoryCollection.Find(s => s.Id == request.SubSubCategoryId).FirstOrDefaultAsync();
            if (subSub == null)
                throw new ArgumentException("SubSubCategory not found");

            existing.SubSubCategoryId = request.SubSubCategoryId;
            existing.SubSubCategoryName = subSub.CategoryName;
            existing.CategoryCode = request.CategoryCode;
            existing.CategoryName = request.CategoryName;
            existing.IsActive = request.IsActive;

            await _collection.ReplaceOneAsync(s => s.Id == id, existing);
        }

        public async Task<string> DeleteAsync(string id)
        {
            var model = await _model.Find(c => c.BrandId == id).AnyAsync();
            if (model)
                return "Brand has active Model so deletion cant be allowed";
            await _collection.DeleteOneAsync(s => s.Id == id);
            return "brand deleted sucessfully";
        }
    }
}
