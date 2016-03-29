namespace PSake.TaskRunner.Helpers.TaskRunner
{
    using System;

    using Microsoft.VisualStudio.TaskRunnerExplorer;

    public class TaskRunnerOption : ITaskRunnerOption
    {
        public TaskRunnerOption(string optionName, uint commandId, Guid commandGroup, bool isEnabled, string command)
        {
            this.Command = command;
            this.Id = commandId;
            this.Guid = commandGroup;
            this.Name = optionName;
            this.Enabled = isEnabled;
            this.Checked = isEnabled;
        }

        public string Command { get; set; }

        public bool Enabled { get; set; }

        public bool Checked { get; set; }

        public Guid Guid { get; }

        public uint Id { get; }

        public string Name { get; }
    }
}
