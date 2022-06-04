using SubtitleClient;

public static class Program 
{
    public static void Main(string[] args) 
    {
        var client = new Client();
        client.loadFile("subtitle.srt");
    }
}