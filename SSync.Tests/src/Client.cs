using SSync.Parse;

namespace SSync.Tests
{
    public class SSyncClientTests
    {

        public string? getRootDirectory()
        {
            string currentRunningDirectory = Directory.GetCurrentDirectory();
            string? debugDirectory = Directory.GetParent(currentRunningDirectory)?.FullName;
            string? binDirectory = Directory.GetParent(debugDirectory ?? "")?.FullName;
            string? rootDirectory = Directory.GetParent(binDirectory ?? "")?.FullName;
            return rootDirectory;
        }

        public string? retrieveMockSubtitle()
        {
            string? rootDirectory = getRootDirectory();
            string? filePath = Path.Combine(rootDirectory ?? "", "mock", "test.srt");
            return filePath;
        }

        [Fact]
        public void as_client()
        {
            string? filePath = retrieveMockSubtitle();
            string? rootDirectory = getRootDirectory();
            string outputFilePath = Path.Combine(rootDirectory ?? "", "mock", "output.srt");
            string[][] replacements = new string[][] {
                new string[] { "NQo", "Não" },
                new string[] { "garconetes", "garçonetes" },
            };
            
            SSyncClient client = new SSyncClient();
            
            Assert.Throws<InvalidSaveFileException>(() => client.saveFile(outputFilePath));

            client.applyChanges(millisecondsOffset: -4000, replacements: replacements);
            client.loadFile(filePath ?? "");
            client.saveFile(outputFilePath);

            string[] subtitleLines = System.IO.File.ReadAllLines(outputFilePath);
            Parser parser = new Parser(subtitleLines);
            Snippet? snippet = parser.getRootSnippet();

            Assert.Equal("Não me lembro de ter\nvendido tanto assim em um sábado.\n", snippet?.subtitle);
            Assert.Equal(47, snippet?.timestamps.start.second);

            snippet = snippet?.getNext();

            Assert.Equal("Está chateado porque as\ngarçonetes mexeram com você?\n", snippet?.subtitle);
            Assert.Equal(58, snippet?.timestamps.end.second);
            
            File.Delete(outputFilePath);
        }

        [Fact]
        public void as_cli()
        {
            string? filePath = retrieveMockSubtitle();
            string? rootDirectory = getRootDirectory();
            string outputFilePath = Path.Combine(rootDirectory ?? "", "mock", "output.srt");
            string[] commands = new string[] {
                filePath ?? "",
                "--o", 
                outputFilePath,
                "--r", "NQo:Não",
                "--r", "garconetes:garçonetes",
                "--ms", "-4000"
            };
            
            SSyncClient client = new SSyncClient();
            client.runAsCLI(commands);

            string[] subtitleLines = System.IO.File.ReadAllLines(outputFilePath);
            Parser parser = new Parser(subtitleLines);
            Snippet? snippet = parser.getRootSnippet();

            Assert.Equal("Não me lembro de ter\nvendido tanto assim em um sábado.\n", snippet?.subtitle);
            Assert.Equal(47, snippet?.timestamps.start.second);

            snippet = snippet?.getNext();

            Assert.Equal("Está chateado porque as\ngarçonetes mexeram com você?\n", snippet?.subtitle);
            Assert.Equal(58, snippet?.timestamps.end.second);

            File.Delete(outputFilePath);
        }
    }
}