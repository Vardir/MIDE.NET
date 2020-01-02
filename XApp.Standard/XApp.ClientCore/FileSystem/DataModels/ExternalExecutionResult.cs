namespace Vardirsoft.XApp.FileSystem
{
    public struct ExternalExecutionResult
    {
        public readonly int ExitCode;
        public readonly string Output;
        public readonly string Errors;

        public ExternalExecutionResult(int exitCode, string output, string errors)
        {
            Output = output;
            Errors = errors;
            ExitCode = exitCode;
        }
    }
}