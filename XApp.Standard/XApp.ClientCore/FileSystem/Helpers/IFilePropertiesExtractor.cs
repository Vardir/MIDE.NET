using System.Collections.Generic;

namespace Vardirsoft.XApp.FileSystem
{
    public interface IFilePropertiesExtractor
    {
        IEnumerable<(string prop, string val)> ExtractProperties(string path);
    }
}