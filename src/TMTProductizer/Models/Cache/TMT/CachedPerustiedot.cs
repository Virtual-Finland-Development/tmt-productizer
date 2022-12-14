using System.Runtime.Serialization;
using CodeGen.Api.TMT.Model;

namespace TMTProductizer.Models.Cache.TMT;

public class CachedPerustiedot
{
    /// <summary>
    /// Purkka for deserialization
    /// </summary>
    public CachedPerustiedot()
    {
        TyonOtsikko = new List<LokalisoituArvo>();
        TyonKuvaus = new List<LokalisoituArvo>();
        TyoAika = string.Empty;
    }

    public CachedPerustiedot(Perustiedot perustiedot)
    {
        TyonOtsikko = perustiedot.TyonOtsikko;
        TyonKuvaus = perustiedot.TyonKuvaus;
        TyoAika = perustiedot.TyoAika;
    }

    public CachedPerustiedot(CachedPerustiedot perustiedot)
    {
        TyonOtsikko = perustiedot.TyonOtsikko;
        TyonKuvaus = perustiedot.TyonKuvaus;
        TyoAika = perustiedot.TyoAika;
    }


    [DataMember(Name = "tyonOtsikko")]
    public List<LokalisoituArvo> TyonOtsikko { get; set; }

    [DataMember(Name = "tyonKuvaus")]
    public List<LokalisoituArvo> TyonKuvaus { get; set; }

    [DataMember(Name = "tyoAika")]
    public string TyoAika { get; set; }
}