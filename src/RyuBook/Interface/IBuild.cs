namespace RyuBook.Interface
{
    public interface IBuild
    {
        string Directory { get; init; }
        string Title { get; init; }
        string Format { get; init; }
        bool Verbose { get; init; }
    }
}