namespace TMTProductizer.Exceptions;

public class JSONParseException : Exception
{
    public JSONParseException()
    {
    }

    public JSONParseException(string message)
        : base(message)
    {
    }

    public JSONParseException(string message, Exception inner)
        : base(message, inner)
    {
    }
}