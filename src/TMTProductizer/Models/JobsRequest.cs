namespace TMTProductizer.Models;

public class JobsRequest
{
    public string Query { get; set; } = null!;
    public LocationQuery Location { get; set; } = null!;
    public RequirementsQuery Requirements { get; set; } = null!;
    public PagingOptions Paging { get; set; } = null!;
}

public class LocationQuery
{
    public IEnumerable<string> Countries { get; set; } = null!;
    public IEnumerable<string> Regions { get; set; } = null!;
    public IEnumerable<string> Municipalities { get; set; } = null!;
}

public class RequirementsQuery
{
    public IEnumerable<string> Occupations { get; set; } = null!;
    public IEnumerable<string> Skills { get; set; } = null!;
}

public class PagingOptions
{
    public int Limit { get; set; } = 100;
    public int Offset { get; set; } = 0;
}
