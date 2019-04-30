using Discord;
using Discord.Commands;
using GoodAdmin_API.Core;
using GoodAdmin_API.Core.Chat;
using GoodAdmin_API.Core.GameViewers.FiveM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Module_GameManager.Core
{
    public class ServerInfo
    {
        public string IPAddress;
        public uint portAddress;
        public string game;
        public Dictionary<string, object> player_usernames;
    }

    public enum GameSupported
    {
        FiveM
    }

    [Group("GameViewer")]
    public class GameViewer : ModuleBase<CommandContext>
    {
        [Command("servers"), RequireBotPermission(GuildPermission.SendMessages), Remarks("Game Viewer")]
        public async Task ServersAsync()
        {
            var config = await Configuration.LoadOrCreateGuildConfig(Context.Guild);
            if (!config.session.ContainsKey("servers"))
                config.session.Add("servers", new List<ServerInfo>());

            var servers = config.session["servers"] as List<ServerInfo>;
            var embed = new EmbedBuilder()
            {
                Title = $@":video_game: Game Viewer - Servers",
                Description = "",
                Color = Color.Green
            };
            await Embeder.SafeEmbedAsync(embed, (ITextChannel)Context.Channel);
            foreach (var server in servers)
            {
                embed = new EmbedBuilder()
                {
                    Title = $@":video_game: Game Viewer - Server #" + servers.IndexOf(server),
                    Fields = new List<EmbedFieldBuilder>()
                    {
                        new EmbedFieldBuilder()
                        {
                            Name = "IP Address",
                            Value = server.IPAddress
                        },
                        new EmbedFieldBuilder()
                        {
                            IsInline = true,
                            Name = "Port Address",
                            Value = server.portAddress
                        },
                        new EmbedFieldBuilder()
                        {
                            Name = "Players ["+ server.player_usernames.Count +" Online]",
                            Value = String.Join("\n", server.player_usernames.Keys)
                        }
                    },
                    Color = Color.Green
                };
                await Embeder.SafeEmbedAsync(embed, (ITextChannel)Context.Channel);
            }
        }

        [Command("server"), RequireBotPermission(GuildPermission.SendMessages), Remarks("Game Viewer")]
        public async Task ServerAsync(uint id)
        {
            ServerInfo server = null;
            if (server != null)
            {
                var embed = new EmbedBuilder()
                {
                    Title = $@":video_game: Game Viewer - Server",
                    Fields = new List<EmbedFieldBuilder>()
                    {
                        new EmbedFieldBuilder()
                        {
                            Name = "IP Address",
                            Value = server.IPAddress
                        },
                        new EmbedFieldBuilder()
                        {
                            IsInline = true,
                            Name = "Port Address",
                            Value = server.portAddress
                        },
                        new EmbedFieldBuilder()
                        {
                            Name = "Players ["+ server.player_usernames.Count +" Online]",
                            Value = String.Join("\n", server.player_usernames.Keys)
                        }
                    },
                    Color = Color.Green
                };
                await Embeder.SafeEmbedAsync(embed, (ITextChannel)Context.Channel);
            }
        }

        [Command("addserver"), RequireBotPermission(GuildPermission.SendMessages), Remarks("Game Viewer")]
        public async Task AddServerAsync(string IPAddress, uint portAddress, [Remainder] string game)
        {
            foreach (var support in Enum.GetNames(typeof(GameSupported)))
            {
                if (support.ToLower() == game.ToLower())
                {
                    try
                    {
                        Enum.TryParse(support, out GameSupported result);
                        await AddServerInfo(result, IPAddress, portAddress);

                        var embed = new EmbedBuilder()
                        {
                            Title = $@":video_game: Game Viewer - Added Server",
                            Fields = new List<EmbedFieldBuilder>()
                            {
                                new EmbedFieldBuilder()
                                {
                                    Name = "IP Address",
                                    Value = IPAddress
                                },
                                new EmbedFieldBuilder()
                                {
                                    IsInline = true,
                                    Name = "Port Address",
                                    Value = portAddress
                                },
                            },
                            Color = Color.Green
                        };
                        await Embeder.SafeEmbedAsync(embed, (ITextChannel)Context.Channel);
                    }
                    catch {}
                    break;
                }
            }
        }

        private async Task AddServerInfo(GameSupported supported, string IPAddress, uint portAddress)
        {
            var config = await Configuration.LoadOrCreateGuildConfig(Context.Guild);
            if (!config.session.ContainsKey("servers"))
                config.session.Add("servers", new List<ServerInfo>());
            
            var servers = config.session["servers"] as List<ServerInfo>;
            servers.Add(new ServerInfo()
            {
                IPAddress = IPAddress,
                portAddress = portAddress,
                player_usernames = new Dictionary<string, object>()
            });

            await config.Save(Context.Guild);
        }

        private async Task UpdateServerInfo(ServerInfo serverInfo)
        {
            Enum.TryParse(serverInfo.game, out GameSupported gameSupported);
            switch(gameSupported)
            {
                case GameSupported.FiveM:
                    //Console.WriteLine(await ServerViewer.GetPlayersInfo(serverInfo.IPAddress + ":" + serverInfo.portAddress));
                    Console.WriteLine(await ServerViewer.GetServerInfo(serverInfo.IPAddress + ":" + serverInfo.portAddress));
                    break;
            }
        }
    }
}
