namespace RyuBook.Models;

using RyuBook.Interface;

[Verb("build", HelpText = "Compiles the book as a ePub.")]
class BuildOption : BaseOptions, IGenerateOptions, IBuildOptions
{
    [Option('t', "title")] public string Title { get; set; } = string.Empty;
    [Option('f', "format")] public string Format { get; set; } = string.Empty;
    [Option('v', "verbose")] public bool Verbose { get; set; } = false;
    public string Author { get; set; } = string.Empty; // Unused
}
