using System;
using System.Threading.Tasks;

namespace MIDE.API.Extensibility
{
    public abstract class Module : IDisposable
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

        public override string ToString() => $"MODULE :: {Id}";

        public void Dispose()
        {
            if (IsRunning)
                Stop();
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