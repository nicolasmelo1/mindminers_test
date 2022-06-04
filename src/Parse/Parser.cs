using SSync.LexicalAnalysis;

namespace SSync.Parse {
    public class Parser {
        private Lexer _lexer;
        private Snippet? _rootSnippet = null;
        private Token? _currentToken = null;

        public Parser(string[] subtitleLines)
        {
            _lexer = new Lexer(subtitleLines);
            _rootSnippet = getNextSnippet();
        }

        private Token? getNextTokenOrNull()
        {
            Token? nextToken = _lexer?.getNextTokenOrNull();
            if (nextToken == null) return null;

            _currentToken = nextToken;
            return nextToken;
        }

        public Snippet? getNextSnippet()
        {
            Token? token = getNextTokenOrNull();
            if (token == null) return null;
            
            return new Snippet(token, this);
        }

        public Snippet? getRootSnippet()
        {
            return _rootSnippet;
        }
    }
}