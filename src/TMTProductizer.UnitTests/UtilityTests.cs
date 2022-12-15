using CodeGen.Api.TMT.Model;
using TMTProductizer.Models.Cache.TMT;
using TMTProductizer.UnitTests.Mocks;
using TMTProductizer.Utils;

namespace TMTProductizer.UnitTests;

public class UtilityTests
{
    [Test]
    public void EnsureTypedCacheKeysWork()
    {
        Assert.AreEqual("test::System.String", CacheUtils.GetTypedCacheKey<string>("test"));
        Assert.AreEqual("test::TMTProductizer.UnitTests.Mocks.TestModel", CacheUtils.GetTypedCacheKey<TestModel>("test"));
    }

    [Test]
    public void EnsureJsonSerializationWorks()
    {
        var testModel = new TestModel { Name = "test" };
        var json = StringUtils.JsonSerializeObject(testModel);
        Assert.AreEqual("{\"name\":\"test\"}", json);

        var deserialized = StringUtils.JsonDeserializeObject<TestModel>(json);
        Assert.AreEqual("test", deserialized?.Name);

        string TmtJson = MockUtils.GetTMTTestResponse();
        var tmtDeserialized = StringUtils.JsonDeserializeObject<Hakutulos>(TmtJson);
        Assert.AreEqual(tmtDeserialized?.IlmoituksienMaara, tmtDeserialized?.Ilmoitukset?.Count);
    }

    [Test]
    public void TestCacheModelTransformations()
    {
        // Test tulos json -> cached tulos object
        string tmtJson = MockUtils.GetTMTTestResponse();
        Hakutulos tmtResults = StringUtils.JsonDeserializeObject<Hakutulos>(tmtJson);
        CachedHakutulos cachedResults = new CachedHakutulos(tmtResults);
        Assert.AreEqual(tmtResults.IlmoituksienMaara, cachedResults.IlmoituksienMaara);

        // Test cached tulos json -> cached tulos object
        string cachedTmtJson = StringUtils.JsonSerializeObject<CachedHakutulos>(cachedResults);
        CachedHakutulos cachedResults2 = StringUtils.JsonDeserializeObject<CachedHakutulos>(cachedTmtJson);
        Assert.AreEqual(cachedResults.IlmoituksienMaara, cachedResults2.IlmoituksienMaara);
    }
}