using Lykke.Service.ICMAdapter.Core.Settings.SlackNotifications;

namespace Lykke.Service.ICMAdapter.Core.Settings
{
    public class AppSettings
    {
        public ICMAdapterSettings ICMAdapterService { get; set; }
        public SlackNotificationsSettings SlackNotifications { get; set; }
    }
}
