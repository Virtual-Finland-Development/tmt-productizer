using System.Collections.Generic;
using System.Text.Json;
using Pulumi;
using Pulumi.Aws.Iam;
using Pulumi.Aws.Lambda;
using Pulumi.Aws.Lambda.Inputs;
using Pulumi.Command.Local;

namespace Deployment.TmtProductizerStack;

public class TmtProductizerStack : Stack
{
    public TmtProductizerStack()
    {
        var role = new Role("tmtProductizerLambdaRole", new RoleArgs
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

        var rolePolicyAttachment = new RolePolicyAttachment("tmtProductizerLambdaRoleAttachment", new RolePolicyAttachmentArgs
        {
            Role = Output.Format($"{role.Name}"),
            PolicyArn = ManagedPolicy.AWSLambdaBasicExecutionRole.ToString()
        });

        var lambdaFunction = new Function("tmtProductizer", new FunctionArgs
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
            Code = new AssetArchive(new Dictionary<string, AssetOrArchive>
            {
                { "folder", new FileArchive("../src/TMTProductizer") }
            })
        });

        var functionUrl = new FunctionUrl("tmtProductizerFunctionUrl", new FunctionUrlArgs
        {
            FunctionName = lambdaFunction.Arn,
            AuthorizationType = "NONE"
        });

        var localCommand = new Command("tmtProductizerAddPermissions", new CommandArgs
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


        Url = functionUrl.FunctionUrlResult;
    }

    [Output] public Output<string> Url { get; set; }
}
