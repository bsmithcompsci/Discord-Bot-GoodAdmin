using Discord.Commands;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace GoodAdmin.Core.Handlers
{
    class ModuleHandler
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
                                await instance.Load(Program.client);
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
                    Console.WriteLine("Added Commands :: '" + (cmd.Module.Group != null && cmd.Module.Group.Length > 0 ? cmd.Module.Group + " " : "") + cmd.Name + "'");
#endif

            }
        }
    }
}
