using System.Collections.Generic;
using System.Text.Json;
using Pulumi;
using Pulumi.Aws.Iam;
using Pulumi.Aws.Lambda;
using Pulumi.Aws.Lambda.Inputs;
using Pulumi.Command.Local;

using Deployment.Resources;
using Pulumi.Aws.CloudWatch;
using Pulumi.Aws.CloudWatch.Inputs;

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

        // Get AuthGW endpoint
        var authGwStackReference = new StackReference($"{Pulumi.Deployment.Instance.OrganizationName}/authentication-gw/{environment}");
        var authenticationGwEndpointUrl = authGwStackReference.GetOutput("endpoint");

        // AWS Secrets Manager
        var secretsManagerFactory = new SecretsManagerFactory();
        var tmtSecretsManager = secretsManagerFactory.CreateTMTSecretManagerItem(tags, role);
        SecretsManagerSecretName = tmtSecretsManager.Name; // For pulumi output

        // DynamoDB
        var dynamoDBCacheFactory = new DynamoDBCacheFactory();
        var dynamoDBCacheTable = dynamoDBCacheFactory.CreateDynamoDBTable(tags, role);
        DynamoDBCacheTableName = dynamoDBCacheTable.Name; // For pulumi output

        // S3 Bucket for cache
        var s3BucketCacheFactory = new S3BucketCacheFactory();
        var s3BucketCache = s3BucketCacheFactory.CreateS3BucketCache(tags, role);
        S3BucketCacheName = s3BucketCache.BucketName; // For pulumi output

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
            Timeout = 30,
            MemorySize = 3072,
            Environment = new FunctionEnvironmentArgs
            {
                Variables =
                {
                    { "ASPNETCORE_ENVIRONMENT", "Development" },
                    { "DynamoDBCacheName", DynamoDBCacheTableName }, // Override appsettings.json with staged value
                    { "TmtSecretsName", SecretsManagerSecretName },
                    { "S3BucketCacheName", S3BucketCacheName },
                    { "AuthGWEndpoint", Output.Format($"{authenticationGwEndpointUrl}") }
                }
            },
            Code = new FileArchive(artifactPath),
            Tags = tags
        });

        // Create cache updating schedule
        var cacheUpdatingLambdaFunction = new Function($"{projectName}-cache-updater-{environment}", new FunctionArgs
        {
            Role = role.Arn,
            Runtime = "dotnet6",
            Handler = "TMTCacheUpdater::TMTCacheUpdater.Function::FunctionHandler",
            Timeout = 120,
            MemorySize = 3072,
            Environment = lambdaFunction.Environment.Apply(env => new FunctionEnvironmentArgs
            {
                Variables = env?.Variables ?? new InputMap<string>()
            }),
            Code = new FileArchive(artifactPath),
            Tags = tags
        });
        var cacheUpdateScheduleRule = new EventRule($"{projectName}-cache-updater-schedule-{environment}", new EventRuleArgs
        {
            Description = "Schedule cache update",
            ScheduleExpression = "cron(30 4 * * ? *)",
            Tags = tags,
        });
        new EventTarget($"{projectName}-cache-updater-target-{environment}", new EventTargetArgs
        {
            Rule = cacheUpdateScheduleRule.Name,
            Arn = cacheUpdatingLambdaFunction.Arn,
            RetryPolicy = new EventTargetRetryPolicyArgs
            {
                MaximumEventAgeInSeconds = 120,
                MaximumRetryAttempts = 0
            }
        });
        new Permission($"{projectName}-cache-updater-permission-{environment}", new()
        {
            Action = "lambda:InvokeFunction",
            Function = cacheUpdatingLambdaFunction.Name,
            Principal = "events.amazonaws.com",
            SourceArn = cacheUpdateScheduleRule.Arn,
        });

        // Lambda function URL
        var functionUrl = new FunctionUrl($"{projectName}-function-url-{environment}", new FunctionUrlArgs
        {
            FunctionName = lambdaFunction.Arn,
            AuthorizationType = "NONE"
        });

        new Command($"{projectName}-add-permissions-command-{environment}", new CommandArgs
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
    [Output] public Output<string> DynamoDBCacheTableName { get; set; }
    [Output] public Output<string> S3BucketCacheName { get; set; }
    [Output] public Output<string> SecretsManagerSecretName { get; set; }
}
