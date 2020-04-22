using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using CommandLine;

namespace RyuBook
{
    class Program
    {
        static readonly string BOOK_TITLE = Path.Combine("src", "title.txt");
        static readonly string BOOK_CONTENT = Path.Combine("src", "book.md");

        static bool PandocCheck
        {
            get
            {
                try
                {
                    var pd = new ProcessStartInfo("pandoc")
                    {
                        WindowStyle = ProcessWindowStyle.Minimized,
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        Arguments = "-v"
                    };
                    Process.Start(pd);
                    return true;
                }
                catch { return false; }
            }
        }

        static void Main(string[] args)
        {
            if ((!File.Exists(Path.Combine(Environment.CurrentDirectory, BOOK_TITLE)))
                && !File.Exists(Path.Combine(Environment.CurrentDirectory, BOOK_CONTENT)))
            {
                Console.WriteLine("Unknown location.");
                return;
            }

            if (!PandocCheck)
            {
                Console.WriteLine("Pandoc not found");
                return;
            }

            Parser.Default.ParseArguments<BuildOption, CleanOption>(args)
                .WithParsed<CleanOption>(o =>
                {
                    var books = Directory.GetFiles(Environment.CurrentDirectory, "*.epub");
                    foreach (var book in books)
                        File.Delete(book);
                })
                .WithParsed<BuildOption>(o =>
                {
                    GenerateBook(o.BookName, o.Verbose);
                })
                .WithNotParsed(err =>
                {
                    Console.WriteLine("Could not parse command.");
                });
        }

        static void GenerateBook(string name, bool verbose)
        {
            var book =
                $"{Path.Combine(Environment.CurrentDirectory, BOOK_TITLE)} {Path.Combine(Environment.CurrentDirectory, BOOK_CONTENT)}";
            var pdArgs = string.IsNullOrEmpty(name) ? $"{book} -o book.epub" : $"{book} -o {name}.epub";
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