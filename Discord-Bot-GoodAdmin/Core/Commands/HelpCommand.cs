﻿using Discord;
using Discord.WebSocket;
using GoodAdmin.Core.API;
using GoodAdmin.Core.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodAdmin.Core.Commands
{
    public class HelpCommand : ACommand
    {
        public HelpCommand() : base("help") { }

        public override async Task Execute(SocketMessage msg, string[] args)
        {
            var dm = await msg.Author.GetOrCreateDMChannelAsync();
            var ch = msg.Channel;
            
            string oldCategory = null;
            int count = 0;
            string content = "";
            foreach (ACommand cmd in MessageHandler.GetCommandHandler().GetCommands().OrderBy( x => x.GetCategory() ))
            {
                if (cmd != this)
                {
                    count++;
                    if (oldCategory != cmd.GetCategory())
                    {
                        if (content == "")
                        {
                            await dm.SendMessageAsync("There was an error with trying to fetch a command. COMMAND: "+cmd.GetType().Name);
                            continue;
                        } else
                        {
                            EmbedBuilder emb = new EmbedBuilder();
                            emb.Title = "Test";
                            emb.Description = "This is my test with embeds in C#!";
                            await dm.SendMessageAsync("", false, emb.Build());
                            await dm.SendMessageAsync("Category : " + cmd.GetCategory() + "\n" + content);
                            content = "";
                            oldCategory = cmd.GetCategory();
                        }
                        
                    }
                    content += Config.config.PREFIX + cmd.GetCommandText() + "\n";
                }
            }
            if (count == 0)
                await dm.SendMessageAsync("No commands are registered at the moment. If you feel this is an inconvience, please feel free to contact my developers!");

            var message = await ch.SendMessageAsync(msg.Author.Mention + " I have sent you all the commands!");
            await Task.Delay(1000 * 5);
            await message.DeleteAsync();
        }
    }
}
