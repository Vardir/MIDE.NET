using System;
using Core.Commands;
using MIDE.API.Extensibility;

namespace Terminal
{
    public class TerminalModule : Module
    {
        public string Header { get; }
        public TerminalExecutionService ExecutionService { get; private set; }

        public TerminalModule() : base("terminal")
        {
            var assembly = typeof(ExecutionService).Assembly;
            var version = assembly.GetName().Version;
            Header = $"MyCMD v{version.Major}.{version.Minor}.{version.Build}";
        }

        protected override ModuleJob ExecuteCommand(object parameter, IModuleExecutionListener listener)
        {
            if (parameter == null)
                return new ModuleJob(ExecutionResult.Empty(), listener);
            if (!(parameter is string str))
                throw new ArgumentException("Expected string parameter for parser");

            ExecutionService.CurrentListener = listener;
            var resultJob = new ModuleJob(ExecutionService.Execute(str), listener);
            return resultJob;
        }

        public override void Initialize()
        {
            ExecutionService = new TerminalExecutionService();

            //activate parser
            ExecutionService.Execute("");
        }
    }
}