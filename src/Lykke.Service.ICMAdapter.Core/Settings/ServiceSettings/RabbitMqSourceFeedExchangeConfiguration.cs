namespace Lykke.Service.ICMAdapter.Core.Settings.ServiceSettings
{
    public class RabbitMqSourceFeedExchangeConfiguration
    {
        public string Exchange { get; set; }

        public string Queue { get; set; }

        public string ConnectionString { get; set; }
    }
}
