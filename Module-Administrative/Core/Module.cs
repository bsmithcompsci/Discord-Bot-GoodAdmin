using GoodAdmin_API;
using System;
using System.Threading.Tasks;

namespace Module_Administrative
{
    public class Module : APIModule
    {
        // Sync
        public Task Load()
        {
            return Task.CompletedTask;
        }
    }
}
