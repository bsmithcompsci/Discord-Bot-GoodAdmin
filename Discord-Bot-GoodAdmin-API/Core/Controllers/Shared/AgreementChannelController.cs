using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using GoodAdmin_API.Core.Chat;

namespace GoodAdmin_API.Core.Controllers.Shared
{
    public class AgreementChannelController : ChannelController
    {
        public AgreementChannelController(IChannel channel, ICategoryChannel parent, IGuild guild) : base(channel, parent, guild)
        {
            this.Initialize += AgreementChannelController_Initialize;
            this.NewMessage += AgreementChannelController_NewMessage;
        }

        private async Task AgreementChannelController_NewMessage(IMessage message)
        {
            Console.WriteLine("Agreement Channel Received : " + message.Content);
            if (message.Content == "agree")
            {
                var members = await GetMemberRole(info.guild);
                if (members != null)
                    await GuildUtils.AssignRole(message.Author, members);
            }

            await message.DeleteAsync();
        }

        private async Task AgreementChannelController_Initialize()
        {
            var embed = new EmbedBuilder()
            {
                Title = ":shield: Terms & Conditions | Rules & Policies :shield:",
                Description = $@"
                    Hello and Welcome to {info.guild.Name}! :smile:

                    Please read any additional information that the community provides to you, to help you better understand what you are agreeing to.
                    By Agreeing via typing `agree` within this chat, will grant you access to the community's channels and membership permissions.

                    Thank you and have a great day!
                ",
                Color = Color.Orange
            };
            await Embeder.SafeEmbedAsync(embed, (ITextChannel)info.channel);
        }

        public static async Task<IRole> GetMemberRole(IGuild guild)
        {
            var config = await Configuration.LoadOrCreateGuildConfig(guild);
            ulong id = 0;

            try
            {
                id = Convert.ToUInt64(config.session["roles-members"]);
                return guild.GetRole(id);
            }
            catch { }

            return null;
        }
    }
}
