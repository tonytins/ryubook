using CommandLine;

namespace RyuBook
{
    [Verb("build")]
    class BuildOption
    {
        [Option('n', "name")] public string BookName { get; set; }

        [Option('v', "verbose")] public bool Verbose { get; set; }
    }

    [Verb("clean")]
    class CleanOption
    {

    }
}