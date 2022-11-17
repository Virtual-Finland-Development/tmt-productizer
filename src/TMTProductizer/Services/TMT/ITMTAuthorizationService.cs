using TMTProductizer.Models;

namespace TMTProductizer.Services.TMT;

public interface ITMTAuthorizationService
{
    Task<TMTAuthorizationDetails> GetTMTAuthorizationDetails();
}
