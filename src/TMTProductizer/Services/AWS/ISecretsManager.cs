using TMTProductizer.Models;

namespace TMTProductizer.Services.AWS;

public interface ISecretsManager
{
    Task<TMTSecrets> GetTMTSecrets();
}
