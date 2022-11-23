using TMTProductizer.Models;

namespace TMTProductizer.Services;

public interface IAPIAuthorizationService
{
    Task<APIAuthorizationPackage> GetAPIAuthorizationPackage();
}
