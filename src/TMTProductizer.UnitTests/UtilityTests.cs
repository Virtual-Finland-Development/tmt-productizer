using TMTProductizer.Utils;

namespace TMTProductizer.UnitTests;

public class TestModel
{
    public string Name { get; set; } = null!;
}

public class UtilityTests
{
    [Test]
    public void EnsureTypedCacheKeysWork()
    {
        Assert.AreEqual("test::System.String", StringUtils.GetTypedCacheKey<string>("test"));
        Assert.AreEqual("test::TMTProductizer.UnitTests.TestModel", StringUtils.GetTypedCacheKey<TestModel>("test"));
    }
}