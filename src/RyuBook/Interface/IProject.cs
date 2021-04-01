namespace RyuBook.Interface
{
    public interface IProject
    {
        string Directory { get; init; }
        string Author { get; init; }
        string Title { get; init; }
        bool Verbose { get; init; }
    }
}