using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using CommandLine;
using Nett;

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
                        .Where(f => f.EndsWith(".epub", StringComparison.OrdinalIgnoreCase)
                                    || f.EndsWith(".docx", StringComparison.OrdinalIgnoreCase)
                                    /* While output to .doc may not be supported by Pandoc,
                                     .docx can still be converted to .doc using other programs,
                                     such as LibreOffice. */
                                    || f.EndsWith(".doc", StringComparison.OrdinalIgnoreCase)
                                    || f.EndsWith(".odt", StringComparison.OrdinalIgnoreCase)
                                    || f.EndsWith(".rtf", StringComparison.OrdinalIgnoreCase)
                                    || f.EndsWith(".html", StringComparison.OrdinalIgnoreCase));

                    foreach (var book in books)
                        File.Delete(Path.Combine(AppConsts.BuildPath, book));
                })
                .WithParsed<InitOption>(o =>
                {
                    // If source directory exists, exit
                    if (EnviromentCheck.IsSrcDirectory) return;

                    var projAuthor = string.IsNullOrEmpty(o.Author)
                        ? "Lorem Ipsum"
                        : o.Author;
                    var projTitle = string.IsNullOrEmpty(o.Author)
                        ? "Book Title"
                        : o.Title;
                    var metadataFile = Path.Combine(AppConsts.SrcPath, AppConsts.MetadateFile);
                    var contentFile = Path.Combine(AppConsts.SrcPath, AppConsts.ContentFile);

                    Directory.CreateDirectory(AppConsts.SrcPath);
                    File.WriteAllText(Path.Combine(AppConsts.SrcPath, contentFile), "# Your book");
                    File.WriteAllText(Path.Combine(AppConsts.SrcPath, metadataFile), $"% {projTitle}{Environment.NewLine}% {projAuthor}");
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
                        Console.ReadKey();
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
            var book = $"{Path.Combine(AppConsts.SrcPath, AppConsts.MetadateFile)} {Path.Combine(AppConsts.SrcPath, AppConsts.ContentFile)}";

            // Remove whitespace and make all letters lowercase
            var projTitle = title
                .Replace("\u0020", string.Empty)
                .ToLowerInvariant();

            // If "title" is empty, output "book.epub"
            var pdArgs = string.IsNullOrEmpty(title)
                ? $"{book} -o {Path.Combine(AppConsts.BuildPath, $"book.{format}")}"
                : $"{book} -o {Path.Combine(AppConsts.BuildPath, $"{projTitle}.{format}")}";

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
