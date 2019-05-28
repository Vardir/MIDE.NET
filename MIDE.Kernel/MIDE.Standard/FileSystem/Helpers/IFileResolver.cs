using System.Diagnostics;
using System.Collections.Generic;

namespace MIDE.FileSystem
{
    public interface IFileResolver
    {
        void ExecuteExternalApplicationAsync(string path, string arguments,
                                             DataReceivedEventHandler outputHandler, DataReceivedEventHandler errorHandler);

        ExternalExecutionResult ExecuteExternalApplication(string path, string arguments);
        IEnumerable<(string prop, string val)> ExtractProperties(string path);
    }
}