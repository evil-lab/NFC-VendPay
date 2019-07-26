using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace UserManager.Server
{
    public abstract class AsyncTcpListener
    {
        private readonly Thread _acceptorThread;
        private readonly IPAddress _ipAddress;
        private readonly int _port;
        private volatile bool _exitFlag;

        protected AsyncTcpListener(IPAddress ipAddress, int port)
        {
            this._ipAddress = ipAddress;
            this._port = port;

            this._acceptorThread = new Thread(this.AcceptProcedure) { IsBackground = true };
        }

        public IPAddress IPAddress
        {
            get { return this._ipAddress; }
        }

        public int Port
        {
            get { return this._port; }
        }

        private void AcceptHandlerCallback(object state)
        {
            var client = (TcpClient)state;
            this.OnAccept(client);
        }

        private void AcceptProcedure()
        {
            var tcpListener = new TcpListener(this.IPAddress, this.Port);
            try
            {
                tcpListener.Start();

                while (false == this._exitFlag)
                {
                    if (tcpListener.Pending())
                    {
                        try
                        {
                            var client = tcpListener.AcceptTcpClient();
                            this.QueueAcceptHandler(client);
                        }
                        catch (Exception exc)
                        {
                            this.QueueErrorHandler(exc);
                        }
                    }
                    else
                    {
                        Thread.Sleep(100);
                    }
                }
            }
            catch (Exception exc)
            {
                this.QueueErrorHandler(exc);
            }
            finally
            {
                tcpListener.Stop();
            }
        }

        private void ErrorHandlerCallback(object state)
        {
            var exc = (Exception)state;
            this.OnError(exc);
        }

        protected abstract void OnAccept(TcpClient client);
        protected abstract void OnError(Exception exc);

        private void QueueAcceptHandler(TcpClient client)
        {
            ThreadPool.QueueUserWorkItem(this.AcceptHandlerCallback, client);
        }

        private void QueueErrorHandler(Exception exception)
        {
            ThreadPool.QueueUserWorkItem(this.ErrorHandlerCallback, exception);
        }

        public virtual void Start()
        {
            this._acceptorThread.Start();
        }

        public virtual void Stop()
        {
            this._exitFlag = true;
            this._acceptorThread.Join();
        }
    }
}