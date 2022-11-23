namespace TMTProductizer.Services.AWS;

public interface ISecretsManager
{
    Task<T> GetSecrets<T>(string secretsName, string secretsRegion);
}
