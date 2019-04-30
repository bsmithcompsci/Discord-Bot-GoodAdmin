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

namespace Module_Administrative.Core.Commands
{
    public class WarnStructure
    {
        public uint id;
        public string reason;
        public ulong warner; // The staff that warned
        public ulong warnie; // the member that got warned
        public long created; 
    }
    
    public class Warn : ModuleBase<CommandContext>
    {
        [Command("warn"), Priority(0), RequireUserPermission(GuildPermission.ManageMessages), RequireBotPermission(GuildPermission.ManageChannels), Remarks("Warning")]
        public async Task WarnCMD(IGuildUser user, [Remainder] string reason)
        {
            var config = await Configuration.LoadOrCreateGuildConfig(Context.Guild);
            var userData = Configuration.GetOrCreateUserInfo(user, config);
            if (userData != null)
            {
                var warns = GetUserWarnings(ref userData);
                var warn = new WarnStructure()
                {
                    id = (uint)(warns.Count + 1),
                    reason = reason,
                    warner = Context.User.Id,
                    warnie = user.Id,
                    created = DateTime.Now.ToBinary()
                };
                warns.Add(warn);
                userData.session["warnings"] = warns;
                userData.Save(config);
                var embed = new EmbedBuilder()
                {
                    Title = ":warning: Warning has been issued to " + user.Username,
                    Description = warn.reason,
                    Fields = new List<EmbedFieldBuilder>()
                    {
                        new EmbedFieldBuilder()
                        {
                            IsInline = true,
                            Name = "Warner",
                            Value = (await Context.Guild.GetUserAsync(warn.warner)).Mention
                        },
                        new EmbedFieldBuilder()
                        {
                            IsInline = true,
                            Name = "Warnie",
                            Value = (await Context.Guild.GetUserAsync(warn.warnie)).Mention
                        }
                    },
                    Color = Color.Red
                };
                var logs = await Configuration.GetBotLoggingChannel(Context.Guild);
                if (logs != null)
                    await Embeder.SafeEmbedAsync(embed, (ITextChannel)logs);
                if (logs.Id != Context.Channel.Id)
                    await Embeder.SafeEmbedAsync(embed, (ITextChannel)Context.Channel);

            }
        }

        [Group("warn")]
        public class SubWarn : ModuleBase<CommandContext>
        {
            [Command("remove"), RequireUserPermission(GuildPermission.ManageMessages), RequireBotPermission(GuildPermission.ManageChannels), Remarks("Warning")]
            public async Task RemoveWarning(IGuildUser user, uint id)
            {
                var config = await Configuration.LoadOrCreateGuildConfig(Context.Guild);
                var userData = Configuration.GetOrCreateUserInfo(user, config);
                if (userData != null)
                {

                    var warns = GetUserWarnings(ref userData);

                    foreach (var warn in warns)
                    {
                        if (warn.id == id)
                        {
                            warns.Remove(warn);
                            userData.session["warnings"] = warns;
                            var embed = new EmbedBuilder()
                            {
                                Title = ":warning: Warning has been removed from " + user.Username,
                                Fields = new List<EmbedFieldBuilder>()
                            {
                                new EmbedFieldBuilder()
                                {
                                    IsInline = true,
                                    Name = "Warner",
                                    Value = (await Context.Guild.GetUserAsync(warn.warner)).Mention
                                },
                                new EmbedFieldBuilder()
                                {
                                    IsInline = true,
                                    Name = "Revoker",
                                    Value = Context.User.Mention
                                },
                                new EmbedFieldBuilder()
                                {
                                    IsInline = true,
                                    Name = "Warnie",
                                    Value = (await Context.Guild.GetUserAsync(warn.warnie)).Mention
                                }
                            },
                                Color = Color.Red
                            };
                            var logs = await Configuration.GetBotLoggingChannel(Context.Guild);
                            if (logs != null)
                                await Embeder.SafeEmbedAsync(embed, (ITextChannel)logs);

                            if (logs.Id != Context.Channel.Id)
                                await Embeder.SafeEmbedAsync(embed, (ITextChannel)Context.Channel);

                            userData.Save(config);
                            break;
                        }
                    }
                }
            }

