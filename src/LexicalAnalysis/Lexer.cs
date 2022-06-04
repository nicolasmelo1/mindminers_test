namespace SSync.LexicalAnalysis {
    public class Lexer {
        private string[] _subtitle = {};
        private int _currentPosition = 0;

        public Lexer(string[] subtitle) {
            _subtitle = subtitle;
        }

        private string? peekNextCharacter(int numberOfCharactersToPeek = 1) 
        {
            int position = _currentPosition + numberOfCharactersToPeek;
            if (position <= _subtitle.Length - 1) return _subtitle[position];
            return null;
        }

        private bool isNextCharacterDifferentFrom(string character, int numberOfCharactersToPeek=1) 
        {
            bool doesNotPassTheEndOfTheSubtitle = _currentPosition + numberOfCharactersToPeek <= _subtitle.Length - 1;
            return doesNotPassTheEndOfTheSubtitle && peekNextCharacter(numberOfCharactersToPeek) != character;
        }
        private Token? handleNextToken() 
        {
            bool hasReachedEndOfSubtitle = _currentPosition >= _subtitle.Length;
            if (hasReachedEndOfSubtitle) return null;
            
            int counter = 0;
            string subtitle = "";
            string? timestamp = "";
            string? order = "";

            while (isNextCharacterDifferentFrom("", counter)) 
            {
                int position = _currentPosition + counter;
                string character = _subtitle[position];
                switch (counter) 
                {
                    case 0:
                        order = character;
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

            _currentPosition += counter + 1;
            return new Token(timestamp, order, subtitle);
        }

        public Token? getNextTokenOrNull() 
        {
            return handleNextToken();
        }
    }
    
}