using Discord.WebSocket;
using GoodAdmin.Core.API;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;


namespace GoodAdmin.Core.Handlers
{
    public static class MessageHandler
    {
        private static List<ACommand> commands = new List<ACommand>();

        public static async Task InstallModules()
        {
            // TODO : Load External DLL Files that will handle the commands.
            if (!Directory.Exists("./plugins"))
                Directory.CreateDirectory("./plugins");

            DirectoryInfo dInfo = new DirectoryInfo("./plugins");
            FileInfo[] files = dInfo.GetFiles("*.dll");
            
            if (files != null)
            {
                foreach (FileInfo file in files)
                {
                    Assembly.LoadFile("./plugins/" + file.Name);
                }
            }

            foreach(Assembly plugin in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach(Type t in plugin.GetTypes())
                {
                    if (t.GetInterface(typeof(GoodAdmin_API.APIModule).Name) != null)
                    {
                        GoodAdmin_API.APIModule module = Activator.CreateInstance(t) as GoodAdmin_API.APIModule;
                        await module.Load();
                    }
                }
            }
        }

        public static async Task handleMessage(SocketMessage msg)
        {
            // TODO : Handle Commands
            // TODO : Handle Simple Moderation stuff


        }
    }
}
