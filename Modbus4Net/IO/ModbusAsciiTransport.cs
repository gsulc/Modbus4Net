using Modbus4Net.Logging;
using Modbus4Net.Utility;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Modbus4Net.IO
{
    /// <summary>
    /// Refined Abstraction - http://en.wikipedia.org/wiki/Bridge_Pattern
    /// </summary>
    public class ModbusAsciiTransport : ModbusSerialTransport, IModbusAsciiTransport
    {
        public ModbusAsciiTransport(IStreamResource streamResource, IModbusFactory modbusFactory, IModbusLogger logger)
            : base(streamResource, modbusFactory, logger)
        {
            Debug.Assert(streamResource != null, "Argument streamResource cannot be null.");
        }

        public override byte[] BuildMessageFrame(IModbusMessage message)
        {
            byte[] msgFrame = message.MessageFrame;

            byte[] msgFrameAscii = ModbusUtility.GetAsciiBytes(msgFrame);
            byte[] lrcAscii = ModbusUtility.GetAsciiBytes(ModbusUtility.CalculateLrc(msgFrame));
            byte[] nlAscii = Encoding.UTF8.GetBytes(Modbus.NewLine.ToCharArray());

            var frame = new MemoryStream(1 + msgFrameAscii.Length + lrcAscii.Length + nlAscii.Length);
            frame.WriteByte((byte)':');
            frame.Write(msgFrameAscii, 0, msgFrameAscii.Length);
            frame.Write(lrcAscii, 0, lrcAscii.Length);
            frame.Write(nlAscii, 0, nlAscii.Length);

            return frame.ToArray();
        }

        public override bool ChecksumsMatch(IModbusMessage message, byte[] messageFrame)
        {
            return ModbusUtility.CalculateLrc(message.MessageFrame) == messageFrame[messageFrame.Length - 1];
        }

        public override byte[] ReadRequest()
        {
            return ReadRequestResponse();
        }

        public override Task<byte[]> ReadRequestAsync()
        {
            return ReadRequestResponseAsync();
        }

        public override IModbusMessage ReadResponse<T>()
        {
            return CreateResponse<T>(ReadRequestResponse());
        }

        public override async Task<IModbusMessage> ReadResponseAsync<T>()
        {
            var response = await ReadRequestResponseAsync();
            return CreateResponse<T>(response);
        }

        internal byte[] ReadRequestResponse()
        {
            // read message frame, removing frame start ':'
            string frameHex = StreamResourceUtility.ReadLine(StreamResource).Substring(1);

            byte[] frame = ModbusUtility.HexToBytes(frameHex);
            Logger.Trace($"RX: {string.Join(", ", frame)}");

            if (frame.Length < 3)
                throw new IOException("Premature end of stream, message truncated.");

            return frame;
        }

        internal async Task<byte[]> ReadRequestResponseAsync()
        {
            // read message frame, removing frame start ':'
            string frameHex = (await StreamResourceUtility.ReadLineAsync(StreamResource)).Substring(1);

            byte[] frame = ModbusUtility.HexToBytes(frameHex);
            Logger.Trace($"RX: {string.Join(", ", frame)}");

            if (frame.Length < 3)
                throw new IOException("Premature end of stream, message truncated.");

            return frame;
        }

        public override void IgnoreResponse()
        {
            ReadRequestResponse();
        }
    }
}
