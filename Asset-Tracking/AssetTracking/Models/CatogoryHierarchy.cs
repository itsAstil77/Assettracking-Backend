namespace AssetTrackingAuthAPI.Models
{
    public class HierarchyNode
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public string Type { get; set; } = "";
        public List<HierarchyNode> Children { get; set; } = new();
    }
}
