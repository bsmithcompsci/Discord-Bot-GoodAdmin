using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace GoodAdmin.Core.Handlers
{
    public static class MessageHandler
    {
        /// <summary>
        /// Searches for Modules that use the GoodAdmin's API, and loads them into the system. Further more adding commands, and other things into the main application. 
        /// </summary>
        /// <returns></returns>
        public static async Task InstallModules()
        {
            // Making space for modules.
            if (!Directory.Exists("./modules"))
                Directory.CreateDirectory("./modules");

            DirectoryInfo dInfo = new DirectoryInfo("./modules");
            FileInfo[] files = dInfo.GetFiles("*.dll");

            // Initializing built-in commands from this source.
            await Program.commands.AddModulesAsync(Assembly.GetEntryAssembly(), Program.services);

            // Loading the module DLLs
            if (files != null)
            {
                foreach (FileInfo file in files)
                {
                    Console.WriteLine("Loading Module :: " + file.Name);
                    Stopwatch stopwatch = new Stopwatch();
                    stopwatch.Start();

                    Assembly module = Assembly.LoadFile(Path.GetFullPath("./modules/") + file.Name);
                    try
                    {
                        foreach (Type t in module.GetTypes())
                        {
                            if (t.GetInterface(typeof(GoodAdmin_API.APIModule).Name) != null)
                            {
                                GoodAdmin_API.APIModule instance = Activator.CreateInstance(t) as GoodAdmin_API.APIModule;
                                await instance.Load();
                                await Program.commands.AddModulesAsync(module, Program.services);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("Error Loading :: " + module.GetName() + " -> ");
                        Console.WriteLine(e.Message);
                        Console.ResetColor();
                    }

                    stopwatch.Stop();
                    Console.WriteLine("Loaded Module :: " + file.Name + " (" + stopwatch.ElapsedMilliseconds + "ms)");
                }

#if DEBUG
                // Displays Debugger information into the console, when compiled in Debug mode.
                foreach (CommandInfo cmd in Program.commands.Commands)
                    Console.WriteLine("Added Commands :: '" + cmd.Name + "'");
#endif
                
            }
        }

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
            if (!result.IsSuccess)
            {
                EmbedBuilder embed = new EmbedBuilder
                {
                    Color = Color.DarkRed,
                    Footer = new EmbedFooterBuilder
                    {
                        Text = "Please contact my developers, I had an oopsie!"
                    },
                    Title = "[Internal Error] Command Input : `" + message.Content + "`",
                    Description = "```\n" + result.ErrorReason + "```"
                };
                await context.Channel.SendMessageAsync(embed : embed.Build());
            }
        }
    }
}
