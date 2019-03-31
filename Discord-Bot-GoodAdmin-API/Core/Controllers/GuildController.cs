using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodAdmin_API.Core.Controllers
{
    public struct GuildControllerStruct
    {
        public IGuild guild;
    }

    public class GuildController
    {
        public delegate Task Initialize();

        public delegate Task CreateChannel();
        public delegate Task EditChannel();
        public delegate Task RemovedChannel();

        public delegate Task NewMessage();
        public delegate Task NewCommand();
        public delegate Task EditedMessage();
        public delegate Task RemovedMessage();

        public delegate Task AddRole();
        public delegate Task RemovedRole();
        public delegate Task ChangedRole();

        protected static GuildControllerStruct info;

        public void Init(object attachment)
        {
            info = new GuildControllerStruct()
            {
                guild = (IGuild)attachment
            };
        }

        public GuildControllerStruct GetInfo()
        {
            return info;
        }
    }
}
