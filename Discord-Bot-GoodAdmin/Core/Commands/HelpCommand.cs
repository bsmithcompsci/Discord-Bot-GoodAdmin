using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Linq;
using System.Threading.Tasks;

namespace GoodAdmin.Core.Commands
{
    /// <summary>
    /// The Class that holds the Help command.
    /// </summary>
    public class HelpCommand : ModuleBase<CommandContext>
    {
        /// <summary>
        /// Help will display all the commands to the user privately. besides this command itself.
        /// </summary>
        /// <returns></returns>
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
            await dm.SendMessageAsync(embed: embed.Build());

            string oldCategory = "";
            int count = 0;
            string content = "";
            foreach (CommandInfo cmd in Program.commands.Commands.OrderBy(f => f.Remarks))
            {
                // Filters through all the commands that aren't the help command.
                if (cmd.Name.ToLower() != "help")
                {
                    count++;
                    content += Config.config.PREFIX + cmd.Name + (cmd.Summary != null && cmd.Summary.Trim().Length > 0 ? " - " + cmd.Summary : "") + "\n";
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
                await dm.SendMessageAsync(embed : new EmbedBuilder
                {
                    Title = "No Commands Found",
                    Description = "No commands are registered at the moment. If you feel this is an inconvience, please feel free to contact my developers!",
                    Color = Color.DarkOrange
                }.Build());
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
