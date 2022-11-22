using Pulumi;
using Pulumi.Aws.Iam;
using Pulumi.Aws.SecretsManager;

namespace Deployment.Resources;

public class SecretsManagerFactory
{
    public Secret CreateTMTSecretManagerItem(InputMap<string> tags, Role role)
    {
        var environment = Pulumi.Deployment.Instance.StackName;
        var projectName = Pulumi.Deployment.Instance.ProjectName;
        var name = $"{projectName}-secrets-manager-{environment}";

        // Create secret manager
        var secretsManager = new Secret(name, new()
        {
            Name = name, // Override the hashed pulumi name for a locally referenceable name
            Description = "Credentials for token retrieval",
            Tags = tags,
        });

        // Prep policy
        var policyDoc = Output.Format($@"{{
            ""Version"": ""2012-10-17"",
            ""Statement"": [
                {{
                    ""Effect"": ""Allow"",
                    ""Action"": [
                        ""secretsmanager:GetSecretValue""
                    ],
                    ""Resource"": [
                        ""{secretsManager.Arn}""
                    ]
                }}
            ]
        }}");

        var secrestManagerPolicy = new Policy($"{projectName}-secrets-manager-policy-{environment}", new()
        {
            Description = "Secrets manager access policy for lambda function",
            PolicyDocument = policyDoc,
            Tags = tags
        });

        // attach policy to role
        new RolePolicyAttachment($"{projectName}-secrets-manager-policy-attachment-{environment}", new()
        {
            Role = role.Name,
            PolicyArn = secrestManagerPolicy.Arn,
        });

        return secretsManager;
    }
}