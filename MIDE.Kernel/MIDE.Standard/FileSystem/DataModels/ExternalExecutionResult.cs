namespace MIDE.FileSystem
{
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