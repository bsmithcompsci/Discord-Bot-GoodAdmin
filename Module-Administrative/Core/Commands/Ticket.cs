using Discord;
using Discord.Commands;
using GoodAdmin_API.Core;
using GoodAdmin_API.Core.Chat;
using GoodAdmin_API.Core.Controllers.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Module_Administrative.Core.Commands
{
    [Group("ticket")]
    public class Ticket : ModuleBase<CommandContext>
    {
        // open #id
        [Command("open"), Summary("Opens a Ticket. `!ticket open <id>`"), RequireUserPermission(GuildPermission.ManageMessages), RequireBotPermission(GuildPermission.ManageMessages), Remarks("Ticket")]
        public async Task OpenAsync(uint id)
        {
            var ticket = SupportChannelController.GetTicketByID(Context.Guild, id);

            if (ticket != null)
            {
                if (ticket.status == TicketStatus.Open)
                {
                    var ticketlogs = await Configuration.GetTicketLoggingChannel(Context.Guild);
                    var embed = new EmbedBuilder()
                    {
                        Title = $@":clipboard: :open_file_folder: Ticket #{id} was opened by:",
                        Description = Context.User.Mention + "\n\n" + ticket.channel.Mention,
                        Color = Color.Green
                    };
                    if (ticketlogs != null)
                        await Embeder.SafeEmbedAsync(embed, (ITextChannel)ticketlogs);
                    else
                        await Embeder.SafeEmbedAsync(embed, (ITextChannel)Context.Channel);

                    if (Context.Channel.Id != ticket.channel.Id)
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

                    ticket.status = TicketStatus.Active;
                    ticket.assigned.Add(Context.User);
                    ticket.assignedHistory.Add(Context.User);

                    await SupportChannelController.SaveTickets(Context.Guild);

                    var ch = await Context.Guild.GetChannelAsync(ticket.channel.Id);
                    await ch.AddPermissionOverwriteAsync(Context.User, new OverwritePermissions(sendMessages: PermValue.Allow, viewChannel: PermValue.Allow, readMessageHistory: PermValue.Allow, attachFiles: PermValue.Allow));
                }
            }
        }

        // delete <#id>
        [Command("delete"), Summary("Deletes a Ticket. `!ticket delete <id>`"), RequireUserPermission(GuildPermission.Administrator), RequireBotPermission(GuildPermission.Administrator), Remarks("Ticket")]
        public async Task DeleteAsync(uint? id = null)
        {
            GuildTicket ticket = null;
            if (id != null)
                ticket = SupportChannelController.GetTicketByID(Context.Guild, id.Value);
            else
                ticket = SupportChannelController.GetChannelTicket(Context.Guild, Context.Channel);

            if (ticket != null)
            {
                if (ticket.status == TicketStatus.Closed)
                {

                    var ticketlogs = await Configuration.GetTicketLoggingChannel(Context.Guild);
                    var embed = new EmbedBuilder()
                    {
                        Title = $@":clipboard: :open_file_folder: Ticket #{ticket.id} was deleted by:",
                        Description = Context.User.Mention,
                        Color = Color.DarkRed
                    };
                    if (ticketlogs != null)
                        await Embeder.SafeEmbedAsync(embed, (ITextChannel)ticketlogs);
                    else
                        await Embeder.SafeEmbedAsync(embed, (ITextChannel)Context.Channel);

                    if (Context.Channel.Id != ticket.channel.Id)
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
                    
                    Console.WriteLine($"Deleted Ticket #{ticket.id} -> " + SupportChannelController.RemoveGuildTicket(Context.Guild, ticket));
                    await SupportChannelController.SaveTickets(Context.Guild);

                    try
                    {
                        await ticket.channel.DeleteAsync();
                    }
                    catch { }
                }
            }
        }

        // close <#id>
        [Command("close"), Summary("Closes a Ticket. `!ticket close <id>`"), RequireUserPermission(GuildPermission.ManageMessages), RequireBotPermission(GuildPermission.ManageMessages), Remarks("Ticket")]
        public async Task CloseAsync(uint? id = null)
        {
            GuildTicket ticket = null;
            if (id != null)
                ticket = SupportChannelController.GetTicketByID(Context.Guild, id.Value);
            else
                ticket = SupportChannelController.GetChannelTicket(Context.Guild, Context.Channel);

            if (ticket != null)
            {

                await Context.Message.DeleteAsync();

                var ticketlogs = await Configuration.GetTicketLoggingChannel(Context.Guild);
                var embed = new EmbedBuilder()
                {
                    Title = $@":clipboard: :open_file_folder: Ticket #{ticket.id} was closed by:",
                    Description = Context.User.Mention,
                    Color = Color.DarkRed
                };
                if (ticketlogs != null)
                    await Embeder.SafeEmbedAsync(embed, (ITextChannel)ticketlogs);
                else
                    await Embeder.SafeEmbedAsync(embed, (ITextChannel)Context.Channel);

                embed = new EmbedBuilder()
                {
                    Title = $@":clipboard: Ticket #{ticket.id} closed by {Context.User.Username}",
                    Description = "Thank you for contacting the support team, if you would like to publish an anonymous survey? Do ```!ticket survey \"Then your message.\"``` The Quotes aren't required.",
                    Footer = new EmbedFooterBuilder()
                    {
                        Text = "This channel will be removed in 5 minute, after the closing of the ticket."
                    },
                    Color = Color.DarkRed
                };
                if (!string.IsNullOrEmpty(embed.Description))
                    await Embeder.SafeEmbedAsync(embed, ticket.channel);

                if (Context.Channel.Id != ticket.channel.Id)
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

                ticket.status = TicketStatus.Closed;

                foreach (var assigned in ticket.assigned)
                {
                    try
                    {
                        Console.WriteLine(assigned.Username + " has been removed from Ticket #" + ticket.id + " - Ticket Closed!");
                        await ticket.channel.RemovePermissionOverwriteAsync(assigned);
                    }
                    catch { }
                }
                ticket.assigned.Clear();
                await SupportChannelController.SaveTickets(Context.Guild);

                Thread t2 = new Thread(async () => {
                    try
                    {
                        await Task.Delay(300000);
                        await ticket.channel.DeleteAsync();
                    } catch { }
                });
                t2.Start();
            }
        }

        // survey #id
        [Command("survey"), Summary("Submits a survey about the ticket."), Remarks("Ticket")]
        public async Task SurveyAsync([Remainder] string msg)
        {
            var ticket = SupportChannelController.GetChannelTicket(Context.Guild, Context.Channel);
            var surveyChannel = await SupportChannelController.GetSupportTicketSurveyLogsChannel(Context.Guild);

            if (ticket != null)
            {
                // Stop Support Team and added individuals from submitting the survey for the creator...
                if (ticket.author.Id != Context.User.Id)
                {
                    var embed = new EmbedBuilder()
                    {
                        Title = $@":clipboard: Ticket #{ticket.id} - Survey",
                        Description = "Sorry, you are not the creator of this ticket, therefore it's the creator's right to create the survey.\n\nThank you, and have a great day. :smile:",
                        Color = Color.Green
                    };
                    if (!string.IsNullOrEmpty(embed.Description))
                        await Embeder.SafeEmbedAsync(embed, ticket.channel);

                    return;
                }

                // Wait until the ticket is closed...
                if (ticket.status == TicketStatus.Closed)
                {
                    var embed = new EmbedBuilder()
                    {
                        Title = $@":clipboard: Ticket #{ticket.id} - Survey Submitted Anonymously",
                        Description = "Thank you for providing our staff with preformance information, we hope you have a great day. :smile:",
                        Color = Color.Green
                    };
                    if (!string.IsNullOrEmpty(embed.Description))
                        await Embeder.SafeEmbedAsync(embed, ticket.channel);

                    var mentionsOfAssigned = "";
                    foreach (var assigned in ticket.assignedHistory)
                        mentionsOfAssigned += assigned.Mention + "\n";

                    embed = new EmbedBuilder()
                    {
                        Title = $@":clipboard: Ticket Survey #{ticket.id}",
                        Description = msg,
                        Fields = new List<EmbedFieldBuilder>()
                        {
                            new EmbedFieldBuilder()
                            {
                                Name = "First Responder",
                                Value = ticket.assignedHistory[0]
                            },
                            new EmbedFieldBuilder()
                            {
                                Name = "Assigned",
                                Value = mentionsOfAssigned
                            }
                        },
                        Color = Color.Green
                    };
                    if (!string.IsNullOrEmpty(embed.Description) && surveyChannel != null)
                        await Embeder.SafeEmbedAsync(embed, (ITextChannel)surveyChannel);

                    await Task.Run(async () =>
                    {
                        await Task.Delay(5000);
                        await ticket.channel.DeleteAsync();
                    });
                }
                else
                {
                    var embed = new EmbedBuilder()
                    {
                        Title = $@":clipboard: Ticket #{ticket.id} - Survey",
                        Description = "Sorry, the support team hasn't closed the ticket with yet. If you have resolved the issue, please formally notify that support team member that you are finished speaking with them.\n\nThank you, and have a great day. :smile:",
                        Color = Color.Green
                    };
                    if (!string.IsNullOrEmpty(embed.Description))
                        await Embeder.SafeEmbedAsync(embed, ticket.channel);
                }
            }
        }

        // assign #id @mention
        [Command("assign"), Summary("Adds a user to a Ticket. `!ticket assign <id> @username`"), RequireUserPermission(GuildPermission.ManageMessages), RequireBotPermission(GuildPermission.ManageMessages), Remarks("Ticket")]
        public async Task AssignAsync(uint id, IGuildUser user)
        {
            var ticket = SupportChannelController.GetTicketByID(Context.Guild, id);

            if (ticket != null)
            {
                if (ticket.assigned.Contains(Context.User))
                {
                    var ticketlogs = await Configuration.GetTicketLoggingChannel(Context.Guild);
                    var embed = new EmbedBuilder()
                    {
                        Title = $@":clipboard: :open_file_folder: Ticket #{id} was assigned by:",
                        Description = Context.User.Mention + "\n\n" + ticket.channel.Mention,
                        Color = Color.Green
                    };
                    embed.AddField(new EmbedFieldBuilder()
                    {
                        Name = "Assigned Person",
                        Value = user.Username
                    });

                    if (ticketlogs != null)
                        await Embeder.SafeEmbedAsync(embed, (ITextChannel)ticketlogs);
                    else
                        await Embeder.SafeEmbedAsync(embed, (ITextChannel)Context.Channel);

                    if (Context.Channel.Id != ticket.channel.Id)
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

                    var ch = await Context.Guild.GetChannelAsync(ticket.channel.Id);
                    await ch.AddPermissionOverwriteAsync(user, new OverwritePermissions(sendMessages: PermValue.Allow, viewChannel: PermValue.Allow, readMessageHistory: PermValue.Allow, attachFiles: PermValue.Allow));

                    Console.WriteLine($"Adding {user.Username} into Ticket #{ticket.id}");
                    ticket.assigned.Add(user);
                    ticket.assignedHistory.Add(user);
                    await SupportChannelController.SaveTickets(Context.Guild);
                }
            }
        }

        // remove #id @mention
        [Command("remove"), Summary("Removes a user from the Ticket. `!ticket remove <id> @username`"), RequireUserPermission(GuildPermission.ManageMessages), RequireBotPermission(GuildPermission.ManageMessages), Remarks("Ticket")]
        public async Task RemoveAsync(uint id, IGuildUser user)
        {
            var ticket = SupportChannelController.GetTicketByID(Context.Guild, id);

            if (ticket != null)
            {
                if (ticket.assigned.Contains(Context.User))
                {
                    var ticketlogs = await Configuration.GetTicketLoggingChannel(Context.Guild);
                    var embed = new EmbedBuilder()
                    {
                        Title = $@":clipboard: :eject:  Ticket #{id} was removed by:",
                        Description = Context.User.Mention + "\n\n" + ticket.channel.Mention,
                        Color = Color.DarkRed
                    };
                    embed.AddField(new EmbedFieldBuilder()
                    {
                        Name = "Removed Person",
                        Value = user.Username
                    });

                    if (ticketlogs != null)
                        await Embeder.SafeEmbedAsync(embed, (ITextChannel)ticketlogs);
                    else
                        await Embeder.SafeEmbedAsync(embed, (ITextChannel)Context.Channel);

                    if (Context.Channel.Id != ticket.channel.Id)
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

                    try
                    {
                        await ticket.channel.RemovePermissionOverwriteAsync(user);
                    }
                    catch { }

                    Console.WriteLine($"Removed {user.Username} into Ticket #{ticket.id}");
                    ticket.assigned.Remove(user);
                    await SupportChannelController.SaveTickets(Context.Guild);
                }
            }
        }

        // list
        [Command("list"), Summary("Lists all active, open, and closed tickets. `!ticket list`"), RequireUserPermission(GuildPermission.ManageMessages), RequireBotPermission(GuildPermission.ManageMessages), Remarks("Ticket")]
        public async Task ListAsync()
        {
            int maxLength = 400;
            var embed = new EmbedBuilder()
            {
                Title = $@":clipboard: Ticket List"
            };
            await Embeder.SafeEmbedAsync(embed, (ITextChannel)Context.Channel);
            foreach (var ticket in SupportChannelController.GetGuildTickets(Context.Guild))
            {
                var assignedMessage = "";

                foreach (var assigned in ticket.assigned)
                {
                    assignedMessage += $"{assigned.Username}\n";
                }

                embed = new EmbedBuilder()
                {
                    Title = $@":clipboard: Ticket #{ticket.id} by {ticket.author.Username} `[{ticket.status.ToString()}]`",
                    Description = $"Created Date: {(ticket.dateTime != null ? ticket.dateTime.ToString() : "UNKNOWN")}\nAuthor : {ticket.author.Mention}\nChannel : {ticket.channel.Mention}\nIncluded:\n{assignedMessage}=================\n" + (string.IsNullOrEmpty(ticket.initialMessage) ? "" : ticket.initialMessage.Length <= maxLength ? ticket.initialMessage : ticket.initialMessage.Substring(0, maxLength) + "...")
                };
                if (!string.IsNullOrEmpty(embed.Description))
                    await Embeder.SafeEmbedAsync(embed, (ITextChannel)Context.Channel);
            }
        }

        // purge
        [Command("purge"), Summary("Purges all tickets. `!ticket purge <[onlyClosed = true]>`"), RequireUserPermission(GuildPermission.Administrator), RequireBotPermission(GuildPermission.Administrator), Remarks("Ticket")]
        public async Task PurgeAsync(bool onlyClosed = true)
        {
            foreach (var ticket in SupportChannelController.GetGuildTickets(Context.Guild))
            {
                var ch = await Context.Guild.GetChannelAsync(ticket.channel.Id);
                if (ch != null)
                {
                    if (onlyClosed)
                    {
                        if (ticket.status == TicketStatus.Closed)
                        {
                            Console.WriteLine($"Deleted Ticket #{ticket.id} -> " + SupportChannelController.RemoveGuildTicket(Context.Guild, ticket));
                            try
                            {
                                await ch.DeleteAsync();
                            } catch { }
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Deleted Ticket #{ticket.id} -> " + SupportChannelController.RemoveGuildTicket(Context.Guild, ticket));
                        try
                        {
                            await ch.DeleteAsync();
                        } catch { }
                    }
                }
            }

            var ticketlogs = await Configuration.GetTicketLoggingChannel(Context.Guild);
            var embed = new EmbedBuilder()
            {
                Title = $@":clipboard: :boom:  Tickets were purged by:",
                Description = Context.User.Mention,
                Color = Color.DarkRed
            };

            if (ticketlogs != null)
                await Embeder.SafeEmbedAsync(embed, (ITextChannel)ticketlogs);
            else
                await Embeder.SafeEmbedAsync(embed, (ITextChannel)Context.Channel);
            
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

            await SupportChannelController.SaveTickets(Context.Guild);
        }
        
        [Command("greeting"), Summary("Displays a professional greeting to the user."), RequireUserPermission(GuildPermission.ManageMessages), RequireBotPermission(GuildPermission.ManageMessages), Remarks("Ticket")]
        public async Task GreetingAsync()
        {
            var ticket = SupportChannelController.GetChannelTicket(Context.Guild, Context.Channel);
            
            await Context.Message.DeleteAsync();

            if (ticket != null)
            {
                var embed = new EmbedBuilder()
                {
                    Title = "Support Team Greeting",
                    Description = $"Hello {ticket.author.Mention}, I am {Context.User.Mention} one of the Support Team Staff for {Context.Guild.Name}. How may I help you today?",
                    Color = Color.Orange
                };
                await Embeder.SafeEmbedAsync(embed, (ITextChannel)Context.Channel);
            }
        }
    }
}
