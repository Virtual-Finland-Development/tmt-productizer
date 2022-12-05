using System.Runtime.Serialization;

namespace TMTProductizer.Models.Cache.TMT;

public class CachedHakeminen
{
    public CachedHakeminen(string hakemuksenUrl, DateTime hakuaikaPaattyy)
    {
        HakemuksenUrl = hakemuksenUrl;
        HakuaikaPaattyy = hakuaikaPaattyy;
    }

    [DataMember(Name = "hakemuksenUrl")]
    public string HakemuksenUrl { get; set; }

    [DataMember(Name = "hakuaikaPaattyy")]
    public DateTime HakuaikaPaattyy { get; set; }
}