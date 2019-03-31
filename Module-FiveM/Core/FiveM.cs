using Discord;
using Discord.Commands;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Module_GameManager
{
    public class FiveM : ModuleBase<CommandContext>
    {
        [Command("server-resource"), RequireBotPermission(GuildPermission.SendMessages), Remarks("Game Viewer - FiveM")]
        public async Task ServerResourceAsync(uint id)
        {
        }
    }
}
