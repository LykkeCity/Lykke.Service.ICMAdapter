using Autofac;
using Autofac.Extensions.DependencyInjection;
using Common;
using Common.Log;
using Lykke.Service.ICMAdapter.Core.Domain.OrderBooks;
using Lykke.Service.ICMAdapter.Core.Domain.Trading;
using Lykke.Service.ICMAdapter.Core.Filters;
using Lykke.Service.ICMAdapter.Core.Handlers;
using Lykke.Service.ICMAdapter.Core.Services;
using Lykke.Service.ICMAdapter.Core.Settings;
using Lykke.Service.ICMAdapter.Core.Settings.ServiceSettings;
using Lykke.Service.ICMAdapter.Core.Throttling;
using Lykke.Service.ICMAdapter.Services;
using Lykke.Service.ICMAdapter.Services.Exchange;
using Lykke.SettingsReader;
using Microsoft.Extensions.DependencyInjection;

namespace Lykke.Service.ICMAdapter.Modules
{
    public class ServiceModule : Module
    {
        private readonly IReloadingManager<ICMAdapterSettings> _settings;
        private readonly ILog _log;
        // NOTE: you can remove it if you don't need to use IServiceCollection extensions to register service specific dependencies
        private readonly IServiceCollection _services;

        public ServiceModule(IReloadingManager<ICMAdapterSettings> settings, ILog log)
        {
            _settings = settings;
            _log = log;

            _services = new ServiceCollection();
        }

        protected override void Load(ContainerBuilder builder)
        {
            // TODO: Do not register entire settings in container, pass necessary settings to services which requires them
            // ex:
            //  builder.RegisterType<QuotesPublisher>()
            //      .As<IQuotesPublisher>()
            //      .WithParameter(TypedParameter.From(_settings.CurrentValue.QuotesPublication))

            builder.RegisterInstance(_log)
                .As<ILog>()
                .SingleInstance();

            builder.RegisterType<HealthService>()
                .As<IHealthService>()
                .SingleInstance();

            builder.RegisterType<StartupManager>()
                .As<IStartupManager>();

            builder.RegisterType<ShutdownManager>()
                .As<IShutdownManager>();

            builder.RegisterGeneric(typeof(RabbitMqHandler<>));

            builder.RegisterInstance(_settings.CurrentValue);

            //builder.RegisterType<ICMExchange>().As<ExchangeBase>().SingleInstance();

            RegisterRabbitMqHandler<TickPrice>(builder, _settings.CurrentValue.RabbitMq.TickPrices, "tickHandler");
            RegisterRabbitMqHandler<TradingOrderBook>(builder, _settings.CurrentValue.RabbitMq.OrderBooks, "orderBookHandler");

            builder.RegisterType<TickPriceHandlerDecorator>()
                .WithParameter((info, context) => info.Name == "rabbitMqHandler",
                    (info, context) => context.ResolveNamed<IHandler<TickPrice>>("tickHandler"))
                .SingleInstance()
                .As<IHandler<TickPrice>>();

            builder.RegisterType<EventsPerSecondPerInstrumentThrottlingManager>()
                .WithParameter("maxEventPerSecondByInstrument", _settings.CurrentValue.MaxEventPerSecondByInstrument)
                .As<IThrottling>().InstancePerDependency();

            builder.RegisterType<ICMTickPriceHarvester>()
                .As<IStartable>()
                .As<IStopable>()
                .AsSelf()
                .SingleInstance();

            builder.RegisterType<ICMModelConverter>()
                .SingleInstance();

            builder.RegisterType<RepeatingTicksFilter>().SingleInstance();

            builder.Populate(_services);
        }

        private static void RegisterRabbitMqHandler<T>(ContainerBuilder container, RabbitMqPublishToExchangeConfiguration exchangeConfiguration, string regKey = "")
        {
            container.RegisterType<RabbitMqHandler<T>>()
                .WithParameter("connectionString", exchangeConfiguration.ConnectionString)
                .WithParameter("exchangeName", exchangeConfiguration.PublishToExchange)
                .WithParameter("enabled", exchangeConfiguration.Enabled)
                .Named<IHandler<T>>(regKey)
                .As<IHandler<T>>();
        }
    }
}
