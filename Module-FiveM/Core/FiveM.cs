using Discord;
using Discord.Commands;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Module_GameManager
{
    [Group("FiveM")]
    public class FiveM : ModuleBase<CommandContext>
    {
        [Command("server-resources"), RequireBotPermission(GuildPermission.SendMessages), Remarks("Game Viewer - FiveM")]
        public async Task ServerResourcesAsync(uint id)
        {
        }
        [Command("players"), RequireBotPermission(GuildPermission.SendMessages), Remarks("Game Viewer - FiveM")]
        public async Task PlayersAsync(uint id)
        {
        }
    }
}
