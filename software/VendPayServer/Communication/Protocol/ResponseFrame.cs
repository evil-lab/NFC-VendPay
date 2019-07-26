namespace com.IntemsLab.Communication.Protocol
{
    public struct ResponseFrame
    {
        private readonly byte _address;
        private readonly byte _commandCode;
        private readonly byte[] _data;
        private readonly byte _frameIndex;
        private readonly byte _returnCode;

        public ResponseFrame(byte address, byte frameIndex, byte commandCode, byte returnCode, byte[] data) : this()
        {
            _address = address;
            _frameIndex = frameIndex;
            _commandCode = commandCode;
            _returnCode = returnCode;
            _data = data;
        }

        public byte Address
        {
            get { return this._address; }
        }

        public byte CommandCode
        {
            get { return this._commandCode; }
        }

        public byte[] Data
        {
            get { return this._data; }
        }

        public byte FrameIndex
        {
            get { return this._frameIndex; }
        }

        public byte ReturnCode
        {
            get { return this._returnCode; }
        }

        public byte[] GetBytes()
        {
            // { 0xFD, address, frameIndex, commandCode, returnCode, [data], crc16Low, crc16High, 0xFE }

            var dataLength = this.Data == null ? 0 : this.Data.Length;
            var buffer = new byte[dataLength + 8];

            buffer[0] = 0xFD;
            buffer[buffer.Length - 1] = 0xFE;

            buffer[1] = this.Address;
            buffer[2] = this.FrameIndex;
            buffer[3] = this.CommandCode;
            buffer[4] = this.ReturnCode;

            if (this.Data != null)
            {
                Bytes.Copy(this.Data, 0, buffer, 5, dataLength);
            }

            var crc16 = Crc16.Compute(buffer, 1, buffer.Length - 4);
            var crc16Low = (byte)(crc16 & 0xFF);
            var crc16High = (byte)((crc16 >> 8) & 0xFF);

            buffer[buffer.Length - 3] = crc16Low;
            buffer[buffer.Length - 2] = crc16High;

            return ByteStuffing.Apply(buffer, 1, buffer.Length - 2);
        }

        public static ResponseFrame Parse(byte[] buffer)
        {
            if (buffer.Length < 8)
            {
                throw new ProtocolException(ProtocolError.IncorrectByteCount);
            }

            buffer = ByteStuffing.Revert(buffer, 1, buffer.Length - 2);

            if (buffer[0] != 0xFD)
            {
                throw new ProtocolException(ProtocolError.IncorrectStartStopBits);
            }

            if (buffer[buffer.Length - 1] != 0xFE)
            {
                throw new ProtocolException(ProtocolError.IncorrectStartStopBits);
            }

            var crc16Low = buffer[buffer.Length - 3];
            var crc16High = buffer[buffer.Length - 2];
            var crc16 = (crc16High << 8) | crc16Low;
            var actualCrc16 = Crc16.Compute(buffer, 1, buffer.Length - 4);

            if (crc16 != actualCrc16)
            {
                throw new ProtocolException(ProtocolError.IncorrectChecksum);
            }

            // { 0xFD, address, frameIndex, commandCode, returnCode, [data], crc16Low, crc16High, 0xFE }
            var address = buffer[1];
            var frameIndex = buffer[2];
            var commandCode = buffer[3];
            var returnCode = buffer[4];
            var data = Bytes.Read(buffer, 5, buffer.Length - 8);
            return new ResponseFrame(address, frameIndex, commandCode, returnCode, data);
        }
    }
}