using System;

namespace RyuBook.Interface;

public interface IBuildOptions : IOptions
{
    string Format { get; set; }
    bool Verbose { get; set; }
}

