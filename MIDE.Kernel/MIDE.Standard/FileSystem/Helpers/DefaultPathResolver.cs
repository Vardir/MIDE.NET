using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace MIDE.FileSystem
{
    public class DefaultPathResolver : IFileResolver
    {
        private static DefaultPathResolver instance;
        public static DefaultPathResolver Instance => instance ?? (instance = new DefaultPathResolver());

        protected DefaultPathResolver() { }

        public virtual void ExecuteExternalApplicationAsync(string path, string arguments,
                                                            DataReceivedEventHandler outputHandler, DataReceivedEventHandler errorHandler)
        {
            Process process = new Process();
            process.StartInfo.FileName = path;
            process.StartInfo.Arguments = arguments;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.CreateNoWindow = true;
            if (outputHandler != null)
                process.OutputDataReceived += outputHandler;
            if (errorHandler != null)
                process.ErrorDataReceived += errorHandler;
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
        }

        public virtual ExternalExecutionResult ExecuteExternalApplication(string path, string arguments)
        {
            Process process = new Process();
            process.StartInfo.FileName = path;
            process.StartInfo.Arguments = arguments;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            string err = process.StandardError.ReadToEnd();
            process.WaitForExit();
            return new ExternalExecutionResult(process.ExitCode, output, err);
        }
        public virtual IEnumerable<(string prop, string val)> ExtractProperties(string path)
        {
            throw new NotImplementedException();
        }
    }
}