using System.Threading.Tasks;

namespace Lykke.Service.ICMAdapter.Core.Services
{
    public interface IShutdownManager
    {
        Task StopAsync();
    }
}
