using System.IO;

namespace RyuBook
{
    public struct BookConsts
    {
        public static readonly string BOOK_TITLE = Path.Combine("src", "title.txt");
        public static readonly string BOOK_CONTENT = Path.Combine("src", "book.md");
    }
}