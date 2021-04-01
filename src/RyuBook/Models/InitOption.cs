using System;
using CommandLine;
using RyuBook.Interface;

namespace RyuBook.Models
{
    [Verb("init", HelpText = "Creates a new project.")]
    record InitOption : IProject
    {
        [Option('d', "dir")] public string Directory { get; init; } = Environment.CurrentDirectory;
        [Option('a', "author")] public string Author { get; init; } = string.Empty;
        [Option('t', "title")] public string Title { get; init; } = string.Empty;

        [Option('v', "verbose")] public bool Verbose { get; init; } = false;
    }
}