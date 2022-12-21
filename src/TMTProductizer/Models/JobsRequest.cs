using System.Text.Json;

namespace TMTProductizer.Models;

public class JobsRequest
{
    public string Query { get; set; } = null!;
    public LocationQuery Location { get; set; } = null!;
    public RequirementsQuery Requirements { get; set; } = null!;
    public PagingOptions Paging { get; set; } = null!;

    /// <summary>
    /// Deep copy of the request
    /// </summary>
    public JobsRequest Clone()
    {
        // Note: maybe there would be a dotnet core way to this? Needs to be simple and fast.
        string serialized = JsonSerializer.Serialize(this);
        return JsonSerializer.Deserialize<JobsRequest>(serialized)!;
    }
}

public class LocationQuery
{
    public IEnumerable<string> Countries { get; set; } = null!;
    public IEnumerable<string> Regions { get; set; } = null!;
    public IEnumerable<string> Municipalities { get; set; } = null!;
}

public class RequirementsQuery
{
    public ICollection<string> Occupations { get; set; } = null!;
    public ICollection<string> Skills { get; set; } = null!;
}

public class PagingOptions
{
    public int Limit { get; set; } = 100;
    public int Offset { get; set; } = 0;
}
