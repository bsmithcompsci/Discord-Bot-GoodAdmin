using Discord;
using Discord.Commands;
using GoodAdmin_API.Core;
using GoodAdmin_API.Core.Chat;
using GoodAdmin_API.Core.Controllers.Shared;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodAdmin.Core.Commands
{
    public class SetupCommand : ModuleBase<CommandContext>
    {
        [Command("setup"), Summary("Sets up the guild/server with the bot."), RequireBotPermission(Discord.GuildPermission.Administrator), RequireUserPermission(Discord.GuildPermission.Administrator), Remarks("Server Management")]
        public async Task SetupCMD(bool cleanInstall = false, string mode="auto")
        {
            var dm = await Context.User.GetOrCreateDMChannelAsync();
            var ch = Context.Channel;

            string buildcontent = "";
            buildcontent += "Build Mode : " + mode + "\n";

            var channelToPostResults = Context.Channel;

            GuildConfig config = await Configuration.LoadOrCreateGuildConfig(Context.Guild);
            config.session.Clear();
            config.controllers.Clear();
            config.tickets.Clear();

            if (config == null) return;

            if (mode.ToLower() == "auto")
            {
                //=========================================================
                    // Cleaning Server...

                if (cleanInstall)
                {
                    buildcontent += "Cleaning Discord Server" + "\n";
                    var allChannels = await Context.Guild.GetChannelsAsync();
                    foreach(var channel in allChannels)
                    {
                        if (channel != (IGuildChannel)Context.Channel)
                            try
                            {
                                await channel.DeleteAsync();
                                buildcontent += "Deleted " + channel.Name + "\n";
                            }
                            catch { }
                    }
                    
                    foreach (var role in Context.Guild.Roles)
                    {
                        try
                        {
                            await role.DeleteAsync();
                            buildcontent += "Deleted " + role.Name + "\n";
                        }
                        catch { }
                    }

                    await Task.Delay(500);
                }

                //=========================================================
                    // Creating Roles...

                var members = await AddRole("Members");
                if (members != null)
                {
                    await members.ModifyAsync((x) => { x.Mentionable = false; x.Hoist = true; });
                    config.session.Add("roles-members", members.Id);
                    buildcontent += "Added role " + $"{members.Name} | {members.Mention}" + "\n";
                }

                var support = await AddRole("Support Team", new GuildPermissions(manageMessages: true));
                if (support != null)
                {
                    await support.ModifyAsync((x) => { x.Mentionable = true; x.Hoist = true; });
                    config.session.Add("roles-support_team", support.Id);
                    buildcontent += "Added role " + $"{support.Name} | {support.Mention}" + "\n";
                }

                var controllers = new List<object[]>();

                //=========================================================
                    // Creating Introduction area...

                buildcontent += "Creating Agreement System - This system is optional, but installed by default.";
                var welcomeCategory = await Context.Guild.CreateCategoryAsync("Welcome to " + Context.Guild.Name, x => { x.Position = 0; });
                buildcontent += "Creating Category Channel : " + welcomeCategory.Name + "\n";
                var agreementChannel = await Context.Guild.CreateTextChannelAsync("rules-and-regulations", x => { x.CategoryId = welcomeCategory.Id; });
                buildcontent += "Creating Text-Channel : " + agreementChannel.Name + "\n";
                
                //=========================================================
                    // Creating Support area...

                buildcontent += "Creating Ticket System - This system is optional, but installed by default.";
                var supportCategory = await Context.Guild.CreateCategoryAsync("Support Area", x => { });
                await supportCategory.AddPermissionOverwriteAsync(Context.Guild.EveryoneRole, new OverwritePermissions(sendMessages: PermValue.Deny, readMessageHistory: PermValue.Deny, viewChannel: PermValue.Deny, sendTTSMessages: PermValue.Deny, manageChannel: PermValue.Deny, manageMessages: PermValue.Deny, manageRoles: PermValue.Deny));
                buildcontent += "Creating Category Channel : " + supportCategory.Name + "\n";

                var supportchatChannel = await Context.Guild.CreateTextChannelAsync("support-chat", x => { x.CategoryId = supportCategory.Id; });
                await supportchatChannel.AddPermissionOverwriteAsync(support, new OverwritePermissions(sendMessages: PermValue.Allow, viewChannel: PermValue.Allow, readMessageHistory: PermValue.Allow));
                buildcontent += "Creating Text-Channel : " + supportchatChannel.Name + "\n";

                var supportChannel = await Context.Guild.CreateTextChannelAsync("support-channel", x => { x.CategoryId = supportCategory.Id; });
                await supportChannel.AddPermissionOverwriteAsync(Context.Guild.EveryoneRole, new OverwritePermissions(sendMessages: PermValue.Allow, viewChannel: PermValue.Allow));
                await supportChannel.ModifyAsync((x) => { x.SlowModeInterval = 30; });
                buildcontent += "Creating Text-Channel : " + supportChannel.Name + "\n";

                var controller = new SupportChannelController(supportChannel, supportCategory, Context.Guild, support);
                controller.InvokeInitialize();
                controllers.Add(new object[] {
                    controller.ToString(),
                    controller.GetInfo().guild.Id,
                    controller.GetInfo().parent.Id,
                    controller.GetInfo().channel.Id
                });
                GlobalInit.controllerHandler.AddController(controller);
                buildcontent += "Tied Controller to Text-Channel : " + supportChannel.Name + "\n";

                //=========================================================
                    // Logs...
                var logsCategory = await Context.Guild.CreateCategoryAsync("Logging Area", x => { });
                await logsCategory.AddPermissionOverwriteAsync(Context.Guild.EveryoneRole, new OverwritePermissions(sendMessages: PermValue.Deny, readMessageHistory: PermValue.Deny, viewChannel: PermValue.Deny, sendTTSMessages: PermValue.Deny, manageChannel: PermValue.Deny, manageMessages: PermValue.Deny, manageRoles: PermValue.Deny));
                buildcontent += "Creating Category Channel : " + supportCategory.Name + "\n";

                var ticketLogsChannel = await Context.Guild.CreateTextChannelAsync("ticket-logs", x => { x.CategoryId = logsCategory.Id; });
                buildcontent += "Creating Text-Channel : " + ticketLogsChannel.Name + "\n";
                await ticketLogsChannel.AddPermissionOverwriteAsync(support, new OverwritePermissions(viewChannel: PermValue.Allow, readMessageHistory: PermValue.Allow));

                var logsChannel = await Context.Guild.CreateTextChannelAsync("bot-logs", x => { x.CategoryId = logsCategory.Id; });
                buildcontent += "Creating Text-Channel : " + logsChannel.Name + "\n";


                channelToPostResults = logsChannel;

                if (cleanInstall)
                    await ((ITextChannel)Context.Channel).DeleteAsync();

                //=========================================================
                    // Saving data....
                if (config != null)
                {
                    config.session.Add("welcome-agreement-channel", agreementChannel.Id);
                    config.session.Add("support-channel", agreementChannel.Id);
                    config.session.Add("logging-channel", logsChannel.Id);
                    config.session.Add("ticket-logging-channel", ticketLogsChannel.Id);
                    config.controllers.AddRange(controllers);
                    await Configuration.SaveGuildConfig(this.Context.Guild, config);
                }
                else
                {
                    buildcontent += "Error : Configuration were able to inserted into the database! (Contact Developers)" + "\n";
                }
            }
            else if (mode.ToLower() == "manual")
            {

            }
            else
                buildcontent += "Error : Build Mode '" + mode + "' not recongized, please use either 'auto' or 'manual'" + "\n";
            
            EmbedBuilder embed = new EmbedBuilder
            {
                Title = "GoodAdmin Setup",
                Description = "```\n" + buildcontent + "\n```",
                Color = Color.Green
            };
#if DEBUG
            embed.AddField(new EmbedFieldBuilder()
            {
                Name = "JSON Data",
                Value = JsonConvert.SerializeObject(config, Formatting.Indented)
            });
#endif
            var sendNotification = await Embeder.SafeEmbedBoolAsync(embed, Context.User, (ITextChannel)channelToPostResults);
        }

        private async Task<IRole> AddRole(string name, GuildPermissions? perms = null, Color? clr = null)
        {
            //var existingRole = GetRole(name);
            //if (existingRole == null)
            //{
                Console.WriteLine($"{name} role has been created!");
                try
                {
                    return await Context.Guild.CreateRoleAsync(name, perms, clr);
                } catch { }
            //}

            //Console.WriteLine($"{name} ->? {existingRole} | {existingRole.Id} | {existingRole.Mention}");

            return null;
        }

        private IRole GetRole(string name)
        {
            foreach (var role in Context.Guild.Roles)
                if (role.Name == name) return role;

            Console.WriteLine("Nulled");

            return null;
        }
    }
}
