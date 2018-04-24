using Lykke.Service.ICMAdapter.Core.Domain.Trading;
using System.Collections.Generic;

namespace Lykke.Service.ICMAdapter.Core.Filters
{
    public class RepeatingTicksFilter
    {
        private Dictionary<string, TickPrice> _latestTickPricePerInstrument;

        public RepeatingTicksFilter()
        {
            _latestTickPricePerInstrument = new Dictionary<string, TickPrice>();
        }

        public bool IsTheSameAsLatestTickPrice(TickPrice currnetTickPrice)
        {
            if (!_latestTickPricePerInstrument.ContainsKey(currnetTickPrice.Asset))
            {
                _latestTickPricePerInstrument.Add(currnetTickPrice.Asset, currnetTickPrice);
                return false;
            }

            var latestTickPriceForInstrument = _latestTickPricePerInstrument[currnetTickPrice.Asset];

            if (latestTickPriceForInstrument.Ask == currnetTickPrice.Ask && latestTickPriceForInstrument.Bid == currnetTickPrice.Bid)
            {
                _latestTickPricePerInstrument[currnetTickPrice.Asset].Timestamp = currnetTickPrice.Timestamp; //update the timestamp only, just to keep track of when tick was recived, but dont consider it when comparing ticks as we only care for ask and bid values. 
                return true;
            }

            _latestTickPricePerInstrument[currnetTickPrice.Asset] = currnetTickPrice;
            return false;
        }
    }
}
