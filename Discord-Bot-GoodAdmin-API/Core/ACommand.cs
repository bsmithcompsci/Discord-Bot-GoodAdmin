using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;

namespace GoodAdmin.Core.API
{
    public abstract class ACommand
    {
        protected string command;

        public ACommand(string command) => this.command = command;

        public abstract void Execute(IGuildUser user, ITextChannel channel, IGuild guild, string[] args);
    }
}
