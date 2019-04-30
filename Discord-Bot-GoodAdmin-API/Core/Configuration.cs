using Discord;
using Discord.WebSocket;
using GoodAdmin_API.Core.Controllers;
using GoodAdmin_API.Core.Controllers.Shared;
using GoodAdmin_API.Core.Database;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodAdmin_API.Core
{
    [Serializable]
    public class GlobalConfig
    {
        public string TOKEN { get; set; }
        public string PREFIX { get; set; }

        public ulong developerGuildID;

        public Dictionary<string, object> session = new Dictionary<string, object>();

        public void Save()
        {
            Configuration.SaveGlobalConfig();
        }
    }

    [Serializable]
    public class GuildConfig
    {
        public Dictionary<string, object> session = new Dictionary<string, object>();
        public List<object[]> controllers = new List<object[]>();
        public List<TicketSimpleStruct> tickets = new List<TicketSimpleStruct>();
        public List<UserConfig> userConfigs = new List<UserConfig>();

        public async Task Save(IGuild guild)
        {
            await Configuration.SaveGuildConfig(guild, this);
        }
    }

    [Serializable]
    public class UserConfig
    {
        public ulong uid;
        public Dictionary<string, object> session = new Dictionary<string, object>();

        public void Save(GuildConfig config)
        {
            Configuration.SaveUserConfig(this, config);
        }
    }

    public class Configuration
    {
        public static string TimeZone = "PST";

        public static GlobalConfig globalConfig;

        public static Dictionary<IGuild, GuildConfig> guildConfigs = new Dictionary<IGuild, GuildConfig>();

        /// <summary>
        /// Loads the Configuration that would either be overriding all guilds or developer information that isn't shared towards public guilds.
        /// </summary>
        /// <returns></returns>
        public static async Task LoadGlobalConfig()
        {
            // Load Global Configurations
            string configRaw = "";
            if (!File.Exists("./gConfig.json"))
            {
                configRaw = JsonConvert.SerializeObject(new GlobalConfig());
                File.WriteAllText("./gConfig.json", configRaw);
            }
            else
                configRaw = File.ReadAllText("./gConfig.json");
            globalConfig = JsonConvert.DeserializeObject<GlobalConfig>(configRaw);

            await BuildConfigurationDatabase();
        }

        public static void SaveGlobalConfig()
        {
            var configRaw = JsonConvert.SerializeObject(globalConfig);
            File.WriteAllText("./gConfig.json", configRaw);
        }

        public static async Task LoadGlobalGuildConfigs(DiscordSocketClient client)
        {
            await SQL.FetchAllAsync("SELECT * FROM guilds", async (rows) => {
                foreach (var row in rows)
                {
                    row.TryGetValue("uid", out object json_uid);
                    row.TryGetValue("config", out object json_config);

                    var json_result = JsonConvert.DeserializeObject<GuildConfig>(json_config.ToString());

                    if (json_result != null && json_uid.ToString() != "")
                    {
                        try
                        {
                            SocketGuild guild = null;
                            foreach (var g in client.Guilds)
                            {
                                if (g.Id == Convert.ToUInt64(json_uid))
                                {
                                    guild = g;
                                    break;
                                }
                            }

                            if (guild != null)
                            {
                                // Checks if the guild is connected to the discord bot, and if the guild wasn't added to the list already...
                                if (GuildConnected(client, guild) && !GuildExists(guild))
                                {
                                    guildConfigs.Add(guild, json_result);
                                    Console.WriteLine("Loaded Guild Configurations : " + guild.Name + (globalConfig.developerGuildID == guild.Id ? " {[Developer Guild]}" : ""));

                                    foreach (object[] controller in json_result.controllers.ToArray())
                                    {
                                        // Issue #1 : Controller issue.
                                        if (controller.Length == 4 && controller[0].ToString() == (new SupportChannelController(null, null, null, supportTeam: null)).ToString())
                                        {
                                            var ch = guild.GetChannel(Convert.ToUInt64(controller[3].ToString()));
                                            if (ch != null)
                                            {
                                                var newController = new SupportChannelController(
                                                    ch,
                                                    (ICategoryChannel)guild.GetChannel(Convert.ToUInt64(controller[2].ToString())),
                                                    guild,
                                                    await SupportChannelController.GetSupportTeam(guild)
                                                );

                                                GlobalInit.controllerHandler.AddController(newController);
                                                Console.WriteLine("Loaded Guild Configurations - Controller["+ controller[0].ToString().Split('.').Last() + "] :: " + ch.Name);
                                            }
                                        }
                                        /*
                                        if (controller.Length == 4 && controller[0].ToString() == (new AgreementChannelController(null, null, null)).ToString())
                                        {
                                            var ch = guild.GetChannel(Convert.ToUInt64(controller[3].ToString()));
                                            if (ch != null)
                                            {
                                                var newController = new AgreementChannelController(
                                                    ch,
                                                    (ICategoryChannel)guild.GetChannel(Convert.ToUInt64(controller[2].ToString())),
                                                    guild
                                                );

                                                GlobalInit.controllerHandler.AddController(newController);
                                                Console.WriteLine("Loaded Guild Configurations - Controller[" + controller[0].ToString().Split('.').Last() + "] :: " + ch.Name);
                                            }
                                        }*/

                                    }

                                    var guildController = new GuildController(
                                        guild
                                    );

                                    GlobalInit.controllerHandler.AddController(guildController);
                                    Console.WriteLine("Loaded Guild Configurations - Controller[" + guildController.ToString().Split('.').Last() + "]");


                                    await LoadGuildTickets(json_config.ToString(), guild);
                                }
                            }

                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                        }
                    }
                }
                Console.WriteLine("Loaded " + guildConfigs.Count + " guilds!");
            });
        }
        
        public static async Task LoadGuildTickets(string json_config, SocketGuild guild)
        {
            if (guild == null) return;
            var json_result = JsonConvert.DeserializeObject<GuildConfig>(json_config.ToString());
            foreach (var ticket in json_result.tickets)
            {
                IChannel ch = null;
                foreach (var channel in guild.Channels)
                {
                    if (channel.Id == ticket.channelUID)
                    {
                        ch = channel;
                        break;
                    }
                }
                if (ch != null)
                {
                    var assignedUsers = new List<IUser>();
                    foreach (var assigned in ticket.assignedUIDs)
                    {
                        var user = guild.GetUser(assigned);
                        if (user != null)
                            assignedUsers.Add(user);
                    }
                    var assignedHistoryUsers = new List<IUser>();
                    foreach (var assigned in ticket.assignedHistoryUIDs)
                    {
                        var user = guild.GetUser(assigned);
                        if (user != null)
                            assignedHistoryUsers.Add(user);
                    }


                    var newTicket = new GuildTicket()
                    {
                        id = ticket.id,
                        initialMessage = ticket.initialMessage,
                        channel = (ITextChannel)ch,
                        author = guild.GetUser(ticket.authorUID),
                        status = (TicketStatus)ticket.statusUID,
                        dateTime = DateTime.FromBinary(ticket.dateTime),
                        assigned = assignedUsers,
                        assignedHistory = assignedHistoryUsers
                    };

                    // Delete the tickets that have been closed... And Add the Active or open ones. And Delete old ones, after 1 week... On Load...
                    if (newTicket.status != TicketStatus.Closed && (newTicket.dateTime != null && newTicket.dateTime.Subtract(DateTime.Now).TotalHours <= 24 * 7))
                    {
                        Console.WriteLine("Ticket #" + ticket.id + " has been loaded!");
                        SupportChannelController.AddGuildTicket(guild, newTicket);
                    }
                    else
                    {
                        try
                        {
                            await newTicket.channel.DeleteAsync();
                        }
                        catch { }
                    }
                }
            }
            // Update the missing ticket channels, and delete them from data.
            await SupportChannelController.SaveTickets(guild);
            Console.WriteLine("Guild Ticket Count : " + json_result.tickets.Count);


#if DEBUG
            Console.WriteLine(json_config);
#endif
        }

        public static async Task BuildConfigurationDatabase()
        {
            //await SQL.ExecuteAsync("CREATE TABLE IF NOT EXISTS users (uid TEXT NOT NULL, guilduid TEXT NOT NULL)", (x) => { }, debug: false);
            await SQL.ExecuteAsync("CREATE TABLE IF NOT EXISTS guilds (uid TEXT NOT NULL, config TEXT NOT NULL)", (x) => { }, debug: false);
        }

        public static UserConfig GetOrCreateUserInfo(IUser user, GuildConfig guild)
        {
            foreach (UserConfig cfg in guild.userConfigs)
                if (cfg.uid == user.Id)
                    return cfg;

            UserConfig usercfg = new UserConfig()
            {
                session = new Dictionary<string, object>(),
                uid = user.Id
            };
            guild.userConfigs.Add(usercfg);

            return usercfg;
        }

        /// <summary>
        /// Loads the Configuration of that guilds, information and loads it into a result for in-scope uses.
        /// </summary>
        /// <param name="guild"></param>
        /// <returns></returns>
        public static Task<GuildConfig> LoadOrCreateGuildConfig(IGuild guild)
        {
            return Task.Run(async () =>
            {
                GuildConfig result = null;
                if (guild == null) return null;
                Parallel.ForEach(guildConfigs, (cfg) =>
                {
                    //Console.WriteLine($"{cfg.Key.Id} | {cfg.Value.session.Count}");
                    if (cfg.Key.Id == guild.Id)
                    {
                        result = cfg.Value;
                    }
                });
                if (result == null)
                {
                    // First Search the Database
                    await SQL.FetchAllAsync("SELECT config FROM guilds WHERE uid=@uid", (x) =>
                    {
                        foreach(var row in x)
                        {
                            bool found = false;
                            foreach (KeyValuePair<string, object> col in row)
                            {
                                found = true;
                                break;
                            }
                            if (found)
                                break;
                        }
                    },
                    new List<System.Data.SQLite.SQLiteParameter>()
                    {
                        new System.Data.SQLite.SQLiteParameter() { ParameterName = "@uid", Value = guild.Id }
                    });
                }
                // Else Create new Data
                if (result == null)
                    return await CreateGuildConfig(guild);
                else
                    return result;
            });
        }
        
        public static void SaveUserConfig(UserConfig usercfg, GuildConfig config)
        {
            for(var i = 0; i < config.userConfigs.Count; i++)
                if(config.userConfigs[i].uid == usercfg.uid)
                {
                    config.userConfigs[i] = usercfg;
                    break;
                }
        }

        /// <summary>
        /// Saves the Guild Configurations to the Guild's Database Row.
        /// </summary>
        /// <param name="guild"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static async Task SaveGuildConfig(IGuild guild, GuildConfig config)
        {
            //if (guildConfigs.ContainsKey(guild))
                guildConfigs[guild] = config;

            await SQL.ExecuteAsync(
                "UPDATE guilds SET config=@config WHERE uid=@uid",
                (x) => { },
                new List<System.Data.SQLite.SQLiteParameter>() {
                    new System.Data.SQLite.SQLiteParameter() { ParameterName = "config", Value = JsonConvert.SerializeObject(config) },
                    new System.Data.SQLite.SQLiteParameter() { ParameterName = "uid", Value = guild.Id
                }
            });
        }

        public static async Task<GuildConfig> CreateGuildConfig(IGuild guild)
        {
            GuildConfig result = null;
            await SQL.ExecuteAsync("INSERT INTO guilds (uid, config) VALUES (@uid, @config)", (x) => { },
                new List<System.Data.SQLite.SQLiteParameter>() {
                    new System.Data.SQLite.SQLiteParameter() { ParameterName = "@uid", Value = guild.Id },
                    new System.Data.SQLite.SQLiteParameter() { ParameterName = "@config", Value = JsonConvert.SerializeObject(new GuildConfig())}
                }
            );

            await SQL.FetchAllAsync("SELECT config FROM guilds WHERE uid=@uid", (x) =>
                {
                    foreach (var row in x)
                    {
                        bool found = false;
                        foreach (KeyValuePair<string, object> col in row)
                        {
                            //Console.WriteLine(col.Value);
                            result = JsonConvert.DeserializeObject<GuildConfig>(col.Value.ToString());
                            found = true;
                            break;
                        }
                        if (found)
                            break;
                    }
                },
                new List<System.Data.SQLite.SQLiteParameter>()
                {
                    new System.Data.SQLite.SQLiteParameter() { ParameterName = "@uid", Value = guild.Id }
                }
            );

            if (result != null && !GuildExists(guild))
                guildConfigs.Add(guild, result);

            return result;
        }

        public static async Task<KeyValuePair<IGuild, GuildConfig>?> RemoveGuildConfig(IGuild guild)
        {
            KeyValuePair<IGuild, GuildConfig>? result = null;

            Parallel.ForEach(guildConfigs, (cfg) =>
            {
                if (cfg.Key.Id == guild.Id)
                    result = new KeyValuePair<IGuild, GuildConfig>(guild, cfg.Value);
            });


            await SQL.ExecuteAsync("DELETE FROM guilds WHERE uid=@uid", (x) => { }, new List<System.Data.SQLite.SQLiteParameter>() {
                    new System.Data.SQLite.SQLiteParameter() { ParameterName = "@uid", Value = guild.Id}
            });

            guildConfigs.Remove(guild);

            return result;
        }

        public static IGuild GetDeveloperGuild(DiscordSocketClient client)
        {
            return client.GetGuild(globalConfig.developerGuildID);
        }

        public static async Task<IGuildChannel> GetDeveloperLogs(DiscordSocketClient client)
        {
            var guild = GetDeveloperGuild(client);
            globalConfig.session.TryGetValue("logs", out object val);
            try
            {
                var ch = await guild.GetChannelAsync(Convert.ToUInt64(val));
                return ch;
            }
            catch { }

            return null;
        }

        public static async Task<IGuildChannel> GetAgreementChannel(IGuild guild)
        {
            GuildConfig config = await LoadOrCreateGuildConfig(guild);
            config.session.TryGetValue("welcome-agreement-channel", out object val);
            try
            {
                var ch = await guild.GetChannelAsync(Convert.ToUInt64(val));
                return ch;
            }
            catch { }

            return null;
        }

        public static async Task<IGuildChannel> GetSupportChannel(IGuild guild)
        {
            GuildConfig config = await LoadOrCreateGuildConfig(guild);
            config.session.TryGetValue("welcome-agreement", out object val);
            try
            {
                var ch = await guild.GetChannelAsync(Convert.ToUInt64(val));
                return ch;
            }
            catch { }

            return null;
        }

        public static async Task<IGuildChannel> GetTicketLoggingChannel(IGuild guild)
        {
            GuildConfig config = await LoadOrCreateGuildConfig(guild);
            config.session.TryGetValue("ticket-logging-channel", out object val);
            try
            {
                var ch = await guild.GetChannelAsync(Convert.ToUInt64(val));
                return ch;
            }
            catch { }

            return null;
        }

        public static async Task<IGuildChannel> GetBotLoggingCategory(IGuild guild)
        {
            Console.WriteLine("Scanning for Logging Category.");
            GuildConfig config = await LoadOrCreateGuildConfig(guild);
            config.session.TryGetValue("logging-category", out object val);
            Console.WriteLine("Logging Category: " + val);
            try
            {
                var ch = await guild.GetChannelAsync(Convert.ToUInt64(val));
                return ch;
            } catch { }

            return null;
        }
        public static async Task<IGuildChannel> GetBotLoggingChannel(IGuild guild)
        {
            GuildConfig config = await LoadOrCreateGuildConfig(guild);
            config.session.TryGetValue("logging-channel", out object val);
            try
            {
                var ch = await guild.GetChannelAsync(Convert.ToUInt64(val));
                return ch;
            }
            catch { }

            return null;
        }

        public static async Task CleanDatabase()
        {
            await SQL.ExecuteAsync("DELETE FROM guilds", (x) => { });
        }

        private static bool GuildExists(IGuild target)
        {
            foreach (var guild in guildConfigs)
            {
                if (guild.Key.Id == target.Id)
                    return true;
            }

            return false;
        }

        private static bool GuildConnected(DiscordSocketClient client, IGuild target)
        {
            return (client.GetGuild(target.Id) != null);
        }
    }
}
