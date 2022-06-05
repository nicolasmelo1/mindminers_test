using SSync.LexicalAnalysis;

namespace SSync.Parse 
{
    /// <summary>
    /// Provides a simple way to interact with the timestamp, it stores the timestamp in hours, 
    /// minutes, seconds and milliseconds. 
    /// </summary>
    public class Timestamp 
    {   
        public int hour=0;
        public int minute=0;
        public int second=0;
        public int millisecond=0;

        /// <summary>
        /// Creates a new Timestamp object based on a `hh:mm:ss,mmm` string where `hh` 
        /// is the hour, `mm` is the minute, `ss` is the second and `mmm` is the millisecond.
        ///     <example>
        ///         <code>
        ///         Timestamp timestamp = new Timestamp("01:02:03,004");
        ///         Console.WriteLine(timestamp.getStringRepresentation());
        ///         </code>
        ///     </example>
        /// </summary>
        /// <param name="timestamp">The timestamp string in the `hh:mm:ss,mmm` format</param>
        public Timestamp(string timestamp)
        {
            string[] parts = timestamp.Split(':');
            Int32.TryParse(parts[0], out hour);
            Int32.TryParse(parts[1], out minute);
            string[] splittedSecondAndMillisecond = parts[2].Split(',');
            Int32.TryParse(splittedSecondAndMillisecond[0], out second);
            Int32.TryParse(splittedSecondAndMillisecond[1], out millisecond);
        }
        
        /// <summary>
        /// Used for retrieving the string representation of the timestamp. You can pass a millisecond offset so we 
        /// will calculate the timestamp off of the timestamp you passed.
        ///     <example>
        ///         <code>
        ///         Timestamp timestamp = new Timestamp("00:00:55,474");
        ///         Console.WriteLine(timestamp.getStringRepresentation()) // 00:00:55,474;
        ///         Console.WriteLine(timestamp.getStringRepresentation(1000)) // 00:00:56,474;
        ///         </code>
        ///     </example>
        /// </summary>
        /// <param name="millisecondOffset">The millisecond offset to add to the timestamp. 
        /// Can be either positive or negative. Defaults to 0.</param>
        /// <returns>The timestamp in the 'hh:mm:ss,fff' format with the offset applied.</returns>
        public string getStringRespresentation(long millisecondsOffset=0)
        {
            long timeInMilliseconds = (hour * 60 * 60 + minute * 60 + second) * 1000 + millisecond + millisecondsOffset;
            bool isTimeBelowZero = timeInMilliseconds < 0;
            if (isTimeBelowZero) 
            {
                long positiveTimeInMilliseconds = Math.Abs(timeInMilliseconds);
                long oneHourInMilliseconds = 60 * 60 * 1000;
                long millisecondsToConsider = oneHourInMilliseconds - (positiveTimeInMilliseconds % oneHourInMilliseconds);
                long exceedingHoursInMilliseconds = ((positiveTimeInMilliseconds / oneHourInMilliseconds) * 60 * 60 * 1000) + oneHourInMilliseconds;
                return $"-{TimeSpan.FromMilliseconds(exceedingHoursInMilliseconds + millisecondsToConsider).ToString(@"hh\:mm\:ss\,fff")}";
            }
            return TimeSpan.FromMilliseconds(timeInMilliseconds).ToString(@"hh\:mm\:ss\,fff");
        } 
    }

    /// <summary>
    /// Just for holding the timestamps information. In a subtitles file the timestamp is formated as `hh:mm:ss,mmm --> hh:mm:ss,mmm`.
    /// The first one means the time that the subtitle should appear on the screen, the other one represents the time it should disappear.
    /// So in other words this holds the `start` timestamp (appears) and the `end` timestamp (disappears).
    /// </summary>
    public class Timestamps 
    {
        public Timestamp start;
        public Timestamp end;

        /// <summary>
        /// Creates new start and end Timestamps instances based on a 
        /// `hh:mm:ss,mmm --> hh:mm:ss,mmm` string where `hh` is the hour, `mm` is the minute, `ss` is the 
        /// second and `mmm` is the millisecond.
        ///     <example>
        ///         <code>
        ///         Timestamps timestamps = new Timestamps("01:02:03,004 --> 10:20:30,400");
        ///         Console.WriteLine(timestamps.start.getStringRepresentation()); // 01:02:03,004
        ///         Console.WriteLine(timestamps.end.getStringRepresentation()); // 10:20:30,400
        ///         </code>
        ///     </example>
        /// </summary>
        /// <param name="timestamps">The timestamps string in the `hh:mm:ss,mmm --> hh:mm:ss,mmm` format</param>
        public Timestamps(string timestampsRange)
        {
            string[] splittedTimestamps = timestampsRange.Split(" --> ");
            start = new Timestamp(splittedTimestamps[0]);
            end = new Timestamp(splittedTimestamps[1]);
        }
    }

