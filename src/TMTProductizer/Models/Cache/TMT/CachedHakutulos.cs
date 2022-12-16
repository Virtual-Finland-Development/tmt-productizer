using CodeGen.Api.TMT.Model;

namespace TMTProductizer.Models.Cache.TMT;

public class CachedHakutulos
{
    /// <summary>
    /// Purkka for deserialization
    /// </summary>
    public CachedHakutulos()
    {

    }

    public CachedHakutulos(Hakutulos hakutulos)
    {
        Ilmoitukset = hakutulos.Ilmoitukset.Select(ilmoitus => new CachedTyopaikkailmoitus(ilmoitus)).ToList();
        IlmoituksienMaara = hakutulos.IlmoituksienMaara;
    }

    public CachedHakutulos(CachedHakutulos hakutulos)
    {
        IlmoituksienMaara = hakutulos.IlmoituksienMaara;
        Ilmoitukset = hakutulos.Ilmoitukset;
    }

    public long IlmoituksienMaara { get; set; } = 0;
    public List<CachedTyopaikkailmoitus> Ilmoitukset { get; set; } = new List<CachedTyopaikkailmoitus>();
}