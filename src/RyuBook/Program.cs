using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using CommandLine;

namespace RyuBook
{
    internal static class Program
    {
        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<InitOption, BuildOption, CleanOption>(args)
                .WithParsed<CleanOption>(o =>
                {
                    var books = Directory.EnumerateFiles(AppConsts.BuildPath, "*.*")
                        .Where(fmt => fmt.EndsWith(".epub", StringComparison.OrdinalIgnoreCase)
                                    || fmt.EndsWith(".docx", StringComparison.OrdinalIgnoreCase)
                                    /* While .doc output may not be supported by Pandoc,
                                     .docx can still be converted to .doc using other programs,
                                     such as LibreOffice. */
                                    || fmt.EndsWith(".doc", StringComparison.OrdinalIgnoreCase)
                                    || fmt.EndsWith(".odt", StringComparison.OrdinalIgnoreCase)
                                    || fmt.EndsWith(".rtf", StringComparison.OrdinalIgnoreCase)
                                    || fmt.EndsWith(".html", StringComparison.OrdinalIgnoreCase));

                    foreach (var book in books)
                    {
                        var path = Path.Combine(AppConsts.BuildPath, book);

                        if (File.Exists(path))
                            File.Delete(path);
                    }
                })
                .WithParsed<InitOption>(o =>
                {
                    // If source directory exists, exit
                    if (EnviromentCheck.IsSrcDirectory) return;

                    // Files
                    var metadataFile = Path.Combine(AppConsts.SrcPath, AppConsts.MetadateFile);
                    var contentFile = Path.Combine(AppConsts.SrcPath, AppConsts.FirstChapterFile);

                    // Metadata
                    var projAuthor = string.IsNullOrEmpty(o.Author)
                        ? "Lorem Ipsum"
                        : o.Author;
                    var projTitle = string.IsNullOrEmpty(o.Author)
                        ? "Book Title"
                        : o.Title;
                    var metadata = new[] {
                        $"title: {projTitle}{Environment.NewLine}",
                        $"author: {projAuthor}{Environment.NewLine}"
                    };
                    var metaContent = string.Empty;

                    // Iterate over an array of metadata and add it
                    // to the metadata content
                    foreach (var data in metadata)
                        metaContent += data;

                    Directory.CreateDirectory(AppConsts.SrcPath);
                    File.WriteAllText(Path.Combine(AppConsts.SrcPath, contentFile), "# Your book");
                    File.WriteAllText(Path.Combine(AppConsts.SrcPath, metadataFile), $"---{Environment.NewLine}{metaContent}{Environment.NewLine}---");
                })
                .WithParsed<BuildOption>(o =>
                {
                    // If the /build directory doesn't exist, create it.
                    if (!Directory.Exists(AppConsts.BuildPath))
                        Directory.CreateDirectory(AppConsts.BuildPath);

                    var allFmt = new[] { "rtf", "odt", "html", "docx", "epub" };

                    if (o.Format.Contains("list", StringComparison.OrdinalIgnoreCase))
                    {
                        var fmts = string.Empty;

                        foreach (var fmt in allFmt)
                            fmts += $"{fmt}, ";

                        var endComma = ", ";
                        var fmtsList = fmts.TrimEnd(endComma.ToCharArray());

                        Console.WriteLine($"These formats are supported: {fmtsList}.");
                    }
                    else
                    {
                        try
                        {
                            var metaDateFile = File.ReadLines(Path.Combine(AppConsts.SrcPath, AppConsts.MetadateFile));
                            var bookTitle = metaDateFile.First().Replace("%\u0020", string.Empty);

                            if (!EnviromentCheck.IsSrcDirAndPandoc) return;

                            if (o.Format.Contains("doc", StringComparison.OrdinalIgnoreCase)
                                || o.Format.Contains("docx", StringComparison.OrdinalIgnoreCase))
                                GenerateBook(bookTitle, "docx");
                            else if (o.Format.Contains("odt", StringComparison.OrdinalIgnoreCase))
                                GenerateBook(bookTitle, "odt");
                            else if (o.Format.Contains("html", StringComparison.OrdinalIgnoreCase))
                                GenerateBook(bookTitle, "html");
                            else if (o.Format.Contains("rtf", StringComparison.OrdinalIgnoreCase))
                                GenerateBook(bookTitle, "rtf");
                            else if (o.Format.Contains("all", StringComparison.OrdinalIgnoreCase))
                                foreach (var fmt in allFmt)
                                    GenerateBook(bookTitle, fmt);
                            else
                                GenerateBook(bookTitle);
                        }
                        catch (IOException err)
                        {
                            if (Debugger.IsAttached)
                                Console.WriteLine($"{err.Message}{Environment.NewLine}{err.StackTrace}");
                            else
                                Console.WriteLine(err.Message);
                        }
                    }

                });
        }

        static void GenerateBook(string title, string format = "epub")
        {
            var allChapters = string.Empty;
            var chapters = Directory.EnumerateFiles(AppConsts.SrcPath,
                "*.*", SearchOption.AllDirectories)
                        .Where(fmt => fmt.EndsWith(".md", StringComparison.OrdinalIgnoreCase))
                        .Where(fmt => fmt.EndsWith(".markdown", StringComparison.OrdinalIgnoreCase));

            foreach (var chapter in chapters)
                allChapters += @$" {chapter}";

            // Remove whitespace and make all letters lowercase
            var projTitle = title
                .Replace("\u0020", string.Empty)
                .ToLowerInvariant();

            var bookSrc = $"{Path.Combine(AppConsts.SrcPath, AppConsts.MetadateFile)} {Path.Combine(AppConsts.SrcPath, allChapters)}";

            // If "title" is empty, output "book.epub"
            var pdArgs = string.IsNullOrEmpty(title)
                ? $"{bookSrc} -o {Path.Combine(AppConsts.BuildPath, $"book.{format}")}"
                : $"{bookSrc} -o {Path.Combine(AppConsts.BuildPath, $"{projTitle}.{format}")}";

            var procInfo = new ProcessStartInfo("pandoc")
            {
                WindowStyle = ProcessWindowStyle.Minimized,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                Arguments = pdArgs,
            };

            Process.Start(procInfo);
        }
    }
}
