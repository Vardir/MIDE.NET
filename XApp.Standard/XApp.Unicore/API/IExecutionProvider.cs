using System.Diagnostics;

namespace Vardirsoft.XApp.API
{
    public interface IExecutionProvider
    {
        /// <summary>
        /// Asynchronously executes an external command or application using the given path and arguments 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="arguments"></param>
        /// <param name="outputHandler">A delegate to method to handle process' output messages, can be omitted</param>
        /// <param name="errorHandler">A delegate to method to handle process' error messages, can be omitted</param>
        /// <returns></returns>
        void ExecuteExternalApplicationAsync(string path, string arguments,
                                             DataReceivedEventHandler outputHandler, DataReceivedEventHandler errorHandler);

        /// <summary>
        /// Synchronously executes an external command or application using the given path and arguments 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="arguments"></param>
        /// <returns></returns>
        ExternalExecutionResult ExecuteExternalApplication(string path, string arguments);
    }

    public struct ExternalExecutionResult
    {
        public readonly int exitCode;
        public readonly string output;
        public readonly string errors;

        public ExternalExecutionResult(int exitCode, string output, string errors)
        {
            this.output = output;
            this.errors = errors;
            this.exitCode = exitCode;
        }
    }
}