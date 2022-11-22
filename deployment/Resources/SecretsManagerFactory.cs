using Pulumi;
using Pulumi.Aws.Iam;
using Pulumi.Aws.SecretsManager;
using System.Text.Json;
using System.Collections.Generic;

namespace Deployment.Resources;

public class SecretsManagerFactory
{
    public (Secret Secret, Policy Policy) CreateTMTSecretManagerItem(InputMap<string> tags)
    {
        var environment = Pulumi.Deployment.Instance.StackName;
        var projectName = Pulumi.Deployment.Instance.ProjectName;
        var name = $"{projectName}-secrets-manager-{environment}";

        var secretsManager = new Secret(name, new()
        {
            Name = name, // Override the hashed pulumi name for a locally referenceable name
            Tags = tags,
        });

        var secrestManagerPolicy = new Policy($"{projectName}-secrets-manager-policy-{environment}", new()
        {
            Description = "Secrets manager access policy for lambda function",
            PolicyDocument = JsonSerializer.Serialize(new Dictionary<string, object?>
            {
                ["Version"] = "2012-10-17",
                ["Statement"] = new[]
                {
                    new Dictionary<string, object?>
                    {
                        ["Action"] = new[]
                        {
                            "secretsmanager:GetSecretValue",
                        },
                        ["Effect"] = "Allow",
                        ["Resource"] = secretsManager.Arn.Apply(arn => $"{arn}"),
                    },
                },
            }),
            Tags = tags,
        });

        return (secretsManager, secrestManagerPolicy);
    }
}