using Discord.WebSocket;
using GoodAdmin.Core.API;
using GoodAdmin.Core.Commands;
using GoodAdmin_API.Core.Handlers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;


namespace GoodAdmin.Core.Handlers
{
    public static class MessageHandler
    {
        private static CommandHandler cmdHandler;

        public static async Task InstallModules()
        {
            // TODO : Load External DLL Files that will handle the commands.
            if (!Directory.Exists("./modules"))
                Directory.CreateDirectory("./modules");

            DirectoryInfo dInfo = new DirectoryInfo("./modules");
            FileInfo[] files = dInfo.GetFiles("*.dll");

            cmdHandler = new CommandHandler();
            cmdHandler.AddCommand(new HelpCommand());

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

            foreach(Assembly plugin in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    foreach (Type t in plugin.GetTypes())
                    {
                        if (t.GetInterface(typeof(GoodAdmin_API.APIModule).Name) != null)
                        {
                            Console.WriteLine("Loading Module :: " + plugin.GetName());
                            Stopwatch stopwatch = new Stopwatch();
                            stopwatch.Start();
                            GoodAdmin_API.APIModule module = Activator.CreateInstance(t) as GoodAdmin_API.APIModule;
                            await module.Load();
                            if (module.LoadCommandHandler() != null)
                            {

                                List<ACommand> cmds = module.LoadCommandHandler().GetCommands();
                                cmdHandler.AddCommands(cmds);
#if DEBUG
                                foreach (ACommand cmd in cmds)
                                    Console.WriteLine("Added Commands :: '" + cmd.GetCommandText() + "'");
#endif
                            }
                            stopwatch.Stop();
                            Console.WriteLine("Loaded Module :: " + plugin.GetName() + " (" + stopwatch.ElapsedMilliseconds + "ms)");
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("Error Loading :: " + plugin.GetName() + " -> ");
                    Console.WriteLine(e.Message);
                    Console.ResetColor();
                }
            }
        }

        public static async Task HandleMessage(SocketMessage msg)
        {
            foreach (ACommand cmd in cmdHandler.GetCommands())
            {
                if ((Config.config.PREFIX + cmd.GetCommandText()).ToLower() == msg.Content.ToLower())
                {
                    // Remove the beginning command arg. Ex: -[!ping]- foo
                    string[] _args = msg.Content.Split(' ');
                    string[] args = new string[_args.Length - 1];

                    for (var i = 1; i < _args.Length; i++)
                        args[i - 1] = _args[i];
                    
                    await cmd.Execute(msg, args);
                    break;
                }
            }
        }

        public static CommandHandler GetCommandHandler()
        {
            return cmdHandler;
        }
    }
}
