using Discord;
using Discord.Commands;
using Discord.WebSocket;
using GoodAdmin_API.Core;
using GoodAdmin_API.Core.Chat;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
            bool sendNotification = true;

            EmbedBuilder embed = new EmbedBuilder
            {
                Title = "GoodAdmin Commands ::",
                Description = "",
                Color = Color.Green
            };
            sendNotification = await Embeder.SafeEmbedBoolAsync(embed, Context.User, (ITextChannel)ch, " ");

            int count = await SendAllCommands(ch);

            if (count == 0)
            {
                embed = new EmbedBuilder
                {
                    Title = "No Commands Found",
                    Description = "No commands are registered at the moment. If you feel this is an inconvience, please feel free to contact my developers!",
                    Color = Color.DarkOrange
                };
                sendNotification = await Embeder.SafeEmbedBoolAsync(embed, Context.User, (ITextChannel)ch);
            }
            else
            {
                embed = new EmbedBuilder
                {
                    Title = "",
                    Description = "Commands Loaded : " + count,
                    Color = Color.Green
                };
                sendNotification = await Embeder.SafeEmbedBoolAsync(embed, Context.User, (ITextChannel)ch);
            }

            if (sendNotification && ch.GetType() != typeof(SocketDMChannel))
            {
                var message = await ch.SendMessageAsync(Context.User.Mention + " I have sent you all the commands!");
                await Task.Delay(1000 * 5);
                await message.DeleteAsync();
            }
        }

        private async Task<int> SendAllCommands(IChannel ch)
        {
            string oldCategory = "";
            int commandCount = 0;
            int groupCount = 0;
            string content = "";
            EmbedBuilder commandEmbed = null;
            foreach (var cmd in GetAllCommands())
            {
                // Changes Category, creates new data...
                if (oldCategory == null || !oldCategory.Equals(cmd.Remarks))
                {
                    // Publishes old Category, if available.
                    if (commandEmbed != null)
                    {
                        if (content == "")
                            await Embeder.SafeSendMessage("There was an error with trying to fetch a command. COMMAND: " + cmd.GetType().Name, Context.User, (ITextChannel)ch, " ");
                        else
                            await Embeder.SafeEmbedBoolAsync(commandEmbed, Context.User, (ITextChannel)ch, " ");
                    }

                    // Creates the Category to be sent.
                    if (cmd.Remarks != null)
                        commandEmbed = new EmbedBuilder
                        {
                            Title = cmd.Remarks,
                            Color = Color.Green
                        };
                    else
                        commandEmbed = null;
                    content = "";
                    groupCount = 0;
                    oldCategory = cmd.Remarks;
                }

                commandCount++;
                if (await HasPermission(cmd))
                {
                    groupCount++;

                    content += Configuration.globalConfig.PREFIX + (cmd.Module.Group != null && cmd.Module.Group.Length > 0 ? cmd.Module.Group + " " : "") + cmd.Name + (cmd.Summary != null && cmd.Summary.Trim().Length > 0 ? " - " + cmd.Summary : "") + "\n";
                    if (commandEmbed != null)
                    {
                        commandEmbed.Title = cmd.Remarks + $"[{groupCount}]";
                        commandEmbed.Description = content;
                    }
                }
            }

            if (commandEmbed != null && content.Length > 0)
                await Embeder.SafeEmbedBoolAsync(commandEmbed, Context.User, (ITextChannel)ch, " ");

            return commandCount;
        }

        private List<CommandInfo> GetAllCommands()
        {
            List<CommandInfo> cmds = new List<CommandInfo>();
            foreach (var cmd in Program.commands.Commands.OrderBy(cmd => cmd.Remarks))
            {
                // Filters through all the commands that aren't the help command.
                if (cmd.Name.ToLower() == "help") continue;

                cmds.Add(cmd);
            }
            return cmds;
        }

        private async Task<bool> HasPermission(CommandInfo cmd)
        {
            foreach (var precondition in cmd.Preconditions)
            {
                var context = new CommandContext(Program.client, Context.Message);
                var check = await precondition.CheckPermissionsAsync(context, cmd, null);

                if (check.Error != null)
                    return false;
            }

            return true;
        }
    }
}
