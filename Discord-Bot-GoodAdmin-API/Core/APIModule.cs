using System.Threading.Tasks;

namespace GoodAdmin_API
{
    /// <summary>
    /// The Base API for the GoodAdmin Bot, and becomes the target interface for the application.
    /// </summary>
    public interface APIModule
    {
        Task Load(Discord.WebSocket.DiscordSocketClient client);
    }
}
