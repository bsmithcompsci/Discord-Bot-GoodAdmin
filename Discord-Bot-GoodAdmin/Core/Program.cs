using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Discord;
using Discord.Commands;
using Discord.WebSocket;
using GoodAdmin.Core.Handlers;
using Microsoft.Extensions.DependencyInjection;

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
            TODO : Agreement to Rules/Terms and Conditions. (Optional Module)
                - Seperated Channel from Members access.
                - Creates a Member Role
                - Deletes all messages, and only reads agree command.
            TODO : Create Category Roles (Optional Module)
                - Example Staff, or Devs
            TODO : Auto-Promotion (Optional Module)
                - Either by Levels or  
            TODO : Levels (Optional Module)
                - UI
                - Progessive System
                - Level by Activity
                    * x1 Message
                    * x1.2 Command
                    * x1.5 Image
                    * x2 Reaction
            TODO : Minigame (Optional Module)
                - Text based Games
                    * Counter
                    * Spam
                    * Texting Nations [Turn Based Game]
            TODO : Ticket System (Optional Module)
                - Creates Private Channel for Staff and the Reporter to speak in.
                - Staff can Append users into discussion /ticket append @user
                - Staff can close ticket /ticket close
                - Staff can open ticket /ticket open #id
            TODO : Logging System
                - Editing Logs
                - Moderation Logs
                - Joining Logs (Shows default/custom message)
                - Leaving Logs (Ban/Kick may show, if enabled, otherwise shows default/custom message)
                - Ban Logs
                - Kick Logs
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
        public static DiscordSocketClient client;
        public static CommandService commands;
        public static IServiceProvider services;

        private async Task MainAsync()
        {
            // Initialization
             client = new DiscordSocketClient();

            // EVENT REGISTRES \\
            client.MessageReceived += MessageHandler.HandleMessage;
            client.Log += Client_Log;
            client.Ready += Client_Ready;

            commands = new CommandService();
            services = new ServiceCollection().BuildServiceProvider();

            // Load Configurations and Modules \\
            await Config.LoadGlobalConfig();
            await MessageHandler.InstallModules();
            
            // Start the Discord Bot \\
            await client.LoginAsync(TokenType.Bot, Config.config.TOKEN);
            await client.StartAsync();

            // Prevents the application from closing.
            await Task.Delay(-1);
        }


        private Task Client_Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }

        private Task Client_Ready()
        {
            Console.WriteLine("GoodAdmin is ready to Administrate!");
            return Task.CompletedTask;
        }

        // Runner Function - Ignore
        public static void Main(string[] args) => new Program().MainAsync().GetAwaiter().GetResult();
    }
}
