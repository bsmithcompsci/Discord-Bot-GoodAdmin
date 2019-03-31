using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

using Discord;
using Discord.Commands;
using Discord.WebSocket;
using GoodAdmin.Core.Handlers;
using GoodAdmin_API.Core;
using GoodAdmin_API.Core.Database;
using GoodAdmin_API.Core.Controllers;
using System.Threading;
using System.Timers;

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
    // Invite Key : https://discordapp.com/oauth2/authorize?client_id=542208147385876480&scope=bot&permissions=8
    public class Program
    {
        public static DiscordSocketClient client;
        public static CommandService commands;
        public static IServiceProvider services;

        public delegate Task TickDelegate(float delta);
        public event TickDelegate Tick;

        public float UIUpdateDifference = 2.0f;
        
        private float lastUIUpdate = 0.0f;

        private async Task MainAsync()
        {
            // Initialization
             client = new DiscordSocketClient();
            
            // Channel Handler stuff \\
            client.MessageReceived      += ChannelHandler.ReceivedMessage;
            client.MessageDeleted       += ChannelHandler.RemovedMessage;
            client.MessageUpdated       += ChannelHandler.EditedMessage;
            client.ChannelCreated       += ChannelHandler.CreatedChannel;
            client.ChannelDestroyed     += ChannelHandler.RemovedChannel;
            client.ChannelUpdated       += ChannelHandler.EditedChannel;

            client.Ready                += Client_Ready;

            // Guild Handler Stuff \\
            client.JoinedGuild          += GuildHandler.JoinedGuild;
            client.LeftGuild            += GuildHandler.LeftGuild;

            // EVENT REGISTRES \\
            client.MessageReceived += MessageHandler.HandleMessage;

            // EVENTS: Logging...
            client.Log                  += Client_Log;

            // UI Updater
            Tick += GameUIDisplayUpdater;

            commands = new CommandService();
            services = new ServiceCollection().BuildServiceProvider();
            GlobalInit.Init();


            // Load Configurations and Modules \\
            await SQL.Verify("localdb");
            await Configuration.LoadGlobalConfig();
            await ModuleHandler.InstallModules();

            // Start the Discord Bot \\
            await client.LoginAsync(TokenType.Bot, Configuration.globalConfig.TOKEN);
            await client.StartAsync();

            System.Timers.Timer timer = new System.Timers.Timer(1000);
            timer.Elapsed += Timer_Elapsed;
            timer.AutoReset = true;
            timer.Enabled = true;
            
            await Task.Delay(-1);
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Tick?.Invoke(1);
        }

        private async Task GameUIDisplayUpdater(float delta)
        {
            if (lastUIUpdate >= UIUpdateDifference)
            {
                await client.SetGameAsync(name: Configuration.globalConfig.PREFIX + $"help | {Configuration.guildConfigs.Count} guilds", type: ActivityType.Listening);

                lastUIUpdate = 0.0f;
            }
            else
            {
                lastUIUpdate += delta;
            }
        }

        private Task Client_Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }

        private async Task Client_Ready()
        {
            Console.WriteLine("GoodAdmin is ready to Administrate!");

            await Configuration.LoadGlobalGuildConfigs(client);
        }

        // Runner Function - Ignore
        public static void Main(string[] args) => new Program().MainAsync().GetAwaiter().GetResult();
    }
}
