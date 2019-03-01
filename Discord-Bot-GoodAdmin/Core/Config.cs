using Discord;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace GoodAdmin.Core
{
    public class ConfigC
    {
        public string TOKEN { get; set; }
        public string PREFIX { get; set; }

        public ulong DevErrorsChannel { get; set; }
        public ulong DevLogsChannel { get; set; }
    }

    // TODO : Create and Connect a guild system...
    public class GuildConfig
    {
        public uint DeletionDelay = 5;

        public bool BotChannelWhitelistMode = true;

        // Channels
        public ulong LogsChannel { get; set; }
        public ulong EditorLogsChannel { get; set; }
        public ulong ErrorsChannel { get; set; }
        public ulong JoinChannel { get; set; }
        public ulong LeaveChannel { get; set; }
        public ulong PunishmentChannel { get; set; } // Mute Channel
        public ulong AgreementChannel { get; set; } // Agreement Channel
        public ulong TicketChannel { get; set; } // Support/Ticket Channel for users to post in.
        public ulong[] BotAccessChannels { get; set; } // Either Blacklist or Whitelist enabled, the bot can only interact or read commands that come into a channel that it is granted perms too. [BotChannelWhitelistMode]
    }

    public class Config
    {
        public static ConfigC config;

        public static List<GuildConfig> guildConfigs = new List<GuildConfig>();

        /// <summary>
        /// Loads the Configuration that would either be overriding all guilds or developer information that isn't shared towards public guilds.
        /// </summary>
        /// <returns></returns>
        public static Task LoadGlobalConfig()
        {
            string configRaw = "";
            if (!File.Exists("./gConfig.json"))
            {
                configRaw = JsonConvert.SerializeObject(new ConfigC());
                File.WriteAllText("./gConfig.json", configRaw);
            }
            else
                configRaw = File.ReadAllText("./gConfig.json");
            config = JsonConvert.DeserializeObject<ConfigC>(configRaw);

            return Task.CompletedTask;
        }

        /// <summary>
        /// Loads the Configuration of that guilds, information and loads it into a result for in-scope uses.
        /// </summary>
        /// <param name="guild"></param>
        /// <returns></returns>
        public static Task<GuildConfig> LoadGuildConfig(IGuild guild)
        {
            // TODO : Receive from the SQL Database the information, then give result of the Guild Configuration for developer use. 
            return null;
        }
    }
}
