using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.WebSocket;

namespace GoodAdmin.Core.Handlers
{
    public class GuildHandler
    {
        /// <summary>
        /// Event Handler, that handles when the GoodAdmin Bot Joins a new Guild.
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public static async Task JoinedGuild(SocketGuild arg)
        {
        }

        /// <summary>
        /// Event Handler, that handles when the GoodAdmin Bot Leaves a Guild.
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public static async Task LeftGuild(SocketGuild arg)
        {
        }
    }
}
