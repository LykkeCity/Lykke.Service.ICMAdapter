using JetBrains.Annotations;
using Lykke.Service.ICMAdapter.Settings.ServiceSettings;
using Lykke.Service.ICMAdapter.Settings.SlackNotifications;

namespace Lykke.Service.ICMAdapter.Settings
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class AppSettings
    {
        public ICMAdapterSettings ICMAdapterService { get; set; }
        public SlackNotificationsSettings SlackNotifications { get; set; }
    }
}
