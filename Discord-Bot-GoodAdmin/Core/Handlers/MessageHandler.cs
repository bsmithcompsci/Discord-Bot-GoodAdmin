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
                    Assembly.LoadFile(Path.GetFullPath("./modules/") + file.Name);
                    stopwatch.Stop();
                    Console.WriteLine("Loaded Module :: " + file.Name + " ("+ stopwatch.ElapsedMilliseconds +"ms)");
                }
            }

            // Loading modules core code, and commands.
            foreach(Assembly module in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    foreach (Type t in module.GetTypes())
                    {
                        if (t.GetInterface(typeof(GoodAdmin_API.APIModule).Name) != null)
                        {
                            Console.WriteLine("Loading Module :: " + module.GetName());
                            Stopwatch stopwatch = new Stopwatch();
                            stopwatch.Start();
                            GoodAdmin_API.APIModule instance = Activator.CreateInstance(t) as GoodAdmin_API.APIModule;
                            await instance.Load();
                            await Program.commands.AddModulesAsync(module, Program.services);
                            stopwatch.Stop();
                            Console.WriteLine("Loaded Module :: " + module.GetName() + " (" + stopwatch.ElapsedMilliseconds + "ms)");
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
            }

#if DEBUG
            // Displays Debugger information into the console, when compiled in Debug mode.
            foreach (CommandInfo cmd in Program.commands.Commands)
                Console.WriteLine("Added Commands :: '" + cmd.Name + "'");
#endif
        }

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
                await context.Channel.SendMessageAsync(result.ErrorReason);
        }
    }
}
