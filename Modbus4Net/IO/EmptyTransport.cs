using Modbus4Net.Logging;
using System;
using System.Threading.Tasks;

namespace Modbus4Net.IO
{
    internal class EmptyTransport : ModbusTransport
    {
        public EmptyTransport(IModbusFactory modbusFactory)
            : base(modbusFactory, NullModbusLogger.Instance)
        {
        }

        public override byte[] ReadRequest()
        {
            throw new NotImplementedException();
        }

        public override IModbusMessage ReadResponse<T>()
        {
            throw new NotImplementedException();
        }

        public override byte[] BuildMessageFrame(IModbusMessage message)
        {
            throw new NotImplementedException();
        }

        public override void Write(IModbusMessage message)
        {
            throw new NotImplementedException();
        }

        public override void OnValidateResponse(IModbusMessage request, IModbusMessage response)
        {
            throw new NotImplementedException();
        }

        public override Task<byte[]> ReadRequestAsync()
        {
            throw new NotImplementedException();
        }

        public override Task<IModbusMessage> ReadResponseAsync<T>()
        {
            throw new NotImplementedException();
        }

        public override Task WriteAsync(IModbusMessage message)
        {
            throw new NotImplementedException();
        }
    }
}
