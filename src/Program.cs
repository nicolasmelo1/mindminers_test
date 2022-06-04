using SSync;
using SSync.CommandLine;

public static class Program 
{
    public static void Main(string[] args) 
    {
        CLParser commandLineArgsParser = new CLParser(args);
        CommandLineArguments commandLineArgs = commandLineArgsParser.args;

        Client client = new Client();
        client.loadFile(commandLineArgs.inputFileName);
        client.applyChanges(
            hoursOffset: commandLineArgs.hoursOffset,
            minutesOffset: commandLineArgs.minutesOffset,
            secondsOffset: commandLineArgs.secondsOffset,
            millisecondsOffset: commandLineArgs.millisecondsOffset,
            replacements: commandLineArgs.replacements
        );
        client.saveFile(commandLineArgs.outputFileName);
    }
}