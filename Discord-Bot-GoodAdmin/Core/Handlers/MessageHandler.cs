using Discord;
using Discord.Commands;
using Discord.WebSocket;
using GoodAdmin_API.Core;
using GoodAdmin_API.Core.Chat;
using System;
using System.Threading.Tasks;

namespace GoodAdmin.Core.Handlers
{
    public static class MessageHandler
    {
        /// <summary>
        /// Event Handler, that handles the input from a user. Either by Direct Messaging or through a Guild.
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static async Task HandleMessage(SocketMessage msg)
        {
            // Verify, if the sender isn't a bot or a web hook.
            if (msg.Author.IsBot) return;
            if (msg.Author.IsWebhook) return;
            SocketUserMessage message = msg as SocketUserMessage;
            if (message == null) return;

            int argPos = 0;

            if (!(message.HasCharPrefix('!', ref argPos) || message.HasMentionPrefix(Program.client.CurrentUser, ref argPos))) return;

            var context = new CommandContext(Program.client, message);

            var result = await Program.commands.ExecuteAsync(context, argPos, Program.services);
            EmbedBuilder embed = null;
            if (!result.IsSuccess)
            {
                embed = new EmbedBuilder
                {
                    Color = Color.DarkRed,
                    Footer = new EmbedFooterBuilder
                    {
                        Text = "Please contact my developers, I had an oopsie!"
                    },
                    Title = $"[{result.Error.Value.ToString()} Error] Command Input : `" + message.Content + "`",
                    Description = "```\n" + result.ErrorReason + "```"
                };
#if DEBUG
                Console.WriteLine(result);
#endif

            }
            
            await Task.Delay(500);
            try
            {
                await message.DeleteAsync();
            }
            catch { }

            if (embed != null)
            {
                var devlogs = await Configuration.GetDeveloperLogs(Program.client);
                IUserMessage mes = null;

                if (devlogs != null && (result.Error == CommandError.Exception || result.Error == CommandError.MultipleMatches || result.Error == CommandError.ObjectNotFound))
                {
                    embed.Title += $" [Guild : {context.Guild.Name} | {context.Guild.Id}]";
                    mes = await Embeder.SafeEmbedAsync(embed, (ITextChannel)devlogs, " ");
                }
                else
                {
                    mes = await Embeder.SafeEmbedAsync(embed, (ITextChannel)context.Channel, " ");
                    await Task.Delay(2000);
                    await mes.DeleteAsync();
                }
            }
        }
    }
}
