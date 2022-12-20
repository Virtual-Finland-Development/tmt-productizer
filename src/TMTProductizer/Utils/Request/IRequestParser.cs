namespace TMTProductizer.Utils.Request;

public interface IRequestParser<T>
{
    Task<T> Parse(T request);
}