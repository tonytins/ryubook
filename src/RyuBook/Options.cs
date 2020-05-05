using CommandLine;

namespace RyuBook
{
    [Verb("build", HelpText = "Compiles the book as a ePub.")]
    class BuildOption
    {
        [Option('t', "title")] public string Title { get; set; }
        [Option('f', "format")] public string Format { get; set; }
    }

    [Verb("clean", HelpText = "Removes all books in the /build directory.")]
    class CleanOption {}

    [Verb("init", HelpText = "Creates a new project.")]
    class InitOption {}
}
