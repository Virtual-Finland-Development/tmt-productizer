/*
 *	Use this code snippet in your app.
 *	If you need more information about configurations or implementing the sample code, visit the AWS docs:
 *	https://aws.amazon.com/developer/language/net/getting-started
 */

using Amazon;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using System.Net;
using System.Text.Json;


using TMTProductizer.Models;

namespace TMTProductizer.Services.AWS;

public class TMTSecretsManager : ITMTSecretsManager
{
    private readonly string _tmtSecretsName;
    private readonly string _tmtSecretsRegion;

    public TMTSecretsManager(string tmtSecretsName, string tmtSecretsRegion)
    {
        _tmtSecretsName = tmtSecretsName;
        _tmtSecretsRegion = tmtSecretsRegion;
    }

    public async Task<TMTSecrets> GetTMTSecrets()
    {
        IAmazonSecretsManager client = new AmazonSecretsManagerClient(RegionEndpoint.GetBySystemName(_tmtSecretsRegion));

        GetSecretValueRequest request = new GetSecretValueRequest
        {
            SecretId = _tmtSecretsName,
            VersionStage = "AWSCURRENT", // VersionStage defaults to AWSCURRENT if unspecified.
        };

        GetSecretValueResponse response;

        try
        {
            response = await client.GetSecretValueAsync(request);
        }
        catch (Exception e)
        {
            // For a list of the exceptions thrown, see
            // https://docs.aws.amazon.com/secretsmanager/latest/apireference/API_GetSecretValue.html
            throw new HttpRequestException("Error reading secrets", e, HttpStatusCode.Unauthorized);
        }

        var parsedSecrets = JsonSerializer.Deserialize<TMTSecrets>(response.SecretString);
        if (parsedSecrets == null)
        {
            throw new HttpRequestException("Could not parse secrets", null, HttpStatusCode.Unauthorized); // Throw 401 if not authorized.
        }


        return parsedSecrets;
    }
}

