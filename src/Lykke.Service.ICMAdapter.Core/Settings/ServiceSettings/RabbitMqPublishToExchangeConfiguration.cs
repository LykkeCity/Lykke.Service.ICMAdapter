namespace Lykke.Service.ICMAdapter.Core.Settings.ServiceSettings
{
    public class RabbitMqPublishToExchangeConfiguration
    {
        public bool Enabled { get; set; }

        public string PublishToExchange { get; set; }

        public string ConnectionString { get; set; }
    }
}
