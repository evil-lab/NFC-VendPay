namespace com.IntemsLab.Communication
{
    public static class Crc16
    {
        private const ushort InitialValue = 0xffff;

        public static ushort Compute(byte[] buffer, int offset, int length)
        {
            var crc = InitialValue;

            for (var i = offset; i < offset + length; i++)
            {
                crc = UpdateCrc(buffer[i], crc);
            }

            return (ushort)~crc;
        }

        private static ushort UpdateCrc(byte b, ushort crc)
        {
            unchecked
            {
                var ch = (byte)(b ^ (byte)(crc & 0x00ff));
                ch = (byte)(ch ^ (ch << 4));
                return (ushort)((crc >> 8) ^ (ch << 8) ^ (ch << 3) ^ (ch >> 4));
            }
        }
    }
}