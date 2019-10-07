using Modbus4Net.IO;
using System;
using System.Threading.Tasks;

namespace Modbus4Net
{
    public interface IModbusTransport : IDisposable
    {
        int Retries { get; set; }

        uint RetryOnOldResponseThreshold { get; set; }

        bool SlaveBusyUsesRetryCount { get; set; }

        int WaitToRetryMilliseconds { get; set; }

        int ReadTimeout { get; set; }

        int WriteTimeout { get; set; }

        T UnicastMessage<T>(IModbusMessage message) where T : IModbusMessage, new();

        Task<T> UnicastMessageAsync<T>(IModbusMessage message) where T : IModbusMessage, new();

        byte[] ReadRequest();

        Task<byte[]> ReadRequestAsync();

        byte[] BuildMessageFrame(IModbusMessage message);

        void Write(IModbusMessage message);

        Task WriteAsync(IModbusMessage message);

        IStreamResource StreamResource { get; }
    }
}