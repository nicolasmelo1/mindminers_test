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

    /// <summary>
    /// This class is used to structure the command line arguments while also giving default values to non passed arguments.
    /// Only the input file is required, the others are optional.
    /// </summary>
    public class CommandLineArguments
    {
        public string inputFileName;
        public string outputFileName = "output.srt";
        public long hoursOffset = 0;
        public long minutesOffset = 0;
        public long secondsOffset = 0;
        public long millisecondsOffset = 0;

        private List<string[]> _replacements = new List<string[]>();

        public CommandLineArguments(string inputFileName)
        {
            this.inputFileName = inputFileName;
        }

        public void addReplacement(string[] replacement)
        {
            _replacements.Add(replacement);
        }

        public string[][] getReplacements()
        {
            return _replacements.ToArray();
        }
    }

    /// <summary>
    /// This class is used for parsing the command line arguments so we can use this as a CLI program not only 
    /// as a library. This returns all of the arguments as a <see cref="CommandLineArguments"/> object which is structured 
    /// for our needs.
    /// </summary>
    public class CLParser 
    {
        private CommandLineArguments _args;
        private string[] _validOutputFileArguments = { "--output", "--o" };
        private IDictionary<string, string[]> _validOffsetArguments = new Dictionary<string, string[]>()
        {
            { "hours", new string[] { "--hours", "--h" } },
            { "minutes", new string[] { "--minutes", "--m" } },
            { "seconds", new string[] { "--seconds", "--s" } },
            { "milliseconds", new string[] { "--milliseconds", "--ms" } }
        };
        private string[] _validReplacementsArguments = { "--replacements", "--r" };

        /// <summary>
        /// Creates a new CLParser object with the given arguments formatted.
        /// Only the input file is required, the others are optional.
        /// <example>
        ///     <code>
        ///     string[] args = { 
        ///         "subtitles.srt", 
        ///         "--output", 
        ///         "output.srt", 
        ///         "--hours", 
        ///         "1", 
        ///         "--minutes", 
        ///         "2", 
        ///         "--seconds", 
        ///         "3", 
        ///         "--milliseconds", 
        ///         "4", 
        ///         "--replacements", 
        ///         "a:b" 
        ///     };
        ///     CLParser parser = new CLParser(args);
        ///     CommandLineArguments arguments = parser.getArgs();
        ///     Console.WriteLine(arguments.inputFileName) // "subtitles.srt";
        ///     </code>
        /// </summary>
        /// <param name="args">The arguments to parse.</param>
        public CLParser(string[] args) 
        {
            _args = this.formatArgs(args);
        }

        /// <summary>
        /// Loops through the given arguments and structures them into a <see cref="CommandLineArguments"/> object.
        /// We also validate if the arguments are correctly structured.
        /// Only the input file is required, the others are optional.
        /// </summary>
        /// <param name="args">The arguments to structure.</param>
        /// <returns>A <see cref="CommandLineArguments"/> object with all of the passed and 
        /// non-passed arguments structured.</returns>
        private CommandLineArguments formatArgs(string[] args) 
        {
            if (args.Length < 1) throw new InvalidInputFileException();
        
            string? inputFileName = args[0];
            List<string[]>? replacements = new List<string[]>();

            CommandLineArguments commandLineArguments = new CommandLineArguments(inputFileName);
            for (int i = 1; i < args.Length; i++)
            {
                handleOutputFileName(commandLineArguments, args, i);
                handleOffsetTypes(commandLineArguments, args, i, "hours", "--h 2");
                handleOffsetTypes(commandLineArguments, args, i, "minutes", "--m 3");
                handleOffsetTypes(commandLineArguments, args, i, "seconds", "--s 5");
                handleOffsetTypes(commandLineArguments, args, i, "milliseconds", "--ms -25");
                handleReplacements(commandLineArguments, args, i);
            }
            return commandLineArguments;
        }

        private void handleOutputFileName(CommandLineArguments commandLineArguments, string[] args, int index)
        {
            if (_validOutputFileArguments.Contains(args[index]))
            {
                if (index + 1 >= args.Length) throw new InvalidParameterException("--o \"newsubtitles.srt\"");
                commandLineArguments.outputFileName = args[index + 1];
            }
        }

        private void handleOffsetTypes(CommandLineArguments commandLineArguments, string[] args, int index, 
            string offsetType, string exceptionExample)
        {
            if (_validOffsetArguments[offsetType].Contains(args[index])) 
            {
                if (index + 1 >= args.Length) throw new InvalidParameterException(exceptionExample);
                long offset = 0;
                Int64.TryParse(args[index + 1], out offset);
                
                switch (offsetType)
                {
                    case "hours":
                        commandLineArguments.hoursOffset = offset;
                        break;
                    case "minutes":
                        commandLineArguments.minutesOffset = offset;
                        break;
                    case "seconds":
                        commandLineArguments.secondsOffset = offset;
                        break;
                    default:
                        commandLineArguments.millisecondsOffset = offset;
                        break;
                }
            }
        }

        /// <summary>
        /// Handles the replacements arguments. Since the replacements can be repeated an N number of times we will append
        /// the replacements one by one to a list inside of the <see cref="CommandLineArguments"/> object.
        /// 
        /// It should follow the following format: stringThatYouWantToReplace:stringThatYouWantToReplaceWith
        /// </summary>
        private void handleReplacements(CommandLineArguments commandLineArguments, string[] args, int index)
        {
            if (_validReplacementsArguments.Contains(args[index]))
            {
                if (index + 1 >= args.Length) throw new InvalidParameterException("--r \"nQo\":\"não\"");
                string replacementsString = args[index + 1];
                string[] replacementsArray = replacementsString.Split(':');
                if (replacementsArray.Length != 2) throw new InvalidParameterException("--r \"nQo\":\"não\"");
                commandLineArguments.addReplacement(replacementsArray);
            }
        }

        /// <summary>
        /// Returns the <see cref="CommandLineArguments"/> object with all of the passed and non-passed arguments structured.
        /// This is just a getter, because `.formatArgs()` is a private method.
        /// </summary>
        public CommandLineArguments getArgs()
        {
            return _args;
        }
    }
}
