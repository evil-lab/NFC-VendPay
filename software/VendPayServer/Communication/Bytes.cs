using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace com.IntemsLab.Communication
{
    public sealed class Bytes : IEnumerable<byte>
    {
        private const int BitsInByte = 8;

        private readonly byte[] _buffer;
        private Encoding _encoding = Encoding.Default;
        private Endianness _endianness = Endianness.Little;

        public Bytes(byte[] buffer, int initialPosition)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }

            this._buffer = buffer;
            this.Position = initialPosition;
        }

        public Bytes(params byte[] buffer) : this(buffer, 0)
        {
        }

        public Bytes(int length) : this(new byte[length], 0)
        {
        }

        public byte[] Buffer
        {
            get { return this._buffer; }
        }

        public Encoding Encoding
        {
            get { return this._encoding; }
            set { this._encoding = value; }
        }

        public bool EndOfBuffer
        {
            get { return this.Position >= this.Length - 1; }
        }

        public Endianness Endianness
        {
            get { return this._endianness; }
            set { this._endianness = value; }
        }

        public byte this[int index]
        {
            get { return this.Buffer[index]; }
            set { this.Buffer[index] = value; }
        }

        public int Length
        {
            get { return this._buffer.Length; }
        }

        public int LowerBound
        {
            get { return this.Buffer.GetLowerBound(0); }
        }

        public int Position { get; private set; }

        public int UpperBound
        {
            get { return this.Buffer.GetUpperBound(0); }
        }

        public IEnumerator<byte> GetEnumerator()
        {
            return ((IEnumerable<byte>)this.Buffer).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public bool AreEqual(params byte[] bytes)
        {
            return this.CompareWith(bytes) == 0;
        }

        public static bool AreEqual(byte[] bufferA, byte[] bufferB)
        {
            if (bufferA.Length != bufferB.Length)
            {
                return false;
            }

            return Compare(bufferA, 0, bufferB, 0, bufferA.Length) == 0;
        }

        public static void Clear(byte[] buffer, int offset, int count)
        {
            ValidateBufferOffsetCount(buffer, offset, count);

            for (var i = 0; i < count; i++)
            {
                buffer[offset + i] = default(byte);
            }
        }

        public void Clear(int offset, int count)
        {
            Clear(this.Buffer, offset, count);
        }

        public void Clear()
        {
            Clear(this.Buffer, 0, this.Buffer.Length);
            this.Position = this.LowerBound;
        }

        public static int Compare(byte[] bufferA, int offsetA, byte[] bufferB, int offsetB, int count)
        {
            ValidateBufferOffsetCount(bufferA, offsetA, count);
            ValidateBufferOffsetCount(bufferB, offsetB, count);

            return CompareUnchecked(bufferA, offsetA, bufferB, offsetB, count);
        }

        private static int CompareUnchecked(byte[] bufferA, int offsetA, byte[] bufferB, int offsetB, int count)
        {
            for (var i = 0; i < count; i++)
            {
                var diff = bufferA[offsetA + i] - bufferB[offsetB + i];
                if (diff != 0)
                {
                    return diff;
                }
            }

            return 0;
        }

        public int CompareWith(byte[] buffer, int offset, int count)
        {
            if (this.Length != count)
            {
                return this.Length - count;
            }

            return Compare(this.Buffer, 0, buffer, offset, count);
        }

        public int CompareWith(byte[] buffer)
        {
            if (this.Length != buffer.Length)
            {
                return this.Length - buffer.Length;
            }

            return Compare(this.Buffer, 0, buffer, 0, buffer.Length);
        }

        public static void Copy(byte[] source, int sourceOffset, byte[] dest, int destOffset, int count)
        {
            ValidateBufferOffsetCount(source, sourceOffset, count);
            ValidateBufferOffsetCount(dest, destOffset, count);

            CopyUnchecked(source, sourceOffset, dest, destOffset, count);
        }

        private static void CopyUnchecked(byte[] source, int sourceOffset, byte[] dest, int destOffset, int count)
        {
            for (var i = 0; i < count; i++)
            {
                dest[destOffset + i] = source[sourceOffset + i];
            }
        }

        public byte[] GetRemainingBytes()
        {
            return this.Read(this.Length - this.Position);
        }

        public static byte[] Read(byte[] buffer, int offset, int count)
        {
            ValidateBufferOffsetCount(buffer, offset, count);

            var result = new byte[count];
            CopyUnchecked(buffer, offset, result, 0, count);
            return result;
        }

        public byte[] Read(int offset, int count)
        {
            return Read(this.Buffer, offset, count);
        }

        public byte[] Read(int count)
        {
            var result = this.Read(this.Position, count);
            this.Position += count;
            return result;
        }

        public byte ReadByte()
        {
            var oldPosition = this.Position;
            this.Position++;
            return this.Buffer[oldPosition];
        }

        public static Int16 ReadInt16(byte[] buffer, int offset, int count, Endianness endianness)
        {
            ValidateTypeSize(sizeof(Int16), count, true);
            ValidateBufferOffsetCount(buffer, offset, count);

            return (Int16)ReadIntegerUnchecked(buffer, offset, count, endianness);
        }

        public Int16 ReadInt16(int offset, int count)
        {
            return ReadInt16(this.Buffer, offset, count, this.Endianness);
        }

        public Int16 ReadInt16(int count)
        {
            var result = this.ReadInt16(this.Position, count);
            this.Position += count;
            return result;
        }

        public Int16 ReadInt16()
        {
            return this.ReadInt16(sizeof(Int16));
        }

        public static Int32 ReadInt32(byte[] buffer, int offset, int count, Endianness endianness)
        {
            ValidateTypeSize(sizeof(Int32), count, true);
            ValidateBufferOffsetCount(buffer, offset, count);

            return (Int32)ReadIntegerUnchecked(buffer, offset, count, endianness);
        }

        public Int32 ReadInt32(int offset, int count)
        {
            return ReadInt32(this.Buffer, offset, count, this.Endianness);
        }

        public Int32 ReadInt32(int count)
        {
            var result = this.ReadInt32(this.Position, count);
            this.Position += count;
            return result;
        }

        public Int32 ReadInt32()
        {
            return this.ReadInt32(sizeof(Int32));
        }

        public static Int64 ReadInt64(byte[] buffer, int offset, int count, Endianness endianness)
        {
            ValidateTypeSize(sizeof(Int64), count, true);
            ValidateBufferOffsetCount(buffer, offset, count);

            return ReadIntegerUnchecked(buffer, offset, count, endianness);
        }

        public Int64 ReadInt64(int offset, int count)
        {
            return ReadInt64(this.Buffer, offset, count, this.Endianness);
        }

        public Int64 ReadInt64(int count)
        {
            var result = this.ReadInt64(this.Position, count);
            this.Position += count;
            return result;
        }

        public Int64 ReadInt64()
        {
            return this.ReadInt64(sizeof(Int64));
        }

        private static Int64 ReadIntegerUnchecked(byte[] buffer, int offset, int count, Endianness endianness)
        {
            Int64 result = 0;

            if (endianness == Endianness.Big)
            {
                for (var i = 0; i < count; i++)
                {
                    result <<= BitsInByte;
                    result |= buffer[offset + i];
                }
            }
            else
            {
                for (var i = 0; i < count; i++)
                {
                    result <<= BitsInByte;
                    result |= buffer[offset + (count - 1) - i];
                }
            }

            return result;
        }

        public static string ReadString(byte[] buffer, int offset, int count, Encoding encoding)
        {
            if (encoding == null)
            {
                throw new ArgumentNullException("encoding");
            }

            ValidateBufferOffsetCount(buffer, offset, count);
            return encoding.GetString(buffer, offset, count);
        }

        public string ReadString(int offset, int count)
        {
            return ReadString(this.Buffer, offset, count, this.Encoding);
        }

        public string ReadString(int count)
        {
            var result = this.ReadString(this.Position, count);
            this.Position += count;
            return result;
        }

        public string ReadNullTerminatedString()
        {
            var nullIndex = -1;
            for (var i = this.Position; i < this.Length; i++)
            {
                if (this.Buffer[i] == 0x00)
                {
                    nullIndex = i;
                    break;
                }
            }

            if (nullIndex == -1)
            {
                throw new InvalidDataException("No NULL termination till the end of the string.");
            }

            var count = nullIndex - this.Position;
            return this.ReadString(count);
        }

        public static UInt16 ReadUInt16(byte[] buffer, int offset, int count, Endianness endianness)
        {
            ValidateTypeSize(sizeof(UInt16), count, true);
            ValidateBufferOffsetCount(buffer, offset, count);

            return (UInt16)ReadUIntegerUnchecked(buffer, offset, count, endianness);
        }

        public UInt16 ReadUInt16(int offset, int count)
        {
            return ReadUInt16(this.Buffer, offset, count, this.Endianness);
        }

        public UInt16 ReadUInt16(int count)
        {
            var result = this.ReadUInt16(this.Position, count);
            this.Position += count;
            return result;
        }

        public UInt16 ReadUInt16()
        {
            return this.ReadUInt16(sizeof(UInt16));
        }

        public static UInt32 ReadUInt32(byte[] buffer, int offset, int count, Endianness endianness)
        {
            ValidateTypeSize(sizeof(UInt32), count, true);
            ValidateBufferOffsetCount(buffer, offset, count);

            return (UInt32)ReadUIntegerUnchecked(buffer, offset, count, endianness);
        }

        public UInt32 ReadUInt32(int offset, int count)
        {
            return ReadUInt32(this.Buffer, offset, count, this.Endianness);
        }

        public UInt32 ReadUInt32(int count)
        {
            var result = this.ReadUInt32(this.Position, count);
            this.Position += count;
            return result;
        }

        public UInt32 ReadUInt32()
        {
            return this.ReadUInt32(sizeof(UInt32));
        }

        public static UInt64 ReadUInt64(byte[] buffer, int offset, int count, Endianness endianness)
        {
            ValidateTypeSize(sizeof(UInt64), count, true);
            ValidateBufferOffsetCount(buffer, offset, count);

            return ReadUIntegerUnchecked(buffer, offset, count, endianness);
        }

        public UInt64 ReadUInt64(int offset, int count)
        {
            return ReadUInt64(this.Buffer, offset, count, this.Endianness);
        }

        public UInt64 ReadUInt64(int count)
        {
            var result = this.ReadUInt64(this.Position, count);
            this.Position += count;
            return result;
        }

        public UInt64 ReadUInt64()
        {
            return this.ReadUInt64(sizeof(UInt64));
        }

        private static UInt64 ReadUIntegerUnchecked(byte[] buffer, int offset, int count, Endianness endianness)
        {
            UInt64 result = 0;

            if (endianness == Endianness.Big)
            {
                for (var i = 0; i < count; i++)
                {
                    result <<= BitsInByte;
                    result |= buffer[offset + i];
                }
            }
            else
            {
                for (var i = 0; i < count; i++)
                {
                    result <<= BitsInByte;
                    result |= buffer[offset + (count - 1) - i];
                }
            }

            return result;
        }

        public void SetPosition(int value)
        {
            if (value < this.LowerBound)
            {
                throw new ArgumentOutOfRangeException("value", "Value cannot be less than lower bound.");
            }

            if (value >= this.UpperBound)
            {
                throw new ArgumentOutOfRangeException("value", "Value cannot be more than upper bound.");
            }

            this.Position = value;
        }

        public static string ToString(byte[] buffer)
        {
            return ToString(buffer, 0, buffer.Length);
        }

        public static string ToString(byte[] buffer, int offset, int count)
        {
            ValidateBufferOffsetCount(buffer, offset, count);
            var sb = new StringBuilder();

            for (var i = 0; i < count; i++)
            {
                sb.AppendFormat("{0:X2}", buffer[offset + i]);
            }

            return sb.ToString();
        }

        public override string ToString()
        {
            return ToString(this.Buffer, 0, this.Buffer.Length);
        }

        private static void ValidateBufferOffsetCount(byte[] buffer, int offset, int count)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer", "Buffer cannot be null.");
            }

            if (offset < 0)
            {
                throw new ArgumentOutOfRangeException("offset",
                    "Offset must be greater or equal to the buffer's lower bound.");
            }

            if (offset > buffer.Length)
            {
                throw new ArgumentOutOfRangeException("offset", "Offset must be less or equal to buffer's upper bound.");
            }

            if (count < 0)
            {
                throw new ArgumentOutOfRangeException("count", "Count must be greater or equal to 0.");
            }

            if (offset + count - 1 >= buffer.Length)
            {
                throw new ArgumentOutOfRangeException("count",
                    "Offset + count - 1 must be less or equal to buffer's upper bound.");
            }
        }

        private static void ValidateTypeSize(int typeSize, int requestedSize, bool allowPartialRead)
        {
            if (requestedSize > typeSize)
            {
                throw new ArgumentOutOfRangeException("requestedSize",
                    "Requested size cannot be greater than actual type size.");
            }

            if (requestedSize < typeSize && false == allowPartialRead)
            {
                throw new ArgumentOutOfRangeException("requestedSize",
                    "Requested size cannot be less than actual type size.");
            }
        }

        public static void Write(byte[] buffer, int offset, params byte[] data)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer", "Data cannot be null.");
            }

            ValidateBufferOffsetCount(buffer, offset, data.Length);
            CopyUnchecked(data, 0, buffer, offset, data.Length);
        }

        public void Write(int offset, params byte[] data)
        {
            Write(this.Buffer, offset, data);
        }

        public void Write(params byte[] data)
        {
            this.Write(this.Position, data);
            this.Position += data.Length;
        }

        public void WriteByte(byte value)
        {
            var oldPosition = this.Position;
            this.Position++;
            this.Buffer[oldPosition] = value;
        }

        public static void WriteInt16(Int16 value, byte[] buffer, int offset, int count, Endianness endianness)
        {
            ValidateTypeSize(sizeof(Int16), count, true);
            ValidateBufferOffsetCount(buffer, offset, count);

            WriteIntegerUnchecked(value, buffer, offset, count, endianness);
        }

        public void WriteInt16(Int16 value, int offset, int count)
        {
            WriteInt16(value, this.Buffer, offset, count, this.Endianness);
        }

        public void WriteInt16(Int16 value, int count)
        {
            this.WriteInt16(value, this.Position, count);
            this.Position += count;
        }

        public void WriteInt16(Int16 value)
        {
            this.WriteInt16(value, sizeof(Int16));
        }

        public static void WriteInt32(int value, byte[] buffer, int offset, int count, Endianness endianness)
        {
            ValidateTypeSize(sizeof(Int32), count, true);
            ValidateBufferOffsetCount(buffer, offset, count);

            WriteIntegerUnchecked(value, buffer, offset, count, endianness);
        }

        public void WriteInt32(int value, int offset, int count)
        {
            WriteInt32(value, this.Buffer, offset, count, this.Endianness);
        }

        public void WriteInt32(int value, int count)
        {
            this.WriteInt32(value, this.Position, count);
            this.Position += count;
        }

        public void WriteInt32(Int32 value)
        {
            this.WriteInt32(value, sizeof(Int32));
        }

        public static void WriteInt64(long value, byte[] buffer, int offset, int count, Endianness endianness)
        {
            ValidateTypeSize(sizeof(Int64), count, true);
            ValidateBufferOffsetCount(buffer, offset, count);

            WriteIntegerUnchecked(value, buffer, offset, count, endianness);
        }

        public void WriteInt64(long value, int offset, int count)
        {
            WriteInt64(value, this.Buffer, offset, count, this.Endianness);
        }

        public void WriteInt64(long value, int count)
        {
            this.WriteInt64(value, this.Position, count);
            this.Position += count;
        }

        public void WriteInt64(Int64 value)
        {
            this.WriteInt64(value, sizeof(Int64));
        }

        private static void WriteIntegerUnchecked(Int64 value, byte[] buffer, int offset, int count,
            Endianness endianness)
        {
            if (endianness == Endianness.Little)
            {
                for (var i = 0; i < count; i++)
                {
                    buffer[offset + i] = (byte)(value & byte.MaxValue);
                    value >>= BitsInByte;
                }
            }
            else
            {
                for (var i = 0; i < count; i++)
                {
                    buffer[offset + (count - 1) - i] = (byte)(value & byte.MaxValue);
                    value >>= BitsInByte;
                }
            }
        }

        public static int WriteString(string value, byte[] buffer, int offset, int count, Encoding encoding)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            if (encoding == null)
            {
                throw new ArgumentNullException("encoding");
            }

            ValidateBufferOffsetCount(buffer, offset, count);
            var stringBytes = encoding.GetBytes(value);
            var actualCount = Math.Min(count, stringBytes.Length);

            CopyUnchecked(stringBytes, 0, buffer, offset, actualCount);
            return actualCount;
        }

        public int WriteString(string value, int offset, int count)
        {
            return WriteString(value, this.Buffer, offset, count, this.Encoding);
        }

        public void WriteString(string value, int count)
        {
            var delta = this.WriteString(value, this.Position, count);
            this.Position += delta;
        }

        public void WriteString(string value)
        {
            var delta = this.WriteString(value, this.Position, this.Encoding.GetByteCount(value));
            this.Position += delta;
        }

        public static void WriteUInt16(UInt16 value, byte[] buffer, int offset, int count, Endianness endianness)
        {
            ValidateTypeSize(sizeof(UInt16), count, true);
            ValidateBufferOffsetCount(buffer, offset, count);

            WriteUIntegerUnchecked(value, buffer, offset, count, endianness);
        }

        public void WriteUInt16(UInt16 value, int offset, int count)
        {
            WriteUInt16(value, this.Buffer, offset, count, this.Endianness);
        }

        public void WriteUInt16(UInt16 value, int count)
        {
            this.WriteUInt16(value, this.Position, count);
            this.Position += count;
        }

        public void WriteUInt16(UInt16 value)
        {
            this.WriteUInt16(value, sizeof(UInt16));
        }

        public static void WriteUInt32(UInt32 value, byte[] buffer, int offset, int count, Endianness endianness)
        {
            ValidateTypeSize(sizeof(UInt32), count, true);
            ValidateBufferOffsetCount(buffer, offset, count);

            WriteUIntegerUnchecked(value, buffer, offset, count, endianness);
        }

        public void WriteUInt32(UInt32 value, int offset, int count)
        {
            WriteUInt32(value, this.Buffer, offset, count, this.Endianness);
        }

        public void WriteUInt32(UInt32 value, int count)
        {
            this.WriteUInt32(value, this.Position, count);
            this.Position += count;
        }

        public void WriteUInt32(UInt32 value)
        {
            this.WriteUInt32(value, sizeof(UInt32));
        }

        public static void WriteUInt64(UInt64 value, byte[] buffer, int offset, int count, Endianness endianness)
        {
            ValidateTypeSize(sizeof(UInt64), count, true);
            ValidateBufferOffsetCount(buffer, offset, count);

            WriteUIntegerUnchecked(value, buffer, offset, count, endianness);
        }

        public void WriteUInt64(UInt64 value, int offset, int count)
        {
            WriteUInt64(value, this.Buffer, offset, count, this.Endianness);
        }

        public void WriteUInt64(UInt64 value, int count)
        {
            this.WriteUInt64(value, this.Position, count);
            this.Position += count;
        }

        public void WriteUInt64(UInt64 value)
        {
            this.WriteUInt64(value, sizeof(UInt64));
        }

        private static void WriteUIntegerUnchecked(UInt64 value, byte[] buffer, int offset, int count,
            Endianness endianness)
        {
            if (endianness == Endianness.Little)
            {
                for (var i = 0; i < count; i++)
                {
                    buffer[offset + i] = (byte)(value & byte.MaxValue);
                    value >>= BitsInByte;
                }
            }
            else
            {
                for (var i = 0; i < count; i++)
                {
                    buffer[offset + (count - 1) - i] = (byte)(value & byte.MaxValue);
                    value >>= BitsInByte;
                }
            }
        }

        // ReSharper restore UnusedParameter.Local

        public static byte Xor(byte[] buffer, int offset, int count)
        {
            ValidateBufferOffsetCount(buffer, offset, count);
            byte result = 0;

            for (var i = 0; i < count; i++)
            {
                result ^= buffer[offset + i];
            }

            return result;
        }

        public static implicit operator byte[](Bytes @this)
        {
            return @this.Buffer;
        }
    }
}