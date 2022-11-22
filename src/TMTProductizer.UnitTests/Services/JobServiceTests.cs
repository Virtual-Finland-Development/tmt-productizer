using System.Net;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using TMTProductizer.Models;
using TMTProductizer.Services;
using TMTProductizer.Services.TMT;
using TMTProductizer.Utils;

namespace TMTProductizer.UnitTests.Services;

public class JobServiceTests
{
    [Test]
    public void TryingToFindJob_WithTmtApiUp_ReturnsListWithData()
    {
        string TmtJson = GetTMTTestResponse();
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
        proxyClientFactory.Setup(service => service.GetTMTProxyClient(It.IsAny<TMTAPIAuthorizationDetails>())).Returns(httpClient);
        proxyClientFactory.SetupGet(service => service.BaseAddress).Returns(httpClient.BaseAddress);
        var tmtAuthorizationService = new Mock<ITMTAPIAuthorizationService>();
        tmtAuthorizationService.Setup(service => service.GetTMTAPIAuthorizationDetails())
            .ReturnsAsync(new TMTAPIAuthorizationDetails());

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

        var sut = new JobService(proxyClientFactory.Object, tmtAuthorizationService.Object, new Logger<JobService>(new LoggerFactory()));

        var result = sut.Find(query);

        result.Should().NotBeNull();
        result.Result.Should().BeOfType<List<Job>>();
        result.Result.Count.Should().BeGreaterThan(0);
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
        proxyClientFactory.Setup(service => service.GetTMTProxyClient(It.IsAny<TMTAPIAuthorizationDetails>())).Returns(httpClient);
        proxyClientFactory.SetupGet(service => service.BaseAddress).Returns(httpClient.BaseAddress);
        var tmtAuthorizationService = new Mock<ITMTAPIAuthorizationService>();
        tmtAuthorizationService.Setup(service => service.GetTMTAPIAuthorizationDetails())
            .ReturnsAsync(new TMTAPIAuthorizationDetails());


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
        var sut = new JobService(proxyClientFactory.Object, tmtAuthorizationService.Object, new Logger<JobService>(new LoggerFactory()));

        var result = sut.Find(query);

        result.Should().NotBeNull();
        result.Result.Should().BeOfType<List<Job>>();
        result.Result.Count.Should().Be(0);
    }


    private string GetTMTTestResponse()
    {
        try
        {
            // Open the text file using a stream reader.
            using (var sr = new StreamReader("../../src/TMTProductizer.UnitTests/Mocks/testTMTResponse.json"))
            {
                // Read the stream to a string, and write the string to the console.
                var line = sr.ReadToEnd();
                return line;
            }
        }
        catch (IOException e)
        {
            Console.WriteLine("The file could not be read:");
            Console.WriteLine(e.Message);
            throw e;
        }
    }
}