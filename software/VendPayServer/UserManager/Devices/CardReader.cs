using System;
using System.IO.Ports;
using System.Linq;
using com.IntemsLab.Common.Model;

namespace UserManager.Devices
{
    public class CardReader
    {
        private readonly SerialPort _port;
        private readonly string _portName;

        private bool _isInit;

        public CardReader()
        {
        }

        public CardReader(string portName)
        {
            _portName = portName;
            // try to create port
            string[] ports = SerialPort.GetPortNames();
            if (ports.Contains(portName))
                _port = new SerialPort(portName, 9600, Parity.None, 8, StopBits.One);
        }

        public bool IsInit
        {
            get { return _isInit; }
            set { _isInit = value; }
        }

        public event EventHandler<CardReaderEventArgs> TagReceived;

        public void Init()
        {
            if (_port != null)
            {
                _port.Open();
                _port.NewLine = "\n";
                _port.DataReceived += OnDataRecieved;
                _isInit = true;
            }
            else
            {
                string msg = String.Format("Can't open port: {0}", _portName);
                throw new CardReaderException(msg, CardReaderException.ReaderError.PortOpenError);
            }
        }

        public void StartListening()
        {
            if ((_port != null) && _port.IsOpen)
            {
                _port.DiscardInBuffer();
                _port.DataReceived += OnDataRecieved;
            }
        }

        public void StopListening()
        {
            if ((_port != null) && _port.IsOpen)
            {
                _port.DataReceived -= OnDataRecieved;
                _port.DiscardInBuffer();
            }
        }

        private void OnDataRecieved(object sender, SerialDataReceivedEventArgs e)
        {
            string str = _port.ReadLine().Trim().ToLower();

            string sUid = String.Empty;
            string[] parts = str.Split(',');
            for (int i = 1; i < 5; i++)
            {
                string sByte = Byte.Parse(parts[i]).ToString("X");
                sUid += sByte.Length > 1 ? sByte : ("0" + sByte);
            }

            if (TagReceived != null)
            {
                var args = new CardReaderEventArgs {Card = new ChipCard(sUid.ToLower())};
                TagReceived(this, args);
            }
        }

        //Internal exception class

        //
        public class CardReaderEventArgs : EventArgs
        {
            public ChipCard Card { get; set; }
        }

        private class CardReaderException : Exception
        {
            public CardReaderException(string msg) : base(msg)
            {
                ErrorCode = ReaderError.Unknown;
            }

            public CardReaderException(string msg, ReaderError errorCode) : this(msg)
            {
                ErrorCode = errorCode;
            }

            private ReaderError ErrorCode { get; set; }

            internal enum ReaderError
            {
                PortOpenError,
                Unknown
            }
        }
    }
}