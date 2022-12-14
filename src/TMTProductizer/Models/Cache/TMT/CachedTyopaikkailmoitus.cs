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
        Osaamisvaatimukset = new CachedOsaamisvaatimukset(tyopaikkailmoitus.Osaamisvaatimukset);
        Sijainti = new CachedSijainti(tyopaikkailmoitus.Sijainti);
        IlmoittajanNimi = tyopaikkailmoitus.IlmoittajanNimi;
        Julkaisupvm = tyopaikkailmoitus.Julkaisupvm;
        Hakeminen = new CachedHakeminen(tyopaikkailmoitus.Hakeminen);
    }

    public CachedTyopaikkailmoitus(CachedTyopaikkailmoitus tyopaikkailmoitus)
    {
        IlmoituksenID = tyopaikkailmoitus.IlmoituksenID;
        Perustiedot = tyopaikkailmoitus.Perustiedot;
        Osaamisvaatimukset = tyopaikkailmoitus.Osaamisvaatimukset;
        Sijainti = tyopaikkailmoitus.Sijainti;
        IlmoittajanNimi = tyopaikkailmoitus.IlmoittajanNimi;
        Julkaisupvm = tyopaikkailmoitus.Julkaisupvm;
        Hakeminen = tyopaikkailmoitus.Hakeminen;
    }

    [DataMember(Name = "ilmoituksenID")]
    public string IlmoituksenID { get; set; } = string.Empty;

    [DataMember(Name = "perustiedot")]
    public CachedPerustiedot Perustiedot { get; set; } = new CachedPerustiedot();

    [DataMember(Name = "osaamisvaatimukset")]
    public CachedOsaamisvaatimukset Osaamisvaatimukset { get; set; } = new CachedOsaamisvaatimukset();

    [DataMember(Name = "sijainti")]
    public CachedSijainti Sijainti { get; set; } = new CachedSijainti();

    [DataMember(Name = "ilmoittajanNimi")]
    public List<LokalisoituArvo> IlmoittajanNimi { get; set; } = new List<LokalisoituArvo>();

    [DataMember(Name = "julkaisupvm")]
    public DateTime Julkaisupvm { get; set; } = DateTime.MinValue;

    [DataMember(Name = "hakeminen")]
    public CachedHakeminen Hakeminen { get; set; } = new CachedHakeminen();
}