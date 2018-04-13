using Autofac;
using Common;
using Common.Log;
using Lykke.RabbitMqBroker;
using Lykke.RabbitMqBroker.Subscriber;
using Lykke.Service.ICMAdapter.Core;
using Lykke.Service.ICMAdapter.Core.Domain.OrderBooks;
using Lykke.Service.ICMAdapter.Core.Domain.Trading;
using Lykke.Service.ICMAdapter.Core.Handlers;
using Lykke.Service.ICMAdapter.Core.Settings;
using Lykke.Service.ICMAdapter.Core.Throttling;
using Lykke.Service.ICMAdapter.Core.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lykke.Service.ICMAdapter.Services.Exchange
{
    public sealed class ICMTickPriceHarvester : IStartable, IStopable
    {
        private readonly ICMAdapterSettings _config;
        private readonly ICMModelConverter _modelConverter;
        private readonly IHandler<TickPrice> _tickPriceHandler;
        private readonly IHandler<TradingOrderBook> _orderBookHandler;
        private readonly IThrottling _orderBooksThrottler;
        private readonly IThrottling _tickPricesThrottler;
        private readonly ILog _log;
        private readonly RabbitMqSubscriber<ICMOrderBook> _rabbit;
        private readonly HashSet<string> _instruments;

        public ICMTickPriceHarvester(
            ICMAdapterSettings config,
            ICMModelConverter modelConverter,
            IHandler<TickPrice> tickPriceHandler,
            IHandler<TradingOrderBook> orderBookHandler,
            IThrottling orderBooksThrottler,
            IThrottling tickPriceThrottler,
            ILog log)
        {
            _config = config;
            _modelConverter = modelConverter;
            _tickPriceHandler = tickPriceHandler;
            _orderBookHandler = orderBookHandler;

            _orderBooksThrottler = orderBooksThrottler;
            _tickPricesThrottler = tickPriceThrottler;

            _log = log;

            _instruments = config.SupportedCurrencySymbols.Select(x => new Instrument(x.LykkeSymbol).Name).ToHashSet();
            var rabbitSettings = new RabbitMqSubscriptionSettings
            {
                ConnectionString = config.RabbitMq.SourceFeed.ConnectionString,
                ExchangeName = config.RabbitMq.SourceFeed.Exchange,
                QueueName = config.RabbitMq.SourceFeed.Queue
            };
            var errorStrategy = new DefaultErrorHandlingStrategy(_log, rabbitSettings);

            _rabbit = new RabbitMqSubscriber<ICMOrderBook>(rabbitSettings, errorStrategy)
                .SetMessageDeserializer(new GenericRabbitModelConverter<ICMOrderBook>())
                .SetMessageReadStrategy(new MessageReadWithTemporaryQueueStrategy())
                .SetConsole(new LogToConsole())
                .SetLogger(_log)
                .Subscribe(HandleOrderBook);
        }

        private async Task HandleOrderBook(ICMOrderBook orderBook)
        {
            if (_instruments.Contains(orderBook.Asset) || _config.UseSupportedCurrencySymbolsAsFilter == false)
            {
                if (!_tickPricesThrottler.NeedThrottle(orderBook.Asset))
                {
                    var tickPrice = _modelConverter.ToTickPrice(orderBook);
                    if (tickPrice != null)
                    {
                        await _tickPriceHandler.Handle(tickPrice);
                    }
                }

                if (!_orderBooksThrottler.NeedThrottle(orderBook.Asset))
                {
                    await TrySendOrderBook(orderBook);
                }
            }
        }

        private async Task TrySendOrderBook(ICMOrderBook orderBook)
        {
            if ((orderBook.Asks == null || !orderBook.Asks.Any()) && (orderBook.Bids == null || !orderBook.Bids.Any()))
            {
                return;
            }

            var orderBookDto = new TradingOrderBook(
                Constants.ICMExchangeName,
                orderBook.Asset,
                orderBook.Asks?.Select(e => new PriceVolume(e.Price, e.Volume)).ToArray() ?? new PriceVolume[] { },
                orderBook.Bids?.Select(e => new PriceVolume(e.Price, e.Volume)).ToArray() ?? new PriceVolume[] { },
                orderBook.Timestamp);

            await _orderBookHandler.Handle(orderBookDto);
        }

        public void Start()
        {
            if (!_config.RabbitMq.TickPrices.Enabled && !_config.RabbitMq.OrderBooks.Enabled)
            {
                return;
            }

            _rabbit.Start();
            _log.WriteInfoAsync(nameof(ICMTickPriceHarvester), "Initializing", "", "Started");
        }

        public void Dispose()
        {
            _rabbit?.Dispose();
        }

        public void Stop()
        {
            if (!_config.RabbitMq.TickPrices.Enabled && !_config.RabbitMq.OrderBooks.Enabled)
            {
                return;
            }
            _rabbit.Stop();
            _log.WriteInfoAsync(nameof(ICMTickPriceHarvester), "Initializing", "", "Stopped");

        }
    }
}
