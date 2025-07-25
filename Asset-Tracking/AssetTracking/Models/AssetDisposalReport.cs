public class AssetDisposalReportFilterDto
{
    public string? Group { get; set; }
    public string? CompanyName { get; set; }
    public string? SiteName { get; set; }
    public string? BuildingName { get; set; }
    public string? FloorName { get; set; }
    public string? Room { get; set; }

    public string? Custodian { get; set; }
    public string? Department { get; set; }

    public string? MainCategory { get; set; }
    public string? SubCategory { get; set; }
    public string? SubSubCategory { get; set; }

    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}
