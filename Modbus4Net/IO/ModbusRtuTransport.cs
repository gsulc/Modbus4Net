using Modbus4Net.Extensions;
using Modbus4Net.Logging;
using Modbus4Net.Utility;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Modbus4Net.IO
{
    /// <summary>
    /// Refined Abstraction - http://en.wikipedia.org/wiki/Bridge_Pattern
    /// </summary>
    public class ModbusRtuTransport : ModbusSerialTransport, IModbusRtuTransport
    {
        public const int RequestFrameStartLength = 7;

        public const int ResponseFrameStartLength = 4;

        public ModbusRtuTransport(IStreamResource streamResource, IModbusFactory modbusFactory, IModbusLogger logger)
            : base(streamResource, modbusFactory, logger)
        {
            if (modbusFactory == null) throw new ArgumentNullException(nameof(modbusFactory));
            Debug.Assert(streamResource != null, "Argument streamResource cannot be null.");
        }

        public int RequestBytesToRead(byte[] frameStart)
        {
            byte functionCode = frameStart[1];

            IModbusFunctionService service = ModbusFactory.GetFunctionServiceOrThrow(functionCode);

            return service.GetRtuRequestBytesToRead(frameStart);
        }

        public int ResponseBytesToRead(byte[] frameStart)
        {
            byte functionCode = frameStart[1];

            if (functionCode > Modbus.ExceptionOffset)
                return 1;

            IModbusFunctionService service = ModbusFactory.GetFunctionServiceOrThrow(functionCode);

            return service.GetRtuResponseBytesToRead(frameStart);
        }

        public virtual byte[] Read(int count)
        {
            byte[] frameBytes = new byte[count];
            int numBytesRead = 0;

            while (numBytesRead != count)
                numBytesRead += StreamResource.Read(frameBytes, numBytesRead, count - numBytesRead);

            return frameBytes;
        }

        public virtual async Task<byte[]> ReadAsync(int count)
        {
            byte[] frameBytes = new byte[count];
            int numBytesRead = 0;

            while (numBytesRead != count)
                numBytesRead += await StreamResource.ReadAsync(frameBytes, numBytesRead, count - numBytesRead);

            return frameBytes;
        }

        public override byte[] BuildMessageFrame(IModbusMessage message)
        {
            byte[] messageFrame = message.MessageFrame;
            byte[] crc = ModbusUtility.CalculateCrc(messageFrame);
            var messageBody = new MemoryStream(messageFrame.Length + crc.Length);

            messageBody.Write(messageFrame, 0, messageFrame.Length);
            messageBody.Write(crc, 0, crc.Length);

            return messageBody.ToArray();
        }

        public override bool ChecksumsMatch(IModbusMessage message, byte[] messageFrame)
        {
            ushort messageCrc = BitConverter.ToUInt16(messageFrame, messageFrame.Length - 2);
            ushort calculatedCrc = BitConverter.ToUInt16(ModbusUtility.CalculateCrc(message.MessageFrame), 0);

            return messageCrc == calculatedCrc;
        }

        public override IModbusMessage ReadResponse<T>()
        {
            byte[] frame = ReadResponse();
            Logger.LogFrameRx(frame);
            return CreateResponse<T>(frame);
        }

        public override async Task<IModbusMessage> ReadResponseAsync<T>()
        {
            byte[] frame = await ReadResponseAsync();
            Logger.LogFrameRx(frame);
            return CreateResponse<T>(frame);
        }

        private byte[] ReadResponse()
        {
            byte[] frameStart = Read(ResponseFrameStartLength);
            byte[] frameEnd = Read(ResponseBytesToRead(frameStart));
            byte[] frame = frameStart.Concat(frameEnd).ToArray();

            return frame;
        }

        private async Task<byte[]> ReadResponseAsync()
        {
            byte[] frameStart = await ReadAsync(ResponseFrameStartLength);
            byte[] frameEnd = await ReadAsync(ResponseBytesToRead(frameStart));
            byte[] frame = frameStart.Concat(frameEnd).ToArray();

            return frame;
        }

        public override void IgnoreResponse()
        {
            byte[] frame = ReadResponse();

            Logger.LogFrameIgnoreRx(frame);
        }

        public override byte[] ReadRequest()
        {
            byte[] frameStart = Read(RequestFrameStartLength);
            byte[] frameEnd = Read(RequestBytesToRead(frameStart));
            byte[] frame = frameStart.Concat(frameEnd).ToArray();

            Logger.LogFrameRx(frame);

            return frame;
        }

        public override async Task<byte[]> ReadRequestAsync()
        {
            byte[] frameStart = await ReadAsync(RequestFrameStartLength);
            byte[] frameEnd = await ReadAsync(RequestBytesToRead(frameStart));
            byte[] frame = frameStart.Concat(frameEnd).ToArray();

            Logger.LogFrameRx(frame);

            return frame;
        }
    }
}
