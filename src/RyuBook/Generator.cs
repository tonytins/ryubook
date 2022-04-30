// This project is licensed under the BSD 3-Clause license.
// See the LICENSE file in the project root for more information.
using RyuBook.Interface;

namespace RyuBook;

public static class Generator
{
    public static void Export(IGenerateOptions gen)
    {
        var srcPath = Path.Combine(gen.Folder, "src");

        // Remove whitespace and make all letters lowercase
        var projTitle = gen.Title
            .Replace("\u0020", string.Empty)
            .ToLowerInvariant();

        var allChapters = string.Empty;
        var listChapters = Directory.EnumerateFiles(srcPath)
            .Where(fmt => fmt.EndsWith(".md", StringComparison.OrdinalIgnoreCase)
                || fmt.EndsWith(".markdown", StringComparison.OrdinalIgnoreCase))
            .ToList();

        // Sort files in alphabetical order to avoid chapters being arranged in the wrong order
        listChapters.Sort();

        allChapters = listChapters.Aggregate(allChapters,
            (current, chapter) => current + @$" {Path.Combine(srcPath, chapter)} ");

        var bookSrc = $"{Path.Combine(srcPath, AppCommon.MetadateFile)} {allChapters}";

        // If "title" is empty, output book in the respective file
        var args = $"{bookSrc} --strip-comments -o {projTitle}.{gen.Format}";

        // Pandoc doesn't support the legacy DOC format
        if (gen.Format.Contains("doc"))
            gen.Format = "docx";

        if (gen.Format.Contains("pdf"))
            args = $" {bookSrc} -t html --strip-comments --toc -o {projTitle}.{gen.Format}";

        if (gen.Format.Contains("epub"))
            args = $"{bookSrc} --strip-comments --toc -o {projTitle}.{gen.Format}";

        var procInfo = new ProcessStartInfo("pandoc")
        {
            WindowStyle = ProcessWindowStyle.Minimized,
            UseShellExecute = false,
            RedirectStandardOutput = false,
            Arguments = args,
        };

        if (Debugger.IsAttached || gen.Verbose)
            Console.WriteLine($"pandoc {args}");

        if (AppCommon.Supported.Contains(gen.Format))
            Process.Start(procInfo);
    }
}

