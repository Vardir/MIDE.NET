using System;
using System.IO;
using System.Reflection;
using MIDE.Standard.FileSystem;
using System.Collections.Generic;
using MIDE.Standard.Application.Configuration;

namespace MIDE.Kernel.ConsoleTest
{
    public class WinFileManager : FileManager
    {
        public void MakeFolder(string path)
        {
            string directory = Path.GetDirectoryName(path);
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);
        }

        public override string MapPath(string path)
        {
            throw new NotImplementedException();
        }
        public override Assembly LoadAssembly(string path)
        {
            throw new NotImplementedException();
        }
        public override IEnumerable<Config> LoadConfigurations()
        {
            //Mimic loading the configurations
            yield return new Config("custom_config_1", true);
            yield return new Config("custom_config_2", 10);
        }

        protected override IEnumerable<(ApplicationPath, string)> LoadPaths()
        {
            //Mimic loading the paths file
            yield return (ApplicationPath.AppAssets, "root/assets/");
            yield return (ApplicationPath.DefaultForSolutions, "root/solutions/");
            yield return (ApplicationPath.Installed, Assembly.GetExecutingAssembly().CodeBase);
            yield return (ApplicationPath.Root, "root/");
            yield return (ApplicationPath.Themes, "root/themes");
            yield return (ApplicationPath.UserSettings, "root/settings");
        }
    }
}