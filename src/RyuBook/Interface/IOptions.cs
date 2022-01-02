namespace RyuBook.Interface;

public interface IOptions
{
    string Folder { get; set; }
}

public interface IBuildOptions : IOptions
{
    string Format { get; set; }
    bool Verbose { get; set; }
}

public interface IBookOptions : IOptions
{
    string Title { get; set; }
    string Author { get; set; }
}

public interface IGenerateOptions : IBookOptions, IBuildOptions
{
}
