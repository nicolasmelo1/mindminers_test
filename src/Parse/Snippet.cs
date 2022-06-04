using SSync.LexicalAnalysis;

namespace SSync.Parse {
    public class Timestamp 
    {   
        public int? hour=0;
        public int? minute=0;
        public int? second=0;
        public int? millisecond=0;

        public Timestamp(string timestamp)
        {
            string[] parts = timestamp.Split(':');
        }
    }

    public class Timestamps 
    {
        public Timestamp? start = null;
        public Timestamp? end = null;

        public Timestamps(string timestampsRange)
        {
            string[] splittedTimestamps = timestampsRange.Split(" ---> ");
            start = new Timestamp(splittedTimestamps[0]);
            end = new Timestamp(splittedTimestamps[1]);
        }
    }

    public class Snippet 
    {
        public int? order=null;
        public Timestamps? timestamps = null;
        private Parser? _parser = null;
        private Snippet? _next = null;
        public Snippet(Token token, Parser parser) 
        {
            _parser = parser;
            timestamps = new Timestamps(token.timestamp);
        }

        public Snippet? getNext()
        {
            bool isNextSnippetDefined = _next != null;
            if (isNextSnippetDefined) return _next;

            _next = _parser?.getNextSnippet();
            return _next;
        }
    }
}