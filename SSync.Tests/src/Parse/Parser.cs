using SSync.Parse;

namespace SSync.Tests.Parse 
{
    public class ParserTests
    {
        [Fact]
        public void validate_root()
        {
            string[] subtitleLines = new string[] {
                "1",
                "00:00:51,533 --> 00:00:55,474",
                "Não me lembro de ter",
                "",
                "2",
                "00:00:57,873 --> 00:01:02,482",
                "Está chateado porque as",
            };
            Parser parser = new Parser(subtitleLines);
            Snippet? snippet = parser.getRootSnippet();
            Assert.Equal("Não me lembro de ter\n", snippet?.subtitle);
            parser.getNextSnippet();
            snippet = parser.getRootSnippet();
            Assert.Equal("Não me lembro de ter\n", snippet?.subtitle);
        }

        [Fact]
        public void validate_multiple_snippets()
        {
            string[] subtitleLines = new string[] {
                "1",
                "00:00:51,533 --> 00:00:55,474",
                "Não me lembro de ter",
                "",
                "2",
                "00:00:57,873 --> 00:01:02,482",
                "Está chateado porque as",
                "garçonetes mexeram com você?"
            };
            Parser parser = new Parser(subtitleLines);
            Snippet? snippet = parser.getRootSnippet();
            Assert.Equal("Não me lembro de ter\n", snippet?.subtitle);
            snippet = parser.getNextSnippet();
            Assert.Equal("Está chateado porque as\ngarçonetes mexeram com você?\n", snippet?.subtitle);
            snippet = parser.getNextSnippet();
            Assert.Null(snippet);
        }
    }
}
