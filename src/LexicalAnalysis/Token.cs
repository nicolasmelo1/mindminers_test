namespace SSync.LexicalAnalysis {
    public class Token {
        public string timestamp;
        public int order = 0;
        public string subtitle = "";

        public Token(string timestamp, int order, string subtitle) {
            this.timestamp = timestamp;
            this.order = order;
            this.subtitle = subtitle;
        }
    }
}