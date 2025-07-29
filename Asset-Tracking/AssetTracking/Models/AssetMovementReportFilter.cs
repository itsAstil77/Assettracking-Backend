public class AssetMovementReportRequest
{
    // TO location
    public List<string>? ToGroup { get; set; }
    public List<string>? ToCompany { get; set; }
    public List<string>? ToSite { get; set; }
    public List<string>? ToBuilding { get; set; }
    public List<string>? ToFloor { get; set; }
    public List<string>? ToRoom { get; set; }
    public List<string>? ToDepartment { get; set; }
    public List<string>? ToCustodian { get; set; }

    // FROM location (inside each Asset)
    public List<string>? FromGroup { get; set; }
    public List<string>? FromCompany { get; set; }
    public List<string>? FromSite { get; set; }
    public List<string>? FromBuilding { get; set; }
    public List<string>? FromFloor { get; set; }
    public List<string>? FromRoom { get; set; }
    public List<string>? FromDepartment { get; set; }
    public List<string>? FromCustodian { get; set; }
    

    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}