            [Command("clear"), RequireUserPermission(GuildPermission.ManageMessages), RequireBotPermission(GuildPermission.ManageChannels), Remarks("Warning")]
            public async Task ClearWarnings(IGuildUser user)
            {
                var config = await Configuration.LoadOrCreateGuildConfig(Context.Guild);
                var userData = Configuration.GetOrCreateUserInfo((IUser)user, config);
                if (userData != null)
                {
                    var warnings = GetUserWarnings(ref userData);
                    warnings.Clear();
                    userData.session["warnings"] = warnings;
                    var embed = new EmbedBuilder()
                    {
                        Title = ":warning: Warning has been cleared for " + user.Username,
                        Fields = new List<EmbedFieldBuilder>()
                    {
                        new EmbedFieldBuilder()
                        {
                            IsInline = true,
                            Name = "Invoker",
                            Value = Context.User.Mention
                        }
                    },
                        Color = Color.Red
                    };
                    var logs = await Configuration.GetBotLoggingChannel(Context.Guild);
                    if (logs != null)
                        await Embeder.SafeEmbedAsync(embed, (ITextChannel)logs);

                    if (logs.Id != Context.Channel.Id)
                        await Embeder.SafeEmbedAsync(embed, (ITextChannel)Context.Channel);
                    userData.Save(config);
                }
            }

            [Command("info"), RequireUserPermission(GuildPermission.ManageMessages), RequireBotPermission(GuildPermission.ManageChannels), Remarks("Warning")]
            public async Task Warninginfo(IGuildUser user, uint id = 0)
            {
                var config = await Configuration.LoadOrCreateGuildConfig(Context.Guild);
                var userData = Configuration.GetOrCreateUserInfo((IUser)user, config);
                if (userData != null)
                {

                    Console.WriteLine($"Listing of {user.Username}'s warnings...");
                    
                    var warns = await GetUserWarnings(user, Context.Guild);
                    if (id > 0)
                    {
                        Console.WriteLine($"id > 0");
                        if (warns != null)
                        {
                            foreach (var warn in warns)
                            {
                                if (warn.id == id)
                                {
                                    var embed = new EmbedBuilder()
                                    {
                                        Title = ":warning: Warning - " + DateTime.FromBinary(warn.created),
                                        Description = warn.reason,
                                        Fields = new List<EmbedFieldBuilder>()
                                {
                                    new EmbedFieldBuilder()
                                    {
                                        Name = "Warner",
                                        Value = await Context.Guild.GetUserAsync(warn.warner)
                                    },
                                    new EmbedFieldBuilder()
                                    {
                                        Name = "Warnie",
                                        Value = await Context.Guild.GetUserAsync(warn.warnie)
                                    }
                                },
                                        Color = Color.Red
                                    };
                                    await Embeder.SafeEmbedAsync(embed, (ITextChannel)Context.Channel);

                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine($"id <= 0");
                        if (warns != null && warns.Count > 0)
                        {
                            Console.WriteLine($"{user.Username} has some warnings...");
                            foreach (var warn in warns)
                            {
                                var embed = new EmbedBuilder()
                                {
                                    Title = ":warning: Warning #" + warn.id,
                                    Description = warn.reason,
                                    Fields = new List<EmbedFieldBuilder>()
                                {
                                    new EmbedFieldBuilder()
                                    {
                                        Name = "Warner",
                                        Value = await Context.Guild.GetUserAsync(warn.warner)
                                    },
                                    new EmbedFieldBuilder()
                                    {
                                        Name = "Warnie",
                                        Value = await Context.Guild.GetUserAsync(warn.warnie)
                                    }
                                },
                                    Footer = new EmbedFooterBuilder()
                                    {
                                        Text = DateTime.FromBinary(warn.created).ToString()
                                    },
                                    Color = Color.Red
                                };
                                await Embeder.SafeEmbedAsync(embed, (ITextChannel)Context.Channel);
                            }
                        }
                        else
                        {
                            Console.WriteLine($"{user.Username} has no warnings...");
                            var embed = new EmbedBuilder()
                            {
                                Title = ":white_check_mark: Clear of Warnings",
                                Description = user.Mention + " is clear of any warnings/infractions!",
                                Color = Color.Red
                            };
                            await Embeder.SafeEmbedAsync(embed, (ITextChannel)Context.Channel);
                        }
                    }
                }
            }
        }

        public static List<WarnStructure> GetUserWarnings(ref UserConfig userData)
        {
            List<WarnStructure> warnings = new List<WarnStructure>();

            userData.session.TryGetValue("warnings", out object val);

            Console.WriteLine("{0} | {1}", val, val.GetType());

            if (val.GetType() != typeof(List<WarnStructure>))
            {
                if (userData.session.ContainsKey("warnings"))
                    foreach (var warn in JsonConvert.DeserializeObject<List<WarnStructure>>(val.ToString()))
                        warnings.Add(warn);
            }
            else
            {
                if (userData.session.ContainsKey("warnings"))
                    foreach (var warn in val as List<WarnStructure>)
                        warnings.Add(warn);
            }

            return warnings;
        }

        public static async Task<List<WarnStructure>> GetUserWarnings(IUser user, IGuild guild)
        {
            var config = await Configuration.LoadOrCreateGuildConfig(guild);
            var userData = Configuration.GetOrCreateUserInfo(user, config);

            return GetUserWarnings(ref userData);
        }
    }
}
