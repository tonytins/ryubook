using System;
using System.Diagnostics;
using System.IO;

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
                && !File.Exists(Path.Combine(Environment.CurrentDirectory, BOOK_CONTENT))) { Console.WriteLine("Unknown location."); return; }
            if (!PandocCheck) { Console.WriteLine("Pandoc not found"); return; }

            var book = $"{Path.Combine(Environment.CurrentDirectory, BOOK_TITLE)} {Path.Combine(Environment.CurrentDirectory, BOOK_CONTENT)}";
            var pd = new ProcessStartInfo("pandoc")
            {
                WindowStyle = ProcessWindowStyle.Minimized,
                Arguments = $"{book} -o book.epub"
            };

            Process.Start(pd);
        }
    }
}