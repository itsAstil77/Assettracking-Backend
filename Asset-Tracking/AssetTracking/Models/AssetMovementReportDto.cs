public class AssetMovementReportDto
{
    public string AssetCode { get; set; }

    // From Location
    public string FromGroup { get; set; }
    public string FromCompany { get; set; }
    public string FromSite { get; set; }
    public string FromBuilding { get; set; }
    public string FromFloor { get; set; }
    public string FromRoom { get; set; }
    public string FromDepartment { get; set; }
    public string FromCustodian { get; set; }

    // To Location (from AssetMovement)
    public string ToGroup { get; set; }
    public string ToCompany { get; set; }
    public string ToSite { get; set; }
    public string ToBuilding { get; set; }
    public string ToFloor { get; set; }
    public string ToRoom { get; set; }
    public string ToDepartment { get; set; }
    public string ToCustodian { get; set; }

    public DateTime MovementDate { get; set; }
    public DateTime MovementInitiatedDate { get; set; }
    public string ReferenceNumber { get; set; }
    public string LastApproval{ get; set; }

    // Optional: Add asset metadata
    public string AssetDescription { get; set; }
    public string MainCategory { get; set; }
    public string SubCategory { get; set; }
    public string SubSubCategory { get; set; }
}
