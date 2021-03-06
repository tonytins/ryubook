using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using CommandLine;

namespace RyuBook
{
    internal static class Program
    {
        static readonly Random _rnd = new Random();
        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<InitOption, BuildOption, CleanOption>(args)
                .WithParsed<CleanOption>(o =>
                {
                    var books = Directory.EnumerateFiles(o.Directory)
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
                        var path = Path.Combine(o.Directory, book);

                        if (File.Exists(path))
                            File.Delete(path);
                    }
                })
                .WithParsed<InitOption>(o =>
                {
                    var srcDir = Path.Combine(o.Directory, "src");

                    // If source directory exists, exit
                    if (Directory.Exists(srcDir))
                        return;

                    // Files
                    var metadataFile = Path.Combine(srcDir, AppConsts.MetadateFile);
                    var firstChapterFile = Path.Combine(srcDir, AppConsts.FirstChapterFile);

                    // Metadata
                    var projAuthor = string.IsNullOrEmpty(o.Author)
                        ? "Lorem Ipsum"
                        : o.Author;
                    var projTitle = string.IsNullOrEmpty(o.Author)
                        ? "Book Title"
                        : o.Title;

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
                    File.WriteAllLinesAsync(Path.Combine(o.Directory, ".gitignore"), gitignore);

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
                .WithParsed<BuildOption>(o =>
                {
                    var srcDir = Path.Combine(o.Directory, "src");

                    var allFmt = new[] { "rtf", "odt", "html", "docx", "epub", "pdf" };

                    if (o.Format.Contains("list", StringComparison.OrdinalIgnoreCase))
                    {
                        var fmtAggregate = allFmt.OrderBy(r => _rnd.Next(allFmt.Length))
                        .Aggregate(string.Empty, (current, fmt) => current + $"{fmt}, ");

                        const string endComma = ", ";
                        Console.WriteLine($"These formats are supported: {fmtAggregate.TrimEnd(endComma.ToCharArray())}.");
                    }
                    else
                    {
                        try
                        {
                            var dirInfo = new DirectoryInfo(o.Directory);
                            var bookTitle = dirInfo.Name;

                            if (!SysCheck.IfPandocExists && !Directory.Exists(srcDir))
                                return;

                            if (o.Format.Contains("doc", StringComparison.OrdinalIgnoreCase)
                                || o.Format.Contains("docx", StringComparison.OrdinalIgnoreCase))
                                GenerateBook(bookTitle, o.Directory, "docx", o.Verbose);
                            else if (o.Format.Contains("odt", StringComparison.OrdinalIgnoreCase))
                                GenerateBook(bookTitle, o.Directory, "odt", o.Verbose);
                            else if (o.Format.Contains("html", StringComparison.OrdinalIgnoreCase))
                                GenerateBook(bookTitle, o.Directory, "html", o.Verbose);
                            else if (o.Format.Contains("rtf", StringComparison.OrdinalIgnoreCase))
                                GenerateBook(bookTitle, o.Directory, "rtf", o.Verbose);
                            else if (o.Format.Contains("pdf", StringComparison.OrdinalIgnoreCase))
                                GenerateBook(bookTitle, o.Directory, "pdf", o.Verbose);
                            else if (o.Format.Contains("all", StringComparison.OrdinalIgnoreCase))
                                foreach (var fmt in allFmt)
                                    GenerateBook(bookTitle, o.Directory, fmt, o.Verbose);
                            else
                                GenerateBook(bookTitle, o.Directory, verbose: o.Verbose);
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

        static void GenerateBook(string title, string dir, string format = "epub", bool verbose = false)
        {
            var srcPath = Path.Combine(dir, "src");

            // Remove whitespace and make all letters lowercase
            var projTitle = title
                .Replace("\u0020", string.Empty)
                .ToLowerInvariant();

            var allChapters = string.Empty;
            var listChapters = Directory.EnumerateFiles(srcPath)
                .Where(fmt => fmt.EndsWith(".md", StringComparison.OrdinalIgnoreCase)
                    || fmt.EndsWith(".markdown", StringComparison.OrdinalIgnoreCase))
                .ToList();

            // Sort files in alphabetical order to avoid chapters being arranged in the wrong order
            listChapters.Sort();

            allChapters = listChapters.Aggregate(allChapters,
                (current, chapter) => current + @$" {Path.Combine(srcPath, chapter)} ");

            var bookSrc = $"{Path.Combine(srcPath, AppConsts.MetadateFile)} {allChapters}";

            // If "title" is empty, output book in the respective file
            var args = $"{bookSrc} --strip-comments -o {projTitle}.{format}";

            if (format.Contains("pdf"))
                args = $" {bookSrc} -t html --strip-comments --toc -o {projTitle}.{format}";

            if (format.Contains("epub"))
                args = $"{bookSrc} --strip-comments --toc -o {projTitle}.{format}";

            var procInfo = new ProcessStartInfo("pandoc")
            {
                WindowStyle = ProcessWindowStyle.Minimized,
                UseShellExecute = false,
                RedirectStandardOutput = false,
                Arguments = args,
            };

            if (Debugger.IsAttached || verbose)
                Console.WriteLine($"pandoc {args}");

            Process.Start(procInfo);
        }
    }
}
