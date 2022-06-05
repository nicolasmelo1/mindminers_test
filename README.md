# Mindminers Backend Challenge
## SSYNC
The SubRip (.srt) format of subtitle files for movies has become quite popular in recent years, among other reasons for its simplicity.

Here is a brief explanation of the format: It contain formatted lines of plain text in groups separated by a blank line. The subtitles are numbered sequentially, starting at 1. The timecode used is hours:minutes:seconds, milliseconds with time units fixed to two digits with leading zero padding, and fractions fixed to three digits also padded by leading zeros (00:00:00,000). The fractional separator used is the comma, as the program was written in France.

##### Example

> 168
> 
> 00:20:41,150 --> 00:20:45,109 
> - How did he do that? 
> - Made him an offer he couldn't refuse.

### Motivation
A Common problem users face with this format are the following:

- The subtitles does not sync well with the content being played.
- The subtitles have a wrong text in them.

### Solution
To solve this issue i've developed SSync. This is not only just a CLI command, but it has the ideal of becoming a tool for subtitles management and creation.

That's why that this has the ideal of being able to be used either as a CLI or a library inside of your own projects. This way you can edit and interact with it programatically with code without the need to run the same commands everytime.

## How to use

#### Programatically

```cs
using SSync;

string[] subtitleFiles = new string[] { "gots1e1.srt", "gots1e2.srt", "gots1e3.srt", "gots1e4.srt"}
SSyncClient client = new SSyncClient();
foreach(string file in subtitleFiles)
{
    client.loadFile(file);
    client.applyChanges(
        hoursOffset: 20
    );
    client.changesState.offset.minute = -10;
    client.saveFile("output.srt");
}
```

#### CLI

```bash
$ dotnet run --project ./SSync "examples/subtitle.srt" --o "examples/output.srt" --s -10 --r "nQo":"não" --r "NQo":"Não"  
```
or
```bash
$ cd ./SSync
$ dotnet run "../examples/subtitle.srt" --o "../examples/output.srt" --s -10 --r "nQo":"não" --r "NQo":"Não"  
```
or
```bash
$ dotnet run --project ./SSync "examples/subtitle.srt" --output "examples/output.srt" --seconds -10 --replacements "nQo":"não" --replacements "NQo":"Não"  
```

This will read the `subtitle.srt` file and output the changes to `changes-test.srt` file. It will subtract 10 seconds from the timestamps and replace "nQo" occurrences to "não" and "NQo" to "Não".


*Obrigatory parameters:* 
- `inputFileName`: This is the first argument, can be the full path or the file only.

*Optional parameters:*
- `--output` or `--o`: The path or just the name of the output file. Defaults to 'output.srt'
- `--hours` or `--h`: The offset in hours. Defaults to 0.
- `--minutes` or `--m`: The offset in minutes. Defaults to 0.
- `--seconds` or `--s`: The offset in seconds. Defaults to 0.
- `--milliseconds` or `--ms`: The offset in milliseconds. Defaults to 0.
- `--replacements` or `--r`: The substitutions/replacements to be done in the subtitle. Can be repeated N times. The structure is "valueToBeReplacedInSubtitle":"valueToBeReplacedWith".


*Important*: Tested on netcore 6 and a macbook. Haven't tested on Windows or Linux machines, but i'm almost sure it will work.


## How to test

```cs
$ dotnet test
```