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

        // AWS Secrets Manager Policy
        var secretsManagerPolicy = new Policy($"{projectName}-secrets-manager-policy-{environment}", new()
        {
            Description = "Read access to secrets manager",
            PolicyDocument = @"{
                ""Version"": ""2012-10-17"",
                ""Statement"": [
                    {
                    ""Action"": [
                        ""secretsmanager:GetSecretValue""
                    ],
                    ""Effect"": ""Allow"",
                    ""Resource"": ""arn:aws:secretsmanager:eu-north-1:433482540854:secret:tmt-api/azure-b2c-auth/client-secrets-K7W9Iq""
                    }
                ]
                }
            ",
            Tags = tags
        });

        // Attach secrets manager policy
        new RolePolicyAttachment($"{projectName}-secrets-manager-policy-attachment-{environment}", new()
        {
            Role = role.Name,
            PolicyArn = secretsManagerPolicy.Arn,
        });

        // DynamoDB
        var dynamoDBCacheFactory = new DynamoDBCacheFactory();
        var dynamoDBPolicy = dynamoDBCacheFactory.createDynamoDBTableAndReturnIAMPolicy(tags);
        new RolePolicyAttachment($"{projectName}-dynamodb-policy-attachment-{environment}", new()
        {
            Role = role.Name,
            PolicyArn = dynamoDBPolicy.Arn,
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
                    { "ASPNETCORE_ENVIRONMENT", "Development" }
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
}
