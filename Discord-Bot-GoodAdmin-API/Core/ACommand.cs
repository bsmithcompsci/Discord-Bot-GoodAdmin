using Discord;
using Discord.WebSocket;
using System.Threading.Tasks;

namespace GoodAdmin.Core.API
{
    public abstract class ACommand
    {
        protected string command;
        protected string category = "general";

        public ACommand(string command, string category = "general")
        {
            this.command = command;
            this.category = category;
        }

        public abstract Task Execute(SocketMessage msg, string[] args);

        public string GetCommandText()
        {
            return this.command;
        }

        public string GetCategory()
        {
            return this.category;
        }
    }
}
