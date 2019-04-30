using Discord;
using Discord.Commands;
using GoodAdmin_API;
using GoodAdmin_API.Core;
using GoodAdmin_API.Core.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Module_Administrative.Core.Commands
{
    public class MuteStructure
    {
        public string reason;
        public ulong muter; // The staff that warned
        public ulong muted; // the member that got warned
        public long created;
        public long retire; // When the mute will retire...
    }

    public class Moderatoration : ModuleBase<CommandContext>
    {
        [Command("kick"), RequireUserPermission(GuildPermission.ManageMessages), RequireBotPermission(GuildPermission.Administrator), Remarks("Admin")]
        public async Task KickAsync(IUser user, [Remainder]string reason)
        {
        }
        [Command("ban"), RequireUserPermission(GuildPermission.ManageMessages), RequireBotPermission(GuildPermission.Administrator), Remarks("Admin")]
        public async Task BanAsync(IUser user, [Remainder]string reason)
        {
        }
        [Command("mute"), RequireUserPermission(GuildPermission.ManageMessages), RequireBotPermission(GuildPermission.ManageRoles), Remarks("Admin")]
        public async Task MuteAsync(IUser user, string timeformat,[Remainder] string reason)
        {
            var config = await Configuration.LoadOrCreateGuildConfig(Context.Guild);
            config.session.TryGetValue("role-muted", out object val);
            var role = GuildUtils.GetRole(Convert.ToUInt64(val), Context.Guild);

            if (role != null)
                await GuildUtils.AssignRole(user, role);
                
            /*
             * s = Seconds
             * m = minutes
             * h = hours
             * d = days
             * w = weeks
             * M = months
             * y = years
            */
            DateTime time = GetTimeFromFormat(timeformat);

            var embed = new EmbedBuilder()
            {
                Title = $@":clipboard: Muted {user.Username}",
                Fields = new List<EmbedFieldBuilder>()
                {
                    new EmbedFieldBuilder()
                    {
                        Name = "When the Mute Retires",
                        Value = time.ToString() + " " + String.Join("", TimeZone.CurrentTimeZone.StandardName.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries).Select(s => s[0]))
                    },
                    new EmbedFieldBuilder()
                    {
                        Name = "Muter",
                        Value = Context.User.Mention
                    }
                }
            };
            var logs = await Configuration.GetBotLoggingChannel(Context.Guild);
            if (logs != null)
                await Embeder.SafeEmbedAsync(embed, (ITextChannel)logs);
            if (logs.Id != Context.Channel.Id)
            {
                var confirmation = await Embeder.SafeEmbedAsync(embed, (ITextChannel)Context.Channel);

                Thread t = new Thread(async () => {
                    try
                    {
                        await Task.Delay(5000);
                        await confirmation.DeleteAsync();
                    }
                    catch { }
                });
                t.Start();
            }

            var userData = Configuration.GetOrCreateUserInfo(user, config);
            if (userData != null)
            {
                userData.session["isMuted"] = new MuteStructure()
                {
                    created = DateTime.Now.ToBinary(),
                    reason = reason,
                    muted = user.Id,
                    muter = Context.User.Id,
                    retire = time.ToBinary()
                };
            }
        }
        [Command("unmute"), RequireUserPermission(GuildPermission.ManageMessages), RequireBotPermission(GuildPermission.ManageRoles), Remarks("Admin")]
        public async Task UnmuteAsync(IUser user)
        {
            var config = await Configuration.LoadOrCreateGuildConfig(Context.Guild);
            config.session.TryGetValue("role-muted", out object val);
            var role = GuildUtils.GetRole(Convert.ToUInt64(val), Context.Guild);
            var embed = new EmbedBuilder()
            {
                Title = $@":clipboard: UnMuted {user.Username}",
                Fields = new List<EmbedFieldBuilder>()
                {
                    new EmbedFieldBuilder()
                    {
                        Name = "UnMuter",
                        Value = Context.User.Mention
                    }
                }
            };
            var logs = await Configuration.GetBotLoggingChannel(Context.Guild);
            if (logs != null)
                await Embeder.SafeEmbedAsync(embed, (ITextChannel)logs);
            if (logs.Id != Context.Channel.Id)
            {
                var confirmation = await Embeder.SafeEmbedAsync(embed, (ITextChannel)Context.Channel);

                Thread t = new Thread(async () => {
                    try
                    {
                        await Task.Delay(5000);
                        await confirmation.DeleteAsync();
                    }
                    catch { }
                });
                t.Start();
            }

            if (role != null)
                await GuildUtils.RemoveRole(user, role);
        }

        private DateTime GetTimeFromFormat(string msg)
        {
            DateTime time = DateTime.Now;
            string storedValue = "";
            for (var i = 0; i < msg.Length; i++)
            {
                if (int.TryParse(msg[i].ToString(), out int value))
                    storedValue += "" + value;
                else
                {
                    int finalValue = 0;
                    if (storedValue != "")
                        int.TryParse(storedValue, out finalValue);
                    
                    switch (msg[i])
                    {
                        case 's':
                            Console.WriteLine("Added Seconds : {0}", finalValue);
                            time = time.AddSeconds(finalValue);
                            break;
                        case 'm':
                            Console.WriteLine("Added Minutes : {0}", finalValue);
                            time = time.AddMinutes(finalValue);
                            break;
                        case 'h':
                            Console.WriteLine("Added Hours : {0}", finalValue);
                            time = time.AddHours(finalValue);
                            break;
                        case 'd':
                            Console.WriteLine("Added Days : {0}", finalValue);
                            time = time.AddDays(finalValue);
                            break;
                        case 'w':
                            Console.WriteLine("Added Weeks : {0}", finalValue);
                            time = time.AddDays(finalValue*7);
                            break;
                        case 'M':
                            Console.WriteLine("Added Months : {0}", finalValue);
                            time = time.AddMonths(finalValue);
                            break;
                        case 'y':
                            Console.WriteLine("Added Years : {0}", finalValue);
                            time = time.AddYears(finalValue);
                            break;
                    }

                    storedValue = "";
                }
            }

            return time;
        }
    }
}
