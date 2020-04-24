using System;
using Nett;
using System.IO;

namespace RyuBook
{
    public class Project
    {
        public string Title { get; set; } = new DirectoryInfo(Environment.CurrentDirectory).Name;

        public static Project GetProject
        {
            get
            {
                var cfgFile = Path.Combine(Environment.CurrentDirectory, AppConsts.ProjectFile);

                return File.Exists(cfgFile) ? Toml.ReadFile<Project>(cfgFile) : new Project();
            }
        }
    }
}
