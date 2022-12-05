using System.Runtime.Serialization;
using CodeGen.Api.TMT.Model;

namespace TMTProductizer.Models.Cache.TMT;

public class CachedTyopaikkailmoitus
{
    /// <summary>
    /// Purkka for deserialization
    /// </summary>
    public CachedTyopaikkailmoitus()
    {

    }

    public CachedTyopaikkailmoitus(Tyopaikkailmoitus tyopaikkailmoitus)
    {
        IlmoituksenID = tyopaikkailmoitus.IlmoituksenID;
        Perustiedot = new CachedPerustiedot(tyopaikkailmoitus.Perustiedot);
        Sijainti = new CachedSijainti(tyopaikkailmoitus.Sijainti);
        IlmoittajanNimi = tyopaikkailmoitus.IlmoittajanNimi;
        Julkaisupvm = tyopaikkailmoitus.Julkaisupvm;
        Hakeminen = new CachedHakeminen(tyopaikkailmoitus.Hakeminen);
    }

    public CachedTyopaikkailmoitus(CachedTyopaikkailmoitus tyopaikkailmoitus)
    {
        IlmoituksenID = tyopaikkailmoitus.IlmoituksenID;
        Perustiedot = tyopaikkailmoitus.Perustiedot;
        Sijainti = tyopaikkailmoitus.Sijainti;
        IlmoittajanNimi = tyopaikkailmoitus.IlmoittajanNimi;
        Julkaisupvm = tyopaikkailmoitus.Julkaisupvm;
        Hakeminen = tyopaikkailmoitus.Hakeminen;
    }

    [DataMember(Name = "ilmoituksenID")]
    public string IlmoituksenID { get; set; }

    [DataMember(Name = "perustiedot")]
    public CachedPerustiedot Perustiedot { get; set; }

    [DataMember(Name = "sijainti")]
    public CachedSijainti Sijainti { get; set; }

    [DataMember(Name = "ilmoittajanNimi")]
    public List<LokalisoituArvo> IlmoittajanNimi { get; set; }

    [DataMember(Name = "julkaisupvm")]
    public DateTime Julkaisupvm { get; set; }

    [DataMember(Name = "hakeminen")]
    public CachedHakeminen Hakeminen { get; set; }
}