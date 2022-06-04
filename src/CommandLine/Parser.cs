namespace SSync.CommandLine 
{   
    public class InvalidInputFileException : Exception 
    {
        public InvalidInputFileException() : base("The first argument should be your subtitles file.") { }
    }

    public class InvalidParameterException : Exception 
    {
        public InvalidParameterException(string example) : base($"The parameter is either incomplete or incorrect. Example: {example}") { }
    }

    public class CommandLineArguments
    {
        public string inputFileName;
        public string outputFileName = "output.srt";
        public long hoursOffset = 0;
        public long minutesOffset = 0;
        public long secondsOffset = 0;
        public long millisecondsOffset = 0;

        public string[][]? replacements = null;
        public CommandLineArguments(string inputFileName)
        {
            this.inputFileName = inputFileName;
        }
    }
    public class CLParser 
    {
        public CommandLineArguments args;
        private string[] _validOutputFileArguments = { "--output", "--o" };
        private string[] _validHoursOffsetArguments = { "--hours", "--h" };
        private string[] _validMinutesOffsetArguments = { "--minutes", "--m" };
        private string[] _validSecondsOffsetArguments = { "--seconds", "--s" };
        private string[] _validMillisecondsOffsetArguments = { "--milliseconds", "--ms" };
        private string[] _validReplacementsArguments = { "--replacements", "--r" };
        public CLParser(string[] args) 
        {
            this.args = this.formatArgs(args);
        }

        private CommandLineArguments formatArgs(string[] args) 
        {
            if (args.Length < 1) throw new InvalidInputFileException();
        
            string? inputFileName = args[0];
            List<string[]>? replacements = new List<string[]>();

            CommandLineArguments commandLineArguments = new CommandLineArguments(inputFileName);
            for (int i = 1; i < args.Length; i++)
            {
                if (_validOutputFileArguments.Contains(args[i]))
                {
                    if (i + 1 >= args.Length) throw new InvalidParameterException("--o \"newsubtitles.srt\"");
                    commandLineArguments.outputFileName = args[i + 1];
                }
                else if (_validReplacementsArguments.Contains(args[i]))
                {
                    if (i + 1 >= args.Length) throw new InvalidParameterException("--s \"nQo\":\"não\"");
                    string replacementsString = args[i + 1];
                    string[] replacementsArray = replacementsString.Split(':');
                    if (replacementsArray.Length != 2) throw new InvalidParameterException("--s \"nQo\":\"não\"");
                    replacements.Add(replacementsArray);
                } 
                else if (_validHoursOffsetArguments.Contains(args[i])) 
                {
                    if (i + 1 >= args.Length) throw new InvalidParameterException("--h 2");
                    Int64.TryParse(args[i + 1], out commandLineArguments.hoursOffset);
                } 
                else if (_validMinutesOffsetArguments.Contains(args[i])) 
                {
                    if (i + 1 >= args.Length) throw new InvalidParameterException("--m 5");
                    Int64.TryParse(args[i + 1], out commandLineArguments.minutesOffset);
                }
                else if (_validSecondsOffsetArguments.Contains(args[i])) 
                {
                    if (i + 1 >= args.Length) throw new InvalidParameterException("--s 10");
                    Int64.TryParse(args[i + 1], out commandLineArguments.secondsOffset);
                }
                else if (_validMillisecondsOffsetArguments.Contains(args[i])) 
                {
                    if (i + 1 >= args.Length) throw new InvalidParameterException("--ms 25");
                    Int64.TryParse(args[i + 1], out commandLineArguments.millisecondsOffset);
                }
            }

            string[][] replacementsAsArray = replacements.ToArray();
            if (replacementsAsArray.Length > 0) commandLineArguments.replacements = replacementsAsArray;

            return commandLineArguments;
        }
    }
}