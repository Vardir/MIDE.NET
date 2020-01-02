using System;
using System.Threading.Tasks;

using Vardirsoft.Shared.CustomImpl.Collections;

using Vardirsoft.XApp.Components;

namespace Vardirsoft.XApp.Extensibility
{
    /// <summary>
    /// A base class for application components that are intended to execute some specific jobs on parallel thread
    /// </summary>
    public abstract class Module : ApplicationComponent, IDisposable
    {
        private Queue<ModuleJob> _pendingJobs;
        
        protected object Lock = new object();

        public bool IsExecuting { get; private set; }
        public AppExtension Extension { get; internal set; }

        public Module(string id) : base(id)
        {
            _pendingJobs = new Queue<ModuleJob>();
        }

        public abstract void Initialize();
        public virtual void Unload()
        {
            Dispose();
        }
        
        /// <summary>
        /// Executes module's routine on parallel thread
        /// </summary>
        public void Execute(object parameter, IModuleExecutionListener listener)
        {
            if (IsExecuting)
            {
                _pendingJobs.Enqueue(new ModuleJob(parameter, listener));

                return;
            }

            IsExecuting = true;
            Task.Run(() => ExecuteCommand(parameter, listener)).ContinueWith(OnExecutionCompleted);
        }

        public override string ToString() => $"MODULE :: {Id}";

        public void Dispose()
        {
            Lock = null;
            _pendingJobs.Clear();
            _pendingJobs = null;
        }

        /// <summary>
        /// An execution process that runs on parallel thread. 
        /// Receives caller and parameter. 
        /// Returns a job with the same caller and execution result context that have to be sent to listener
        /// </summary>
        protected abstract ModuleJob ExecuteCommand(object parameter, IModuleExecutionListener listener);

        private void OnExecutionCompleted(Task<ModuleJob> task)
        {
            IsExecuting = false;
            task.Result.Listener.ReceiveResult(task.Result.Parameter);
            
            if (_pendingJobs.Length > 0)
            {
                var job = _pendingJobs.Dequeue();
                Execute(job.Parameter, job.Listener);
            }
        }
    }

    public struct ModuleJob
    {
        public readonly object Parameter;
        public readonly IModuleExecutionListener Listener;

        public ModuleJob(object parameter, IModuleExecutionListener listener)
        {
            Parameter = parameter;
            Listener = listener;
        }
    }

    public interface IModuleExecutionListener
    {
        void ReceiveStatus(int complition, object context);
        void ReceiveResult(object context);
    }
}