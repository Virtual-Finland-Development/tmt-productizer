using Pulumi;
using Pulumi.Aws.DynamoDB;
using Pulumi.Aws.DynamoDB.Inputs;
using Pulumi.Aws.Iam;
using System.Text.Json;
using System.Collections.Generic;

namespace Deployment.Resources;

public class DynamoDBCacheFactory
{
    public (Table Table, Policy Policy) CreateDynamoDBTable(InputMap<string> tags)
    {
        var environment = Pulumi.Deployment.Instance.StackName;
        var projectName = Pulumi.Deployment.Instance.ProjectName;
        var name = $"{projectName}-dynamodb-cache-{environment}";

        var dynamoDbCacheTable = new Table(name, new()
        {
            Name = name, // Override the hashed pulumi name for a locally referenceable name
            Attributes = new[]
            {
                new TableAttributeArgs
                {
                    Name = "CacheKey",
                    Type = "S",
                },
                new TableAttributeArgs
                {
                    Name = "UpdatedAt",
                    Type = "N",
                },
            },
            HashKey = "CacheKey",
            RangeKey = "UpdatedAt",
            TableClass = "STANDARD",
            BillingMode = "PAY_PER_REQUEST",
            Tags = tags,
        });

        var dynamoDBpolicy = new Policy($"{projectName}-dynamodb-policy-attachment-{environment}", new()
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
                        ["Resource"] = dynamoDbCacheTable.Arn.Apply(arn => $"{arn}"),
                    },
                },
            }),
            Tags = tags,
        });

        return (dynamoDbCacheTable, dynamoDBpolicy);
    }
}