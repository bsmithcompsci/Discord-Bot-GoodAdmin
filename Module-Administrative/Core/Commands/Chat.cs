using Discord;
using Discord.Commands;
using GoodAdmin_API.Core.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Module_Administrative.Core.Commands
{
    public class Chat : ModuleBase<CommandContext>
    {
        /// <summary>
        /// Purges all messages within an amount, with permission checking...
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        [Command("embed"), RequireUserPermission(GuildPermission.ManageMessages), RequireBotPermission(GuildPermission.ManageMessages), Remarks("Chat")]
        public async Task EmbedAsync([Remainder] string message)
        {
            string[] seperated = message.Split('|');

            EmbedBuilder embed = null;

            if (seperated.Length > 1)
                embed = new EmbedBuilder()
                {
                    Title = seperated[0],
                    Description = seperated[1],
                    Color = Color.Orange
                };
            else
                embed = new EmbedBuilder()
                {
                    Title = "",
                    Description = message,
                    Color = Color.Orange
                };

            await Embeder.SafeEmbedAsync(embed, (ITextChannel)Context.Channel);
        }
    }
}
