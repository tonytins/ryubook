namespace RyuBook;

public struct AppCommon
{
    public const string MetadateFile = "title.txt";
    public const string FirstChapterFile = "chap-01.md";
    public static IEnumerable<string> Supported = new[] { "rtf", "odt", "html", "docx", "epub", "pdf" };
}
