namespace RyuBook.Models;

using RyuBook.Interface;

[Verb("init", HelpText = "Creates a new project.")]
class InitOption : BaseOptions, IBookOptions
{
    [Option('a', "author")] public string Author { get; set; } = string.Empty;
    [Option('t', "title")] public string Title { get; set; } = string.Empty;
}
