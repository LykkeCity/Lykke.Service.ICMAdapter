using System;
using System.Collections.Generic;

namespace Lykke.Service.ICMAdapter.Core.Throttling
{
    public class EventsPerSecondPerInstrumentThrottlingManager : IThrottling
    {
        private readonly Dictionary<string, EventsCounter> _eventsPerInstrument = new Dictionary<string, EventsCounter>();
        private readonly int _maxEventPerSecondByInstrument;

        public EventsPerSecondPerInstrumentThrottlingManager(int maxEventPerSecondByInstrument)
        {
            _maxEventPerSecondByInstrument = maxEventPerSecondByInstrument;
        }

        public bool NeedThrottle(string instrument)
        {
            var result = false;

            if (_maxEventPerSecondByInstrument == 0)
            {
                return false;
            }

            if (!_eventsPerInstrument.ContainsKey(instrument))
            {
                _eventsPerInstrument.Add(instrument, new EventsCounter());
            }

            if (_eventsPerInstrument[instrument].NumberOfEventsInLastTimeFrame >= _maxEventPerSecondByInstrument)
            {
                var now = DateTime.UtcNow;
                if ((now - _eventsPerInstrument[instrument].LastEventTimeStamp).TotalMilliseconds >= 1000)
                {
                    _eventsPerInstrument[instrument].NumberOfEventsInLastTimeFrame = 0;
                    _eventsPerInstrument[instrument].LastEventTimeStamp = now;
                }
                else
                {
                    result = true;
                }
            }

            _eventsPerInstrument[instrument].NumberOfEventsInLastTimeFrame++;
            return result;
        }
    }
}
