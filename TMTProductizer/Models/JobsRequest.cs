namespace TMTProductizer.Models;

public class JobsRequest
{
    public string Query { get; set; }
    public LocationQuery LocationQuery { get; set; }
}

public class LocationQuery
{
    public IEnumerable<string> Countries { get; set; }
    public IEnumerable<string> Regions { get; set; }
    public IEnumerable<string> Municipalities { get; set; }
}