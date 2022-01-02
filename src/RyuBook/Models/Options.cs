using RyuBook.Interface;

namespace RyuBook.Models;

class BaseOptions : IOptions
{
    [Option('d', "dir")] public string Folder { get; set; } = Environment.CurrentDirectory;
}


[Verb("build", HelpText = "Compiles the book as a ePub.")]
class BuildOption : BaseOptions, IGenerateOptions
{
    [Option('t', "title")] public string Title { get; set; } = string.Empty;
    [Option('f', "format")] public string Format { get; set; } = string.Empty;
    [Option('v', "verbose")] public bool Verbose { get; set; } = false;
    public string Author { get; set; } = string.Empty; // Unused
}

[Verb("clean", HelpText = "Removes all books in the /build directory.")] class CleanOption : BaseOptions { }

[Verb("init", HelpText = "Creates a new project.")]
class InitOption : BaseOptions, IBookOptions
{
    [Option('a', "author")] public string Author { get; set; } = string.Empty;
    [Option('t', "title")] public string Title { get; set; } = string.Empty;
}
