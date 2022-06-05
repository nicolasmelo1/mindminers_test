using SSync.LexicalAnalysis;

namespace SSync.Tests.LexicalAnalysis 
{
    public class LexerTests 
    {
        [Fact]
        public void creation() 
        {
            string[] subtitleLines = new string[] {
                "1",
                "00:00:51,533 --> 00:00:55,474",
                "Não me lembro de ter",
                "vendido tanto assim em um sábado.",
                "",
                "2",
                "00:00:57,873 --> 00:01:02,482",
                "Está chateado porque as",
                "garçonetes mexeram com você?"
            };
            Lexer lexer = new Lexer(subtitleLines);
            Token? token = lexer.getNextTokenOrNull();
            Assert.Equal("00:00:51,533 --> 00:00:55,474", token?.timestamp);
        }

        [Fact]
        public void multiple_tokens() 
        {
            string[] subtitleLines = new string[] {
                "1",
                "00:00:51,533 --> 00:00:55,474",
                "Não me lembro de ter",
                "vendido tanto assim em um sábado.",
                "",
                "2",
                "00:00:57,873 --> 00:01:02,482",
                "Está chateado porque as",
                "garçonetes mexeram com você?"
            };
            Lexer lexer = new Lexer(subtitleLines);
            Token? token = lexer.getNextTokenOrNull();
            Assert.Equal("00:00:51,533 --> 00:00:55,474", token?.timestamp);
            token = lexer.getNextTokenOrNull();
            Assert.Equal("Está chateado porque as\ngarçonetes mexeram com você?\n", token?.subtitle);
            token = lexer.getNextTokenOrNull();
            Assert.Null(token);
        }

        [Fact]
        public void incomplete()
        {
            string[] subtitleLines = new string[] {
                "1",
                "00:00:51,533 --> 00:00:55,474",
                "Não me lembro de ter",
                "vendido tanto assim em um sábado.",
                "",
                "2",
            };
            Lexer lexer = new Lexer(subtitleLines);
            Token? token = lexer.getNextTokenOrNull();
            token = lexer.getNextTokenOrNull();
            Assert.Equal(2, token?.order);
            Assert.Null(lexer.getNextTokenOrNull());

        }

        [Fact]
        public void custom_order()
        {
            string[] subtitleLines = new string[] {
                "10",
                "00:00:51,533 --> 00:00:55,474",
                "Não me lembro de ter",
                "vendido tanto assim em um sábado.",
            };
            Lexer lexer = new Lexer(subtitleLines);
            Token? token = lexer.getNextTokenOrNull();
            Assert.Equal(1, token?.order);
        }
    }
}
