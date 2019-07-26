using System;
using System.IO;

namespace com.IntemsLab.Communication.Protocol
{
    internal static class ByteStuffing
    {
        public static byte[] Apply(byte[] buffer, int offset, int length)
        {
            using (var memoryStream = new MemoryStream())
            {
                memoryStream.Write(buffer, 0, offset);

                for (var i = 0; i < length; i++)
                {
                    switch (buffer[offset + i])
                    {
                        case 0xFF:
                            memoryStream.WriteByte(0xFF);
                            memoryStream.WriteByte(0x00);
                            break;

                        case 0xFE:
                            memoryStream.WriteByte(0xFF);
                            memoryStream.WriteByte(0x01);
                            break;

                        case 0xFD:
                            memoryStream.WriteByte(0xFF);
                            memoryStream.WriteByte(0x02);
                            break;

                        default:
                            memoryStream.WriteByte(buffer[offset + i]);
                            break;
                    }
                }

                memoryStream.Write(buffer, offset + length, buffer.Length - offset - length);
                return memoryStream.ToArray();
            }
        }

        public static byte[] Revert(byte[] buffer, int offset, int length)
        {
            using (var memoryStream = new MemoryStream())
            {
                memoryStream.Write(buffer, 0, offset);

                var pair = false;
                for (var i = 0; i < length; i++)
                {
                    if (pair)
                    {
                        pair = false;
                        switch (buffer[offset + i])
                        {
                            case 0x00:
                                memoryStream.WriteByte(0xFF);
                                break;

                            case 0x01:
                                memoryStream.WriteByte(0xFE);
                                break;

                            case 0x02:
                                memoryStream.WriteByte(0xFD);
                                break;

                            default:
                                throw new FormatException(String.Format("Invalid byte pair '0xFF 0x{0:X2}'.",
                                    buffer[offset + i]));
                        }
                        continue;
                    }

                    if (buffer[offset + i] == 0xFF)
                    {
                        pair = true;
                        continue;
                    }

                    memoryStream.WriteByte(buffer[offset + i]);
                }

                if (pair)
                {
                    throw new FormatException("Incomplete byte pair.");
                }

                memoryStream.Write(buffer, offset + length, buffer.Length - offset - length);
                return memoryStream.ToArray();
            }
        }
    }
}