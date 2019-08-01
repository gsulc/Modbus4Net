﻿using Modbus4Net.Data;
using Modbus4Net.Message;

namespace Modbus4Net.Device.MessageHandlers
{
    internal class ReadCoilsService : ModbusFunctionServiceBase<ReadCoilsInputsRequest>
    {
        public ReadCoilsService()
            : base(ModbusFunctionCodes.ReadCoils)
        {
        }

        public override IModbusMessage CreateRequest(byte[] frame)
        {
            return CreateModbusMessage<ReadCoilsInputsRequest>(frame);
        }

        public override int GetRtuRequestBytesToRead(byte[] frameStart)
        {
            return 1;
        }

        public override int GetRtuResponseBytesToRead(byte[] frameStart)
        {
            return frameStart[2] + 1;
        }

        protected override IModbusMessage Handle(ReadCoilsInputsRequest request, ISlaveDataStore dataStore)
        {
            bool[] discretes = dataStore.CoilDiscretes.ReadPoints(request.StartAddress, request.NumberOfPoints);

            var data = new DiscreteCollection(discretes);

            return new ReadCoilsInputsResponse(
                request.FunctionCode,
                request.SlaveAddress,
                data.ByteCount, data);
        }
    }
}