using System.Collections.Generic;

namespace XApp.FileSystem
{
    public interface IFilePropertiesExtractor
    {
        IEnumerable<(string prop, string val)> ExtractProperties(string path);
    }
}