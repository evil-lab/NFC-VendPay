using System.Collections.Generic;
using System.Linq;

namespace com.IntemsLab.Communication.Protocol
{
    public sealed class RequestFrameBuffer
    {
        private const int BufferSize = 4096;
        private const byte FrameStartMarker = 0xFD;
        private const byte FrameStopMarker = 0xFE;
        private readonly byte[] _buffer = new byte[BufferSize];

        private readonly Queue<byte[]> _frameQueue = new Queue<byte[]>();
        private int _pointer;

        private void EnqueueCommand()
        {
            var frame = new byte[this._pointer];

            for (var i = 0; i < this._pointer; i++)
            {
                frame[i] = this._buffer[i];
            }

            this._frameQueue.Enqueue(frame);
            this._pointer = 0;
        }

        public bool TryGetFrame(out byte[] frame)
        {
            if (this._frameQueue.Any())
            {
                frame = this._frameQueue.Dequeue();
                return true;
            }

            frame = null;
            return false;
        }

        public void Write(byte[] buffer, int offset, int count)
        {
            for (var i = offset; i < offset + count; ++i)
            {
                this.WriteByte(buffer[i]);
            }
        }

        private void WriteByte(byte b)
        {
            if (this._pointer == 0)
            {
                if (b == FrameStartMarker)
                {
                    this.WriteByteWithoutChecks(b);
                }
            }
            else
            {
                this.WriteByteWithoutChecks(b);

                if (b == FrameStopMarker)
                {
                    this.EnqueueCommand();
                }
            }
        }

        private void WriteByteWithoutChecks(byte b)
        {
            this._buffer[this._pointer++] = b;
        }
    }
}