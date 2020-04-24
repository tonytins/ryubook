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

        public static bool IsSrcDirectory
        {
            get
            {
                try
                {
                    return (File.Exists(Path.Combine(Environment.CurrentDirectory, AppConsts.MetadateFile))) || File.Exists(Path.Combine(Environment.CurrentDirectory, AppConsts.ContentFile));
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
                   return IsSrcDirectory && IsPandoc;

                }
                catch { return false; }

            }
        }
    }
}
