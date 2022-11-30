using System.Net;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using TMTProductizer.Models;
using TMTProductizer.Services;
using TMTProductizer.Services.AWS;
using TMTProductizer.UnitTests.Mocks;
using TMTProductizer.Utils;

namespace TMTProductizer.UnitTests.Services;

public class JobServiceTests
{
    [Test]
    public void TryingToFindJob_WithTmtApiUp_ReturnsListWithData()
    {
        string TmtJson = MockUtils.GetTMTTestResponse();
        var handler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        handler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(TmtJson)
            });
        var httpClient = new HttpClient(handler.Object) { BaseAddress = new Uri("http://localhost/") };
        var proxyClientFactory = new Mock<IProxyHttpClientFactory>();
        proxyClientFactory.Setup(service => service.GetProxyClient(It.IsAny<APIAuthorizationPackage>())).Returns(httpClient);
        proxyClientFactory.SetupGet(service => service.BaseAddress).Returns(httpClient.BaseAddress);
        var tmtAuthorizationService = new Mock<IAPIAuthorizationService>();
        tmtAuthorizationService.Setup(service => service.GetAPIAuthorizationPackage())
            .ReturnsAsync(new APIAuthorizationPackage());
        var tmtApiResultsCacheService = new Mock<IS3BucketCache>();

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

        var sut = new JobService(proxyClientFactory.Object, tmtAuthorizationService.Object, tmtApiResultsCacheService.Object, new Logger<JobService>(new LoggerFactory()));

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
        var tmtApiResultsCacheService = new Mock<IS3BucketCache>();

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
        var sut = new JobService(proxyClientFactory.Object, tmtAuthorizationService.Object, tmtApiResultsCacheService.Object, new Logger<JobService>(new LoggerFactory()));

        var result = sut.Find(query);

        result.Should().NotBeNull();
        result.Result.Should().BeOfType<(List<Job>, long)>();
        result.Result.jobs.Count.Should().Be(0);
    }
}
