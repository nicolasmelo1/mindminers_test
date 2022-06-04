using SSync.LexicalAnalysis;

namespace SSync.Parse {
    /// <summary>
    /// Provides a simple way to interact with the timestamp, it stores the timestamp in hours, minutes, seconds and milliseconds. 
    /// </summary>
    public class Timestamp 
    {   
        public int hour=0;
        public int minute=0;
        public int second=0;
        public int millisecond=0;

        /// <summary>
        /// Creates a new Timestamp object based on a `hh:mm:ss,mmm` string where `hh` is the hour, `mm` is the minute, `ss` is the second and `mmm` is the millisecond.
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
        /// Used for retrieving the string representation of the timestamp. You can pass a millisecond offset so we will calculate the timestamp off of 
        /// the timestamp you passed.
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
                return $"-{TimeSpan.FromMilliseconds(exceedingHoursInMilliseconds + millisecondsToConsider).ToString(@"hh\:mm\:ss\.fff")}";
            }
            return TimeSpan.FromMilliseconds(timeInMilliseconds).ToString(@"hh\:mm\:ss\,fff");
        } 
    }

    public class Timestamps 
    {
        public Timestamp start;
        public Timestamp end;

        public Timestamps(string timestampsRange)
        {
            string[] splittedTimestamps = timestampsRange.Split(" --> ");
            start = new Timestamp(splittedTimestamps[0]);
            end = new Timestamp(splittedTimestamps[1]);
        }
    }

    public class Snippet 
    {
        public Timestamps timestamps;
        public int order;
        public string subtitle;
        private Parser _parser;
        private Snippet? _next = null;
        public Snippet(Token token, Parser parser) 
        {
            _parser = parser;
            timestamps = new Timestamps(token.timestamp);
            order = token.order;
            subtitle = token.subtitle;
        }

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

        public Snippet? getNext()
        {
            bool isNextSnippetDefined = _next != null;
            if (isNextSnippetDefined) return _next;

            _next = _parser?.getNextSnippet();
            return _next;
        }
    }
}