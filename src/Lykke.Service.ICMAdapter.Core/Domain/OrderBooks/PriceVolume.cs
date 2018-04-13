using Newtonsoft.Json;
namespace Lykke.Service.ICMAdapter.Core.Domain.OrderBooks
{
    public sealed class PriceVolume
    {
        public PriceVolume(decimal price, decimal volume)
        {
            Price = price;
            Volume = System.Math.Abs(volume);
        }
        [JsonProperty("price")]
        public decimal Price { get; set; }
        [JsonProperty("volume")]
        public decimal Volume { get; set; }
    }
}
