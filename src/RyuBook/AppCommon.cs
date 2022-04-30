// This project is licensed under the BSD 3-Clause license.
// See the LICENSE file in the project root for more information.
namespace RyuBook;

public struct AppCommon
{
    public const string MetadateFile = "title.txt";
    public const string FirstChapterFile = "chap-01.md";
    public static IEnumerable<string> Supported = new[] { "rtf", "odt", "html", "docx", "epub", "pdf" };
}
