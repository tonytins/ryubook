using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using CommandLine;
using Nett;

namespace RyuBook
{
    internal static class Program
    {
        static readonly string _buildPath = Path.Combine(Environment.CurrentDirectory, "build");

        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<InitOption, BuildOption, CleanOption>(args)
                .WithParsed<CleanOption>(o =>
                {
                    var books = Directory.EnumerateFiles(_buildPath, "*.*")
                        .Where(f => f.EndsWith(".epub", StringComparison.OrdinalIgnoreCase)
                                    || f.EndsWith(".docx", StringComparison.OrdinalIgnoreCase)
                                    || f.EndsWith(".rtf", StringComparison.OrdinalIgnoreCase)
                                    || f.EndsWith(".html", StringComparison.OrdinalIgnoreCase));

                    foreach (var book in books)
                        File.Delete(Path.Combine(_buildPath, book));
                })
                .WithParsed<InitOption>(o =>
                {
                    var projFile = Path.Combine(Environment.CurrentDirectory, AppConsts.ProjectFile);
                    var proj = new Project();

                    // If source directory doesn't exist, create it
                    if (!EnviromentCheck.IsSrcDirectory)
                        Directory.CreateDirectory(Path.Combine(Environment.CurrentDirectory, "src"));

                    // If project file doesn't exist, create it
                    if (!File.Exists(projFile))
                        Toml.WriteFile(proj, projFile);
                })
                .WithParsed<BuildOption>(o =>
                {
                    // If the /build directory doesn't exist, create it.
                    if (!Directory.Exists(_buildPath))
                        Directory.CreateDirectory(_buildPath);

                    if (!EnviromentCheck.IsSrcDirAndPandoc) return;

                    if (string.IsNullOrEmpty(o.Format))
                        GenerateBook(File.Exists(AppConsts.ProjectFile)
                        ? Project.GetProject.Title
                        : o.Title, o.Format);
                    else
                        GenerateBook(File.Exists(AppConsts.ProjectFile)
                            ? Project.GetProject.Title
                            : o.Title);
                });
        }

        static void GenerateBook(string title)
            => GenerateBook(title, string.Empty);
        static void GenerateBook(string title,  string format)
        {
            var book = $"{Path.Combine(Environment.CurrentDirectory, AppConsts.MetadateFile)} {Path.Combine(Environment.CurrentDirectory, AppConsts.ContentFile)}";

            var pdArgs = string.IsNullOrEmpty(format) ?
                PandocArgs(book, title) :
                PandocArgs(book, title, format);

            if (format.Contains("doc") || format.Contains("docx"))
                pdArgs = PandocArgs(book, title, "docx");

            if (format.Contains("html")) pdArgs = PandocArgs(book, title, "html");

            if (format.Contains("rtf")) pdArgs = PandocArgs(book, title, "rtf");

            var procInfo = new ProcessStartInfo("pandoc")
            {
                WindowStyle = ProcessWindowStyle.Minimized,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                Arguments = pdArgs,
            };

            Process.Start(procInfo);
        }

        static string PandocArgs(string book, string title, string format = "epub")
        {
            // Remove whitespace and make all letters lowercase
            var projTitle = title
                .Replace("\u0020", string.Empty)
                .ToLowerInvariant();

            // If "title" is empty, output "book.epub"
            return string.IsNullOrEmpty(title)
                ? $"{book} -o {Path.Combine(_buildPath, $"book.{format}")}"
                : $"{book} -o {Path.Combine(_buildPath, $"{projTitle}.{format}")}";
        }
    }
}
