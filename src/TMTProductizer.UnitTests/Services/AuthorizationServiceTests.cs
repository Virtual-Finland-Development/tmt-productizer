using System.Net;
using TMTProductizer.Services;
using Moq;
using Moq.Protected;
using Microsoft.AspNetCore.Http;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;

namespace TMTProductizer.UnitTests.Services;

public class AuthorizationServiceTests
{
    /**
    * Mockup service for testing
    */
    private IAuthorizationService getAuthorizationServiceMock() {
        var handler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        handler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.Unauthorized,
                Content = new StringContent("")
            });
        var httpClient = new HttpClient(handler.Object) { BaseAddress = new Uri("http://localhost/") };
        var authorizationService = new AuthorizationService(httpClient, new Mock<IWebHostEnvironment>().Object);

        return authorizationService;
    }

    [Test]
    public void TryingToAuthorize_BadHeaders_ReturnsAccessDeniedException()
    {
        // Test bad headers
        Action badHeadersAction = () => {
            var httpContext = new DefaultHttpContext {
                Request = {
                    Headers = {
                        { "Authorization", "Bearer: fake_token" },
                    }
                }
            };
            var authorizationService = this.getAuthorizationServiceMock();
            authorizationService.Authorize(httpContext.Request).Wait();
        };
        badHeadersAction.Should().Throw<HttpRequestException>()
            .WithMessage("Missing headers");

    }

    [Test]
    public void TryingToAuthorize_InvalidHeaders_ReturnsAccessDeniedException()
    {
        // Test good, but invalid headers
        Action invalidHeadersAction = () => {
            var httpContext = new DefaultHttpContext {
                Request = {
                    Headers = {
                        { "Authorization", "Bearer abba-bubba-cobra" },
                        { "X-Authorization-Provider", "fake_provider" },
                    }
                }
            };

            var authorizationService = this.getAuthorizationServiceMock();
            authorizationService.Authorize(httpContext.Request).Wait();
        };
        invalidHeadersAction.Should().Throw<HttpRequestException>()
            .WithMessage("Access Denied");
    }
}
