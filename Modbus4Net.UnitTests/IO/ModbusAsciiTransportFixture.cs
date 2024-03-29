﻿using Modbus4Net.IO;
using Modbus4Net.Logging;
using Modbus4Net.Message;
using Moq;
using System.IO;
using System.Text;
using Xunit;

namespace Modbus4Net.UnitTests.IO
{
    public class ModbusAsciiTransportFixture
    {
        private static IStreamResource StreamResource => new Mock<IStreamResource>(MockBehavior.Strict).Object;

        [Fact]
        public void BuildMessageFrame()
        {
            byte[] expected = { 58, 48, 50, 48, 49, 48, 48, 48, 48, 48, 48, 48, 49, 70, 67, 13, 10 };
            var request = new ReadCoilsInputsRequest(ModbusFunctionCodes.ReadCoils, 2, 0, 1);
            byte[] actual = new ModbusAsciiTransport(StreamResource, new ModbusFactory(), NullModbusLogger.Instance)
                .BuildMessageFrame(request);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ReadRequestResponse()
        {
            var mock = new Mock<IStreamResource>(MockBehavior.Strict);
            IStreamResource stream = mock.Object;
            var transport = new ModbusAsciiTransport(stream, new ModbusFactory(), NullModbusLogger.Instance);
            int calls = 0;
            byte[] bytes = Encoding.ASCII.GetBytes(":110100130025B6\r\n");

            mock.Setup(s => s.Read(It.Is<byte[]>(x => x.Length == 1), 0, 1))
                .Returns((byte[] buffer, int offset, int count) =>
                {
                    buffer[offset] = bytes[calls++];
                    return 1;
                });

            Assert.Equal(new byte[] { 17, 1, 0, 19, 0, 37, 182 }, transport.ReadRequestResponse());
            mock.VerifyAll();
        }

        [Fact]
        public void ReadRequestResponseNotEnoughBytes()
        {
            var mock = new Mock<IStreamResource>(MockBehavior.Strict);
            IStreamResource stream = mock.Object;
            var transport = new ModbusAsciiTransport(stream, new ModbusFactory(), NullModbusLogger.Instance);
            int calls = 0;
            byte[] bytes = Encoding.ASCII.GetBytes(":10\r\n");

            mock.Setup(s => s.Read(It.Is<byte[]>(x => x.Length == 1), 0, 1))
                .Returns((byte[] buffer, int offset, int count) =>
                {
                    buffer[offset] = bytes[calls++];
                    return 1;
                });

            Assert.Throws<IOException>(() => transport.ReadRequestResponse());
            mock.VerifyAll();
        }

        [Fact]
        public void ChecksumsMatchSucceed()
        {
            var transport = new ModbusAsciiTransport(StreamResource, new ModbusFactory(), NullModbusLogger.Instance);
            var message = new ReadCoilsInputsRequest(ModbusFunctionCodes.ReadCoils, 17, 19, 37);
            byte[] frame = { 17, ModbusFunctionCodes.ReadCoils, 0, 19, 0, 37, 182 };

            Assert.True(transport.ChecksumsMatch(message, frame));
        }

        [Fact]
        public void ChecksumsMatchFail()
        {
            var transport = new ModbusAsciiTransport(StreamResource, new ModbusFactory(), NullModbusLogger.Instance);
            var message = new ReadCoilsInputsRequest(ModbusFunctionCodes.ReadCoils, 17, 19, 37);
            byte[] frame = { 17, ModbusFunctionCodes.ReadCoils, 0, 19, 0, 37, 181 };

            Assert.False(transport.ChecksumsMatch(message, frame));
        }
    }
}
