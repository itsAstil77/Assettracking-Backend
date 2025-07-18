using AssetTrackingAuthAPI.Models;
using Microsoft.Extensions.Options;
using AssetTrackingAuthAPI.Config;
using MongoDB.Driver;

namespace AssetTrackingAuthAPI.Services
{
    public class SiteService
    {
        private readonly IMongoCollection<Site> _siteCollection;
        private readonly IMongoCollection<Company> _companyCollection;
        private readonly IMongoCollection<Building> _Building;

        public SiteService(IOptions<MongoDbSettings> mongoDbSettings)
        {
            var mongoClient = new MongoClient(mongoDbSettings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(mongoDbSettings.Value.DatabaseName);

            _siteCollection = mongoDatabase.GetCollection<Site>("Sites");
            _companyCollection = mongoDatabase.GetCollection<Company>("Companies");
            _Building = mongoDatabase.GetCollection<Building>("Buildings");
        }

        public async Task<List<Site>> GetAllAsync() =>
            await _siteCollection.Find(_ => true).ToListAsync();

        public async Task<List<Site>> GetAllSitesbycompanyidAsync(string CompanyId)=>
        
            await _siteCollection.Find(c => c.CompanyId == CompanyId).ToListAsync();
        

        public async Task<Site?> GetByIdAsync(string id) =>
            await _siteCollection.Find(s => s.Id == id).FirstOrDefaultAsync();

        public async Task CreateAsync(SiteRequest request)
        {
            var company = await _companyCollection.Find(c => c.Id == request.CompanyId).FirstOrDefaultAsync();
            if (company == null)
                throw new ArgumentException($"Company with Id '{request.CompanyId}' does not exist.");

            var site = new Site
            {
                CompanyId = company.Id,
                CompanyName = company.CompanyName,
                SiteCode = request.SiteCode,
                SiteName = request.SiteName,
                Description = request.Description,
                IsActive = request.IsActive
            };

            await _siteCollection.InsertOneAsync(site);
        }

        public async Task UpdateAsync(string id, SiteRequest request)
        {
            var site = await GetByIdAsync(id);
            if (site == null)
                throw new ArgumentException("Site not found");

            var company = await _companyCollection.Find(c => c.Id == request.CompanyId).FirstOrDefaultAsync();
            if (company == null)
                throw new ArgumentException($"Company with Id '{request.CompanyId}' does not exist.");

            site.CompanyId = company.Id;
            site.CompanyName = company.CompanyName;
            site.SiteCode = request.SiteCode;
            site.SiteName = request.SiteName;
            site.Description = request.Description;
            site.IsActive = request.IsActive;

            await _siteCollection.ReplaceOneAsync(s => s.Id == id, site);
        }

        public async Task<string> DeleteAsync(string id)
        {
            var site = await _Building.Find(c => c.SiteId == id).AnyAsync();
            if (site)
                return "site has active buildings so deletion not allowed";
            var result = await _siteCollection.DeleteOneAsync(s => s.Id == id);
            return "deletion sucessfully";
            if (result.DeletedCount == 0)
                throw new ArgumentException("Site not found");
        }
    }
}
