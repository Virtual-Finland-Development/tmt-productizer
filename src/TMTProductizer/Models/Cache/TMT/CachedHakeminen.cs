using System.Runtime.Serialization;
using CodeGen.Api.TMT.Model;

namespace TMTProductizer.Models.Cache.TMT;

public class CachedHakeminen
{
    /// <summary>
    /// Purkka for deserialization
    /// </summary>

    public CachedHakeminen()
    {


    }

    public CachedHakeminen(Hakeminen hakeminen)
    {
        HakemuksenUrl = hakeminen.HakemuksenUrl;
        HakuaikaPaattyy = hakeminen.HakuaikaPaattyy;
    }

    public CachedHakeminen(CachedHakeminen hakeminen)
    {
        HakemuksenUrl = hakeminen.HakemuksenUrl;
        HakuaikaPaattyy = hakeminen.HakuaikaPaattyy;
    }

    [DataMember(Name = "hakemuksenUrl")]
    public string HakemuksenUrl { get; set; } = string.Empty;

    [DataMember(Name = "hakuaikaPaattyy")]
    public DateTime HakuaikaPaattyy { get; set; } = DateTime.MinValue;
}