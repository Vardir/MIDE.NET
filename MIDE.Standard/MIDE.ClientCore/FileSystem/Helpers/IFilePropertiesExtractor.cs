using System.Collections.Generic;

namespace MIDE.FileSystem
{
    public interface IFilePropertiesExtractor
    {
        IEnumerable<(string prop, string val)> ExtractProperties(string path);
    }
}