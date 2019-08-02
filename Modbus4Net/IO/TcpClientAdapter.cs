using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Modbus4Net.IO
{
    public class TcpClientAdapter : IStreamResource
    {
        private TcpClient _tcpClient;

        public TcpClientAdapter(string hostname, int port)
        {
            Debug.Assert(!string.IsNullOrEmpty(hostname), "Argument hostname cannot be null or empty.");

            Hostname = hostname;
            Port = port;
        }

        internal TcpClientAdapter(TcpClient tcpClient)
        {
            Debug.Assert(tcpClient != null, "Argument tcpClient cannot be null.");
            _tcpClient = tcpClient;
            var endpoint = (IPEndPoint)tcpClient.Client.RemoteEndPoint;
            if (endpoint != null)
            {
                Hostname = endpoint.Address.ToString();
                Port = endpoint.Port;
            }
        }

        public string Hostname { get; private set; }

        public int Port { get; private set; }

        public bool Connected => _tcpClient != null && _tcpClient.Connected;

        public int InfiniteTimeout => Timeout.Infinite;

        public int ReadTimeout
        {
            get => _tcpClient.GetStream().ReadTimeout;
            set => _tcpClient.GetStream().ReadTimeout = value;
        }

        public int WriteTimeout
        {
            get => _tcpClient.GetStream().WriteTimeout;
            set => _tcpClient.GetStream().WriteTimeout = value;
        }

        public void Connect()
        {
            if (_tcpClient == null || !_tcpClient.Connected)
            {
                if (_tcpClient != null)
                    Disconnect();
                    
                _tcpClient = new TcpClient();
                _tcpClient.Client.Connect(Hostname, Port);
            }
        }

        public void Disconnect()
        {
#if NET45
                    _tcpClient.Close();
#elif NETSTANDARD16
                    _tcpClient.Dispose();
#endif
        }

        public void Write(byte[] buffer, int offset, int size)
        {
            GetStream().Write(buffer, offset, size);
        }

        public int Read(byte[] buffer, int offset, int size)
        {
            return GetStream().Read(buffer, offset, size);
        }

        public void DiscardInBuffer()
        {
            GetStream().Flush();
        }

        private NetworkStream GetStream()
        {
            try
            {
                return _tcpClient.GetStream();
            }
            catch (NullReferenceException) // the client hasn't been created yet
            {
                Connect();
            }
            catch (ObjectDisposedException) // the client has been closed
            {
                Connect();
            }
            catch (InvalidOperationException) // not connected to a remote host
            {
                Connect();
            }
            return _tcpClient.GetStream();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Disconnect();
            }
        }
    }
}
