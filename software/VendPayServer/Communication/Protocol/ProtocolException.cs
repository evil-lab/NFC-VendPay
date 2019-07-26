using System;

namespace com.IntemsLab.Communication.Protocol
{
    [Serializable]
    public class ProtocolException : Exception
    {
        private readonly ProtocolError _error;

        public ProtocolException(ProtocolError error, string message) : base(message)
        {
            this._error = error;
        }

        public ProtocolException(ProtocolError error) : base(error.ToString())
        {
            this._error = error;
        }

        public ProtocolError Error
        {
            get { return this._error; }
        }
    }
}