using JetBrains.Annotations;

namespace Lykke.Service.ICMAdapter.Settings.ServiceSettings
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class ICMAdapterSettings
    {
        public DbSettings Db { get; set; }
    }
}
