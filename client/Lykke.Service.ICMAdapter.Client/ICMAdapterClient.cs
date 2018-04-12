using System;
using Common.Log;

namespace Lykke.Service.ICMAdapter.Client
{
    public class ICMAdapterClient : IICMAdapterClient, IDisposable
    {
        private readonly ILog _log;

        public ICMAdapterClient(string serviceUrl, ILog log)
        {
            _log = log;
        }

        public void Dispose()
        {
            //if (_service == null)
            //    return;
            //_service.Dispose();
            //_service = null;
        }
    }
}
