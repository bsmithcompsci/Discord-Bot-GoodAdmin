using Discord;
using Discord.Commands;
using GoodAdmin_API.Core;
using GoodAdmin_API.Core.Chat;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Module_Test.Core.Commands
{
    [Group("dev")]
    public class Debug : ModuleBase<CommandContext>
    {
        [Group("guild")]
        public class Guild : ModuleBase<CommandContext>
        {
            [Command("info")]
            public async Task Info()
            {
                if (Context.User.Id != 269671051892359170) return;

                var config = await Configuration.LoadOrCreateGuildConfig(Context.Guild);
                string content = "";
                if (config == null)
                    content = ":skull_crossbones: Failed to load or create the Guild's information";
                else
                    content = JsonConvert.SerializeObject(config, Formatting.Indented);

                // Creates the Category to be sent.
                var commandEmbed = new EmbedBuilder
                {
                    Title = ":hammer_pick: Developer Guild Inspection",
                    Description = content,
                    Color = Color.Green
                };
                await Embeder.SafeEmbedBoolAsync(commandEmbed, Context.User, (ITextChannel)Context.Channel, " ");
            }

        }

        [Group("debug")]
        public class DebugGroup : ModuleBase<CommandContext>
        {
            [Command("setguild")]
            public async Task SetDevGuild()
            {
                if (Context.User.Id != 269671051892359170) return;

                Configuration.globalConfig.developerGuildID = Context.Guild.Id;

                // Creates the Category to be sent.
                var commandEmbed = new EmbedBuilder
                {
                    Title = ":hammer_pick: Developer Guild Setup",
                    Description = $"Developer Guild has been set to {Context.Guild.Name}!",
                    Color = Color.Green
                };
                await Embeder.SafeEmbedBoolAsync(commandEmbed, Context.User, (ITextChannel)Context.Channel, " ");

                Configuration.SaveGlobalConfig();
            }

            [Command("logs")]
            public async Task Logs()
            {
                if (Context.User.Id != 269671051892359170) return;
                if (Configuration.globalConfig.developerGuildID != Context.Guild.Id) return;

                Configuration.globalConfig.session.TryGetValue("logs", out object val);

                if (val == null)
                    Configuration.globalConfig.session.Add("logs", Context.Channel.Id);
                else
                    Configuration.globalConfig.session["logs"] = Context.Channel.Id;

                // Creates the Category to be sent.
                var commandEmbed = new EmbedBuilder
                {
                    Title = ":hammer_pick: Developer Guild Setup - Logs",
                    Description = $"Developer Logs has been set to {Context.Channel.Name}!",
                    Color = Color.Green
                };
                await Embeder.SafeEmbedBoolAsync(commandEmbed, Context.User, (ITextChannel)Context.Channel, " ");

                Configuration.SaveGlobalConfig();
            }

            [Command("guilds")]
            public async Task GetAllGuilds()
            {
                if (Context.User.Id != 269671051892359170) return;

                Configuration.globalConfig.developerGuildID = Context.Guild.Id;

                string content = "";

                foreach (var guild in Configuration.guildConfigs)
                    content += guild.Key.Name + $"[{guild.Key.Id}]\n";

                // Creates the Category to be sent.
                var commandEmbed = new EmbedBuilder
                {
                    Title = ":hammer_pick: Developer Guild List",
                    Description = content,
                    Color = Color.Green
                };
                await Embeder.SafeEmbedBoolAsync(commandEmbed, Context.User, (ITextChannel)Context.Channel, " ");

                Configuration.SaveGlobalConfig();
            }

        }


    }
}
