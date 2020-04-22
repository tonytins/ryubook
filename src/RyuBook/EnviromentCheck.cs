using System;
using System.Diagnostics;
using System.IO;

namespace RyuBook
{
    public struct EnviromentCheck
    {
        static bool IsPandoc
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

        public static bool IsDirectory
        {
            get
            {
                try
                {
                    return (File.Exists(Path.Combine(Environment.CurrentDirectory, BookConsts.BOOK_TITLE))) || File.Exists(Path.Combine(Environment.CurrentDirectory, BookConsts.BOOK_CONTENT));
                }
                catch { return false; }
            }
        }

        public static bool IsDirAndPandoc
        {
            get
            {
                try
                {
                   return IsDirectory && IsPandoc;

                }
                catch { return false; }

            }
        }
    }
}