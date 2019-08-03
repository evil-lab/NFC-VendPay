using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using NLog;
using com.IntemsLab.Communication;
using com.IntemsLab.Communication.Protocol;

namespace com.IntemsLab.Server.Network
{
    public class ProtocolListener : AsyncTcpListener
    {
        private const int BufferSize = 4096;
        private const int ReadTimeout = 3000;

        private Logger _log;

        public ProtocolListener(IPAddress ipAddress, int port) : base(ipAddress, port)
        {
            _log = LogManager.GetLogger("fileLogger");
        }

        private static ResponseFrame CreateError(byte address, byte frameIndex, byte commandCode, ProtocolError error)
        {
            return new ResponseFrame(address, frameIndex, commandCode, error.Code, null);
        }

        private static ResponseFrame CreateError(ProtocolError error)
        {
            return new ResponseFrame(0, 0xFF, 0xFF, error.Code, null);
        }

        protected ResponseFrame CreateResponse(RequestFrame request)
        {
            var args = new ProtocolEventArgs(request);
            // Notify listeners: DeviceRequestProcessor (executed in current thread)
            this.Request.Fire(this, args);
            return args.Response;
        }

        protected override void OnAccept(TcpClient client)
        {
            _log.Debug("OnAccept() | Message received.");
            using (var stream = client.GetStream())
            {
                //TODO:Засерает консоль!!!
                var response = CreateError(ProtocolError.UnknownError);
                try
                {
                    var request = ReadRequest(stream);
                    try
                    {
                        response = this.CreateResponse(request);
                    }
                    catch (ProtocolException exc)
                    {
                        _log.Error("ProtocolListener.OnAccept() | Response creation error: {0}", exc.Message);
                        _log.Error("ProtocolListener.OnAccept() | Stack trace: {0}\n", exc.StackTrace);
                        response = CreateError(request.Address, request.FrameIndex, request.CommandCode, exc.Error);
                    }
                }
                catch (ProtocolException exc)
                {
                    Console.WriteLine("Read request error occured: {0}", exc.Error);
                    response = CreateError(exc.Error);
                }
                catch (Exception ex)
                {
                    response = CreateError(ProtocolError.UnknownError);
                    Console.WriteLine("Unexpected exception: {0}", ex.Message);
                    Console.WriteLine("Stack trace:");
                    Console.WriteLine(ex.StackTrace);
                }
                finally
                {
                    WriteResponse(stream, response);
                }
            }
        }

        public event EventHandler<ErrorEventArgs> Error;

        protected override void OnError(Exception exc)
        {
            Error.Fire(this, new ErrorEventArgs(exc));
        }

        private static RequestFrame ReadRequest(NetworkStream stream)
        {
            RequestFrame result;
            var frameBuffer = new RequestFrameBuffer();
            var buffer = new byte[BufferSize];
            var countdown = Countdown.StartNew(ReadTimeout);

            while (true)
            {
                int bytesRead;
                if (stream.DataAvailable && (bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    frameBuffer.Write(buffer, 0, bytesRead);
                }

                byte[] requestFrame;
                if (frameBuffer.TryGetFrame(out requestFrame))
                {
                    result = RequestFrame.Parse(requestFrame);
                    break;
                }
                if (countdown.IsOver)
                {
                    throw new ProtocolException(ProtocolError.Timeout);
                }
            }

            return result;
        }

        public event EventHandler<ProtocolEventArgs> Request;

        private static void WriteResponse(NetworkStream stream, ResponseFrame response)
        {
            var responseFrame = response.GetBytes();
            stream.Write(responseFrame, 0, responseFrame.Length);
        }
    }
}