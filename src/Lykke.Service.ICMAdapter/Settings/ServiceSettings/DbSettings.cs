using Lykke.SettingsReader.Attributes;

namespace Lykke.Service.ICMAdapter.Settings.ServiceSettings
{
    public class DbSettings
    {
        [AzureTableCheck]
        public string LogsConnString { get; set; }
    }
}
