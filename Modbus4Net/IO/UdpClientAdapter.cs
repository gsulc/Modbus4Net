using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Modbus4Net.IO
{
    public class UdpClientAdapter : IStreamResource
    {
        // strategy for cross platform r/w
        private const int MaxBufferSize = ushort.MaxValue; // this doesn't seem right
        private UdpClient _udpClient;
        private byte[] _buffer = new byte[MaxBufferSize];
        private int _bufferOffset;

        public UdpClientAdapter(string hostname, int port)
        {
            Hostname = hostname;
            Port = port;
            _udpClient = new UdpClient(port);
        }

        public UdpClientAdapter(UdpClient udpClient)
        {
            _udpClient = udpClient ?? throw new ArgumentNullException(nameof(udpClient));
            var endpoint = (IPEndPoint)_udpClient.Client.RemoteEndPoint;
            if (endpoint != null)
            {
                Hostname = endpoint.Address.ToString();
                Port = endpoint.Port;
            }
        }

        public string Hostname { get; private set; }

        public int Port { get; private set; }

        public bool Connected => _udpClient != null && _udpClient.Client.Connected;

        public int InfiniteTimeout => Timeout.Infinite;

        public int ReadTimeout
        {
            get => _udpClient.Client.ReceiveTimeout;
            set => _udpClient.Client.ReceiveTimeout = value;
        }

        public int WriteTimeout
        {
            get => _udpClient.Client.SendTimeout;
            set => _udpClient.Client.SendTimeout = value;
        }

        public void Connect()
        {
            if (_udpClient == null)
                _udpClient = new UdpClient(Port);
            if (!_udpClient.Client.Connected)
                _udpClient.Client.Connect(Hostname, Port);
        }

        public void Disconnect()
        {
#if NET45
                    _udpClient.Close();
#elif NETSTANDARD16
                    _udpClient.Dispose();
#endif
        }

        public void DiscardInBuffer()
        {
            // no-op
        }

        public int Read(byte[] buffer, int offset, int count)
        {
            VerifyArgs(buffer, offset, count);

            if (!Connected)
                Connect();

            if (_bufferOffset == 0)
                _bufferOffset = _udpClient.Client.Receive(_buffer);

            if (_bufferOffset < count)
                throw new IOException("Not enough bytes in the datagram.");

            Buffer.BlockCopy(_buffer, 0, buffer, offset, count);
            _bufferOffset -= count;
            Buffer.BlockCopy(_buffer, count, _buffer, 0, _bufferOffset);

            return count;
        }

        public async Task<int> ReadAsync(byte[] buffer, int offset, int count)
        {
            VerifyArgs(buffer, offset, count);

            if (!Connected)
                Connect();
            
            if (_bufferOffset == 0)
            {
                var result = await _udpClient.ReceiveAsync();
                _buffer = result.Buffer;
                _bufferOffset = result.Buffer.Length;
            }

            if (_bufferOffset < count)
                throw new IOException("Not enough bytes in the datagram.");

            Buffer.BlockCopy(_buffer, 0, buffer, offset, count);
            _bufferOffset -= count;
            Buffer.BlockCopy(_buffer, count, _buffer, 0, _bufferOffset);

            return count;
        }

        public void Write(byte[] buffer, int offset, int count)
        {
            VerifyArgs(buffer, offset, count);

            if (!Connected)
                Connect();

            _udpClient.Client.Send(buffer.Skip(offset).Take(count).ToArray());
        }

        public Task WriteAsync(byte[] buffer, int offset, int count)
        {
            VerifyArgs(buffer, offset, count);

            if (!Connected)
                Connect();

            byte[] bytes = buffer.Skip(offset).Take(count).ToArray();
            return _udpClient.SendAsync(bytes, count, Hostname, Port);
        }

        private void VerifyArgs(byte[] buffer, int offset, int count)
        {
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));

            if (offset < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(offset),
                    "Argument offset must be greater than or equal to 0.");
            }

            if (offset > buffer.Length)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(offset),
                    "Argument offset cannot be greater than the length of buffer.");
            }

            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(count),
                    "Argument count must be greater than or equal to 0.");
            }

            if (count > buffer.Length - offset)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(count),
                    "Argument count cannot be greater than the length of buffer minus offset.");
            }
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
