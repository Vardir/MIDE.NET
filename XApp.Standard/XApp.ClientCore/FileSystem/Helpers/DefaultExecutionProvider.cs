using System.Diagnostics;

using XApp.Helpers;

namespace XApp.FileSystem
{
    public class DefaultExecutionProvider : IExecutionProvider
    {
        public virtual void ExecuteExternalApplicationAsync(string path, string arguments,
                                                            DataReceivedEventHandler outputHandler, DataReceivedEventHandler errorHandler)
        {
            var process = new Process();
            process.StartInfo.FileName = path;
            process.StartInfo.Arguments = arguments;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.CreateNoWindow = true;

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
            var process = new Process();
            process.StartInfo.FileName = path;
            process.StartInfo.Arguments = arguments;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.Start();

            var output = process.StandardOutput.ReadToEnd();
            var err = process.StandardError.ReadToEnd();

            process.WaitForExit();

            return new ExternalExecutionResult(process.ExitCode, output, err);
        }
    }
}