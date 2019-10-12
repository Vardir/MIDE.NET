using System.Collections.Generic;

namespace Vardirsoft.XApp.API
{
    public interface IFilePropertiesExtractor
    {
        IEnumerable<(string prop, string val)> ExtractProperties(string path);
    }
}