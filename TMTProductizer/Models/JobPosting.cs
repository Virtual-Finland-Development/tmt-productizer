using System.Text.Json.Serialization;

namespace TMTProductizer.Models;

public class Job
{
    public string? Employer { get; set; }
    public Location Location { get; set; }
    public BasicInfo BasicInfo { get; set; }

    [JsonPropertyName("published_at")] 
    public DateTime PublishedAt { get; set; }
}

public class BasicInfo
{
    public string? Title { get; set; }
    public string? Description { get; set; }
}

public class Location
{
    public string City { get; set; }
}