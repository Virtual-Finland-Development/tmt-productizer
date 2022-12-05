using System.Runtime.Serialization;
using CodeGen.Api.TMT.Model;

namespace TMTProductizer.Models.Cache.TMT;

public class CachedPerustiedot
{
    public CachedPerustiedot(List<LokalisoituArvo> tyonOtsikko, List<LokalisoituArvo> tyonKuvaus, string tyoAika)
    {
        TyonOtsikko = tyonOtsikko;
        TyonKuvaus = tyonKuvaus;
        TyoAika = tyoAika;
    }

    [DataMember(Name = "tyonOtsikko")]
    public List<LokalisoituArvo> TyonOtsikko { get; set; }

    [DataMember(Name = "tyonKuvaus")]
    public List<LokalisoituArvo> TyonKuvaus { get; set; }

    [DataMember(Name = "tyoAika")]
    public string TyoAika { get; set; }
}