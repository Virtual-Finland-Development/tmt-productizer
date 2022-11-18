namespace TMTProductizer.Models;

public class Job
{
    public string Employer { get; set; } = string.Empty;
    public Location Location { get; set; } = null!;
    public BasicInfo BasicInfo { get; set; } = null!;
    public DateTime PublishedAt { get; set; }
    public DateTime ApplicationEndDate { get; set; }
    public string? ApplicationUrl { get; set; }
}

public class BasicInfo
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string WorkTimeType { get; set; } = null!;
}

public class Location
{
    public string Municipality { get; set; } = string.Empty;
    public string Postcode { get; set; } = null!;
}
