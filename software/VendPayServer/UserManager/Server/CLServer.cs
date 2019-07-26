using System;
using System.IO;
using System.Net;
using com.IntemsLab.Common;
using com.IntemsLab.Communication.Protocol;
using com.IntemsLab.Common.Model;

namespace UserManager.Server
{
    class CLServer
    {
        private readonly Vending _vending;
        private ProtocolListener _listener;


        public CLServer(FileStorageHelper storage)
        {
            _vending = new Vending(storage);
        }

        private void ListenerOnError(object sender, ErrorEventArgs e)
        {
            Console.WriteLine("Listener error:");
            var exc = e.GetException();
            Console.WriteLine("{0}: {1}", exc.GetType().Name, exc.Message);
        }

        private void ListenerOnRequest(object sender, ProtocolEventArgs e)
        {
            Console.WriteLine("Received command code: {0}", e.Response.CommandCode);
            switch (e.Request.CommandCode)
            {
                case 0x01:
                    e.Response = _vending.GetUserInfo(e.Request);
                    break;

                case 0x02:
                    e.Response = _vending.MakeSale(e.Request);
                    break;

                case 0x03:
                    e.Response = _vending.CancelSale(e.Request);
                    break;

                default:
                    Console.WriteLine("Invalid command received");
                    throw new ProtocolException(ProtocolError.InvalidCommand);
            }
        }

        public void Start()
        {
            _listener = new ProtocolListener(IPAddress.Any, 6767);
            _listener.Error += ListenerOnError;
            _listener.Request += ListenerOnRequest;
            _listener.Start();
        }

        public void Stop()
        {
            _listener.Stop();
            _listener = null;
        }
    }
}
