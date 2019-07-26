namespace com.IntemsLab.Communication.Protocol
{
    public struct RequestFrame
    {
        private readonly byte _address;
        private readonly byte _commandCode;
        private readonly byte[] _data;
        private readonly byte _frameIndex;

        public RequestFrame(byte address, byte frameIndex, byte commandCode, byte[] data) : this()
        {
            this._address = address;
            this._frameIndex = frameIndex;
            this._commandCode = commandCode;
            this._data = data;
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

        public byte[] GetBytes()
        {
            // { 0xFD, address, frameIndex, commandCode, [data], crc16Low, crc16High, 0xFE }

            var dataLength = this.Data == null ? 0 : this.Data.Length;
            var buffer = new byte[dataLength + 7];

            buffer[0] = 0xFD;
            buffer[buffer.Length - 1] = 0xFE;

            buffer[1] = this.Address;
            buffer[2] = this.FrameIndex;
            buffer[3] = this.CommandCode;

            if (this.Data != null)
            {
                Bytes.Copy(this.Data, 0, buffer, 4, dataLength);
            }

            var crc16 = Crc16.Compute(buffer, 1, buffer.Length - 4);
            var crc16Low = (byte)(crc16 & 0xFF);
            var crc16High = (byte)((crc16 >> 8) & 0xFF);

            buffer[buffer.Length - 3] = crc16Low;
            buffer[buffer.Length - 2] = crc16High;

            return ByteStuffing.Apply(buffer, 1, buffer.Length - 2);
        }

        public static RequestFrame Parse(byte[] buffer)
        {
            if (buffer.Length < 7)
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

            // { 0xFD, address, frameIndex, commandCode, [data], crc16Low, crc16High, 0xFE }
            var address = buffer[1];
            var frameIndex = buffer[2];
            var commandCode = buffer[3];
            var data = Bytes.Read(buffer, 4, buffer.Length - 7);
            return new RequestFrame(address, frameIndex, commandCode, data);
        }
    }
}