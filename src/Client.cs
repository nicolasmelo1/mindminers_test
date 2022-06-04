using SSync.LexicalAnalysis;

namespace SSync {
    public class Client 
    {
        private string? _parser;
        public Client()
        {
            _parser = null;
        }
        public void loadFile(string fileName) 
        {
            string[] lines = System.IO.File.ReadAllLines(fileName);
            Lexer lexer = new Lexer(lines);
            Token? token = lexer.getNextTokenOrNull();
            Console.WriteLine(token?.subtitle);
            Token? token2 = lexer.getNextTokenOrNull();
            Console.WriteLine(token2?.subtitle);
        }
    }
}