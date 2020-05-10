using System;
using System.IO;

namespace RyuBook
{
    public struct AppConsts
    {
        public const string MetadateFile = "title.txt";
        public const string FirstChapterFile = "01-chapter1.md";

        public static readonly string BuildPath = Path.Combine(Environment.CurrentDirectory, "build");
        public static readonly string SrcPath = Path.Combine(Environment.CurrentDirectory, "src");
    }
}
