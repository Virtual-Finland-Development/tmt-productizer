
namespace TMTProductizer.UnitTests.Mocks;

public static class MockUtils
{

    public static string GetTMTTestResponse()
    {
        string mockDataFilepath = "./Mocks/testTMTResponse.json";

        try
        {
            // Open the text file using a stream reader.
            using (var sr = new StreamReader(mockDataFilepath))
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