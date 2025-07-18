using AssetTrackingAuthAPI.Config;
using AssetTrackingAuthAPI.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace AssetTrackingAuthAPI.Services
{
    public class ModelService
    {
        private readonly IMongoCollection<Model> _modelCollection;
        private readonly IMongoCollection<Brand> _brandCollection;

        public ModelService(IOptions<MongoDbSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            var database = client.GetDatabase(settings.Value.DatabaseName);

            _modelCollection = database.GetCollection<Model>("Models");
            _brandCollection = database.GetCollection<Brand>("Brands");
        }

        public async Task<List<Model>> GetAllAsync() =>
            await _modelCollection.Find(_ => true).ToListAsync();

        public async Task<Model?> GetByIdAsync(string id) =>
            await _modelCollection.Find(m => m.Id == id).FirstOrDefaultAsync();

        public async Task CreateAsync(ModelRequest request)
        {
            var brand = await _brandCollection.Find(b => b.Id == request.BrandId).FirstOrDefaultAsync();
            if (brand == null)
                throw new ArgumentException("Brand not found");

            var model = new Model
            {
                BrandId = request.BrandId,
                BrandName = brand.CategoryName,
                ModelCode = request.ModelCode,
                ModelName = request.ModelName,
                IsActive = request.IsActive
            };

            await _modelCollection.InsertOneAsync(model);
        }

        public async Task<List<Model>> getmodelbybrand(string id) =>
         await _modelCollection.Find(b => b.BrandId == id).ToListAsync();

        public async Task UpdateAsync(string id, ModelRequest request)
        {
            var existing = await GetByIdAsync(id);
            if (existing == null)
                throw new ArgumentException("Model not found");

            var brand = await _brandCollection.Find(b => b.Id == request.BrandId).FirstOrDefaultAsync();
            if (brand == null)
                throw new ArgumentException("Brand not found");

            existing.BrandId = request.BrandId;
            existing.BrandName = brand.CategoryName;
            existing.ModelCode = request.ModelCode;
            existing.ModelName = request.ModelName;
            existing.IsActive = request.IsActive;

            await _modelCollection.ReplaceOneAsync(m => m.Id == id, existing);
        }

        public async Task DeleteAsync(string id)
        {
            await _modelCollection.DeleteOneAsync(m => m.Id == id);
        }
    }
}
