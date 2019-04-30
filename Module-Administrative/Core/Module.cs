using Discord;
using Discord.Commands;
using Discord.WebSocket;
using GoodAdmin_API;
using GoodAdmin_API.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Module_Administrative
{
    public class Module : APIModule
    {
        public static DiscordSocketClient client;

        // Sync
        public Task Load(DiscordSocketClient client)
        {
            new Configuration();

            Module.client = client;

            //GlobalInit.Setup += GlobalInit_Setup;

            client.MessageReceived += Client_MessageReceived;

            return Task.CompletedTask;
        }

        // Handles Mutes
        private async Task Client_MessageReceived(SocketMessage msg)
        {
            // Verify, if the sender isn't a bot or a web hook.
            if (msg.Author.IsBot) return;
            if (msg.Author.IsWebhook) return;
            SocketUserMessage message = msg as SocketUserMessage;
            if (message == null) return;

            var context = new CommandContext(client, message);
            if (context == null) return; 
            var guildinfo = await Configuration.LoadOrCreateGuildConfig(context.Guild);
            if (guildinfo == null) return;
            /*
            var data = Configuration.GetOrCreateUserInfo(msg.Author, guildinfo);

            if (data.session.ContainsKey("isMuted"))
                if (Convert.ToBoolean(data.session["isMuted"]))
                    await msg.DeleteAsync();
            */
            guildinfo.session.TryGetValue("role-muted", out object val);
            var role = GuildUtils.GetRole(Convert.ToUInt64(val), context.Guild);

            if (GuildUtils.UserHasRole(msg.Author, role))
                await msg.DeleteAsync();
        }

        /*
        private async Task GlobalInit_Setup(Discord.Commands.CommandContext Context, GuildConfig config)
        {
            string buildcontent = "";
            var controllers = new List<object[]>();

        }*/
    }
}
