using System.Runtime.Serialization;
using CodeGen.Api.TMT.Model;

namespace TMTProductizer.Models.Cache;

public class CachedTyopaikkailmoitus
{
    public CachedTyopaikkailmoitus(Tyopaikkailmoitus tyopaikkailmoitus)
    {
        Perustiedot = tyopaikkailmoitus.Perustiedot;
        Sijainti = tyopaikkailmoitus.Sijainti;
        IlmoittajanNimi = tyopaikkailmoitus.IlmoittajanNimi;
        Julkaisupvm = tyopaikkailmoitus.Julkaisupvm;
        Hakeminen = tyopaikkailmoitus.Hakeminen;
    }

    /// <summary>
    /// Gets or Sets Perustiedot
    /// </summary>
    [DataMember(Name = "perustiedot")]
    public Perustiedot Perustiedot { get; set; }

    /// <summary>
    /// Gets or Sets Sijainti
    /// </summary>
    [DataMember(Name = "sijainti")]
    public Sijainti Sijainti { get; set; }

    /// <summary>
    /// Gets or Sets Sijainti
    /// </summary>
    [DataMember(Name = "ilmoittajanNimi")]
    public List<LokalisoituArvo> IlmoittajanNimi { get; set; }

    /// <summary>
    /// Gets or Sets Sijainti
    /// </summary>
    [DataMember(Name = "julkaisupvm")]
    public DateTime Julkaisupvm { get; set; }

    /// <summary>
    /// Gets or Sets Sijainti
    /// </summary>
    [DataMember(Name = "hakeminen")]
    public Hakeminen Hakeminen { get; set; }
}