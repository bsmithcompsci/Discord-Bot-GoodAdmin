using Discord;
using Discord.Commands;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Module_Administrative
{
    public class Purge : ModuleBase<CommandContext>
    {
        /// <summary>
        /// Purges all messages within an amount, with permission checking...
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        [Command("purge"), RequireUserPermission(GuildPermission.ManageMessages), RequireBotPermission(GuildPermission.ManageMessages), Remarks("Admin")]
        public async Task PurgeAsync(uint amount)
        {
            IEnumerable<IMessage> messages = await Context.Channel.GetMessagesAsync((int)amount + 1).FlattenAsync();
            await ((ITextChannel)Context.Channel).DeleteMessagesAsync(messages);

            IUserMessage msg = await ReplyAsync("Deleted **" + messages.Count() + "** messages from the channel.");
            await Task.Delay(5 * 1000);
            await msg.DeleteAsync();
        }
    }
}
