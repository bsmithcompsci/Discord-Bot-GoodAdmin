using Discord;
using Discord.Commands;
using GoodAdmin_API.Core.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodAdmin_API.Core
{
    public class GlobalInit
    {
        public static Controllers.ControllerHandler controllerHandler;

        public delegate Task SetupDelegate(CommandContext Context, GuildConfig config);
        public static event SetupDelegate Setup;

        public static void Init()
        {
            controllerHandler = new Controllers.ControllerHandler();
        }

        public static async Task InvokeSetup(CommandContext Context, GuildConfig config)
        {
            if (Setup == null) return;
            try
            {
                /*
                IAsyncResult result = Setup.BeginInvoke(Context, config, null, null);
                if (result != null)
                    return await Setup.EndInvoke(result);
                */
                await Setup.Invoke(Context, config);
            } catch(Exception ex) {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Failed Module["+ ex.Source +"] Setup :: "+ ex.Message +" \n" + ex.StackTrace);
                Console.ResetColor();
            }
        }

    }
}
