using GoodAdmin_API;
using GoodAdmin_API.Core.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Module_Test
{
    public class Class1 : APIModule
    {
        // Sync
        public Task Load()
        {
            Console.WriteLine("Loaded Test");
            return Task.CompletedTask;
        }

        public CommandHandler LoadCommandHandler()
        {
            CommandHandler handler = new CommandHandler();
            handler.AddCommand(new Ping());

            return handler;
        }
    }
}
