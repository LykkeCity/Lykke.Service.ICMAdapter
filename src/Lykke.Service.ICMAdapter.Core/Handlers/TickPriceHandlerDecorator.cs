using Lykke.Service.ICMAdapter.Core.Domain.Trading;
using System.Threading.Tasks;

namespace Lykke.Service.ICMAdapter.Core.Handlers
{
    public class TickPriceHandlerDecorator : IHandler<TickPrice>
    {
        private readonly IHandler<TickPrice> _rabbitMqHandler;

        public TickPriceHandlerDecorator(IHandler<TickPrice> rabbitMqHandler)
        {
            _rabbitMqHandler = rabbitMqHandler;
        }

        public async Task Handle(TickPrice message)
        {
            await _rabbitMqHandler.Handle(message);
        }
    }
}