    /// <summary>
    /// This class is used to hold EACH subtitle information. You know that each subtitle appears and fades at a given timeframe.
    /// So each subtitle that appears and fades is represented as a Snippet. The Snippet class is a linked list so the first one 
    /// points to the next one and so on until we reach the end. This preserves the order of the subtitles.
    /// The information stored is the `order` of the subtitle, the next subtitle, the subtitle text and the start and end timestamps.
    /// </summary>
    public class Snippet 
    {
        public Timestamps timestamps;
        public int order;
        public string subtitle;
        private Parser _parser;
        private Snippet? _next = null;

        /// <summary>
        /// Crates a new Snippet instance from a given token. We also store the parser so we can retrieve the next Snippet in the list.
        ///     <example>
        ///         <code>
        ///         string[] subtitleLines = System.IO.File.ReadAllLines("subtitles.srt");
        ///         Lexer lexer = new Lexer(subtitleLines);
        ///         Snippet snippet = new Snippet(lexer.getNextTokenOrNull());
        ///         </code>
        ///     </example>
        /// </summary>
        /// <param name="token">The token with the raw data to create the Snippet from.</param>
        /// <param name="parser">The parser instance so we can retrieve the next Snippet.</param>
        public Snippet(Token token, Parser parser) 
        {
            _parser = parser;
            timestamps = new Timestamps(token.timestamp);
            order = token.order;
            subtitle = token.subtitle;
        }

        /// <summary>
        /// Retrieve the snippet as a string representation instead of a class instance. This is what we will use
        /// if we want to generate a new subtitles file.
        /// It recieves all of the changes that you want to make to this subtitle and applies them.
        ///     <example>
        ///         <code>
        ///         string[] subtitleLines = new string[] { 
        ///             "1", 
        ///             "00:00:51,533 --> 00:00:55,474",
        ///             "Não me lembro de ter",
        ///             "vendido tanto assim em um sábado.",
        ///             ""
        ///         };
        ///         Lexer lexer = new Lexer(subtitleLines);
        ///         Snippet snippet = new Snippet(lexer.getNextTokenOrNull());
        ///         Console.WriteLine(snippet.getStringRepresentation(100)); // 1\n00:00:51,633 --> 00:00:55,574\nNão me lembro de ter\nvendido tanto assim em um sábado.\n
        ///         </code>
        ///     </example>
        /// </summary>
        /// <param name="millisecondsOffset">The offset in milliseconds to add (or subtract) to the timestamps.</param>
        /// <param name="replacements">All of the replacements that you want to make in the subtitle.</param>
        /// <returns>The snippet as a string that goes to the actual file.</returns>
        public string getStringRepresentation(long offsetInMilliseconds=0, IDictionary<string, string>? replacements=null)
        {
            string newTimestamp = $"{timestamps.start.getStringRespresentation(offsetInMilliseconds)}" + 
                $" --> {timestamps.end.getStringRespresentation(offsetInMilliseconds)}";
            bool isLastCharacterOfTheSubtitleANewLine = subtitle[subtitle.Length - 1] == '\n';
            string newSubtitle = isLastCharacterOfTheSubtitleANewLine ? subtitle.Substring(0, subtitle.Length - 1) : subtitle;
            foreach (KeyValuePair<string, string> replacement in replacements ?? new Dictionary<string, string>())
            {
                newSubtitle = newSubtitle.Replace(replacement.Key, replacement.Value);
            }

            return $"{order}\n{newTimestamp}\n{newSubtitle}\n";
        }

        /// <summary>
        /// Retrieve the next Snippet in the list. If _next is null this means that we have 
        /// not retrieved it yet from the parser, otherwise we have already retrieved it. 
        /// We save the next Snippet in the _next argument, so we can be sure that whenever we want to
        /// retrieve the next Snippet in the linked list it will always be the same one.
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
        ///     Snippet snippet = new Snippet(lexer.getNextTokenOrNull());
        ///     Console.WriteLine(snippet.getStringRepresentation(100)); // 1\n00:00:51,633 --> 00:00:55,574\nNão me lembro de ter\nvendido tanto assim em um sábado.\n
        ///     snippet = snippet.getNext();
        ///     Console.WriteLine(snippet.getStringRepresentation(100)); // 2\n00:00:57,883 --> 00:01:02,482\nEstá chateado porque as\ngarçonetes mexeram com você?\n
        ///     </code>
        /// </example>
        /// </summary>
        /// <returns>The next Snippet in the linked list.</returns>
        public Snippet? getNext()
        {
            bool isNextSnippetDefined = _next != null;
            if (isNextSnippetDefined) return _next;

            _next = _parser?.getNextSnippet();
            return _next;
        }
    }
}
