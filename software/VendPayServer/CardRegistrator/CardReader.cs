using System;
using System.IO.Ports;
using System.Linq;
using UserManager.BL;

namespace UserManager.Devices
{
    public class CardReader
    {
        private readonly string _portName;
        private readonly SerialPort _port;

        private bool _isInit;

        public CardReader()
        {}

        public CardReader(string portName)
        {
            _portName = portName;
            // try to create port
            var ports = SerialPort.GetPortNames();
            if (ports.Contains(portName))
                _port = new SerialPort(portName, 9600, Parity.None, 8, StopBits.One);
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

        public bool IsInit
        {
            get { return _isInit; }
            set { _isInit = value; }
        }

        public void StartListening()
        {
            if ((_port != null)&&_port.IsOpen)
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
            var str = _port.ReadLine().Trim().ToLower();
            if (TagReceived != null)
            {
                var args = new CardReaderEventArgs {Card = new ChipCard(str)};
                TagReceived(this, args);
            }
        }

        //Internal exception class
        class CardReaderException : Exception
        {
            internal enum ReaderError
            {
                PortOpenError,
                Unknown
            }

            public CardReaderException(string msg) : base(msg)
            {
                ErrorCode = ReaderError.Unknown;
            }

            public CardReaderException(string msg, ReaderError errorCode) : this(msg)
            {
                ErrorCode = errorCode;
            }

            private ReaderError ErrorCode
            {
                get; set;
            }
        }

        //
        public class CardReaderEventArgs : EventArgs
        {
            public ChipCard Card
            {
                get; set;
            }
        }
    }
}
