using System.Runtime.Serialization;
using CodeGen.Api.TMT.Model;

namespace TMTProductizer.Models.Cache.TMT;

public class CachedSijainti
{
    /// <summary>
    /// Purkka for deserialization
    /// </summary>
    public CachedSijainti()
    {
        Kunta = new List<string>();
        Maa = new List<string>();
        Maakunta = new List<string>();
        Toimipaikka = new CachedToimipaikka();
    }

    public CachedSijainti(Sijainti sijainti)
    {
        Kunta = sijainti.Kunta;
        Maa = sijainti.Maa;
        Maakunta = sijainti.Maakunta;
        Toimipaikka = new CachedToimipaikka(sijainti.Toimipaikka);
    }

    public CachedSijainti(CachedSijainti sijainti)
    {
        Kunta = sijainti.Kunta;
        Maa = sijainti.Maa;
        Maakunta = sijainti.Maakunta;
        Toimipaikka = sijainti.Toimipaikka;
    }

    [DataMember(Name = "kunta")]
    public List<string> Kunta { get; set; }

    [DataMember(Name = "maa")]
    public List<string> Maa { get; set; }

    [DataMember(Name = "maakunta")]
    public List<string> Maakunta { get; set; }

    [DataMember(Name = "toimipaikka", EmitDefaultValue = false)]
    public CachedToimipaikka Toimipaikka { get; set; }
}