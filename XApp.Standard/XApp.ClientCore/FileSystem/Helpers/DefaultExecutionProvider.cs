using System.Diagnostics;

using Vardirsoft.Shared.Helpers;

namespace Vardirsoft.XApp.FileSystem
{
    public class DefaultExecutionProvider : IExecutionProvider
    {
        public virtual void ExecuteExternalApplicationAsync(string path, string arguments, DataReceivedEventHandler outputHandler, DataReceivedEventHandler errorHandler)
        {
            var process = new Process 
            {
                StartInfo =
                {
                    FileName = path,
                    Arguments = arguments,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                }
            };

            if (outputHandler.HasValue())
            {   
                process.OutputDataReceived += outputHandler;
            }

            if (errorHandler.HasValue())
            {   
                process.ErrorDataReceived += errorHandler;
            }

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
        }

        public virtual ExternalExecutionResult ExecuteExternalApplication(string path, string arguments)
        {
            var process = new Process 
            {
                StartInfo = 
                {
                    FileName = path,
                    Arguments = arguments,
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true
                }
            };
            process.Start();

            var output = process.StandardOutput.ReadToEnd();
            var err = process.StandardError.ReadToEnd();

            process.WaitForExit();

            return new ExternalExecutionResult(process.ExitCode, output, err);
        }
    }
}