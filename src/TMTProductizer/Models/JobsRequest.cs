namespace TMTProductizer.Models;

public class JobsRequest
{
    public string Query { get; set; }
    public LocationQuery Location { get; set; }
    public PagingOptions Paging { get; set; }
}

public class LocationQuery
{
    public IEnumerable<string> Countries { get; set; }
    public IEnumerable<string> Regions { get; set; }
    public IEnumerable<string> Municipalities { get; set; }
}

public class PagingOptions
{
    public int Limit { get; set; } = 100;
    public int Offset { get; set; } = 0;
}
