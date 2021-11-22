namespace RyuBook;

public static class Generator
{
    public static void Export(string title, string dir, string format = "epub", bool verbose = false)
    {
        var srcPath = Path.Combine(dir, "src");

        // Remove whitespace and make all letters lowercase
        var projTitle = title
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

        var bookSrc = $"{Path.Combine(srcPath, AppConsts.MetadateFile)} {allChapters}";

        // If "title" is empty, output book in the respective file
        var args = $"{bookSrc} --strip-comments -o {projTitle}.{format}";

        if (format.Contains("pdf"))
            args = $" {bookSrc} -t html --strip-comments --toc -o {projTitle}.{format}";

        if (format.Contains("epub"))
            args = $"{bookSrc} --strip-comments --toc -o {projTitle}.{format}";

        var procInfo = new ProcessStartInfo("pandoc")
        {
            WindowStyle = ProcessWindowStyle.Minimized,
            UseShellExecute = false,
            RedirectStandardOutput = false,
            Arguments = args,
        };

        if (Debugger.IsAttached || verbose)
            Console.WriteLine($"pandoc {args}");

        Process.Start(procInfo);
    }
}

