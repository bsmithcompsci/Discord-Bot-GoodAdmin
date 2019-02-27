using Discord.WebSocket;
using GoodAdmin.Core.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Module_Test
{
    public class Ping : ACommand
    {
        public Ping() : base("ping") {}

        public override async Task Execute(SocketMessage msg, string[] args)
        {
            var dm = await msg.Author.GetOrCreateDMChannelAsync();
            await dm.SendMessageAsync("Pong");
        }
    }
}
