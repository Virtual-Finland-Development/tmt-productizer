using TMTCacheUpdater;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var function = new Function();
        await function.FunctionHandler();
    }
}