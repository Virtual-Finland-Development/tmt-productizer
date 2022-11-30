using CodeGen.Api.TMT.Model;
using TMTProductizer.UnitTests.Mocks;
using TMTProductizer.Utils;

namespace TMTProductizer.UnitTests;

public class UtilityTests
{
    [Test]
    public void EnsureTypedCacheKeysWork()
    {
        Assert.AreEqual("test::System.String", StringUtils.GetTypedCacheKey<string>("test"));
        Assert.AreEqual("test::TMTProductizer.UnitTests.Mocks.TestModel", StringUtils.GetTypedCacheKey<TestModel>("test"));
    }

    [Test]
    public void EnsureJsonSerialisationWorks()
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
}