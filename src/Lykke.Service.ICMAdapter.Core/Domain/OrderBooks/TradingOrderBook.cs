using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Lykke.Service.ICMAdapter.Core.Domain.OrderBooks
{
    public class TradingOrderBook
    {
        public TradingOrderBook(string source, string assetPairId, IReadOnlyCollection<PriceVolume> asks, IReadOnlyCollection<PriceVolume> bids, DateTime timestamp)
        {
            Source = source;
            AssetPairId = assetPairId;
            Asks = asks;
            Bids = bids;
            Timestamp = timestamp;
        }

        [JsonProperty("source")]
        public string Source { get; }

        [JsonProperty("asset")]
        public string AssetPairId { get; }

        [JsonProperty("timestamp")]
        public DateTime Timestamp { get; }

        [JsonProperty("asks")]
        public IReadOnlyCollection<PriceVolume> Asks { get; }

        [JsonProperty("bids")]
        public IReadOnlyCollection<PriceVolume> Bids { get; }
    }
}
