namespace Lykke.Service.ICMAdapter.Core.Throttling
{
    public interface IThrottling
    {
        bool NeedThrottle(string instrument);
    }
}
