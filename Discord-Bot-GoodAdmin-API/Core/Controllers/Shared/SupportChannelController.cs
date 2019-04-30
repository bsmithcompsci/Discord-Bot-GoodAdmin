using Discord;
using GoodAdmin_API.Core;
using GoodAdmin_API.Core.Chat;
using GoodAdmin_API.Core.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodAdmin_API.Core.Controllers.Shared
{
    public enum TicketStatus
    {
        Open, // No-one has opened it.
        Active, // Someone is assigned to it.
        Closed // The ticket has been resolved...
    }
    public class GuildTicket
    {
        public uint id;
        public string initialMessage;
        public ITextChannel channel;
        public IUser author;
        public TicketStatus status;
        public List<IUser> assigned;
        public List<IUser> assignedHistory;
        public DateTime dateTime;
    }

    public struct TicketSimpleStruct
    {
        public uint id;
        public string initialMessage;
        public ulong channelUID;
        public ulong authorUID;
        public uint statusUID;
        public List<ulong> assignedUIDs;
        public List<ulong> assignedHistoryUIDs;
        public long dateTime;
    }

    public class SupportChannelController : ChannelController
    {
        public static Dictionary<IGuild, List<GuildTicket>> activeTickets = new Dictionary<IGuild, List<GuildTicket>>();
        protected IRole supportTeam;

        public SupportChannelController(IChannel channel, ICategoryChannel parent, IGuild guild, IRole supportTeam) : base(channel, parent, guild)
        {
            this.Initialize += SupportChannelController_Initialize;
            this.NewMessage += SupportChannelController_NewMessage;
            this.supportTeam = supportTeam;
        }

        private async Task SupportChannelController_Initialize()
        {
            if (supportTeam != null)
            {
                var embed = new EmbedBuilder()
                {
                    Title = ":shield: Support Channel :shield:",
                    Description = $@"
                    Hello and welcome to the support channel, the {supportTeam.Mention} will be gladly willing to help you, when your ticket is published.
                    
                    To publish a ticket, simply type within this channel, and your message will be forwarded into a private text channel within this category
                    for you and the staff assigned to discuss the issue. Keep in mind, this method is more private and secured by our system.

                    Thank you and have a great day!
                ",
                    Color = Color.Orange
                };
                await Embeder.SafeEmbedAsync(embed, (ITextChannel)info.channel);
            }
        }

        private async Task SupportChannelController_NewMessage(IMessage message)
        {
            Console.WriteLine("Support Channel :: " + message.Content);
            
            await message.DeleteAsync();

            if (info.parent != null)
            {
                if (CanCreateNewTicket(info.guild, message.Author))
                {
                    var id = GetNextGuildTicketID(info.guild);
                    var ticketChannel = await info.guild.CreateTextChannelAsync("Ticket #" + id, x => { x.CategoryId = info.parent.Id; });
                    await ticketChannel.AddPermissionOverwriteAsync(message.Author, new OverwritePermissions(sendMessages: PermValue.Allow, viewChannel: PermValue.Allow, readMessageHistory: PermValue.Allow, attachFiles: PermValue.Allow));
                    var embed = new EmbedBuilder()
                    {
                        Title = $"Ticket #" + id,
                        Description = message.Content + "\n\n" + message.Author.Mention,
                        Color = Color.Blue
                    };
                    await Embeder.SafeEmbedAsync(embed, ticketChannel);
                    
                    AddGuildTicket(info.guild, new GuildTicket()
                    {
                        id = id,
                        initialMessage = message.Content,
                        author = message.Author,
                        channel = ticketChannel,
                        status = TicketStatus.Open,
                        assigned = new List<IUser>(),
                        assignedHistory = new List<IUser>(),
                        dateTime = DateTime.Now
                    });
                    
                    var config = await Configuration.LoadOrCreateGuildConfig(info.guild);
                    if (config != null)
                    {
                        config.tickets.Add(new TicketSimpleStruct()
                        {
                            id = id,
                            initialMessage = message.Content,
                            authorUID = message.Author.Id,
                            channelUID = ticketChannel.Id,
                            statusUID = (uint)TicketStatus.Open.GetHashCode(),
                            assignedUIDs = new List<ulong>(),
                            assignedHistoryUIDs = new List<ulong>(),
                            dateTime = DateTime.Now.ToBinary()
                        });
                        await Configuration.SaveGuildConfig(info.guild, config);
                        Console.WriteLine("Databased Ticket #" + id);
                    }
                    Console.WriteLine("Added Ticket #" + id);
                    
                    var ticketlogs = await Configuration.GetTicketLoggingChannel(info.guild);
                    var ticketEmbed = new EmbedBuilder()
                    {
                        Title = $@":clipboard: :open_file_folder: Ticket #{id} was created by:",
                        Description = message.Author.Mention + "\n=========================\n" + message.Content,
                        Color = Color.Blue
                    };
                    if (ticketlogs != null)
                        await Embeder.SafeEmbedAsync(ticketEmbed, (ITextChannel)ticketlogs);
                }
                else
                {
                    Console.WriteLine($"{message.Author.Username} has too many tickets!");
                }
            }
        }

        public static async Task<IRole> GetSupportTeam(IGuild guild)
        {
            var config = await Configuration.LoadOrCreateGuildConfig(guild);
            ulong id = 0;
            
            try
            {
                id = Convert.ToUInt64(config.session["roles-support-team"]);
                return guild.GetRole(id);
            }
            catch { }

            return null;
        }

        public static async Task<IGuildChannel> GetSupportChatChannel(IGuild guild)
        {
            var config = await Configuration.LoadOrCreateGuildConfig(guild);

            if (!config.session.ContainsKey("support-chat"))
                return null;

            return await guild.GetChannelAsync(Convert.ToUInt64(config.session["support-chat"]));
        }

        public static async Task<IGuildChannel> GetSupportTicketChannel(IGuild guild)
        {
            var config = await Configuration.LoadOrCreateGuildConfig(guild);

            if (!config.session.ContainsKey("support-channel"))
                return null;

            return await guild.GetChannelAsync(Convert.ToUInt64(config.session["support-channel"]));
        }

        public static async Task<IGuildChannel> GetSupportTicketLogsChannel(IGuild guild)
        {
            var config = await Configuration.LoadOrCreateGuildConfig(guild);

            if (!config.session.ContainsKey("ticket-logging-channel"))
                return null;

            return await guild.GetChannelAsync(Convert.ToUInt64(config.session["ticket-logging-channel"]));
        }

        public static async Task<IGuildChannel> GetSupportTicketSurveyLogsChannel(IGuild guild)
        {
            var config = await Configuration.LoadOrCreateGuildConfig(guild);

            if (!config.session.ContainsKey("ticket-survey-logging-channel"))
                return null;

            return await guild.GetChannelAsync(Convert.ToUInt64(config.session["ticket-survey-logging-channel"]));
        }

        public static async Task SaveTickets(IGuild guild)
        {
            var config = await Configuration.LoadOrCreateGuildConfig(guild);
            if (config == null) return;
            config.tickets.Clear();
            foreach (var ticket in GetGuildTickets(guild))
            {
                var assignedUIDs = new List<ulong>();
                foreach (var assigned in ticket.assigned)
                    assignedUIDs.Add(assigned.Id);

                config.tickets.Add(new TicketSimpleStruct()
                {
                    id = ticket.id,
                    assignedUIDs = assignedUIDs,
                    authorUID = ticket.author.Id,
                    channelUID = ticket.channel.Id,
                    dateTime = ticket.dateTime.ToBinary(),
                    initialMessage = ticket.initialMessage,
                    statusUID = (uint)ticket.status.GetHashCode()
                });
            }

            await config.Save(guild);
        }

        public static bool CanCreateNewTicket(IGuild guild, IUser user)
        {
            int count = 0;
            foreach (var ticket in GetUserActiveTickets(guild, user))
            {
                if (ticket.status != TicketStatus.Closed)
                    count++;
            }
            return (count < 2);
        }

        public static uint GetNextGuildTicketID(IGuild guild)
        {
            uint result = 0;
            foreach(var ticket in GetGuildTickets(guild))
            {
                if (ticket.id > result)
                    result = ticket.id;
            }
            return result+1;
        }

        public static void AddGuildTicket(IGuild guild, GuildTicket ticket)
        {
            if (!activeTickets.ContainsKey(guild))
                activeTickets.Add(guild, new List<GuildTicket>());
            
            activeTickets[guild].Add(ticket);
        }

        public static bool RemoveGuildTicket(IGuild guild, GuildTicket ticket)
        {
            if (!activeTickets.ContainsKey(guild))
                activeTickets.Add(guild, new List<GuildTicket>());

            return activeTickets[guild].Remove(ticket);
        }

        public static List<GuildTicket> GetGuildTickets(IGuild guild)
        {
            List<GuildTicket> listedTickets = new List<GuildTicket>();
            foreach (var row in activeTickets)
                if (row.Key.Id == guild.Id)
                    listedTickets.AddRange(row.Value.OrderBy(ticket => ticket.dateTime));
            return listedTickets;
        }

        public static List<GuildTicket> GetUserActiveTickets(IGuild guild, IUser user)
        {
            List<GuildTicket> listedTickets = new List<GuildTicket>();
            foreach (var row in activeTickets)
                if (row.Key.Id == guild.Id)
                    foreach (var col in row.Value.OrderBy(col => col.dateTime))
                        if (col.author.Id == user.Id)
                            listedTickets.Add(col);
            return listedTickets;
        }

        public static GuildTicket GetChannelTicket(IGuild guild, IChannel ch)
        {
            foreach (var row in activeTickets)
                if (row.Key.Id == guild.Id)
                    foreach (var col in row.Value)
                        if (col.channel.Id == ch.Id)
                            return col;
            return null;
        }

        public static GuildTicket GetTicketByID(IGuild guild, uint id)
        {
            foreach (var row in activeTickets)
                if (row.Key.Id == guild.Id)
                    foreach (var col in row.Value)
                        if (col.id == id)
                            return col;
            return null;
        }
    }
}
