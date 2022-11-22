using Pulumi;
using Pulumi.Aws.DynamoDB;
using Pulumi.Aws.DynamoDB.Inputs;
using Pulumi.Aws.Iam;

namespace Deployment.Resources;

public class DynamoDBCacheFactory
{
    public Table CreateDynamoDBTable(InputMap<string> tags, Role role)
    {
        var environment = Pulumi.Deployment.Instance.StackName;
        var projectName = Pulumi.Deployment.Instance.ProjectName;
        var name = $"{projectName}-dynamodb-cache-{environment}";

        // Create table
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
            },
            HashKey = "CacheKey",
            RangeKey = null,
            Ttl = new TableTtlArgs
            {
                AttributeName = "TimeToLive",
                Enabled = true,
            },
            TableClass = "STANDARD",
            BillingMode = "PAY_PER_REQUEST",
            Tags = tags,
        });

        // Prep policy
        var policyDoc = Output.Format($@"{{
            ""Version"": ""2012-10-17"",
            ""Statement"": [
                {{
                    ""Effect"": ""Allow"",
                    ""Action"": [
                        ""dynamodb:UpdateItem"",
                        ""dynamodb:PutItem"",
                        ""dynamodb:GetItem"",
                        ""dynamodb:DescribeTable"",
                        ""dynamodb:Scan""
                    ],
                    ""Resource"": [
                        ""{dynamoDbCacheTable.Arn}""
                    ]
                }}
            ]
        }}");

        var dynamoDBpolicy = new Policy($"{projectName}-dynamodb-policy-attachment-{environment}", new()
        {
            Description = "DynamoDB policy for lambda function",
            PolicyDocument = policyDoc,
            Tags = tags,
        });

        // Attach to role
        new RolePolicyAttachment($"{projectName}-dynamodb-policy-attachment-{environment}", new()
        {
            Role = role.Name,
            PolicyArn = dynamoDBpolicy.Arn,
        });

        return dynamoDbCacheTable;
    }
}