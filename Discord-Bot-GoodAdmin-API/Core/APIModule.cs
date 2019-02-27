using GoodAdmin_API.Core.Handlers;
using System.Threading.Tasks;

namespace GoodAdmin_API
{
    public interface APIModule
    {
        Task Load();
        CommandHandler LoadCommandHandler();
    }
}
