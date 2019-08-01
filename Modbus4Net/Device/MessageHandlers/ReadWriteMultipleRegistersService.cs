using Modbus4Net.Data;
using Modbus4Net.Message;
using System;
using System.Linq;

namespace Modbus4Net.Device.MessageHandlers
{
    internal class ReadWriteMultipleRegistersService : ModbusFunctionServiceBase<ReadWriteMultipleRegistersRequest>
    {
        public ReadWriteMultipleRegistersService()
            : base(ModbusFunctionCodes.ReadWriteMultipleRegisters)
        {
        }

        public override IModbusMessage CreateRequest(byte[] frame)
        {
            return CreateModbusMessage<ReadWriteMultipleRegistersRequest>(frame);
        }

        public override int GetRtuRequestBytesToRead(byte[] frameStart)
        {
            throw new NotSupportedException();
        }

        public override int GetRtuResponseBytesToRead(byte[] frameStart)
        {
            throw new NotSupportedException();
        }

        protected override IModbusMessage Handle(ReadWriteMultipleRegistersRequest request, ISlaveDataStore dataStore)
        {
            ushort[] pointsToWrite = request.WriteRequest.Data
                .ToArray();

            dataStore.HoldingRegisters.WritePoints(request.ReadRequest.StartAddress, pointsToWrite);

            ushort[] readPoints = dataStore.HoldingRegisters.ReadPoints(request.ReadRequest.StartAddress,
                request.ReadRequest.NumberOfPoints);

            var data = new RegisterCollection(readPoints);

            return new ReadHoldingInputRegistersResponse(
                request.FunctionCode,
                request.SlaveAddress,
                data);
        }
    }
}