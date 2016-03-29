namespace PSake.TaskRunner.Helpers.TaskRunner
{
    using System;
    using System.Windows.Media;

    using Microsoft.VisualStudio.TaskRunnerExplorer;

    using PSake.TaskRunner.TaskRunner;

    internal abstract class TaskRunnerConfigBase : ITaskRunnerConfig
    {
        private static ImageSource SharedIcon;
        private BindingsPersister _bindingsPersister;
        private ITaskRunnerCommandContext _context;

        protected TaskRunnerConfigBase(TaskRunnerProvider provider, ITaskRunnerCommandContext context, ITaskRunnerNode hierarchy)
        {
            this._bindingsPersister = new BindingsPersister(provider);
            this.TaskHierarchy = hierarchy;
            this._context = context;
        }

        /// <summary>
        /// TaskRunner icon
        /// </summary>
        public virtual ImageSource Icon => SharedIcon ?? (SharedIcon = this.LoadRootNodeIcon());

        public ITaskRunnerNode TaskHierarchy { get; }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public string LoadBindings(string configPath)
        {
            try
            {
                return this._bindingsPersister.Load(configPath);
            }
            catch
            {
                return "<binding />";
            }
        }

        public bool SaveBindings(string configPath, string bindingsXml)
        {
            try
            {
                Telemetry.TrackEvent("Updated bindings");
                return this._bindingsPersister.Save(configPath, bindingsXml);
            }
            catch
            {
                return false;
            }
        }

        protected virtual void Dispose(bool isDisposing)
        {
        }

        protected virtual ImageSource LoadRootNodeIcon()
        {
            return null;
        }
    }
}
