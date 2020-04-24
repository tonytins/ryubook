using System;
using System.Diagnostics;
using System.IO;
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
                    var books = Directory.GetFiles(_buildPath, "*.epub");

                    // If no books exist, do nothing
                    if (books.Length == 0) return;

                    foreach (var book in books)
                        File.Delete(Path.Combine(_buildPath, book));
                })
                .WithParsed<InitOption>(o =>
                {
                    var cfgFile = Path.Combine(Environment.CurrentDirectory, AppConsts.ConfigFile);
                    var proj = new ProjectFile();

                    // If source directory doesn't exist, create it
                    if (!EnviromentCheck.IsSrcDirectory)
                        Directory.CreateDirectory(Path.Combine(Environment.CurrentDirectory, "src"));

                    // If project file doesn't exist, create it
                    if (!File.Exists(cfgFile))
                        Toml.WriteFile(proj, cfgFile);
                })
                .WithParsed<BuildOption>(o =>
                {
                    // If the /build directory doesn't exist, create it.
                    if (!Directory.Exists(_buildPath))
                        Directory.CreateDirectory(_buildPath);

                    if (!EnviromentCheck.IsSrcDirAndPandoc) return;

                    GenerateBook(File.Exists(AppConsts.ConfigFile) ? ProjectFile.GetProject.Title : o.Title);
                });
        }

        static void GenerateBook(string title)
        {
            var book = $"{Path.Combine(Environment.CurrentDirectory, AppConsts.MetadateFile)} {Path.Combine(Environment.CurrentDirectory, AppConsts.ContentFile)}";

            var newTitle = title
                .Replace("\u0020", string.Empty)
                .ToLowerInvariant();

            var pdArgs = string.IsNullOrEmpty(title)
                ? $"{book} -o {Path.Combine(_buildPath, "book.epub")}"
                : $"{book} -o {Path.Combine(_buildPath, $"{newTitle}.epub")}";

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
