using System;
using Nett;
using System.IO;

namespace RyuBook
{
    public class ProjectFile
    {
        public string Title { get; set; } = new DirectoryInfo(Environment.CurrentDirectory).Name;

        public static ProjectFile GetProject
        {
            get
            {
                var cfgFile = Path.Combine(Environment.CurrentDirectory, AppConsts.ConfigFile);

                return File.Exists(cfgFile) ? Toml.ReadFile<ProjectFile>(cfgFile) : new ProjectFile();
            }
        }
    }
}
