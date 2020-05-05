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
                    var projFile = Path.Combine(Environment.CurrentDirectory, AppConsts.ProjectFile);
                    var proj = new Project();
                    var metadataFile = Path.Combine(AppConsts.SrcPath, AppConsts.MetadateFile);
                    var contentFile = Path.Combine(AppConsts.SrcPath, AppConsts.ContentFile);
                    var sampleMetadata = $"% Book Title{Environment.NewLine}% Lorem Ipsum";

                    // If source directory doesn't exist, create it
                    if (!EnviromentCheck.IsSrcDirectory)
                    {
                        Directory.CreateDirectory(AppConsts.SrcPath);
                        File.WriteAllText(Path.Combine(AppConsts.SrcPath, contentFile), "# Your book");
                        File.WriteAllText(Path.Combine(AppConsts.SrcPath, metadataFile), sampleMetadata);
                    }

                    // If project file doesn't exist, create it
                    if (!File.Exists(projFile))
                        Toml.WriteFile(proj, projFile);
                })
                .WithParsed<BuildOption>(o =>
                {
                    // If the /build directory doesn't exist, create it.
                    if (!Directory.Exists(AppConsts.BuildPath))
                        Directory.CreateDirectory(AppConsts.BuildPath);

                    if (!EnviromentCheck.IsSrcDirAndPandoc) return;

                    if (o.Format.Contains("doc",  StringComparison.OrdinalIgnoreCase)
                        || o.Format.Contains("docx",  StringComparison.OrdinalIgnoreCase))
                    {
                        GenerateBook(File.Exists(AppConsts.ProjectFile)
                            ? Project.GetProject.Title
                            : o.Title, "docx");
                    }
                    else if (o.Format.Contains("odt",  StringComparison.OrdinalIgnoreCase))
                    {
                        GenerateBook(File.Exists(AppConsts.ProjectFile)
                            ? Project.GetProject.Title
                            : o.Title, "odt");
                    }
                    else if (o.Format.Contains("html",  StringComparison.OrdinalIgnoreCase))
                    {
                        GenerateBook(File.Exists(AppConsts.ProjectFile)
                            ? Project.GetProject.Title
                            : o.Title, "html");
                    }
                    else if (o.Format.Contains("rtf",  StringComparison.OrdinalIgnoreCase))
                    {

                        GenerateBook(File.Exists(AppConsts.ProjectFile)
                            ? Project.GetProject.Title
                            : o.Title, "rtf");
                    }
                    else if (o.Format.Contains("all", StringComparison.OrdinalIgnoreCase))
                    {
                        var allFmt = new[] {"rtf", "odt", "html", "docx", "epub"};

                        foreach (var fmt in allFmt)
                            GenerateBook(File.Exists(AppConsts.ProjectFile)
                                ? Project.GetProject.Title
                                : o.Title, fmt);

                    }
                    else
                        GenerateBook(File.Exists(AppConsts.ProjectFile)
                            ? Project.GetProject.Title
                            : o.Title);
                });
        }

        static void GenerateBook(string title,  string format = "epub")
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
