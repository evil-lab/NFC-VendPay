using System;

namespace com.IntemsLab.Communication.Protocol
{
    public struct ProtocolError
    {
        private readonly byte _code;
        private readonly string _description;
        private readonly string _name;

        private ProtocolError(string name, byte code, string description)
        {
            this._name = name;
            this._code = code;
            this._description = description;
        }

        public byte Code
        {
            get { return this._code; }
        }

        public string Description
        {
            get { return this._description; }
        }

        public static ProtocolError IncorrectByteCount
        {
            get { return new ProtocolError("INCORRECT_BYTE_COUNT", 0xC5, "Byte count requirement was not met."); }
        }

        public static ProtocolError IncorrectChecksum
        {
            get { return new ProtocolError("INVALID_CRC", 0xFF, "The checksum of a request is not correct"); }
        }

        public static ProtocolError IncorrectStartStopBits
        {
            get
            {
                return new ProtocolError("INCORRECT_START_STOP_BITS", 0xEB,
                    "There was an error with either start or stop bits.");
            }
        }

        public static ProtocolError InvalidCommand
        {
            get { return new ProtocolError("INVALID_COMMAND", 0xFE, "Request contains an unknown command code."); }
        }

        public string Name
        {
            get { return this._name; }
        }

        public static ProtocolError Success
        {
            get { return new ProtocolError("ERROR_OK", 0x00, "No error"); }
        }

        public static ProtocolError Timeout
        {
            get { return new ProtocolError("TIMEOUT", 0xC9, "Time limit exceeded."); }
        }

        public static ProtocolError UnknownError
        {
            get { return new ProtocolError("UNKNOWN_ERROR", 0x9C, "Unexpected error occured."); }
        }

        public override string ToString()
        {
            return String.Format("{0} 0x{1:X2} {2}", this.Name, this.Code, this.Description);
        }
    }
}