using System.Threading.Tasks;

namespace Lykke.Service.ICMAdapter.Core.Handlers
{
    public interface IHandler<in T>
    {
        Task Handle(T message);
    }
}
