using GoodAdmin.Core.API;
using System.Collections.Generic;

namespace GoodAdmin_API.Core.Handlers
{
    public class CommandHandler
    {
        private List<ACommand> commands;

        public CommandHandler() => this.commands = new List<ACommand>();

        public void AddCommand(ACommand cmd) => this.commands.Add(cmd);

        public void AddCommands(List<ACommand> cmds) => this.commands.AddRange(cmds);

        public void RemoveCommand(ACommand cmd) => this.commands.Remove(cmd);

        public void RemoveCommand(string command)
        {
            ACommand cmd = this.GetCommand(command);
            if (cmd != null)
                this.RemoveCommand(cmd);
        }

        public ACommand GetCommand(string command)
        {
            foreach (ACommand cmd in commands)
                if (cmd.GetCommandText().ToLower() == command.ToLower())
                    return cmd;

            return null;
        }

        public List<ACommand> GetCommands()
        {
            return this.commands;
        }

    }
}
