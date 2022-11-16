using TMTProductizer.Models;

namespace TMTProductizer.Services.TMT;

public interface ITMT_AuthorizationService
{
    Task<TMTAuthorizationDetails> GetTMTAuthorizationDetails();
}
