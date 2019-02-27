using GoodAdmin_API;
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
    }
}
