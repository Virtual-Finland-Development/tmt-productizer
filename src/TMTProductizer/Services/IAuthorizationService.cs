namespace TMTProductizer.Services;

public interface IAuthorizationService
{
    Task Authorize(HttpRequest request);
}
