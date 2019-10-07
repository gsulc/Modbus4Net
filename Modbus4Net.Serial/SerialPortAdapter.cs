using Modbus4Net.IO;
using System;
using System.Diagnostics;
using System.IO.Ports;
using System.Threading.Tasks;

namespace Modbus4Net.Serial
{
    public class SerialPortAdapter : IStreamResource
    {
        private const string NewLine = "\r\n";
        private SerialPort _serialPort;

        public SerialPortAdapter(SerialPort serialPort)
        {
            Debug.Assert(serialPort != null, "Argument serialPort cannot be null.");

            _serialPort = serialPort;
            _serialPort.NewLine = NewLine;
        }

        public bool Connected => _serialPort.IsOpen;

        public int InfiniteTimeout => SerialPort.InfiniteTimeout;

        public int ReadTimeout
        {
            get => _serialPort.ReadTimeout;
            set => _serialPort.ReadTimeout = value;
        }

        public int WriteTimeout
        {
            get => _serialPort.WriteTimeout;
            set => _serialPort.WriteTimeout = value;
        }

        public void Connect()
        {
            _serialPort.Open();
        }

        public void Disconnect()
        {
            _serialPort.Close();
        }

        public void DiscardInBuffer()
        {
            _serialPort.DiscardInBuffer();
        }

        public int Read(byte[] buffer, int offset, int count)
        {
            try
            {
                return _serialPort.Read(buffer, offset, count);
            }
            catch
            {
                if (!Connected)
                {
                    Connect();
                    return _serialPort.Read(buffer, offset, count);
                }
                throw;
            }
        }

        public Task<int> ReadAsync(byte[] buffer, int offset, int count)
        {
            try
            {
                return _serialPort.BaseStream.ReadAsync(buffer, offset, count);
            }
            catch
            {
                if (!Connected)
                {
                    Connect();
                    return _serialPort.BaseStream.ReadAsync(buffer, offset, count);
                }
                throw;
            }
        }

        public void Write(byte[] buffer, int offset, int count)
        {
            try
            {
                _serialPort.Write(buffer, offset, count);
            }
            catch
            {
                if (!Connected)
                {
                    Connect();
                    _serialPort.Write(buffer, offset, count);
                }
                throw;
            }
        }

        public Task WriteAsync(byte[] buffer, int offset, int count)
        {
            try
            {
                return _serialPort.BaseStream.WriteAsync(buffer, offset, count);
            }
            catch
            {
                if (!Connected)
                {
                    Connect();
                    return _serialPort.BaseStream.WriteAsync(buffer, offset, count);
                }
                throw;
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
                _serialPort?.Dispose();
                _serialPort = null;
            }
        }
    }
}
