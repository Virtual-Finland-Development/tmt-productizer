using System.Runtime.Serialization;
using CodeGen.Api.TMT.Model;

namespace TMTProductizer.Models.Cache.TMT;

public class CachedOsaamisvaatimukset
{
    /// <summary>
    /// Purkka for deserialization
    /// </summary>
    public CachedOsaamisvaatimukset()
    {

    }

    public CachedOsaamisvaatimukset(Osaamisvaatimukset osaamisvaatimukset)
    {
        Ammatit = osaamisvaatimukset.Ammatit;
        Osaamiset = osaamisvaatimukset.Osaamiset;
    }

    public CachedOsaamisvaatimukset(CachedOsaamisvaatimukset osaamisvaatimukset)
    {
        Ammatit = osaamisvaatimukset.Ammatit;
        Osaamiset = osaamisvaatimukset.Osaamiset;
    }


    [DataMember(Name = "ammatit")]
    public List<LuokiteltuArvo> Ammatit { get; set; } = new List<LuokiteltuArvo>();

    [DataMember(Name = "osaamiset")]
    public List<LuokiteltuArvo> Osaamiset { get; set; } = new List<LuokiteltuArvo>();
}