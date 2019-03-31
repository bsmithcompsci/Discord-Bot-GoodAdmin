using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading.Tasks;

namespace GoodAdmin_API.Core.Attributes
{
    public class CommandTiedControllerAttribute : Attribute
    {
        IChannel Channel { get; set; }
        public CommandTiedControllerAttribute(IChannel channel)
        {
            this.Channel = channel;
        }
    }
}
