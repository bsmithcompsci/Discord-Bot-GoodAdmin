using Discord.WebSocket;
using GoodAdmin.Core.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Module_Test
{
    public class EmbedC : ACommand
    {
        public EmbedC() : base("embed") { }

        public override async Task Execute(SocketMessage msg, string[] args)
        {
            EmbedBuilder emb = new EmbedBuilder();
            emb.Title = "Test";
            emb.Description = "This is my test with embeds in C#!";
            await dm.SendMessageAsync("", false, emb.Build());
        }
    }
}