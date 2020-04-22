using System.IO;

namespace RyuBook
{
    public struct AppConsts
    {
        public static readonly string BookTitle = Path.Combine("src", "title.txt");
        public static readonly string BookContent = Path.Combine("src", "book.md");
        public const string ConfigFile = "ryubook.toml";
    }
}