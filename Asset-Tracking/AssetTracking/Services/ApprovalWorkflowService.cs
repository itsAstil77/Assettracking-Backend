using AssetTrackingAuthAPI.Models;
using AssetTrackingAuthAPI.Controllers;
using MongoDB.Bson;
using MongoDB.Driver;
using AssetTrackingAuthAPI.Config;
namespace AssetTrackingAuthAPI.Services
{
    public class ApprovalWorkflowService
    {
        private readonly IMongoCollection<ApprovalWorkflow> _Approval;

        public ApprovalWorkflowService(IMongoDatabase database)
        {
            _Approval = database.GetCollection<ApprovalWorkflow>("ApprovalWorkflow");
        }

        public async Task<List<ApprovalWorkflow>> GetAllWorkflowAsync() =>

            await _Approval.Find(_ => true).ToListAsync();

        public async Task<string> Createworkflow(ApprovalWorkflow app)
        {app.Id = ObjectId.GenerateNewId().ToString();


            await _Approval.InsertOneAsync(app);
            return ("inserted sucessfully");


        }
        public async Task<(bool sucess, string message)> updateworkflowAsync(string id, ApprovalWorkflow app)
        {
            var check = await _Approval.Find(x => x.Id == app.Id).FirstOrDefaultAsync();
            if (check == null)
                return (false, "No Workflow found ");
            else
                await _Approval.ReplaceOneAsync(y => y.Id == app.Id, app);
            return (true, "Workflow Updated Sucessfully");
        }

        public async Task<(bool sucess, string message)> deleteworkflow(string id)
        {
            var del = await _Approval.Find(x => x.Id == id).FirstOrDefaultAsync();
            if (del == null)
                return (false, "No workflow found to delete");

            else
                await _Approval.DeleteOneAsync(x => x.Id == id);
            return (true, "Workflow deleted sucessfully");    
        }




    }
}
