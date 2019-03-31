using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using GoodAdmin_API.Core;
using GoodAdmin_API.Core.Chat;

namespace GoodAdmin.Core.Handlers
{
    public class GuildHandler
    {
        /// <summary>
        /// Event Handler, that handles when the GoodAdmin Bot Joins a new Guild.
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public static async Task JoinedGuild(SocketGuild guild)
        {
            Console.WriteLine(guild.Name + " has joined the AdminBot crew!");

            var devlogs = await Configuration.GetDeveloperLogs(Program.client);

            var embed = new EmbedBuilder
            {
                Color = Color.Green,
                Title = $"Guild has joined the GoodAdmin Party",
                Fields = new List<EmbedFieldBuilder>()
                {
                    new EmbedFieldBuilder()
                    {
                        Name = "Name",
                        Value = guild.Name
                    },

                    new EmbedFieldBuilder()
                    {
                        Name = "Owner",
                        Value = guild.Owner.Mention
                    }
                }

            };
            if (devlogs != null)
                await Embeder.SafeEmbedAsync(embed, (ITextChannel)devlogs, " ");

            embed = new EmbedBuilder
            {
                Color = Color.Orange,
                Title = $":crown: Welcome to the Party! :crown:",
                Description = "Type `!setup` and follow the instructions from there!"
            };
            ITextChannel ch = null;
            try
            {
                ch = (ITextChannel)guild.Channels.First();
            }
            catch { }
            await Embeder.SafeEmbedAsync(embed, guild.Owner, ch, " ");

            await Configuration.CreateGuildConfig(guild);
        }

        /// <summary>
        /// Event Handler, that handles when the GoodAdmin Bot Leaves a Guild.
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public static async Task LeftGuild(SocketGuild guild)
        {
            Console.WriteLine(guild.Name + " has left the AdminBot crew!");

            var devlogs = await Configuration.GetDeveloperLogs(Program.client);

            var embed = new EmbedBuilder
            {
                Color = Color.DarkRed,
                Title = $"Guild has left the GoodAdmin Party",
                Fields = new List<EmbedFieldBuilder>()
                {
                    new EmbedFieldBuilder()
                    {
                        Name = "Name",
                        Value = guild.Name
                    }
                }

            };
            if (devlogs != null)
                await Embeder.SafeEmbedAsync(embed, (ITextChannel)devlogs, " ");

            await Configuration.RemoveGuildConfig(guild);
        }
    }
}
