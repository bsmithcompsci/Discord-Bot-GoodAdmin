using Discord;
using Discord.Commands;
using System.Threading.Tasks;

namespace Module_Test
{
    public class Ping : ModuleBase<CommandContext>
    {
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
