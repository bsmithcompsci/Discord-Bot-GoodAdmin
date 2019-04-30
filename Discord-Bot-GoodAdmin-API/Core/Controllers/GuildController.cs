using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodAdmin_API.Core.Controllers
{
    public class GuildController : IController
    {
        public IGuild Guild { get; protected set; }
        public GuildConfig Config { get; protected set; }
        
        public delegate Task InitializeDelegate();
        public event InitializeDelegate Initialized;

        public delegate Task ChannelCreatedDelegate(SocketChannel channel);
        public event ChannelCreatedDelegate ChannelCreated;
        public delegate Task ChannelEditedDelegate(SocketChannel oldChannel, SocketChannel newChannel);
        public event ChannelEditedDelegate ChannelEdited;
        public delegate Task ChannelRemovedDelegate(SocketChannel channel);
        public event ChannelRemovedDelegate ChannelRemoved;

        public delegate Task UserJoinedDelegate(SocketGuildUser user);
        public event UserJoinedDelegate UserJoined;
        public delegate Task UserLeftDelegate(SocketGuildUser user);
        public event UserLeftDelegate UserLeft;
        public delegate Task UserBannedDelegate(SocketUser user, SocketGuild guild);
        public event UserBannedDelegate UserBanned;
        public delegate Task UserUnbannedDelegate(SocketUser user, SocketGuild guild);
        public event UserUnbannedDelegate UserUnbanned;

        public delegate Task MessageReceivedDelegate(SocketMessage msg);
        public event MessageReceivedDelegate MessageReceived;
        public delegate Task CommandReceivedDelegate(SocketMessage msg, Discord.Commands.CommandInfo command);
        public event CommandReceivedDelegate CommandReceived;
        public delegate Task MessageEditedDelegate(SocketMessage oldMessage, SocketMessage newMessage, ISocketMessageChannel channel);
        public event MessageEditedDelegate MessageEdited;
        public delegate Task MessageRemovedDelegate(SocketMessage message, ISocketMessageChannel channel);
        public event MessageRemovedDelegate MessageRemoved;

        public delegate Task RoleCreatedDelegate(SocketRole role);
        public event RoleCreatedDelegate RoleCreated;
        public delegate Task RoleEditedDelegate(SocketRole oldRole, SocketRole newRole);
        public event RoleEditedDelegate RoleEdited;
        public delegate Task RoleRemovedDelegate(SocketRole role);
        public event RoleRemovedDelegate RoleRemoved;

        public delegate Task OnTickDelegate();
        public event OnTickDelegate OnTick;

        public GuildController(IGuild guild) => Init(new object[] { guild });
        public void Init(object[] attachment) {
            this.Guild = (IGuild)attachment[0];
            this.Config = Configuration.LoadOrCreateGuildConfig(Guild).GetAwaiter().GetResult();
        }

        // Invoking Events
        public void InvokeTick()            => OnTick?.Invoke();
        public void InvokeInitialize()      => Initialized?.Invoke();

        public void InvokeChannelCreated(SocketChannel channel)                                 => ChannelCreated?.Invoke(channel);
        public void InvokeChannelEdited(SocketChannel oldChannel, SocketChannel newChannel)     => ChannelEdited?.Invoke(oldChannel, newChannel);
        public void InvokeChannelRemoved(SocketChannel channel)                                 => ChannelRemoved?.Invoke(channel);

        public void InvokeUserJoined(SocketGuildUser user)                  => UserJoined?.Invoke(user);
        public void InvokeUserLeft(SocketGuildUser user)                    => UserLeft?.Invoke(user);
        public void InvokeUserBanned(SocketUser user, SocketGuild guild)    => UserBanned?.Invoke(user, guild);
        public void InvokeUserUnbanned(SocketUser user, SocketGuild guild)  => UserUnbanned?.Invoke(user, guild);

        public void InvokeMessageReceived(SocketMessage msg)                                                                    => MessageReceived?.Invoke(msg);
        public void InvokeCommandReceived(SocketMessage msg, Discord.Commands.CommandInfo command)                              => CommandReceived?.Invoke(msg, command);
        public void InvokeMessageEdited(SocketMessage oldMessage, SocketMessage newMessage, ISocketMessageChannel channel)      => MessageEdited?.Invoke(oldMessage, newMessage, channel);
        public void InvokeMessageRemoved(SocketMessage message, ISocketMessageChannel channel)                                  => MessageRemoved?.Invoke(message, channel);

        public void InvokeRoleCreated(SocketRole role)                              => RoleCreated?.Invoke(role);
        public void InvokeRoleEdited(SocketRole oldRole, SocketRole newRole)        => RoleEdited?.Invoke(oldRole, newRole);
        public void InvokeRoleRemoved(SocketRole role)                              => RoleRemoved?.Invoke(role);
    }
}
