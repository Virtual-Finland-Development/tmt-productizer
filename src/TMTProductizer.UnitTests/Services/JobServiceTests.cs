using System.Net;
using CodeGen.Api.TMT.Model;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using TMTProductizer.Models;
using TMTProductizer.Models.Cache.TMT;
using TMTProductizer.Services;
using TMTProductizer.UnitTests.Mocks;
using TMTProductizer.Utils;

namespace TMTProductizer.UnitTests.Services;

public class JobServiceTests
{
    [Test]
    public void TryingToFindJob_WithTmtApiUp_ReturnsListWithData()
    {
        string tmtJson = MockUtils.GetTMTTestResponse();
        Hakutulos tmtResults = StringUtils.JsonDeserializeObject<Hakutulos>(tmtJson);
        CachedHakutulos cachedResults = new CachedHakutulos(tmtResults);

        var handler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        handler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(tmtJson)
            });
        var httpClient = new HttpClient(handler.Object) { BaseAddress = new Uri("http://localhost/") };
        var proxyClientFactory = new Mock<IProxyHttpClientFactory>();
        proxyClientFactory.Setup(service => service.GetProxyClient(It.IsAny<APIAuthorizationPackage>())).Returns(httpClient);
        proxyClientFactory.SetupGet(service => service.BaseAddress).Returns(httpClient.BaseAddress);
        var tmtAuthorizationService = new Mock<IAPIAuthorizationService>();
        tmtAuthorizationService.Setup(service => service.GetAPIAuthorizationPackage())
            .ReturnsAsync(new APIAuthorizationPackage());
        var tmtApiResultsCacheService = new Mock<ITMTAPIResultsCacheService>();
        var jobFetcher = new Mock<ITMTJobsFetcher>();
        jobFetcher.Setup(service => service.FetchTMTAPIResults()).ReturnsAsync(cachedResults);

        var query = new JobsRequest
        {
            Query = "",
            Location = new LocationQuery
            {
                Countries = new List<string>(),
                Regions = new List<string>(),
                Municipalities = new List<string>()
            },
            Paging = new PagingOptions
            {
                Limit = 1,
                Offset = 0
            }
        };

        var sut = new JobService(jobFetcher.Object, new Logger<JobService>(new LoggerFactory()));

        var result = sut.Find(query);

        result.Should().NotBeNull();
        result.Result.Should().BeOfType<(List<Job>, long)>();
        result.Result.jobs.Count.Should().BeGreaterThan(0);
    }

    [Test]
    public void TryingToFindJob_WithTmtApiDown_ReturnsEmptyList()
    {
        var handler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        handler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Content = new StringContent("errorMessage")
            });
        var httpClient = new HttpClient(handler.Object) { BaseAddress = new Uri("http://localhost/") };
        var proxyClientFactory = new Mock<IProxyHttpClientFactory>();
        proxyClientFactory.Setup(service => service.GetProxyClient(It.IsAny<APIAuthorizationPackage>())).Returns(httpClient);
        proxyClientFactory.SetupGet(service => service.BaseAddress).Returns(httpClient.BaseAddress);
        var tmtAuthorizationService = new Mock<IAPIAuthorizationService>();
        tmtAuthorizationService.Setup(service => service.GetAPIAuthorizationPackage())
            .ReturnsAsync(new APIAuthorizationPackage());
        var tmtApiResultsCacheService = new Mock<ITMTAPIResultsCacheService>();

        var query = new JobsRequest
        {
            Query = "",
            Location = new LocationQuery
            {
                Countries = new List<string>(),
                Regions = new List<string>(),
                Municipalities = new List<string>()
            },
            Paging = new PagingOptions
            {
                Limit = 1,
                Offset = 20
            }
        };
        var jobFetcher = new TMTJobsFetcher(proxyClientFactory.Object, tmtAuthorizationService.Object, tmtApiResultsCacheService.Object, new Logger<TMTJobsFetcher>(new LoggerFactory()));
        var sut = new JobService(jobFetcher, new Logger<JobService>(new LoggerFactory()));

        var result = sut.Find(query);

        result.Should().NotBeNull();
        result.Result.Should().BeOfType<(List<Job>, long)>();
        result.Result.jobs.Count.Should().Be(0);
    }
}
