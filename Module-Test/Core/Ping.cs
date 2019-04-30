using Discord;
using Discord.Commands;
using System.Threading.Tasks;

namespace Module_Development
{
    public class Ping : ModuleBase<CommandContext>
    {
        /// <summary>
        /// Tests Ping and Pong Command, a basic command to test not only connection, and the bot's preformance. It also gives a template for developers to copy any paste.
        /// </summary>
        /// <returns></returns>
        [Command("ping"), Remarks("General")]
        public async Task PingAsync()
        {
            var dm = await Context.User.GetOrCreateDMChannelAsync();
            EmbedBuilder embed = new EmbedBuilder
            {
                Title = "Pong!"
            };

            await dm.SendMessageAsync(embed: embed.Build());

            await Task.Delay(2 * 1000);

            await Context.Message.DeleteAsync();
        }
    }
}
