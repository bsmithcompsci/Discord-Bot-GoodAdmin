using Discord;
using Discord.WebSocket;
using GoodAdmin_API.Core;
using GoodAdmin_API.Core.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodAdmin_API
{
    public class GuildUtils
    {
        public static async Task<IRole> AddRole(string name, IGuild guild, GuildPermissions? perms = null, Color? clr = null)
        {
            var result = await guild.CreateRoleAsync(name, perms, clr);
            return result;
        }

        public static async Task AssignRole(IUser user, IRole role)
        {
            SocketGuildUser sgu = (IUser)user as SocketGuildUser;
            await sgu.AddRoleAsync(role);
        }
        public static async Task RemoveRole(IUser user, IRole role)
        {
            SocketGuildUser sgu = (IUser)user as SocketGuildUser;
            await sgu.RemoveRoleAsync(role);
        }

        public static IRole GetRole(string name, IGuild guild)
        {
            foreach (var role in guild.Roles)
                if (role.Name == name) return role;

            return null;
        }

        public static IRole GetRole(ulong id, IGuild guild)
        {
            foreach (var role in guild.Roles)
                if (role.Id == id) return role;

            return null;
        }

        public static async Task<IChannel> GetChannel(ulong id, IGuild guild)
        {
            foreach(IGuildChannel channel in await guild.GetChannelsAsync())
            {
                if (channel.Id == id)
                    return channel;
            }
            return null;
        }

        public static bool UserHasRole(IUser user, IRole role)
        {
            SocketGuildUser sgu = (IUser)user as SocketGuildUser;
            return sgu.Roles.Contains(role);
        }

        public static GuildController GetGuildController(IGuild guild)
        {
            foreach(var controller in GlobalInit.controllerHandler.GetControllers())
                if (controller is GuildController con)
                    if (con.Guild.Id == guild.Id)
                        return con;

            return null;
        }
    }
}
