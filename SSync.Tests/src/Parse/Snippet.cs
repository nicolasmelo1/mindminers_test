using SSync.Parse;

namespace SSync.Tests.Parse 
{
   public class SnippetTests
   {
        [Fact]
        public void creation()
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
            Assert.Equal(0, snippet?.timestamps.start.hour);
            Assert.Equal(0, snippet?.timestamps.start.minute);
            Assert.Equal(51, snippet?.timestamps.start.second);
            Assert.Equal(533, snippet?.timestamps.start.millisecond);

            Assert.Equal(0, snippet?.timestamps.end.hour);
            Assert.Equal(0, snippet?.timestamps.end.minute);
            Assert.Equal(55, snippet?.timestamps.end.second);
            Assert.Equal(474, snippet?.timestamps.end.millisecond);
        }
   }

}