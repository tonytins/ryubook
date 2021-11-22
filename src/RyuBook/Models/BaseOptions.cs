using RyuBook.Interface;

namespace RyuBook.Models;

class BaseOptions : IOptions
{
    [Option('d', "dir")] public string Folder { get; set; } = Environment.CurrentDirectory;
}
