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
        var json = StringUtils.JsonSerialiseObject(testModel);
        Assert.AreEqual("{\"name\":\"test\"}", json);

        var deserialised = StringUtils.JsonDeserialiseObject<TestModel>(json);
        Assert.AreEqual("test", deserialised?.Name);

        string TmtJson = MockUtils.GetTMTTestResponse();
        var tmtDeserialised = StringUtils.JsonDeserialiseObject<Hakutulos>(TmtJson);
        Assert.AreEqual(tmtDeserialised?.IlmoituksienMaara, 3);
    }
}