namespace PSake.TaskRunner.Helpers.TaskRunner
{
    using System.Windows.Media;

    using Microsoft.VisualStudio.TaskRunnerExplorer;

    using PSake.TaskRunner.TaskRunner;

    internal class TaskRunnerConfig : TaskRunnerConfigBase
    {
        private ImageSource _rootNodeIcon;

        public TaskRunnerConfig(TaskRunnerProvider provider, ITaskRunnerCommandContext context, ITaskRunnerNode hierarchy)
            : base(provider, context, hierarchy)
        {
        }

        public TaskRunnerConfig(TaskRunnerProvider provider, ITaskRunnerCommandContext context, ITaskRunnerNode hierarchy, ImageSource rootNodeIcon)
            : this(provider, context, hierarchy)
        {
            this._rootNodeIcon = rootNodeIcon;
        }

        protected override ImageSource LoadRootNodeIcon()
        {
            return this._rootNodeIcon;
        }
    }
}
