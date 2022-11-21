namespace TMTProductizer.Models;

public class DynamoDBCacheTableItem
{
    public string CacheKey { get; set; }
    public string CacheValue { get; set; } // JSON string
    public Int64 CreatedAt { get; set; } // Unix timestamp
}