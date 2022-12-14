using System.Runtime.Serialization;
using CodeGen.Api.TMT.Model;

namespace TMTProductizer.Models.Cache.TMT;

public class CachedToimipaikka
{
    /// <summary>
    /// Purkka for deserialization
    /// </summary>
    public CachedToimipaikka()
    {

    }

    public CachedToimipaikka(Toimipaikka toimipaikka)
    {
        Postitoimipaikka = toimipaikka.Postitoimipaikka;
        Postinumero = toimipaikka.Postinumero;
    }

    public CachedToimipaikka(CachedToimipaikka toimipaikka)
    {
        Postitoimipaikka = toimipaikka.Postitoimipaikka;
        Postinumero = toimipaikka.Postinumero;
    }

    [DataMember(Name = "postitoimipaikka")]
    public string Postitoimipaikka { get; set; } = string.Empty;

    [DataMember(Name = "postinumero")]
    public string Postinumero { get; set; } = string.Empty;
}