using System;
using XApp.Collections;
using XApp.Components;
using System.Threading.Tasks;

namespace XApp.Extensibility
{
    /// <summary>
    /// A base class for application components that are intended to execute some specific jobs on parallel thread
    /// </summary>
    public abstract class Module : ApplicationComponent, IDisposable
    {
        protected object _lock = new object();
        private Queue<ModuleJob> pendingJobs;
        
        public bool IsExecuting { get; private set; }
        public AppExtension Extension { get; internal set; }

        public Module(string id) : base(id)
        {
            pendingJobs = new Queue<ModuleJob>();
        }

        public abstract void Initialize();
        public virtual void Unload()
        {
            Dispose();
        }
        
        /// <summary>
        /// Executes module's routine on parallel thread
        /// </summary>
        /// <param name="parameter"></param>
        public void Execute(object parameter, IModuleExecutionListener listener)
        {
            if (IsExecuting)
            {
                pendingJobs.Enqueue(new ModuleJob(parameter, listener));

                return;
            }

            IsExecuting = true;
            Task.Run(() => ExecuteCommand(parameter, listener)).ContinueWith(OnExecutionCompleted);
        }

        public override string ToString() => $"MODULE :: {Id}";

        public void Dispose()
        {
            _lock = null;
            pendingJobs.Clear();
            pendingJobs = null;
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
            task.Result.listener.ReceiveResult(task.Result.parameter);
            
            if (pendingJobs.Length > 0)
            {
                var job = pendingJobs.Dequeue();
                Execute(job.parameter, job.listener);
            }
        }
    }

    public struct ModuleJob
    {
        public readonly object parameter;
        public readonly IModuleExecutionListener listener;

        public ModuleJob(object parameter, IModuleExecutionListener listener)
        {
            this.parameter = parameter;
            this.listener = listener;
        }
    }

    public interface IModuleExecutionListener
    {
        void ReceiveStatus(int complition, object context);
        void ReceiveResult(object context);
    }
}