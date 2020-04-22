using System;
using System.Diagnostics;
using System.IO;
using CommandLine;
using Nett;

namespace RyuBook
{
    internal static class Program
    {
        const string _buildPath = "build";

        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<InitOption, BuildOption, CleanOption>(args)
                .WithParsed<CleanOption>(o =>
                {
                    if (!EnviromentCheck.IsDirAndPandoc) return;

                    var books = Directory.GetFiles(Path.Combine(Environment.CurrentDirectory, _buildPath), "*.epub");
                    foreach (var book in books)
                        File.Delete(Path.Combine(_buildPath, book));
                })
                .WithParsed<InitOption>(o =>
                {
                    var cfgFile = Path.Combine(Environment.CurrentDirectory, AppConsts.ConfigFile);
                    var cfg = new Settings();

                    if (!EnviromentCheck.IsDirectory)
                        Directory.CreateDirectory(Path.Combine(Environment.CurrentDirectory, "src"));

                    if (!File.Exists(cfgFile))
                        Toml.WriteFile(cfg, cfgFile);
                })
                .WithParsed<BuildOption>(o =>
                {
                    if (!Directory.Exists(Path.Combine(Environment.CurrentDirectory, _buildPath)))
                        Directory.CreateDirectory("build");

                    if (EnviromentCheck.IsDirAndPandoc)
                        GenerateBook(string.IsNullOrEmpty(o.BookName) ? Settings.GetSettings.Name : o.BookName,
                            o.Verbose);
                });
        }

        static void GenerateBook(string name, bool verbose)
        {
            var book = $"{Path.Combine(Environment.CurrentDirectory, AppConsts.BookTitle)} {Path.Combine(Environment.CurrentDirectory, AppConsts.BookContent)}";
            
            var pdArgs = string.IsNullOrEmpty(name)
                ? $"{book} -o {Path.Combine(Environment.CurrentDirectory, _buildPath, "book.epub")}"
                : $"{book} -o {Path.Combine(Environment.CurrentDirectory, _buildPath, $"{name}.epub")}";

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