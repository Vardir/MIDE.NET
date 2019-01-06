using System.Reflection;
using System.Collections.Generic;
using MIDE.Application.Configuration;

namespace MIDE.FileSystem
{
    public abstract class FileManager
    {
        protected Dictionary<ApplicationPath, string> paths;

        public FileManager()
        {
            paths = new Dictionary<ApplicationPath, string>();
            foreach (var path in LoadPaths())
            {
                if (!paths.ContainsKey(path.Item1))
                    paths.Add(path.Item1, path.Item2);
                else
                    paths[path.Item1] = path.Item2;
            }
        }

        public virtual string GetPath(ApplicationPath folder) => paths[folder];

        public abstract string MapPath(string path);
        public abstract Assembly LoadAssembly(string path);
        public abstract IEnumerable<Config> LoadConfigurations();

        protected abstract IEnumerable<(ApplicationPath, string)> LoadPaths();
    }

    public enum ApplicationPath
    {
        UserSettings, DefaultForSolutions,
        AppAssets, Root, Installed, Themes
    }
}