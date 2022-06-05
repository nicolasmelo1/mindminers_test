using SSync.LexicalAnalysis;

namespace SSync.Parse 
{
    /// <summary>
    /// Used for parsing each line of the file and transforming it into a Snippet class. It follows the same logic as a parser 
    /// for a programming language but instead of creating an AST it creates a LinkedList of Snippets. The LinkedList is used
    /// so that we can keep the order of the snippets and also that we can load everything lazily which is useful for perfomance.
    /// 
    /// So how does it work? When we create a new instance we create a new Lexer instance passing all of the lines of the file
    /// subtitles file as an array. The Lexer will hold the state of the position that we are currently reading inside of the file.
    /// The idea of the Lexer is to "pop" tokens. Tokens are like the "raw data" of the file before becoming a snippet. Usually all the Tokens
    /// should follow the same format and pattern.
    ///
    /// After that we append a new Snippet to the _rootSnippet variable. So we can then use the _rootSnippet variable to get the first Snippet
    /// and iterate over it to get all of the snippets using the `getNext()` in the Snippet class.
    ///
    /// Each snippet holds the parser instance so they are all in sync with one another.
    /// </summary>
    public class Parser 
    {
        private Lexer _lexer;
        private Snippet? _rootSnippet = null;
        private Token? _currentToken = null;

        /// <summary>
        /// Creates a new Parser instance. When we do that we create a new Lexer instance and define a new _rootSnippet
        /// so you can start iterating over all of the snippets. It loads everything really quickly since this is lazy loaded.
        /// </summary>
        /// <param name="subtitleLines">Each line of the subtitles file.</param>
        public Parser(string[] subtitleLines)
        {
            _lexer = new Lexer(subtitleLines);
            _rootSnippet = getNextSnippet();
        }

        /// <summary>
        /// Tries to retrieve the next token from the Lexer. We usually use just the `getNextSnippet` method instead of using this method.
        /// That's why this is private and the other one is public.
        /// </summary>
        /// <returns>The next token or null if we have reached the end of the file.</returns>
        private Token? _getNextTokenOrNull()
        {
            Token? nextToken = _lexer?.getNextTokenOrNull();
            if (nextToken == null) return null;

            _currentToken = nextToken;
            return nextToken;
        }

        /// <summary>
        /// Retrieves the token of the Snippet that we are currently reading and then creates a new Snippet instance.
        /// </summary>
        /// <returns>The next Snippet or null if we have reached the end of the file.</returns>
        public Snippet? getNextSnippet()
        {
            Token? token = _getNextTokenOrNull();
            if (token == null) return null;
            
            return new Snippet(token, this);
        }

        /// <summary>
        /// Getter method for retrieving the rootSnippet value instead of giving access to it directly.
        /// </summary>
        /// <returns>This will ALWAYS be the head of the LinkedList.</returns>
        public Snippet? getRootSnippet()
        {
            return _rootSnippet;
        }
    }
}
