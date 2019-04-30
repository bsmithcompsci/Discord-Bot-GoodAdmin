using Discord;
using Discord.Commands;
using GoodAdmin_API;
using GoodAdmin_API.Core;
using GoodAdmin_API.Core.Chat;
using GoodAdmin_API.Core.Controllers;
using GoodAdmin_API.Core.Controllers.Shared;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GoodAdmin.Core.Commands
{
    [Group("setup")]
    public class SetupCommand : ModuleBase<CommandContext>
    {
        private ITextChannel channelToPostResults = null;

        [Command("build"), Summary("Sets up the guild/server with the bot."), RequireBotPermission(Discord.GuildPermission.Administrator), RequireUserPermission(Discord.GuildPermission.Administrator), Remarks("Server Management")]
        public async Task SetupCMD(bool cleanInstall = false, string mode="auto")
        {
            var dm = await Context.User.GetOrCreateDMChannelAsync();
            var ch = Context.Channel;

            string buildcontent = "";
            buildcontent += "Build Mode : " + mode + "\n";

            var embed = new EmbedBuilder()
            {
                Title = $@":construction: Setup - Building...",
                Description = "Standby this may take a moment.",
                Color = Color.Green
            };
            var log = await Embeder.SafeEmbedAsync(embed, (ITextChannel)Context.Channel);

            Thread t = new Thread(async () => {
                try
                {
                    await Task.Delay(5000);
                    await log.DeleteAsync();
                }
                catch { }
            });
            t.Start();

            GuildConfig config = await Configuration.LoadOrCreateGuildConfig(Context.Guild);
            if (config == null) return;

            config.session.Clear();
            config.controllers.Clear();
            config.tickets.Clear();

            await GlobalInit.controllerHandler.AddController(new GuildController(Context.Guild), Context.Guild);

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
                buildcontent += await _SetOrCreateMemberRole();
                buildcontent += await _SetOrCreateMutedRole();

                //=========================================================
                // Creating Introduction area...
                buildcontent += "Creating Agreement System - This system is optional, but installed by default.";
                var welcomeCategory = await Context.Guild.CreateCategoryAsync("Welcome to " + Context.Guild.Name, x => { x.Position = 0; });
                buildcontent += "Creating Category Channel : " + welcomeCategory.Name + "\n";
                buildcontent += await _SetOrCreateAgreementChannel(category: welcomeCategory);

                //=========================================================
                // Logs...
                var loggingCategory = await CreateLoggingCategory();
                await SetLoggingChannel(category: loggingCategory);

                //=========================================================
                    // Saving data....
                if (config != null)
                {
                    await config.Save(Context.Guild);
                }
                else
                {
                    buildcontent += "Error : Configuration were able to inserted into the database! (Contact Developers)" + "\n";
                }
                buildcontent += "Loading Module(s) Setup Features\n";

                #region Administrative Module

                //=========================================================
                // Creating Support area...


                buildcontent += "Creating Ticket System - This system is optional, but installed by default.\n";
                
                try
                {
                    Console.WriteLine("Creating Ticket System - Creating Support Role...");
                    IRole support = await GuildUtils.AddRole("Support Team", Context.Guild); 
                    buildcontent += await _SetOrCreateSupportRole(support);
                    var supportCategory = await CreateSupportCategory();
                    var ticketLoggingChannel = await CreateTicketLoggingChannel(loggingCategory);
                    buildcontent += await _SetOrCreateSupportTextChannel(category: supportCategory);
                    buildcontent += await _SetTicketLoggingChannel(channel: ticketLoggingChannel, category: loggingCategory);
                    buildcontent += await _SetTicketSurveyLoggingChannel(category: loggingCategory);
                    buildcontent += await _SetOrCreateSupportTicketChannel(category: supportCategory, support: support, ticketLogsChannel: ticketLoggingChannel);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Module Setup Error - Support Team Creation...");
                    Console.WriteLine(ex);
                }

                if (config != null)
                {
                    await config.Save(Context.Guild);
                }
                #endregion

                buildcontent += "\n\nFinished";
            }
            else if (mode.ToLower() == "manual")
            {

            }
            else
                buildcontent += "Error : Build Mode '" + mode + "' not recongized, please use either 'auto' or 'manual'" + "\n";
            
            embed = new EmbedBuilder
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
            if (channelToPostResults != null)
                await Embeder.SafeEmbedBoolAsync(embed, Context.User, (ITextChannel)channelToPostResults);

            if (cleanInstall)
                await ((ITextChannel)Context.Channel).DeleteAsync();
        }

        [Command("agreementchannel"), Summary("Sets the Agreement Channel"), RequireBotPermission(Discord.GuildPermission.Administrator), RequireUserPermission(Discord.GuildPermission.Administrator), Remarks("Server Management")]
        public async Task SetOrCreateAgreementChannel(ITextChannel channel = null)
        {
            await _SetOrCreateAgreementChannel(channel);
        }
        private async Task<string> _SetOrCreateAgreementChannel(ITextChannel channel = null, ICategoryChannel category = null)
        {
            string buildcontent = "";
            GuildConfig config = await Configuration.LoadOrCreateGuildConfig(Context.Guild);
            
            if (channel == null)
                channel = await Context.Guild.CreateTextChannelAsync("newcomers", x => { if (category != null) x.CategoryId = category.Id; });

            if (channel != null)
            {
                buildcontent += "Created Text-Channel : " + channel.Name + "\n";
                config.session["welcome-agreement-channel"] = channel.Id;

                var controllers = new List<object[]>();
                var controller = new AgreementChannelController(channel, category, Context.Guild);
                await GlobalInit.controllerHandler.AddController(controller, Context.Guild, new object[] {
                    Context.Guild.Id,
                    category.Id,
                    channel.Id
                });
                buildcontent += "Tied Agreement Controller to Text-Channel : " + channel.Name + "\n";

                controller.InvokeInitialize();
                config.controllers.AddRange(controllers);
                await config.Save(Context.Guild);

                var embed = new EmbedBuilder()
                {
                    Title = $@":construction: Setup - Set Agreement Channel",
                    Description = "By setting this up, automatically new comers will have to agree with the terms and services of the community; additionally they will not have access to the community until they finish agreeing.",
                    Color = Color.Green
                };
                var log = await Embeder.SafeEmbedAsync(embed, (ITextChannel)Context.Channel);

                Thread t = new Thread(async () => {
                    try
                    {
                        await Task.Delay(10000);
                        await log.DeleteAsync();
                    }
                    catch { }
                });
                t.Start();
                return buildcontent+ "Created a Text-Channel : agreement-channel\n";
            }

            return "Failed to Create a Text-Channel : agreement-channel\n";
        }

        [Command("memberrole"), Summary("Sets or Creates the Member Role"), RequireBotPermission(Discord.GuildPermission.Administrator), RequireUserPermission(Discord.GuildPermission.Administrator), Remarks("Server Management")]
        public async Task SetOrCreateMemberRole(IRole role = null)
        {
            await _SetOrCreateMemberRole(role);
        }
        private async Task<string> _SetOrCreateMemberRole(IRole role = null)
        {
            GuildConfig config = await Configuration.LoadOrCreateGuildConfig(Context.Guild);
            if (role == null)
                role = await GuildUtils.AddRole("Members", Context.Guild);

            if (role != null)
            {
                await role.ModifyAsync((x) => { x.Mentionable = false; x.Hoist = true; });
                config.session["roles-members"] = role.Id;
                await config.Save(Context.Guild);
                var embed = new EmbedBuilder()
                {
                    Title = $@":construction: Setup - Created/Set Member Role",
                    Description = role.Mention,
                    Color = Color.Green
                };
                var log = await Embeder.SafeEmbedAsync(embed, (ITextChannel)Context.Channel);

                Thread t = new Thread(async () => {
                    try
                    {
                        await Task.Delay(10000);
                        await log.DeleteAsync();
                    }
                    catch { }
                });
                t.Start();
                return "Added role " + $"{role.Name} | {role.Mention}" + "\n";
            }
            return "Failed Adding Role : Members\n";
        }

        [Command("mutedrole"), Summary("Sets or Creates the Muted Role"), RequireBotPermission(Discord.GuildPermission.Administrator), RequireUserPermission(Discord.GuildPermission.Administrator), Remarks("Server Management")]
        public async Task SetOrCreateMutedRole(IRole role = null)
        {
            await _SetOrCreateMutedRole(role);
        }
        private async Task<string> _SetOrCreateMutedRole(IRole role = null)
        {
            GuildConfig config = await Configuration.LoadOrCreateGuildConfig(Context.Guild);
            if (role == null)
                role = await GuildUtils.AddRole("Muted", Context.Guild);

            if (role != null)
            {
                await role.ModifyAsync((x) => { x.Mentionable = false; x.Hoist = true; });
                config.session["roles-muted"] = role.Id;
                await config.Save(Context.Guild);
                var embed = new EmbedBuilder()
                {
                    Title = $@":construction: Setup - Created/Set Muted Role",
                    Description = role.Mention,
                    Color = Color.Green
                };
                var log = await Embeder.SafeEmbedAsync(embed, (ITextChannel)Context.Channel);

                Thread t = new Thread(async () => {
                    try
                    {
                        await Task.Delay(10000);
                        await log.DeleteAsync();
                    }
                    catch { }
                });
                t.Start();
                return "Added role " + $"{role.Name} | {role.Mention}" + "\n";
            }
            return "Failed to Add a Role : Muted\n";
        }

        [Command("supportrole"), Summary(""), RequireBotPermission(Discord.GuildPermission.Administrator), RequireUserPermission(Discord.GuildPermission.Administrator), Remarks("Server Management")]
        public async Task SetOrCreateSupportRole(IRole role = null)
        {
            await _SetOrCreateSupportRole(role);
        }
        private async Task<string> _SetOrCreateSupportRole(IRole role = null)
        {
            string buildcontent = "";
            GuildConfig config = await Configuration.LoadOrCreateGuildConfig(Context.Guild);
            if (role == null)
                role = await GuildUtils.AddRole("Support Team", Context.Guild);

            if (role != null)
            {
                await role.ModifyAsync((x) => { x.Mentionable = true; x.Hoist = true; });
                config.session["roles-support-team"] = role.Id;

                var embed = new EmbedBuilder()
                {
                    Title = $@":construction: Setup - Created/Set Support Team",
                    Description = role.Mention,
                    Color = Color.Green
                };
                var log = await Embeder.SafeEmbedAsync(embed, (ITextChannel)Context.Channel);

                Thread t = new Thread(async () => {
                    try
                    {
                        await Task.Delay(10000);
                        await log.DeleteAsync();
                    }
                    catch { }
                });
                t.Start();
                return buildcontent+"Added role " + $"{role.Name} | {role.Mention}" + "\n";
            }
            else
            {
                config.session["roles-support-team"] = Convert.ToUInt64(0);
            }
            await Configuration.SaveGuildConfig(Context.Guild, config);
            return "Failed to Add a Role : Support Team\n";
        }
        
        [Command("supportchat"), Summary(""), RequireBotPermission(Discord.GuildPermission.Administrator), RequireUserPermission(Discord.GuildPermission.Administrator), Remarks("Server Management")]
        public async Task SetOrCreateSupportTextChannel(ITextChannel channel = null, ICategoryChannel category = null)
        {
            await _SetOrCreateSupportTextChannel(channel, category);
        }
        private async Task<string> _SetOrCreateSupportTextChannel(ITextChannel channel = null, ICategoryChannel category = null)
        {
            Console.WriteLine("Creating Ticket System - Creating Chat Channel");
            if (channel == null)
            {
                //if(category == null)
                //    category = await CreateSupportCategory();

                if (channel == null)
                    channel = await Context.Guild.CreateTextChannelAsync("support-chat", x => { if (category != null) x.CategoryId = category.Id; });

                if (channel != null)
                {
                    var embed = new EmbedBuilder()
                    {
                        Title = $@":construction: Setup - Created/Set Support Chat",
                        Description = "",
                        Color = Color.Green
                    };
                    var log = await Embeder.SafeEmbedAsync(embed, (ITextChannel)Context.Channel);

                    Thread t = new Thread(async () => {
                        try
                        {
                            await Task.Delay(10000);
                            await log.DeleteAsync();
                        }
                        catch { }
                    });
                    t.Start();
                }

                return "Created a Text-Channel : " + channel.Name + "\n";
            }

            return "Failed to Create a Text-Channel : support-chat\n";
        }

        [Command("ticket"), Summary(""), RequireBotPermission(Discord.GuildPermission.Administrator), RequireUserPermission(Discord.GuildPermission.Administrator), Remarks("Server Management")]
        public async Task SetOrCreateSupportTicketChannel(ITextChannel channel = null, ICategoryChannel category = null)
        {
            await _SetOrCreateSupportTicketChannel(channel, category);
        }
        private async Task<string> _SetOrCreateSupportTicketChannel(ITextChannel channel = null, ICategoryChannel category = null, IRole support = null, ITextChannel ticketLogsChannel = null)
        {
            string buildcontent = "";
            GuildConfig config = await Configuration.LoadOrCreateGuildConfig(Context.Guild);
            var controllers = new List<object[]>();

            if (channel == null)
                channel = await Context.Guild.CreateTextChannelAsync("support-channel", x => { if (category != null) x.CategoryId = category.Id; });
            await channel.AddPermissionOverwriteAsync(Context.Guild.EveryoneRole, new OverwritePermissions(sendMessages: PermValue.Allow, viewChannel: PermValue.Allow));
            await channel.ModifyAsync((x) => { x.SlowModeInterval = 30; });
            if (channel != null)
            {
                Console.WriteLine("Creating Ticket System - Creating Support Channel");

                config.session["support-channel"] = channel.Id;

                if (support != null)
                {
                    await channel.AddPermissionOverwriteAsync(support, new OverwritePermissions(sendMessages: PermValue.Allow, viewChannel: PermValue.Allow, readMessageHistory: PermValue.Allow));
                    if (ticketLogsChannel != null)
                        await ticketLogsChannel.AddPermissionOverwriteAsync(support, new OverwritePermissions(viewChannel: PermValue.Allow, readMessageHistory: PermValue.Allow));

                    var controller = new SupportChannelController(channel, category, Context.Guild, support);
                    await GlobalInit.controllerHandler.AddController(controller, Context.Guild, new object[] {
                        Context.Guild.Id,
                        category.Id,
                        channel.Id
                    });
                    
                    controller.InvokeInitialize();
                    config.controllers.AddRange(controllers);
                    buildcontent += "Tied Support Controller to Text-Channel : " + channel.Name + "\n";
                }
                await config.Save(Context.Guild);
                var embed = new EmbedBuilder()
                {
                    Title = $@":construction: Setup - Created/Set Support Ticket Channel",
                    Description = "",
                    Color = Color.Green
                };
                var log = await Embeder.SafeEmbedAsync(embed, (ITextChannel)Context.Channel);

                Thread t = new Thread(async () => {
                    try
                    {
                        await Task.Delay(10000);
                        await log.DeleteAsync();
                    }
                    catch { }
                });
                t.Start();
                return buildcontent + "Created a Text-Channel : " + channel.Name + "\n";
            }

            return "Failed to Create a Text-Channel : support-channel\n";
        }

        [Command("logging"), Summary(""), RequireBotPermission(Discord.GuildPermission.Administrator), RequireUserPermission(Discord.GuildPermission.Administrator), Remarks("Server Management")]
        public async Task SetLoggingChannel(ITextChannel channel = null, ICategoryChannel category = null)
        {
            await _SetLoggingChannel(channel, category);
        }
        private async Task<string> _SetLoggingChannel(ITextChannel channel = null, ICategoryChannel category = null)
        {
            string buildcontent = "";
            GuildConfig config = await Configuration.LoadOrCreateGuildConfig(Context.Guild);
            //string buildcontent = "";
            if (category == null)
            {
                //category = await CreateLoggingCategory();
                //buildcontent += "Creating Category Channel : " + category.Name + "\n";
            }
            if (channel == null)
                channel = await Context.Guild.CreateTextChannelAsync("bot-logs", x => { if (category != null) x.CategoryId = category.Id; });
            if(channel != null)
            {
                channelToPostResults = channel;
                config.session["logging-channel"] = channel.Id;
                var embed = new EmbedBuilder()
                {
                    Title = $@":construction: Setup - Created/Set Logging Channel",
                    Description = "",
                    Color = Color.Green
                };
                var log = await Embeder.SafeEmbedAsync(embed, (ITextChannel)Context.Channel);

                Thread t = new Thread(async () => {
                    try
                    {
                        await Task.Delay(10000);
                        await log.DeleteAsync();
                    }
                    catch { }
                });
                t.Start();
                return buildcontent + "Created a Text-Channel : " + channel.Name + "\n";
            }

            return "Failed to Create a Text-Channel : bot-logging\n";
        }

        [Command("ticketlogging"), Summary(""), RequireBotPermission(Discord.GuildPermission.Administrator), RequireUserPermission(Discord.GuildPermission.Administrator), Remarks("Server Management")]
        public async Task SetTicketLoggingChannel(ITextChannel channel = null, ICategoryChannel category = null)
        {
            await _SetTicketLoggingChannel(channel, category);
        }
        private async Task<string> _SetTicketLoggingChannel(ITextChannel channel = null, ICategoryChannel category = null)
        {
            GuildConfig config = await Configuration.LoadOrCreateGuildConfig(Context.Guild);

            if (channel == null)
            {
                /*if (category == null)
                {
                    category = await CreateLoggingCategory();
                    buildcontent += "Creating Category Channel : " + category.Name + "\n";
                }*/

                if (category != null)
                    channel = await CreateTicketLoggingChannel(category);
                
                var embed = new EmbedBuilder()
                {
                    Title = $@":construction: Setup - Created/Set Ticket Logging Channel",
                    Description = "",
                    Color = Color.Green
                };
                var log = await Embeder.SafeEmbedAsync(embed, (ITextChannel)Context.Channel);

                Thread t = new Thread(async () => {
                    try
                    {
                        await Task.Delay(10000);
                        await log.DeleteAsync();
                    }
                    catch { }
                });
                t.Start();
                return "Created a Text-Channel : " + channel.Name + "\n";
            }

            return "Failed to Create a Text-Channel : ticket-logging\n";
        }

        [Command("surveylogging"), Summary(""), RequireBotPermission(Discord.GuildPermission.Administrator), RequireUserPermission(Discord.GuildPermission.Administrator), Remarks("Server Management")]
        public async Task SetTicketSurveyLoggingChannel(ITextChannel channel = null, ICategoryChannel category = null)
        {
            await _SetTicketSurveyLoggingChannel(channel, category);
        }
        private async Task<string> _SetTicketSurveyLoggingChannel(ITextChannel channel = null, ICategoryChannel category = null)
        {
            GuildConfig config = await Configuration.LoadOrCreateGuildConfig(Context.Guild);

            if (channel == null)
            {
                if (category != null)
                    channel = await CreateTicketSurveyLoggingChannel(category);

                var embed = new EmbedBuilder()
                {
                    Title = $@":construction: Setup - Created/Set Ticket Logging Channel",
                    Description = "",
                    Color = Color.Green
                };
                var log = await Embeder.SafeEmbedAsync(embed, (ITextChannel)Context.Channel);

                Thread t = new Thread(async () => {
                    try
                    {
                        await Task.Delay(10000);
                        await log.DeleteAsync();
                    }
                    catch { }
                });
                t.Start();
                return "Created a Text-Channel : " + channel.Name + "\n";
            }

            return "Failed to Create a Text-Channel : ticket-logging\n";
        }

        private async Task<ICategoryChannel> CreateLoggingCategory()
        {
            GuildConfig config = await Configuration.LoadOrCreateGuildConfig(Context.Guild);
            var category = await Context.Guild.CreateCategoryAsync("Logging Area", x => { });
            await category.AddPermissionOverwriteAsync(Context.Guild.EveryoneRole, new OverwritePermissions(sendMessages: PermValue.Deny, readMessageHistory: PermValue.Deny, viewChannel: PermValue.Deny, sendTTSMessages: PermValue.Deny, manageChannel: PermValue.Deny, manageMessages: PermValue.Deny, manageRoles: PermValue.Deny));
            
            config.session["logging-category"] = category.Id;
            return category;
        }

        private async Task<ITextChannel> CreateTicketLoggingChannel(ICategoryChannel category)
        {
            GuildConfig config = await Configuration.LoadOrCreateGuildConfig(Context.Guild);
            var channel = await Context.Guild.CreateTextChannelAsync("ticket-logging", x => { x.CategoryId = category.Id; });
            await channel.AddPermissionOverwriteAsync(Context.Guild.EveryoneRole, new OverwritePermissions(sendMessages: PermValue.Deny, readMessageHistory: PermValue.Deny, viewChannel: PermValue.Deny, sendTTSMessages: PermValue.Deny, manageChannel: PermValue.Deny, manageMessages: PermValue.Deny, manageRoles: PermValue.Deny));

            config.session["ticket-logging-channel"] = channel.Id;
            return channel;
        }

        private async Task<ITextChannel> CreateTicketSurveyLoggingChannel(ICategoryChannel category)
        {
            GuildConfig config = await Configuration.LoadOrCreateGuildConfig(Context.Guild);
            var channel = await Context.Guild.CreateTextChannelAsync("ticket-survey-logging", x => { x.CategoryId = category.Id; });
            await channel.AddPermissionOverwriteAsync(Context.Guild.EveryoneRole, new OverwritePermissions(sendMessages: PermValue.Deny, readMessageHistory: PermValue.Deny, viewChannel: PermValue.Deny, sendTTSMessages: PermValue.Deny, manageChannel: PermValue.Deny, manageMessages: PermValue.Deny, manageRoles: PermValue.Deny));

            config.session["ticket-survey-logging-channel"] = channel.Id;
            return channel;
        }

        private async Task<ICategoryChannel> CreateSupportCategory()
        {
            Console.WriteLine("Creating Ticket System - Creating Category");
            var supportCategory = await Context.Guild.CreateCategoryAsync("Support Area", x => { });
            await supportCategory.AddPermissionOverwriteAsync(Context.Guild.EveryoneRole, new OverwritePermissions(sendMessages: PermValue.Deny, readMessageHistory: PermValue.Deny, viewChannel: PermValue.Deny, sendTTSMessages: PermValue.Deny, manageChannel: PermValue.Deny, manageMessages: PermValue.Deny, manageRoles: PermValue.Deny));
            return supportCategory;
        }
    }
}
