namespace PSake.TaskRunner.TaskRunner
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;

    using Microsoft.VisualStudio.TaskRunnerExplorer;

    using PSake.TaskRunner.Helpers.TaskRunner;

    internal class TrimmingStringComparer : IEqualityComparer<string>
    {
        private char _toTrim;
        private IEqualityComparer<string> _basisComparison;

        public TrimmingStringComparer(char toTrim)
            : this(toTrim, StringComparer.Ordinal)
        {
        }

        public TrimmingStringComparer(char toTrim, IEqualityComparer<string> basisComparer)
        {
            this._toTrim = toTrim;
            this._basisComparison = basisComparer;
        }

        public bool Equals(string x, string y)
        {
            string realX = x?.TrimEnd(this._toTrim);
            string realY = y?.TrimEnd(this._toTrim);
            return this._basisComparison.Equals(realX, realY);
        }

        public int GetHashCode(string obj)
        {
            string realObj = obj?.TrimEnd(this._toTrim);
            return realObj != null ? this._basisComparison.GetHashCode(realObj) : 0;
        }
    }

    [TaskRunnerExport(Constants.FILENAME)]
    class TaskRunnerProvider : ITaskRunner
    {
        private ImageSource _icon;
        private HashSet<string> _dynamicNames = new HashSet<string>(new TrimmingStringComparer('\u200B'));

        public void SetDynamicTaskName(string dynamicName)
        {
            this._dynamicNames.Remove(dynamicName);
            this._dynamicNames.Add(dynamicName);
        }

        public string GetDynamicName(string name)
        {
            IEqualityComparer<string> comparer = new TrimmingStringComparer('\u200B');
            return this._dynamicNames.FirstOrDefault(x => comparer.Equals(name, x));
        }

        public TaskRunnerProvider()
        {
            this._icon = new BitmapImage(new Uri(@"pack://application:,,,/CommandTaskRunner;component/Resources/project.png"));
        }

        public List<ITaskRunnerOption> Options
        {
            get { return null; }
        }

        public async Task<ITaskRunnerConfig> ParseConfig(ITaskRunnerCommandContext context, string configPath)
        {
            return await Task.Run(() =>
            {
                ITaskRunnerNode hierarchy = this.LoadHierarchy(configPath);

                if (!hierarchy.Children.Any() && !hierarchy.Children.First().Children.Any())
                    return null;

                Telemetry.TrackEvent("Tasks loaded");

                return new TaskRunnerConfig(this, context, hierarchy, this._icon);
            });
        }

        private ITaskRunnerNode LoadHierarchy(string configPath)
        {
            ITaskRunnerNode root = new TaskRunnerNode(Constants.TASK_CATEGORY);
            string rootDir = Path.GetDirectoryName(configPath);
            var commands = TaskParser.LoadTasks(configPath);

            if (commands == null)
                return root;

            TaskRunnerNode tasks = new TaskRunnerNode("Commands");
            tasks.Description = "A list of command to execute";
            root.Children.Add(tasks);

            foreach (CommandTask command in commands.OrderBy(k => k.Name))
            {
                string cwd = command.WorkingDirectory ?? rootDir;

                // Add zero width space
                string commandName = command.Name += "\u200B";
                this.SetDynamicTaskName(commandName);

                TaskRunnerNode task = new TaskRunnerNode(commandName, true)
                {
                    Command = new TaskRunnerCommand(cwd, command.FileName, command.Arguments),
                    Description = $"Filename:\t {command.FileName}\r\nArguments:\t {command.Arguments}"
                };

                tasks.Children.Add(task);
            }

            return root;
        }
    }
}
