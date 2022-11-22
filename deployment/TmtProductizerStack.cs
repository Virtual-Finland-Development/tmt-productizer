using System.Collections.Generic;
using System.Text.Json;
using Pulumi;
using Pulumi.Aws.Iam;
using Pulumi.Aws.Lambda;
using Pulumi.Aws.Lambda.Inputs;
using Pulumi.Command.Local;

using Deployment.Resources;

namespace Deployment.TmtProductizerStack;

public class TmtProductizerStack : Stack
{
    public TmtProductizerStack()
    {
        var config = new Config();
        var environment = Pulumi.Deployment.Instance.StackName;
        var projectName = Pulumi.Deployment.Instance.ProjectName;
        var artifactPath = config.Get("artifactPath") ?? "release/";
        var tags = new InputMap<string>
        {
            {
                "vfd:stack", environment
            },
            {
                "vfd:project", projectName
            }
        };

        var role = new Role($"{projectName}-lambda-role-{environment}", new RoleArgs
        {
            AssumeRolePolicy = JsonSerializer.Serialize(new Dictionary<string, object?>
            {
                { "Version", "2012-10-17" },
                {
                    "Statement", new[]
                    {
                        new Dictionary<string, object?>
                        {
                            { "Action", "sts:AssumeRole" },
                            { "Effect", "Allow" },
                            { "Sid", "" },
                            {
                                "Principal", new Dictionary<string, object?>
                                {
                                    { "Service", "lambda.amazonaws.com" }
                                }
                            }
                        }
                    }
                }
            })
        });

        // AWS Secrets Manager
        var secretsManagerFactory = new SecretsManagerFactory();
        var tmtSecretsManager = secretsManagerFactory.CreateTMTSecretManagerItem(tags);
        SecretsManagerSecretName = tmtSecretsManager.Secret.Name; // For pulumi output
        new RolePolicyAttachment($"{projectName}-secrets-manager-policy-attachment-{environment}", new()
        {
            Role = role.Name,
            PolicyArn = tmtSecretsManager.Policy.Arn,
        });

        // DynamoDB
        var dynamoDBCacheFactory = new DynamoDBCacheFactory();
        var dynamoDBCache = dynamoDBCacheFactory.CreateDynamoDBTable(tags);
        DynamoDBCacheTableName = dynamoDBCache.Table.Name; // For pulumi output
        new RolePolicyAttachment($"{projectName}-dynamodb-policy-attachment-{environment}", new()
        {
            Role = role.Name,
            PolicyArn = dynamoDBCache.Policy.Arn,
        });

        var rolePolicyAttachment = new RolePolicyAttachment($"{projectName}-lambda-role-attachment-{environment}",
            new RolePolicyAttachmentArgs
            {
                Role = Output.Format($"{role.Name}"),
                PolicyArn = ManagedPolicy.AWSLambdaBasicExecutionRole.ToString()
            });

        var lambdaFunction = new Function($"{projectName}-{environment}", new FunctionArgs
        {
            Role = role.Arn,
            Runtime = "dotnet6",
            Handler = "TMTProductizer",
            Timeout = 15,
            Environment = new FunctionEnvironmentArgs
            {
                Variables =
                {
                    { "ASPNETCORE_ENVIRONMENT", "Development" },
                    { "DynamoDBCacheName", DynamoDBCacheTableName }, // Override appsettings.json with staged value
                }
            },
            Code = new FileArchive(artifactPath),
            Tags = tags
        });

        var functionUrl = new FunctionUrl($"{projectName}-function-url-{environment}", new FunctionUrlArgs
        {
            FunctionName = lambdaFunction.Arn,
            AuthorizationType = "NONE"
        });

        var command = new Command($"{projectName}-add-permissions-command-{environment}", new CommandArgs
        {
            Create = Output.Format(
                $"aws lambda add-permission --function-name {lambdaFunction.Arn} --action lambda:InvokeFunctionUrl --principal '*' --function-url-auth-type NONE --statement-id FunctionUrlAllowAccess")
        }, new CustomResourceOptions
        {
            DeleteBeforeReplace = true,
            DependsOn = new InputList<Resource>
            {
                lambdaFunction
            }
        });


        ApplicationUrl = functionUrl.FunctionUrlResult;
    }

    [Output] public Output<string> ApplicationUrl { get; set; }
    [Output] public Output<string> DynamoDBCacheTableName { get; set; } // Use with local development
    [Output] public Output<string> SecretsManagerSecretName { get; set; } // TODO: Add secrets manager pulumi creation
}
