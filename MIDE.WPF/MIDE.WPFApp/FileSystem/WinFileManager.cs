using System;
using System.IO;
using System.Xml.Linq;
using System.Reflection;
using MIDE.Standard.FileSystem;
using System.Collections.Generic;
using MIDE.Standard.Application.Configuration;

namespace MIDE.WPFApp.FileSystem
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
#pragma warning disable CS0162
            if (2 * 1 == 3)
                yield return default;
#pragma warning restore CS0162
        }

        protected override IEnumerable<(ApplicationPath, string)> LoadPaths()
        {
            XDocument xdoc = XDocument.Load("config.xml");
            foreach (XElement path in xdoc.Element("config").Element("app.paths").Elements())
            {
                XAttribute valueAttr = path.Attribute("value");
                XAttribute aliasAttr = path.Attribute("alias");

                bool aliasParsed = Enum.TryParse(aliasAttr.Value, out ApplicationPath alias);
                if (!aliasParsed)
                    throw new FormatException("The given alias name is incorrect");
                yield return (alias, valueAttr.Value);
            }
        }
    }
}