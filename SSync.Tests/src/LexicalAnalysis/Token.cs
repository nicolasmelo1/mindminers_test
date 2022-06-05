using SSync.LexicalAnalysis;

namespace SSync.Tests.LexicalAnalysis 
{
    public class TokenTests
    {
        [Fact]
        public void creation() 
        {
            string subtitle = "This is a subtitle";
            string timestamp = "00:00:00,000 --> 00:00:00,000";
            int order = 1;
            var token = new Token(timestamp, order, subtitle);
            Assert.Equal(order, token.order);
            Assert.Equal(timestamp, token.timestamp);
            Assert.Equal(subtitle, token.subtitle);
        }
    }
}
