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

        public static bool IsSrcDirectory => Directory.Exists(AppConsts.SrcPath);

        public static bool IsSrcDirAndPandoc
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
