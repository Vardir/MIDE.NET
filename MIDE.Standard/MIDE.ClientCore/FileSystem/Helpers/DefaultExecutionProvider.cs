using System.Diagnostics;

namespace MIDE.FileSystem
{
    public class DefaultExecutionProvider : IExecutionProvider
    {
        private static DefaultExecutionProvider instance;
        public static DefaultExecutionProvider Instance => instance ?? (instance = new DefaultExecutionProvider());

        protected DefaultExecutionProvider() { }

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
    }
}