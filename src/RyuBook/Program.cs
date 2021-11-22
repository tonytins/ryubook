using System.Globalization;
using RyuBook.Interface;
using RyuBook.Models;

namespace RyuBook;

static class Program
{
    static readonly Random _rnd = new();

    static void Main(string[] args) => Parser.Default.ParseArguments<IBuildOptions, IBookOptions, IGenerateOptions>(args)
            .WithParsed<CleanOption>(options =>
            {
                var books = Directory.EnumerateFiles(options.Folder)
                    .Where(fmt => fmt.EndsWith(".epub", StringComparison.OrdinalIgnoreCase)
                     /*
                      * While .doc output may not be supported by Pandoc,
                      * .docx can still be converted to .doc using other programs,
                      * such as LibreOffice.
                      */
                     || fmt.EndsWith(".docx", StringComparison.OrdinalIgnoreCase)
                     || fmt.EndsWith(".doc", StringComparison.OrdinalIgnoreCase)
                     || fmt.EndsWith(".odt", StringComparison.OrdinalIgnoreCase)
                     || fmt.EndsWith(".rtf", StringComparison.OrdinalIgnoreCase)
                     || fmt.EndsWith(".html", StringComparison.OrdinalIgnoreCase)
                     || fmt.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase));

                foreach (var book in books)
                {
                    var path = Path.Combine(options.Folder, book);

                    if (File.Exists(path))
                        File.Delete(path);
                }
            })
            .WithParsed<InitOption>(options =>
            {
                var srcDir = Path.Combine(options.Folder, "src");

                // If source directory exists, exit
                if (Directory.Exists(srcDir))
                    return;

                // Files
                var metadataFile = Path.Combine(srcDir, AppConsts.MetadateFile);
                var firstChapterFile = Path.Combine(srcDir, AppConsts.FirstChapterFile);

                // Metadata
                var projAuthor = string.IsNullOrEmpty(options.Author)
                ? "Lorem Ipsum"
                : options.Author;
                var projTitle = string.IsNullOrEmpty(options.Author)
                    ? "Book Title"
                    : options.Title;

                /*
                 * It's easier (and more readable) to construct the metadata out of an array
                 * because the content isn't very dense.
                 */
                var metadata = new[] {
                        $"---",
                        $"title: {projTitle}",
                        $"author: {projAuthor}",
                        $"language: {CultureInfo.CurrentCulture.Name}",
                        "---"
                };

                var gitignore = new[] { "*.rtf", "*.odt", "*.html", "*.doc[x]", "*.epub", "*.pdf" };

                Directory.CreateDirectory(srcDir);
                File.WriteAllTextAsync(Path.Combine(srcDir, firstChapterFile), $"# Chapter 1{Environment.NewLine}");
                File.WriteAllLinesAsync(Path.Combine(srcDir, metadataFile), metadata);
                File.WriteAllLinesAsync(Path.Combine(options.Folder, ".gitignore"), gitignore);

                if (SysCheck.IfGitExists)
                {
                    var pd = new ProcessStartInfo("git")
                    {
                        WindowStyle = ProcessWindowStyle.Minimized,
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        Arguments = "init"
                    };
                    Process.Start(pd);
                }
            })
            .WithParsed<BuildOption>(build =>
            {
                var srcDirectory = Path.Combine(build.Folder, "src");

                var allFormats = new[] { "rtf", "odt", "html", "docx", "epub", "pdf" };

                if (build.Format.Contains("list", StringComparison.OrdinalIgnoreCase))
                {
                    var fmtAggregate = allFormats.OrderBy(r => _rnd.Next(allFormats.Length))
                    .Aggregate(string.Empty, (current, fmt) => current + $"{fmt}, ");

                    const string endComma = ", ";
                    Console.WriteLine($"These formats are supported: {fmtAggregate.TrimEnd(endComma.ToCharArray())}.");
                }
                else
                {
                    try
                    {
                        var dirInfo = new DirectoryInfo(build.Folder);
                        var bookTitle = dirInfo.Name;

                        if (!SysCheck.IfPandocExists && !Directory.Exists(srcDirectory))
                            return;

                        Generator.Export(build);

                    }
                    catch (IOException err)
                    {
                        Console.WriteLine(Debugger.IsAttached
                            ? $"{err.Message}{Environment.NewLine}{err.StackTrace}"
                            : err.Message);
                    }
                }

            });
}
