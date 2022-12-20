

using System.Text.Json.Serialization;

namespace TMTProductizer.Models.Repositories;

public class OccupationCodeSet
{
    public class Occupation
    {
        [JsonPropertyName("notation")]
        public string? Notation { get; set; }

        [JsonPropertyName("uri")]
        public string? Uri { get; set; }

        [JsonPropertyName("prefLabel")]
        public LanguageTranslations? Name { get; set; }

        [JsonPropertyName("broader")]
        public List<string>? Broader { get; set; }
    }

    public class LanguageTranslations
    {
        [JsonPropertyName("fi")]
        public string? Finland { get; set; }
        [JsonPropertyName("sv")]
        public string? Swedish { get; set; }
        [JsonPropertyName("en")]
        public string? English { get; set; }
    }
}