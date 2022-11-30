using System.Collections.Generic;
using System.Text.Json;
using Pulumi;
using Pulumi.Aws.Iam;
using Pulumi.Aws.Lambda;
using Pulumi.Aws.Lambda.Inputs;
using Pulumi.Command.Local;

using Deployment.Resources;
using Pulumi.Aws.CloudWatch;

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
            Timeout = 15,
            MemorySize = 1024,
            Environment = new FunctionEnvironmentArgs
            {
                Variables =
                {
                    { "ASPNETCORE_ENVIRONMENT", "Development" },
                    { "DynamoDBCacheName", DynamoDBCacheTableName }, // Override appsettings.json with staged value
                    { "TmtSecretsName", SecretsManagerSecretName },
                    { "S3BucketCacheName", S3BucketCacheName },
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
            Handler = "TMTCacheUpdater",
            Timeout = 120,
            MemorySize = 1024,
            Environment = lambdaFunction.Environment.Apply(env => new FunctionEnvironmentArgs
            {
                Variables = env?.Variables ?? new InputMap<string>()
            }),
            Code = new FileArchive(artifactPath),
            Tags = tags
        });
        var cacheUpdateSchedule = new EventRule($"{projectName}-cache-updater-schedule-{environment}", new EventRuleArgs
        {
            Description = "Schedule cache update",
            ScheduleExpression = "rate(1 day)",
            Tags = tags,
        });
        var cacheUpdateScheduleTarget = new EventTarget($"{projectName}-cache-updater-target-{environment}", new EventTargetArgs
        {
            Rule = cacheUpdateSchedule.Name,
            Arn = cacheUpdatingLambdaFunction.Arn,
            Input = JsonSerializer.Serialize(new Dictionary<string, object?>
            {
                { "action", "update" }
            })
        });

        // Lambda function URL
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
    [Output] public Output<string> DynamoDBCacheTableName { get; set; }
    [Output] public Output<string> S3BucketCacheName { get; set; }
    [Output] public Output<string> SecretsManagerSecretName { get; set; }
}
