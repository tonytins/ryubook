using System;
using Nett;
using System.IO;

namespace RyuBook
{
    public class Settings
    {
        public string Name { get; set; }

        public static Settings GetSettings
        {
            get
            {
                var cfgFile = Path.Combine(Environment.CurrentDirectory, "ryubook.toml");

                return File.Exists(cfgFile) ? Toml.ReadFile<Settings>(cfgFile) : new Settings();
            }
        }
    }
}