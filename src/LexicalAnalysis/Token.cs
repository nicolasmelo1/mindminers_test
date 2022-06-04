namespace SSync.LexicalAnalysis {
    public class Token {
        public string timestamp;
        public string order;
        public string subtitle = "";

        public Token(string timestamp, string order, string subtitle) {
            this.timestamp = timestamp;
            this.order = order;
            this.subtitle = subtitle;
        }
    }
}