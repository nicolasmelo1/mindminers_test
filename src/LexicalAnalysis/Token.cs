namespace SSync.LexicalAnalysis 
{
    /// <summary>
    /// This is the raw data of a snippet. We don't do any type of conversion here, it just holds
    /// and keep the data kinda structured.
    /// </summary>
    public class Token 
    {
        public string timestamp;
        public int order = 0;
        public string subtitle = "";
        
        /// <summary>
        /// Creates a new token with the given timestamp, order and subtitle.
        /// <example>
        ///     <code>
        ///     Token token = new Token("00:00:00,000", 1, "This is a subtitle");
        ///     </code>
        /// </example>
        /// </summary>
        /// <param name="timestamp">The timestamp of the token as a string in 
        /// the 'hh:mm:ss,mmm --> hh:mm:ss,mmm' format </param>
        /// <param name="order">The order of the token in the file. This is 
        /// automatically generated and not retrieved from the file.</param>
        /// <param name="subtitle">The subtitle of this snippet, the text that 
        /// appears on the screen.</param>
        public Token(string timestamp, int order, string subtitle) 
        {
            this.timestamp = timestamp;
            this.order = order;
            this.subtitle = subtitle;
        }
    }
}
