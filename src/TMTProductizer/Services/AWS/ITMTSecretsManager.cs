using TMTProductizer.Models;

namespace TMTProductizer.Services.AWS;

public interface ITMTSecretsManager
{
    Task<TMTSecrets> GetTMTSecrets();
}
