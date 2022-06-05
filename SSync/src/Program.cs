using SSync;

public static class Program 
{
    public static void Main(string[] args) 
    {
        SSyncClient client = new SSyncClient();
        client.runAsCLI(args);
    }
}
