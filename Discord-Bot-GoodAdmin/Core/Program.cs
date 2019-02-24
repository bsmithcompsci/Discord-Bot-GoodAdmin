using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;

namespace GoodAdmin.Core
{
    // Docs: https://discord.foxbot.me/docs/guides/getting_started/intro.html
    /*
        Functionality
            TODO : Create Basic Moderation Commands
                - Kick
                - Ban
                - Warn
                - Inspect
                - Purge
            TODO : Create Category Roles
                - Example Staff, or Devs
            TODO : Auto-Promotion
                - Either by Levels or  
            TODO : Levels
                - UI
                - Progessive System
                - Level by Activity
            TODO : Minigame
                - Text based Games    
            TODO : Ticket System
                - Creates Private Channel for Staff and the Reporter to speak in.
                - Staff can Append users into discussion /ticket append @user
                - Staff can close ticket /ticket close
                - Staff can open ticket /ticket open #id
        API
            TODO : Simpliar Private-Message to Users
            TODO : Simpliar Channel Management
            TODO : Simpliar User Management

        Possbile Features later:
            TODO : API Access to Kypter-Hosting
                - Operation Controls: Start, Stop, Restart
                - Execute Commands into Service
    */
    public class Program
    {
        public static void Main(string[] args) => new Program().MainAsync().GetAwaiter().GetResult();

        private async Task MainAsync()
        {
        }
    }
}
