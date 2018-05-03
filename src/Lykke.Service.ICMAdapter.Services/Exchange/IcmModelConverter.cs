using Lykke.Service.ICMAdapter.Core.Domain.OrderBooks;
using Lykke.Service.ICMAdapter.Core.Domain.Trading;
using Lykke.Service.ICMAdapter.Core.Settings;
using System.Linq;

namespace Lykke.Service.ICMAdapter.Services.Exchange
{
    public sealed class ICMModelConverter
    {
        private readonly ICMAdapterSettings _configuration;

        public ICMModelConverter(ICMAdapterSettings configuration)
        {
            _configuration = configuration;
        }

        public TickPrice ToTickPrice(ICMOrderBook orderBook)
        {
            if (orderBook.Asks != null && orderBook.Asks.Any() && orderBook.Bids != null && orderBook.Bids.Any())
            {
                return new TickPrice(new Instrument(orderBook.Asset),
                    orderBook.Timestamp,
                    orderBook.Asks.Select(x => x.Price).Min(),
                    orderBook.Bids.Select(x => x.Price).Max()
                );
            }

            return null;
        }
    }
}
