using CodeGen.Api.TMT.Model;
namespace TMTProductizer.Models.Cache;

public class CachedHakutulos
{
    public CachedHakutulos(Hakutulos hakutulos)
    {
        this.Ilmoitukset = hakutulos.Ilmoitukset.Select(ilmoitus => new CachedTyopaikkailmoitus(ilmoitus)).ToList();
        this.IlmoituksienMaara = hakutulos.IlmoituksienMaara;
    }

    public long IlmoituksienMaara { get; set; }
    public List<CachedTyopaikkailmoitus> Ilmoitukset { get; set; }
}