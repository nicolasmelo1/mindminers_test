namespace SSync.LexicalAnalysis 
{
    /// <summary>
    /// As explained in the <see cref="Parser">Parser</see> class, this class is used to generate 
    /// <see cref="Token">Tokens</see>.
    /// 
    /// The idea is that this class will keep the state of the position that we are in the subtitle array.
    /// So we can retrieve one token at a time only when needed.
    ///
    /// This comes from the idea of how a programming language works. Each raw data (token) will then
    /// be converted to a <see cref="Snippet">Snippet</see> object.
    /// </summary>
    public class Lexer 
    {
        private string[] _subtitle = {};
        private int _currentOrder = 0;
        private int _currentPosition = 0;
        
        /// <summary>
        /// Creates a new Lexer object to traverse the given array and gererate <see cref="Token">Tokens</see>.
        /// <example>
        ///     <code>
        ///     string[] subtitleLines = new string[] {
        ///         "1",
        ///         "00:00:51,533 --> 00:00:55,474",
        ///         "Não me lembro de ter",
        ///         "vendido tanto assim em um sábado.",
        ///         ""
        ///         "2",
        ///         "00:00:57,873 --> 00:01:02,482",
        ///         "Está chateado porque as",
        ///         "garçonetes mexeram com você?"
        ///     };
        ///     Lexer lexer = new Lexer(subtitleLines);
        ///     Token token = lexer.getNextTokenOrNull();
        ///     Console.WriteLine(token.timestamp); // 00:00:51,533 --> 00:00:55,474
        ///     </code>
        /// </example>
        /// </summary>
        /// <param name="subtitleLines">An array where each string is a line 
        /// from the original subtitle file.</param>
        public Lexer(string[] subtitleLines) 
        {
            _subtitle = subtitleLines;
        }

        /// <summary>
        /// Peeks to see if the value of the next line in the array.
        /// <example>
        ///     <code>
        ///     string[] subtitleLines = new string[] {
        ///         "1",
        ///         "00:00:51,533 --> 00:00:55,474",
        ///         "Não me lembro de ter",
        ///         "vendido tanto assim em um sábado.",
        ///         ""
        ///         "2",
        ///         "00:00:57,873 --> 00:01:02,482",
        ///         "Está chateado porque as",
        ///         "garçonetes mexeram com você?"
        ///     };
        ///     Lexer lexer = new Lexer(subtitleLines);
        ///     PrivateObject lexerObj = new PrivateObject(lexer);
        ///     lexerObj.Invoke("peekNextLine") // "00:00:51,533 --> 00:00:55,474" 
        ///     // the current is at index 0 so the next will be at index 1
        ///     </code>
        /// </example>
        /// </summary>
        /// <param name="numberOfCharactersToPeek">Sometimes we want to peek 2, 3, 4 or 
        /// whatever characters ahead.</param>
        /// <returns>The next line in the array, or null if there is no next line.</returns>
        private string? peekNextLine(int numberOfCharactersToPeek = 1) 
        {
            int position = _currentPosition + numberOfCharactersToPeek;
            if (position <= _subtitle.Length - 1) return _subtitle[position];
            return null;
        }

        /// <summary>
        /// Checks if the next peeked line is differnt from a given string
        /// <example>
        ///     <code>
        ///     string[] subtitleLines = new string[] {
        ///         "1",
        ///         "00:00:51,533 --> 00:00:55,474",
        ///         "Não me lembro de ter",
        ///         "vendido tanto assim em um sábado."
        ///     };
        ///     Lexer lexer = new Lexer(subtitleLines);
        ///     PrivateObject lexerObj = new PrivateObject(lexer);
        ///     lexerObj.Invoke("isNextLineDifferentFrom", "1") // true
        ///     </code>
        /// </example>
        /// </summary>
        /// <param name="str">The string to compare with the next line.</param>
        /// <param name="numberOfCharactersToPeek">Sometimes we want to peek 2, 3, 4 or
        /// whatever characters ahead.</param>
        /// <returns>True if the next line is different from the given string, false otherwise.</returns>
        private bool isNextLineDifferentFrom(string str, int numberOfCharactersToPeek=1) 
        {
            bool doesNotPassTheEndOfTheSubtitle = _currentPosition + numberOfCharactersToPeek <= _subtitle.Length - 1;
            return doesNotPassTheEndOfTheSubtitle && peekNextLine(numberOfCharactersToPeek) != str;
        }

        /// <summary>
        /// Retrieves the next <see cref="Token">Token</see> in the array that represents a subtitle at a given
        /// timeframe.
        /// We always add a newline to the end of the subtitle.
        /// <example>
        ///     <code>
        ///     string[] subtitleLines = new string[] {
        ///         "1",
        ///         "00:00:51,533 --> 00:00:55,474",
        ///         "Não me lembro de ter",
        ///         "vendido tanto assim em um sábado."
        ///     }
        ///     Lexer lexer = new Lexer(subtitleLines);
        ///     Token token = lexer.getNextTokenOrNull();
        ///     Console.WriteLine(token.subtitle); // "Não me lembro de ter\nvendido tanto assim em um sábado.\n"
        ///     </code>
        /// </example>
        /// </summary>
        /// <returns>The next <see cref="Token">Token</see> in the array, or null if there is no next token.</returns>
        public Token? getNextTokenOrNull() 
        {
            bool hasReachedEndOfSubtitle = _currentPosition >= _subtitle.Length;
            if (hasReachedEndOfSubtitle) return null;
            
            int counter = 0;
            string subtitle = "";
            string? timestamp = "";

            while (isNextLineDifferentFrom("", counter)) 
            {
                int position = _currentPosition + counter;
                string character = _subtitle[position];
                switch (counter) 
                {
                    case 0:
                        break;
                    case 1:
                        timestamp = character;
                        break;
                    default:
                        subtitle += character + "\n";
                        break;
                }
                counter++;
            }
            _currentOrder++;
            _currentPosition += counter + 1;
            return new Token(timestamp, _currentOrder, subtitle);
        }
    }
}
