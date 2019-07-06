using System.Collections.Generic;

namespace MIDE.API
{
    public interface IFilePropertiesExtractor
    {
        IEnumerable<(string prop, string val)> ExtractProperties(string path);
    }
}