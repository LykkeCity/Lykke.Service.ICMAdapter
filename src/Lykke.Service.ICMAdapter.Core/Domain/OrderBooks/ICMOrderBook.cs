using System;

namespace Lykke.Service.ICMAdapter.Core.Domain.OrderBooks
{
    public class ICMOrderBook
    {
        public string Source { get; set; }

        public string Asset { get; set; }

        public DateTime Timestamp { get; set; }

        public PriceVolume[] Asks { get; set; }

        public PriceVolume[] Bids { get; set; }
    }
}
