using TMTProductizer.Models;

namespace TMTProductizer.Services.TMT;

public interface ITMTAPIAuthorizationService
{
    Task<AuthorizationPackage> GetAuthorizationPackage();
}
