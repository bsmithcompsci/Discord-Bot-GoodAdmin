using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodAdmin_API.Core.Controllers
{
    public struct ChannelControllerStruct
    {
        public IGuild guild;
        public ICategoryChannel parent;
        public IChannel channel;
    }

    public class ChannelController : IController
    {
        public delegate Task InitializeDelegate();
        public event InitializeDelegate Initialize;

        public delegate Task ChannelDeletedDelegate();
        public event ChannelDeletedDelegate ChannelDeleted;

        public delegate Task ChannelEditedDelegate(IChannel newChannel);
        public event ChannelEditedDelegate ChannelEdited;

        public delegate Task NewMessageDelegate(IMessage message);
        public event NewMessageDelegate NewMessage;

        public delegate Task RemovedMessageDelegate(Cacheable<IMessage, ulong> messages);
        public event RemovedMessageDelegate RemovedMessage;

        public delegate Task EditedMessageDelegate(IMessage message);
        public event EditedMessageDelegate EditedMessage;

        protected static ChannelControllerStruct info;

        public ChannelController(IChannel channel, ICategoryChannel parent, IGuild guild) => Init(new object[] { channel, parent, guild });
        public void Init(object[] attachment)
        {
            if (attachment.Length != 3) return;
            if (attachment[0] != null || attachment[1] != null || attachment[2] != null) return;
            Console.WriteLine("Loading Channel Controller | {0} | {1} | {2}", attachment[0], attachment[1], attachment[2]);
            info = new ChannelControllerStruct()
            {
                channel = (IChannel)attachment[0],
                parent = (ICategoryChannel)attachment[1],
                guild = (IGuild)attachment[2]
            };
        }

        public void InvokeInitialize() => this.Initialize?.Invoke();
        public void InvokeChannelDeleted() => this.ChannelDeleted?.Invoke();
        public void InvokeChannelEdited(IChannel channel)
        {
            this.ChannelEdited?.Invoke(channel);
            info.channel = channel;
        }
        public void InvokeNewMessage(IMessage message) => this.NewMessage?.Invoke(message);
        public void InvokeRemovedMessage(Cacheable<IMessage, ulong> messages) => this.RemovedMessage?.Invoke(messages);
        public void InvokeEditedMessage(IMessage message) => this.EditedMessage?.Invoke(message);

        public ChannelControllerStruct GetInfo()
        {
            return info;
        }
    }
}
