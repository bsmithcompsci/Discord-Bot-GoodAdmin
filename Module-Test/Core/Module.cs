using GoodAdmin_API;
using System;
using System.Threading.Tasks;

namespace Module_Test
{
    public class Module : APIModule
    {
        // Sync
        public Task Load()
        {
            Console.WriteLine("Loaded Test");
            return Task.CompletedTask;
        }
    }
}
