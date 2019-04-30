using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using GoodAdmin_API;
using GoodAdmin_API.Core;
using GoodAdmin_API.Core.Chat;
using GoodAdmin_API.Core.Controllers;

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

            var guildcontroller = new GuildController(guild);
            await GlobalInit.controllerHandler.AddController(guildcontroller, guild, new object[] { guild });
            guildcontroller.InvokeInitialize();
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

        internal static async Task MessageReceived(SocketMessage msg)
        {
            //var controller = GuildUtils.GetGuildController(guild);

            //controller.InvokeMessageReceived(msg);
        }

        internal static async Task MessageDeleted(Cacheable<IMessage, ulong> history, ISocketMessageChannel channel)
        {
            //var controller = GuildUtils.GetGuildController(guild);

            //controller.InvokeMessageRemoved();
        }

        internal static async Task MessageUpdated(Cacheable<IMessage, ulong> history, SocketMessage message, ISocketMessageChannel channel)
        {
            //var controller = GuildUtils.GetGuildController(guild);

            //controller.InvokeMessageEdited(user, guild);
        }

        internal static async Task ChannelCreated(SocketChannel channel)
        {
            //var controller = GuildUtils.GetGuildController();

            //controller.InvokeChannelCreated(channel);
        }

        internal static async Task ChannelDestroyed(SocketChannel channel)
        {
            //var controller = GuildUtils.GetGuildController(guild);

            //controller.InvokeChannelRemoved(channel);
        }

        internal static async Task ChannelUpdated(SocketChannel oldChannel, SocketChannel newChannel)
        {
            //var controller = GuildUtils.GetGuildController(guild);

            //controller.InvokeChannelEdited(oldChannel, newChannel);
        }

        internal static async Task UserBanned(SocketUser user, SocketGuild guild)
        {
            //var controller = GuildUtils.GetGuildController(guild);

            //controller.InvokeUserBanned(user, guild);
        }

        internal static async Task UserUnbanned(SocketUser user, SocketGuild guild)
        {
            //var controller = GuildUtils.GetGuildController(guild);

            //controller.InvokeUserUnBanned(user, guild);
        }

        internal static async Task UserJoined(SocketGuildUser user)
        {
            //var controller = GuildUtils.GetGuildController(guild);

            //controller.InvokeUserJoined(user);
        }

        internal static async Task UserLeft(SocketGuildUser user)
        {
            //var controller = GuildUtils.GetGuildController(guild);

            //controller.InvokeUserLeft(user);
        }

        internal static async Task RoleDeleted(SocketRole role)
        {
            //var controller = GuildUtils.GetGuildController(guild);

            //controller.InvokeRoleRemoved(role);
        }

        internal static async Task RoleUpdated(SocketRole oldRole, SocketRole newRole)
        {
            //var controller = GuildUtils.GetGuildController(guild);

            //controller.InvokeRoleEdited(oldRole, newRole);
        }

        internal static async Task RoleCreated(SocketRole role)
        {
            //var controller = GuildUtils.GetGuildController(guild);

            //controller.RoleCreated(role);
        }

    }
}
