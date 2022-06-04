namespace SubtitleClient {
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
            foreach (string line in lines) 
            {
                Console.WriteLine(line);
            }
        }
    }
}