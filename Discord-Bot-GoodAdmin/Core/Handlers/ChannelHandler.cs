using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using GoodAdmin_API.Core;
using GoodAdmin_API.Core.Controllers;

namespace GoodAdmin.Core.Handlers
{
    public class ChannelHandler
    {
        internal static Task ReceivedMessage(SocketMessage msg)
        {
            // Verify, if the sender isn't a bot or a web hook.
            if (msg.Author.IsBot) return Task.CompletedTask;
            if (msg.Author.IsWebhook) return Task.CompletedTask;
            SocketUserMessage message = msg as SocketUserMessage;
            if (message == null) return Task.CompletedTask;
            
            foreach (var controller in GlobalInit.controllerHandler.GetControllers())
            {
                if (controller is ChannelController)
                {
                    var con = (ChannelController)controller;
                    if (con.GetInfo().channel.Id == message.Channel.Id)
                    {
                        con.InvokeNewMessage(message);
                        break;
                    }
                }
            }
            return Task.CompletedTask;
        }

        internal static Task RemovedMessage(Cacheable<IMessage, ulong> messages, ISocketMessageChannel channel)
        {
            foreach (var controller in GlobalInit.controllerHandler.GetControllers())
            {
                if (controller.GetType() == typeof(ChannelController))
                {
                    var con = (ChannelController)controller;
                    if (con.GetInfo().channel.Id == channel.Id)
                    {
                        con.InvokeRemovedMessage(messages);
                        break;
                    }
                }
            }
            return Task.CompletedTask;
        }

        internal static Task EditedMessage(Cacheable<IMessage, ulong> messages, SocketMessage msg, ISocketMessageChannel channel)
        {
            // Verify, if the sender isn't a bot or a web hook.
            if (msg.Author.IsBot) return Task.CompletedTask;
            if (msg.Author.IsWebhook) return Task.CompletedTask;
            SocketUserMessage message = msg as SocketUserMessage;
            if (message == null) return Task.CompletedTask;

            foreach (var controller in GlobalInit.controllerHandler.GetControllers())
            {
                if (controller.GetType() == typeof(ChannelController))
                {
                    var con = (ChannelController)controller;
                    if (con.GetInfo().channel.Id == channel.Id)
                    {
                        con.InvokeEditedMessage(message);
                        break;
                    }
                }
            }
            return Task.CompletedTask;
        }

        internal static Task CreatedChannel(SocketChannel channel)
        {
            // Deceprecated
            return Task.CompletedTask;
        }

        internal static Task RemovedChannel(SocketChannel channel)
        {
            foreach (var controller in GlobalInit.controllerHandler.GetControllers())
            {
                if (controller.GetType() == typeof(ChannelController))
                {
                    var con = (ChannelController)controller;
                    if (con.GetInfo().channel.Id == channel.Id)
                    {
                        con.InvokeChannelDeleted();
                        GlobalInit.controllerHandler.RemoveController(controller);
                        break;
                    }
                }
            }
            return Task.CompletedTask;
        }

        internal static Task EditedChannel(SocketChannel oldchannel, SocketChannel newchannel)
        {
            foreach (var controller in GlobalInit.controllerHandler.GetControllers())
            {
                if (controller.GetType() == typeof(ChannelController))
                {
                    var con = (ChannelController)controller;
                    if (con.GetInfo().channel.Id == oldchannel.Id)
                    {
                        if (newchannel.GetType() == typeof(ITextChannel))
                            con.InvokeChannelEdited(newchannel);
                        break;
                    }
                }
            }
            return Task.CompletedTask;
        }
    }
}
