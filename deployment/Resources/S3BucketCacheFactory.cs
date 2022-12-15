using Pulumi;
using Pulumi.Aws.Iam;
using Pulumi.Aws.S3;

namespace Deployment.Resources;

public class S3BucketCacheFactory
{
    public Bucket CreateS3BucketCache(InputMap<string> tags, Role role)
    {
        var environment = Pulumi.Deployment.Instance.StackName;
        var project_name = Pulumi.Deployment.Instance.ProjectName;
        var name = $"{project_name}-s3-cache-{environment}";

        var bucket = new Bucket(name, new()
        {
            Acl = "private",
            Tags = tags,
        });

        // Prep policy
        var policyDoc = Output.Format($@"{{
            ""Version"": ""2012-10-17"",
            ""Statement"": [
                {{
                    ""Effect"": ""Allow"",
                    ""Action"": [
                        ""s3:ListBucket"",
                        ""s3:PutObject"",
                        ""s3:GetObject"",
                        ""s3:DeleteObject""
                    ],
                    ""Resource"": [
                        ""{bucket.Arn}"",
                        ""{bucket.Arn}/*""
                    ]
                }}
            ]
        }}");

        var s3_bucket_policy = new Policy($"{project_name}-s3-cache-policy-{environment}", new()
        {
            Description = "Secrets manager access policy for lambda function",
            PolicyDocument = policyDoc,
            Tags = tags
        });

        // attach policy to role
        new RolePolicyAttachment($"{project_name}-s3-cache-policy-attachment-{environment}", new()
        {
            Role = role.Name,
            PolicyArn = s3_bucket_policy.Arn,
        });

        return bucket;
    }
}