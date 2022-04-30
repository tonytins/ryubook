// This project is licensed under the BSD 3-Clause license.
// See the LICENSE file in the project root for more information.
namespace RyuBook;

public struct SysCheck
{
    public static bool DetectPandoc
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

    public static bool DetectGit
    {
        get
        {
            try
            {
                var pd = new ProcessStartInfo("git")
                {
                    WindowStyle = ProcessWindowStyle.Minimized,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    Arguments = "--version"
                };
                Process.Start(pd);
                return true;
            }
            catch { return false; }
        }
    }
}
