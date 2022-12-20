namespace TMTProductizer.Services.AWS;
public interface IS3BucketCache : ICacheService
{
    public void SetBucketName(string bucketName);
}