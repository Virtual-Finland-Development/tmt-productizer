namespace TMTProductizer.Services;

public interface IAuthorizationService
{
    Task<object?> Authorize(HttpRequest request);
}
