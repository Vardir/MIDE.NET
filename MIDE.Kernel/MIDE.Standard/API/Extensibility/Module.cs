using System.Threading.Tasks;

namespace MIDE.Standard.API.Extensibility
{
    public abstract class Module
    {
        protected object _lock = new object();

        public bool IsRunning { get; private set; }
        public string Id { get; }
        public AppExtension Extension { get; internal set; }

        public Module(string id)
        {
            Id = id;
        }

        public void Stop()
        {
            //TODO: send notification to suspend the thread
        }
        public virtual void Unload()
        {
            //TODO: free up all the resources
        }

        public async Task<ModuleExecutionResult> Run()
        {
            IsRunning = true;
            var result = await Execute();
            IsRunning = false;
            return result;
        }        

        protected abstract Task<ModuleExecutionResult> Execute();
    }

    public struct ModuleExecutionResult
    {
        public readonly bool IsSuccess;
        public readonly string Message;
        public readonly object ReturnValue;

        public ModuleExecutionResult(object returnValue, string message = null)
        {
            Message = message;
            IsSuccess = message != null;
            ReturnValue = returnValue;
        }
    }
}