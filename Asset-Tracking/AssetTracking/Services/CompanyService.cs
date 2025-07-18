using AssetTrackingAuthAPI.Config;
using AssetTrackingAuthAPI.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace AssetTrackingAuthAPI.Services
{
    public class CompanyService
    {
        private readonly IMongoCollection<Company> _companyCollection;
        private readonly IMongoCollection<Group> _groupCollection;
        private readonly IMongoCollection<Site> _site;

        public CompanyService(IOptions<MongoDbSettings> dbSettings)
        {
            var client = new MongoClient(dbSettings.Value.ConnectionString);
            var db = client.GetDatabase(dbSettings.Value.DatabaseName);

            _companyCollection = db.GetCollection<Company>("Companies");
            _groupCollection = db.GetCollection<Group>("Groups");
            _site = db.GetCollection<Site>("Sites");
        }
        public async Task<List<Company>> GetCompanyByGroupidIdAsync(string GroupId) =>
            await _companyCollection.Find(c => c.GroupId == GroupId).ToListAsync();

        public async Task<List<Company>> GetAllAsync() =>
            await _companyCollection.Find(_ => true).ToListAsync();

        public async Task<Company?> GetByIdAsync(string id) =>
            await _companyCollection.Find(c => c.Id == id).FirstOrDefaultAsync();

        public async Task CreateAsync(CompanyRequest request)
        {
            var group = await _groupCollection.Find(g => g.Id == request.GroupId).FirstOrDefaultAsync();
            if (group == null)
                throw new ArgumentException($"Group with Id '{request.GroupId}' does not exist.");

            var company = new Company
            {
                GroupId = group.Id,
                GroupName = group.GroupName, // Save group name here for quick reference
                CompanyCode = request.CompanyCode,
                CompanyName = request.CompanyName,
                Description = request.Description,
                IsActive = request.IsActive
            };

            await _companyCollection.InsertOneAsync(company);
        }

        public async Task UpdateAsync(string id, CompanyRequest request)
        {
            var company = await GetByIdAsync(id);
            if (company == null)
                throw new ArgumentException("Company not found");

            var group = await _groupCollection.Find(g => g.Id == request.GroupId).FirstOrDefaultAsync();
            if (group == null)
                throw new ArgumentException($"Group with Id '{request.GroupId}' does not exist.");

            company.GroupId = group.Id;
            company.GroupName = group.GroupName;
            company.CompanyCode = request.CompanyCode;
            company.CompanyName = request.CompanyName;
            company.Description = request.Description;
            company.IsActive = request.IsActive;

            await _companyCollection.ReplaceOneAsync(c => c.Id == id, company);
        }

        public async Task<string> DeleteAsync(string id)
        {
            var result = await _site.Find(c => c.CompanyId == id).AnyAsync();
            if (result)
                return "company has active sites so deletion cant allowed";
            await _companyCollection.DeleteOneAsync(c => c.Id == id);
            return "company deleted sucessfully";    
            
        }
    }
}
