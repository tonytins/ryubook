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

                    if (Directory.Exists(_buildPath))
                    {
                        var books = Directory.GetFiles(_buildPath, "*.epub");

                        foreach (var book in books)
                            File.Delete(Path.Combine(_buildPath, book));
                    }
                })
                .WithParsed<InitOption>(o =>
                {
                    var cfgFile = Path.Combine(Environment.CurrentDirectory, AppConsts.ConfigFile);
                    var cfg = new ProjectFile();

                    if (!EnviromentCheck.IsDirectory)
                        Directory.CreateDirectory(Path.Combine(Environment.CurrentDirectory, "src"));

                    if (!File.Exists(cfgFile))
                        Toml.WriteFile(cfg, cfgFile);
                })
                .WithParsed<BuildOption>(o =>
                {
                    if (!Directory.Exists(_buildPath))
                        Directory.CreateDirectory("build");

                    if (EnviromentCheck.IsDirAndPandoc)
                        GenerateBook(string.IsNullOrEmpty(o.BookName) ? ProjectFile.GetProject.Name : o.BookName,
                            o.Verbose);
                });
        }

        static void GenerateBook(string name, bool verbose)
        {
            var book = $"{Path.Combine(Environment.CurrentDirectory, AppConsts.BookTitle)} {Path.Combine(Environment.CurrentDirectory, AppConsts.BookContent)}";

            var pdArgs = string.IsNullOrEmpty(name)
                ? $"{book} -o {Path.Combine(_buildPath, "book.epub")}"
                : $"{book} -o {Path.Combine(_buildPath, $"{name}.epub")}";

            var procInfo = new ProcessStartInfo("pandoc")
            {
                WindowStyle = ProcessWindowStyle.Minimized,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                Arguments = pdArgs,
            };

            if (verbose)
            {
                procInfo.UseShellExecute = true;
                procInfo.RedirectStandardOutput = false;
            }

            Process.Start(procInfo);
        }
    }
}