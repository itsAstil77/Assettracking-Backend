using AssetTrackingAuthAPI.Models;
using AssetTrackingAuthAPI.Config;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace AssetTrackingAuthAPI.Services
{
    public class HierarchyCategoryService
    {
        private readonly IMongoCollection<Group> _groupCollection;
        private readonly IMongoCollection<MainCategory> _mainCategoryCollection;
        private readonly IMongoCollection<SubCategory> _subCategoryCollection;
        private readonly IMongoCollection<SubSubCategory> _subSubCategoryCollection;
        private readonly IMongoCollection<Brand> _brandCollection;
        private readonly IMongoCollection<Model> _modelCollection;

        public HierarchyCategoryService(IOptions<MongoDbSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            var database = client.GetDatabase(settings.Value.DatabaseName);

            _groupCollection = database.GetCollection<Group>("Groups");
            _mainCategoryCollection = database.GetCollection<MainCategory>("MainCategories");
            _subCategoryCollection = database.GetCollection<SubCategory>("SubCategories");
            _subSubCategoryCollection = database.GetCollection<SubSubCategory>("SubSubCategories");
            _brandCollection = database.GetCollection<Brand>("Brands");
            _modelCollection = database.GetCollection<Model>("Models");
        }

        public async Task<List<HierarchyNode>> GetFullCategoryHierarchyAsync()
        {
            var groups = await _groupCollection.Find(_ => true).ToListAsync();
            var mainCategories = await _mainCategoryCollection.Find(_ => true).ToListAsync();
            var subCategories = await _subCategoryCollection.Find(_ => true).ToListAsync();
            var subSubCategories = await _subSubCategoryCollection.Find(_ => true).ToListAsync();
            var brands = await _brandCollection.Find(_ => true).ToListAsync();
            var models = await _modelCollection.Find(_ => true).ToListAsync();

            List<HierarchyNode> hierarchy = new();

            foreach (var group in groups)
            {
                var groupNode = new HierarchyNode
                {
                    Id = group.Id,
                    Name = group.GroupName,
                    Type = "Group"
                };

                var mains = mainCategories.Where(m => m.GroupId == group.Id).ToList();
                foreach (var main in mains)
                {
                    var mainNode = new HierarchyNode
                    {
                        Id = main.Id,
                        Name = main.CategoryName,
                        Type = "MainCategory"
                    };

                    var subs = subCategories.Where(s => s.MainCategoryId == main.Id).ToList();
                    foreach (var sub in subs)
                    {
                        var subNode = new HierarchyNode
                        {
                            Id = sub.Id,
                            Name = sub.CategoryName,
                            Type = "SubCategory"
                        };

                        var subsubs = subSubCategories.Where(ss => ss.SubCategoryId == sub.Id).ToList();
                        foreach (var subsub in subsubs)
                        {
                            var subSubNode = new HierarchyNode
                            {
                                Id = subsub.Id,
                                Name = subsub.CategoryName,
                                Type = "SubSubCategory"
                            };

                            var brandList = brands.Where(b => b.SubSubCategoryId == subsub.Id).ToList();
                            foreach (var brand in brandList)
                            {
                                var brandNode = new HierarchyNode
                                {
                                    Id = brand.Id,
                                    Name = brand.CategoryName,
                                    Type = "Brand"
                                };

                                var modelList = models.Where(mo => mo.BrandId == brand.Id).ToList();
                                foreach (var model in modelList)
                                {
                                    var modelNode = new HierarchyNode
                                    {
                                        Id = model.Id,
                                        Name = model.ModelName,
                                        Type = "Model"
                                    };
                                    brandNode.Children.Add(modelNode);
                                }

                                subSubNode.Children.Add(brandNode);
                            }

                            subNode.Children.Add(subSubNode);
                        }

                        mainNode.Children.Add(subNode);
                    }

                    groupNode.Children.Add(mainNode);
                }

                hierarchy.Add(groupNode);
            }

            return hierarchy;
        }
    }
}
