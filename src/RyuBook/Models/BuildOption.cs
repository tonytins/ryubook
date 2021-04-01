using System;
using CommandLine;
using RyuBook.Interface;

namespace RyuBook.Models
{
    [Verb("build", HelpText = "Compiles the book as a ePub.")]
    record BuildOption : IBuild
    {
        [Option('d', "dir")] public string Directory { get; init; } = Environment.CurrentDirectory;
        [Option('t', "title")] public string Title { get; init; } = string.Empty;
        [Option('f', "format")] public string Format { get; init; } = string.Empty;
        [Option('v', "verbose")] public bool Verbose { get; init; } = false;
    }
}
