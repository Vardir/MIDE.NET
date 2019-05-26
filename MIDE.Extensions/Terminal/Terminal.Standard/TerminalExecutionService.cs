using MyCMD.Kernel.Commands;
using MIDE.API.Extensibility;

namespace Terminal
{
    public class TerminalExecutionService : ExecutionService
    {
        public IModuleExecutionListener CurrentListener { get; set; }
    }
}