using MongoDB.Driver;
using Microsoft.Extensions.Options;
using AssetTrackingAuthAPI.Models;
using AssetTrackingAuthAPI.Config;
using System.Linq.Expressions;

namespace AssetTrackingAuthAPI.Services
{
    public class PurchaseInfoService
    {
        private readonly IMongoCollection<PurchaseInfo> _assetCollection;
        private readonly IMongoCollection<Asset> _asset;

        public PurchaseInfoService(IOptions<MongoDbSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            var database = client.GetDatabase(settings.Value.DatabaseName);
            _assetCollection = database.GetCollection<PurchaseInfo>("PurchaseInfo");
            _asset = database.GetCollection<Asset>("Assets");

        }

        public async Task<List<PurchaseInfo>> GetAllPurchaseInfoAsync() =>
             await _assetCollection.Find(_ => true).ToListAsync();


        public async Task<(bool sucess, string message)> CreateAsycn(PurchaseInfo purchase)
        {
            var exist = await _asset.Find(x => x.PurchaseCode == purchase.PurchaseCode).FirstOrDefaultAsync();
            if (exist == null)
                return (false, "Purchasecode not exists");
            else
                await _assetCollection.InsertOneAsync(purchase);
            return (true, "asset Added Sucessfully");
        }
        public async Task<(bool sucess, String Message)> UpdatePurchaseInfoAsync(PurchaseInfo purchase, String id)
        {
            var Existing = await _assetCollection.Find(a => a.Id == id).FirstOrDefaultAsync();
            if (Existing == null)
                return (false, "Asset Not Found");
            else
                await _assetCollection.ReplaceOneAsync(x => x.Id == id, purchase);
            return (true, "Asset Updated Sucessfully");
        }
        public async Task<(string message,bool sucess)> DeletePurchaseInfo(string id)
        {
            var exist = await _assetCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
            if (exist == null)
                return ("No Asset Found", false);
            else
                await _assetCollection.DeleteOneAsync(x => x.Id == id);
            return ("Purchaseinfo deleted sucessfully", true);    
        }
    }
}