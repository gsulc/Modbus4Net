﻿using Modbus4Net.Data;
using Modbus4Net.Message;

namespace Modbus4Net.Device.MessageHandlers
{
    internal class ReadHoldingRegistersService : ModbusFunctionServiceBase<ReadHoldingInputRegistersRequest>
    {
        public ReadHoldingRegistersService()
            : base(ModbusFunctionCodes.ReadHoldingRegisters)
        {
        }

        public override IModbusMessage CreateRequest(byte[] frame)
        {
            return CreateModbusMessage<ReadHoldingInputRegistersRequest>(frame);
        }

        public override int GetRtuRequestBytesToRead(byte[] frameStart)
        {
            return 1;
        }

        public override int GetRtuResponseBytesToRead(byte[] frameStart)
        {
            return frameStart[2] + 1;
        }

        protected override IModbusMessage Handle(ReadHoldingInputRegistersRequest request, ISlaveDataStore dataStore)
        {
            ushort[] registers = dataStore.HoldingRegisters.ReadPoints(request.StartAddress, request.NumberOfPoints);

            RegisterCollection data = new RegisterCollection(registers);

            return new ReadHoldingInputRegistersResponse(request.FunctionCode, request.SlaveAddress, data);
        }
    }
}