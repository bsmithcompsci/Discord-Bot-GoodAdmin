using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Linq;
using System.Threading.Tasks;

namespace GoodAdmin.Core.Commands
{
    public class HelpCommand : ModuleBase<CommandContext>
    {
        [Command("help"), Remarks("General")]
        public async Task Execute()
        {
            // Initializes some shortcuts.
            var dm = await Context.User.GetOrCreateDMChannelAsync();
            var ch = Context.Channel;

            EmbedBuilder embed = new EmbedBuilder
            {
                Title = "GoodAdmin Commands ::",
                Description = "",
                Color = Color.Green
            };

            string oldCategory = "";
            int count = 0;
            string content = "";
            foreach (CommandInfo cmd in Program.commands.Commands.OrderBy(f => f.Remarks))
            {
                // Filters through all the commands that aren't the help command.
                if (cmd.Name.ToLower() != "help")
                {
                    count++;
                    content += Config.config.PREFIX + cmd.Name + "\n";
                    if (oldCategory != cmd.Remarks)
                    {
                        if (content == "")
                            await dm.SendMessageAsync("There was an error with trying to fetch a command. COMMAND: " + cmd.GetType().Name);
                        else
                        {
                            embed = new EmbedBuilder
                            {
                                Title = cmd.Remarks,
                                Description = content,
                                Color = Color.Green
                            };
                            await dm.SendMessageAsync(embed: embed.Build());
                            content = "";
                            oldCategory = cmd.Remarks;
                        } 
                    }
                }
            }
            if (count == 0)
                await dm.SendMessageAsync("No commands are registered at the moment. If you feel this is an inconvience, please feel free to contact my developers!");
            else
            {
                embed = new EmbedBuilder
                {
                    Title = "",
                    Description = "Commands Loaded : " + count,
                    Color = Color.Green
                };
                await dm.SendMessageAsync(embed: embed.Build());
            }

            if (ch.GetType() != typeof(SocketDMChannel))
            {
                var message = await ch.SendMessageAsync(Context.User.Mention + " I have sent you all the commands!");
                await Task.Delay(1000 * 5);
                await message.DeleteAsync();
            }
        }
    }
}
