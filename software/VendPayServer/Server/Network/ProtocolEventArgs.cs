using System;
using com.IntemsLab.Communication.Protocol;

namespace com.IntemsLab.Server.Network
{
    public sealed class ProtocolEventArgs : EventArgs
    {
        private readonly RequestFrame _request;

        public ProtocolEventArgs(RequestFrame request)
        {
            this._request = request;
        }

        public RequestFrame Request
        {
            get { return this._request; }
        }

        public ResponseFrame Response { get; set; }
    }
}