using Pulumi;
using Pulumi.Aws.DynamoDB;
using Pulumi.Aws.DynamoDB.Inputs;
using Pulumi.Aws.Iam;
using System.Text.Json;
using System.Collections.Generic;

namespace Deployment.Resources;

public class DynamoDBCacheFactory
{
    public Policy createDynamoDBTableAndReturnIAMPolicy(InputMap<string> tags)
    {
        var environment = Pulumi.Deployment.Instance.StackName;
        var projectName = Pulumi.Deployment.Instance.ProjectName;
        var name = "tmt-productizer-dynamodb-cache";

        var cacheTable = new Table($"{name}-{environment}", new()
        {
            Attributes = new[]
            {
                new TableAttributeArgs
                {
                    Name = "CacheKey",
                    Type = "S",
                },
                new TableAttributeArgs
                {
                    Name = "CreatedAt",
                    Type = "N",
                },
            },
            HashKey = "CacheKey",
            RangeKey = "CreatedAt",
            TableClass = "STANDARD",
            BillingMode = "PAY_PER_REQUEST",
            Tags = tags,
        });


        var dynamoDBPolicy = new Policy($"{projectName}-dynamodb-policy-attachment-{environment}", new()
        {
            Description = "DynamoDB policy for lambda function",
            PolicyDocument = JsonSerializer.Serialize(new Dictionary<string, object?>
            {
                ["Version"] = "2012-10-17",
                ["Statement"] = new[]
                {
                    new Dictionary<string, object?>
                    {
                        ["Action"] = new[]
                        {
                            "dynamodb:UpdateItem",
                            "dynamodb:PutItem",
                            "dynamodb:GetItem",
                            "dynamodb:DescribeTable",
                        },
                        ["Effect"] = "Allow",
                        ["Resource"] = cacheTable.Arn,
                    },
                },
            }),
            Tags = tags,
        });

        return dynamoDBPolicy;
    }
}