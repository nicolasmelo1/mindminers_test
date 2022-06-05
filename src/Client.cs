using SSync.Parse;

namespace SSync 
{
    public class InvalidSaveFileException : Exception 
    {
        public InvalidSaveFileException() : base("You should try to call `loadFile` before trying to save a file") { }
    }

    /// <summary>
    /// Used for holding the offsets data. It holds everything separated in hours, minutes, seconds and milliseconds,
    /// but when it needs to be used it is converted to milliseconds using the `inMilliseconds()` method.
    /// </summary>
    public class Offset 
    {
        public long hour = 0;
        public long minute = 0;
        public long second = 0;
        public long millisecond = 0;

        /// <summary>
        /// Saves the offset data.
        /// <example>
        ///     <code>
        ///     long hoursOffset = 1;
        ///     long minutesOffset = 2;
        ///     long secondsOffset = 3;
        ///     long millisecondsOffset = 4;
        ///     Offset offset = new Offset(hoursOffset, minutesOffset, secondsOffset, millisecondsOffset);
        ///     </code>
        /// </example>
        /// </summary>
        public Offset(long hour, long minute, long second, long millisecond) 
        {
            this.hour = hour;
            this.minute = minute;
            this.second = second;
            this.millisecond = millisecond;
        }

        /// <summary>
        /// Converts the offset data to milliseconds so it can be used inside of the code.
        /// <example>
        ///     <code>
        ///     long hoursOffset = 1;
        ///     long minutesOffset = 2;
        ///     long secondsOffset = 3;
        ///     long millisecondsOffset = 4;
        ///     Offset offset = new Offset(hoursOffset, minutesOffset, secondsOffset, millisecondsOffset);
        ///     Console.WriteLine(offset.inMilliseconds()); // 3723004
        ///     </code>
        /// </example>
        /// </summary>
        public long inMilliseconds() 
        {
            return (hour * 60 * 60 + minute * 60 + second) * 1000 + millisecond;
        }
    }

    /// <summary>
    /// Used for holding the state of the changes to be made when we want to generate a new file. This is Public so it can be accessed and 
    /// modified by the user at any given time before the output file is generated.
    /// </summary>
    public class ChangesState
    {
        public Offset offset;
        public IDictionary<string, string> subtitleChanges;

        /// <summary>
        /// Creates a new ChangesState object to hold the changes to be made when we want to generate a new file. All of the parameters are
        /// optional.
        /// <example>
        ///     <code>
        ///     long hoursOffset = 1;
        ///     long minutesOffset = 2;
        ///     ChangesState changesState = new ChangesState(hoursOffset);
        ///     ChangesState changesState = new ChangesState(minutesOffset: minutesOffset);
        ///     </code>
        /// </example>
        /// </summary>
        /// <param name="hoursOffset">The hours offset to be used when generating the timestamps of the new file.</param>
        /// <param name="minutesOffset">The minutes offset to be used when generating the timestamps of the new file.</param>
        /// <param name="secondsOffset">The seconds offset to be used when generating the timestamps of the new file.</param>
        /// <param name="millisecondsOffset">The milliseconds offset to be used when generating the timestamps of the new file.</param>
        /// <param name="replacements">The replacements to make in the subtitle output. 
        /// It should follow the { valueToReplaceInOriginal, valueToReplaceWith } format.</param>
        public ChangesState(long hoursOffset=0, long minutesOffset=0, long secondsOffset=0, 
            long millisecondsOffset=0, string[][]? replacements=null) 
        {
            this.offset = new Offset(hoursOffset, minutesOffset, secondsOffset, millisecondsOffset);
            this.subtitleChanges = new Dictionary<string, string>();
  
            foreach (string[] replacement in replacements ?? Enumerable.Empty<string[]>())
            {
                string key = replacement[0];
                string value = replacement[1];
                this.subtitleChanges.Add(key, value);
            }            
        }
    }

    /// <summary>
    /// This is the main entrypoint of the hole program, it is responsible for loading and outputting the changes to a new file.
    /// The idea is that you can either use this as a normal CLI program but you can also use it as a library so you can interact
    /// with code directly.
    /// Be aware that this loads the file into memory lazily, so this means we will only effectively load each Snippet of the subtitle 
    /// once we need it.
    /// </summary> 
    public class SSyncClient 
    {
        private Snippet? _rootSnippet = null;
        public ChangesState? changesState = null;
        public SSyncClient() { }

