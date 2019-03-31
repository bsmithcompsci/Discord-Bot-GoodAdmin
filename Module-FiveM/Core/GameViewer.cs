using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Module_GameManager.Core
{
    public class GameViewer : ModuleBase<CommandContext>
    {
        [Command("servers"), RequireBotPermission(GuildPermission.SendMessages), Remarks("Game Viewer")]
        public async Task ServerAsync()
        {
            
        }

        [Command("server"), RequireBotPermission(GuildPermission.SendMessages), Remarks("Game Viewer")]
        public async Task ServerAsync(uint id)
        {
        }
    }
}
