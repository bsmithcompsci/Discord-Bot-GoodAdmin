using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodAdmin_API.Core.Chat
{
    public class Embeder
    {
        public static async Task<bool> SafeEmbedBoolAsync(EmbedBuilder embed, IUser target, ITextChannel channel, string customFooter = "")
        {
            var dm = await target.GetOrCreateDMChannelAsync();

            if (embed.Description != null && embed.Description.Length >= 2000)
                embed.Description = embed.Description.Substring(0, 2000);

            try
            {
                await dm.SendMessageAsync(embed: embed.Build());
                return true;
            }
            catch
            {
                if (channel == null) return false;
                embed.Footer = new EmbedFooterBuilder
                {
                    Text = (customFooter == "" ? "GoodAdmin wasn't able to send " + target.Username + " a private message, due to their privacy policies on Discord!" : customFooter)
                };
                await channel.SendMessageAsync(embed: embed.Build());
                return false;
            }
        }

        public static async Task<IUserMessage> SafeEmbedAsync(EmbedBuilder embed, IUser target, ITextChannel channel, string customFooter = "")
        {
            var dm = await target.GetOrCreateDMChannelAsync();
            if (embed.Description != null && embed.Description.Length >= 2000)
                embed.Description = embed.Description.Substring(0, 2000);

            try
            {
                return await dm.SendMessageAsync(embed: embed.Build());
            }
            catch
            {
                if (channel == null) return null;
                embed.Footer = new EmbedFooterBuilder
                {
                    Text = (customFooter == "" ? "GoodAdmin wasn't able to send " + target.Username + " a private message, due to their privacy policies on Discord!" : customFooter)
                };
                return await channel.SendMessageAsync(embed: embed.Build());
            }
        }

        public static async Task<IUserMessage> SafeEmbedAsync(EmbedBuilder embed, ITextChannel channel, string customFooter = "")
        {
            if (embed.Description != null && embed.Description.Length >= 2000)
                embed.Description = embed.Description.Substring(0, 2000);

            try
            {
                return await channel.SendMessageAsync(embed: embed.Build());
            }
            catch
            { }

            return null;
        }

        public static async Task<IUserMessage> SafeSendMessage(string message, IUser target, ITextChannel channel, string customFooter = "")
        {
            var dm = await target.GetOrCreateDMChannelAsync();
            if (message.Length >= 2000)
                message = message.Substring(0, 2000);

            try
            {
                return await dm.SendMessageAsync(message);
            }
            catch
            {
                if (channel == null) return null;
                EmbedBuilder embed = new EmbedBuilder
                {
                    Description = message,
                    Footer = new EmbedFooterBuilder
                    {
                        Text = (customFooter == "" ? "GoodAdmin wasn't able to send " + target.Username + " a private message, due to their privacy policies on Discord!" : customFooter)
                    }
                };

                return await channel.SendMessageAsync(embed: embed.Build());
            }
        }
    }
}
