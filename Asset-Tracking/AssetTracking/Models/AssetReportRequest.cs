public class AssetReportRequest
{
    // Location hierarchy (optional)
    public List<string>? GroupName { get; set; }
    public List<string>? CompanyName { get; set; }
    public List<string>? SiteName { get; set; }
    public List<string>? BuildingName { get; set; }
    public List<string>? FloorName { get; set; }
    public List<string>? RoomName { get; set; }

    // Category hierarchy
    public List<string>? MainCategory { get; set; }
    public List<string>? SubCategory { get; set; }
    public List<string>? SubSubCategory { get; set; }

    // Other filters
    public List<string>? Department { get; set; }
    public List<string>? Custodian { get; set; }

    public List<string>? AssetCode { get; set; }
}
