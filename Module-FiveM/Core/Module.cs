using GoodAdmin_API;
using Module_GameManager.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Module_GameManager
{
    public class Module : APIModule
    {
        // Sync
        public Task Load(Discord.WebSocket.DiscordSocketClient client)
        {
            return Task.CompletedTask;
        }
    }
}
