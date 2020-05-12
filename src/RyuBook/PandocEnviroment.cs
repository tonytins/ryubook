using System.Diagnostics;
using System.IO;

namespace RyuBook
{
    public struct PandocEnviroment
    {
        public static bool IfPandocExists
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
    }
}
