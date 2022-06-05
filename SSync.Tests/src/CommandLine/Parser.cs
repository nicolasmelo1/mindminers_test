using SSync.CommandLine;

namespace SSync.Tests.CommandLine 
{   
    public class CommandLineArgumentsTests
    {
        [Fact]
        public void creation() 
        {
            var args = new CommandLineArguments("subtitles.srt");
            Assert.Equal("subtitles.srt", args.inputFileName);
            Assert.Equal("output.srt", args.outputFileName);
            Assert.Equal(0, args.hoursOffset);
            Assert.Equal(0, args.minutesOffset);
            Assert.Equal(0, args.secondsOffset);
            Assert.Equal(0, args.millisecondsOffset);
            Assert.Null(args.replacements);
        }

        [Fact]
        public void creation_with_replacements() 
        {
            var args = new CommandLineArguments("subtitles.srt");
            args.replacements = new string[][] { new string[] { "a", "b" }, new string[] { "c", "d" } };
            Assert.Equal("subtitles.srt", args.inputFileName);
            Assert.Equal("output.srt", args.outputFileName);
            Assert.Equal(0, args.hoursOffset);
            Assert.Equal(0, args.minutesOffset);
            Assert.Equal(0, args.secondsOffset);
            Assert.Equal(0, args.millisecondsOffset);
            Assert.NotNull(args.replacements);
            Assert.Equal(2, args.replacements.Length);
            Assert.Equal("b", args.replacements[0][1]);
        }

        [Fact]
        public void update() 
        {
            string newOutputFileName = "new-output.srt";
            var args = new CommandLineArguments("subtitles.srt");
            args.outputFileName = newOutputFileName;
            Assert.Equal(newOutputFileName, args.outputFileName);
        }
    }

    public class CLParserTests
    {
        [Fact]
        public void creation() 
        {
            var parser = new CLParser(new string[] { "subtitles.srt", "--output", "output.srt" });
            var args = parser.getArgs();
            Assert.Equal("subtitles.srt", args.inputFileName);
            Assert.Equal("output.srt", args.outputFileName);
            Assert.Null(args.replacements);
        }

        [Fact]
        public void creation_with_replacements() 
        {
            var parser = new CLParser(new string[] { "subtitles.srt", "--output", "output.srt", "--replacements", "a:b" });
            var args = parser.getArgs();
            Assert.Equal(new string[][] { new string[] { "a", "b" } }, args.replacements);
        }

        [Fact]
        public void creation_with_offset() 
        {
            var parser = new CLParser(new string[] { "subtitles.srt", "--hours", "1", "--minutes", "2", "--seconds", "3", "--milliseconds", "4" });
            var args = parser.getArgs();
            Assert.Equal(1, args.hoursOffset);
            Assert.Equal(2, args.minutesOffset);
            Assert.Equal(3, args.secondsOffset);
            Assert.Equal(4, args.millisecondsOffset);
        }

        [Fact]
        public void incomplete_args() 
        {
            Assert.Throws<InvalidParameterException>(() => {
                var parser = new CLParser(new string[] { "subtitles.srt", "--output" });
            });
        }

        [Fact]
        public void invalid_replacements() 
        {
            Assert.Throws<InvalidParameterException>(() => {
                var parser = new CLParser(new string[] { "subtitles.srt", "--output", "output.srt", "--replacements", "a" });
            });
        }
    }
}