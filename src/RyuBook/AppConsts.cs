using System.IO;

namespace RyuBook
{
    public struct AppConsts
    {
        public static readonly string MetadateFile = Path.Combine("src", "title.txt");
        public static readonly string ContentFile = Path.Combine("src", "book.md");
        public const string ConfigFile = "ryubook.toml";
    }
}
