namespace RyuBook.Interface;

public interface IBookOptions : IOptions
{
    string Title { get; set; }
    string Author { get; set; }
}

