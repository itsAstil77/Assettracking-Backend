public class AssetMovementReportRequest
{
    // TO location (from AssetMovement)
    public string? ToGroup{ get; set; }
    public string? ToCompany { get; set; }
    public string? ToSite { get; set; }
    public string? ToBuilding { get; set; }
    public string? ToFloor { get; set; }
    public string? ToRoom { get; set; }
    public string? ToDepartment { get; set; }
    public string? ToCustodian { get; set; }

    // FROM location (inside each Asset)
    public string? FromGroup{ get; set; }
    public string? FromCompany { get; set; }
    public string? FromSite { get; set; }
    public string? FromBuilding { get; set; }
    public string? FromFloor { get; set; }
    public string? FromRoom { get; set; }
    public string? FromDepartment { get; set; }
    public string? FromCustodian { get; set; }
}