        /// <summary>
        /// Load the file into memory and initializes the parser and the lexer. So we can parse the contents lazily. Retrieve only the first 
        /// root snippet of the subtitle so we can interact with it.
        /// <example>
        ///     <code>
        ///         var client = new Client();
        ///         client.loadFile("subtitles.srt");
        ///         client.loadFile("C://usercomputer/subtitles/gots1E1.srt");
        ///     </code>
        /// </example>
        /// </summary>
        /// <param name="fileName">The complete path or the name of the file to load</param>
        public void loadFile(string fileName) 
        {
            string[] subtitleLines = System.IO.File.ReadAllLines(fileName);
            Parser parser = new Parser(subtitleLines);
            _rootSnippet = parser.getRootSnippet();
        }

        /// <summary>
        /// This appends the changes to the subtitles so when we generate a new file we will apply all of those changes to the subtitles 
        /// saved in memory.
        /// All of the parameters are optional because then again you can either use this client in a library or as a CLI program so we 
        /// try to make both APIs as similar as possible.
        /// <example>
        ///     <code>
        ///         var client = new Client();
        ///         client.loadFile("subtitles.srt");
        ///         strings[][] replacements = new strings[][] {
        ///             new strings[] { "nQo", "não" }
        ///         };
        ///         client.applyChanges(hoursOffset: 1, minutesOffset: 2, secondsOffset: 3, millisecondsOffset: 4, replacements: replacements);
        ///         client.saveFile("output.srt");
        ///     </code>
        /// </example>
        /// </summary>
        /// <param name="hoursOffset">The number of hours to add (or subtract) from the timestamp</param>
        /// <param name="minutesOffset">The number of minutes to add (or subtract) from the timestamp</param>
        /// <param name="secondsOffset">The number of seconds to add (or subtract) from the timestamp</param>
        /// <param name="millisecondsOffset">The number of milliseconds to add (or subtract) from the timestamp</param>
        /// <param name="replacements">The replacements to make in the subtitle output. 
        /// It should follow the { valueToReplaceInOriginal, valueToReplaceWith } format</param>
        public void applyChanges(long hoursOffset=0, long minutesOffset=0, long secondsOffset=0, 
        long millisecondsOffset=0, string[][]? replacements=null) 
        {
            this.changesState = new ChangesState(
                hoursOffset, minutesOffset, secondsOffset, millisecondsOffset, replacements
            );
        }

        /// <summary>
        /// Save the subtitles to a new file while also parsing the data from the original file. It uses a linked list approach 
        /// so we can load everything lazily in memory instead of loading everything in memory in the beginning.
        /// The problem of this approach is that we don't have an easy way to access all of the indexes of the subtitle snippets.
        /// This means that the complexity for anything that needs to access a specific snippet is O(n) where n is the number of
        /// snippets in the subtitle.
        /// <example>
        ///     <code>
        ///         var client = new Client();
        ///         client.loadFile("subtitles.srt");
        ///         strings[][] replacements = new strings[][] {
        ///             new strings[] { "nQo", "não" }
        ///         };
        ///         client.applyChanges(hoursOffset: 1, minutesOffset: 2, secondsOffset: 3, millisecondsOffset: 4, replacements: replacements);
        ///         client.saveFile("output.srt");
        ///     </code>
        /// </example>
        /// </summary>
        /// <param name="fileName">The complete path or the name of the file that you wish to save the new subtitles data to.</param>
        public void saveFile(string fileName) 
        {
            bool isRootSnippetDefined = _rootSnippet != null;
            if (!isRootSnippetDefined) throw new InvalidSaveFileException();

            long offsetInMilliseconds = this.changesState?.offset?.inMilliseconds() ?? 0;
            IDictionary<string, string>? replacements = this.changesState?.subtitleChanges;

            List<string> lines = new List<string>();
            Snippet? nextSnippet = _rootSnippet;
            while (nextSnippet != null)
            {
                lines.Add(nextSnippet.getStringRepresentation(offsetInMilliseconds, replacements));
                nextSnippet = nextSnippet.getNext();
            }
            System.IO.File.WriteAllLines(fileName, lines.ToArray());
        }
    }
}