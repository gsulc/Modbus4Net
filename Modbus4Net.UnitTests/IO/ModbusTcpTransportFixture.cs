﻿using System;
using System.IO;
using System.Linq;
using Castle.Core.Logging;
using Moq;
using Modbus4Net.Data;
using Modbus4Net.IO;
using Modbus4Net.Logging;
using Modbus4Net.Message;
using Modbus4Net.UnitTests.Message;
using Xunit;

namespace Modbus4Net.UnitTests.IO
{
    public class ModbusTcpTransportFixture
    {
        private IStreamResource StreamResourceMock => new Mock<IStreamResource>(MockBehavior.Strict).Object;

        [Fact]
        public void BuildMessageFrame()
        {
            var mock = new Mock<ModbusIpTransport>(StreamResourceMock, new ModbusFactory(),  NullModbusLogger.Instance) { CallBase = true };
            var message = new ReadCoilsInputsRequest(ModbusFunctionCodes.ReadCoils, 2, 10, 5);

            byte[] result = mock.Object.BuildMessageFrame(message);
            Assert.Equal(new byte[] { 0, 0, 0, 0, 0, 6, 2, 1, 0, 10, 0, 5 }, result);
            mock.VerifyAll();
        }

        [Fact]
        public void GetMbapHeader()
        {
            var message = new WriteMultipleRegistersRequest(3, 1, MessageUtility.CreateDefaultCollection<RegisterCollection, ushort>(0, 120));
            message.TransactionId = 45;
            Assert.Equal(new byte[] { 0, 45, 0, 0, 0, 247, 3 }, ModbusIpTransport.GetMbapHeader(message));
        }

        [Fact]
        public void Write()
        {
            var streamMock = new Mock<IStreamResource>(MockBehavior.Strict);
            var mock = new Mock<ModbusIpTransport>(streamMock.Object, new ModbusFactory(), NullModbusLogger.Instance) { CallBase = true };
            var request = new ReadCoilsInputsRequest(ModbusFunctionCodes.ReadCoils, 1, 1, 3);

            streamMock.Setup(s => s.Write(It.IsNotNull<byte[]>(), 0, 12));

            mock.Setup(t => t.GetNewTransactionId()).Returns(ushort.MaxValue);

            mock.Object.Write(request);

            Assert.Equal(ushort.MaxValue, request.TransactionId);

            mock.VerifyAll();
            streamMock.VerifyAll();
        }

        [Fact]
        public void ReadRequestResponse()
        {
            var mock = new Mock<IStreamResource>(MockBehavior.Strict);
            var request = new ReadCoilsInputsRequest(ModbusFunctionCodes.ReadCoils, 1, 1, 3);
            int calls = 0;
            byte[][] source =
            {
                new byte[] { 45, 63, 0, 0, 0, 6 },
                new byte[] { 1 }.Concat(request.ProtocolDataUnit).ToArray()
            };

            mock.Setup(s => s.Read(It.Is<byte[]>(x => x.Length == 6), 0, 6))
                .Returns((byte[] buf, int offset, int count) =>
                {
                    Array.Copy(source[calls++], buf, 6);
                    return 6;
                });

            Assert.Equal(
                new byte[] { 45, 63, 0, 0, 0, 6, 1, 1, 0, 1, 0, 3 },
                ModbusIpTransport.ReadRequestResponse(mock.Object, NullModbusLogger.Instance));

            mock.VerifyAll();
        }

        [Fact]
        public void ReadRequestResponse_ConnectionAbortedWhileReadingMBAPHeader()
        {
            var mock = new Mock<IStreamResource>(MockBehavior.Strict);
            mock.Setup(s => s.Read(It.Is<byte[]>(x => x.Length == 6), 0, 6)).Returns(3);
            mock.Setup(s => s.Read(It.Is<byte[]>(x => x.Length == 6), 3, 3)).Returns(0);

            Assert.Throws<IOException>(() => ModbusIpTransport.ReadRequestResponse(mock.Object, NullModbusLogger.Instance));
            mock.VerifyAll();
        }

        [Fact]
        public void ReadRequestResponse_ConnectionAbortedWhileReadingMessageFrame()
        {
            var mock = new Mock<IStreamResource>(MockBehavior.Strict);

            mock.Setup(s => s.Read(It.Is<byte[]>(x => x.Length == 6), 0, 6)).Returns(6);
            mock.Setup(s => s.Read(It.Is<byte[]>(x => x.Length == 6), 0, 6)).Returns(3);
            mock.Setup(s => s.Read(It.Is<byte[]>(x => x.Length == 6), 3, 3)).Returns(0);

            Assert.Throws<IOException>(() => ModbusIpTransport.ReadRequestResponse(mock.Object, NullModbusLogger.Instance));
            mock.VerifyAll();
        }

        [Fact]
        public void GetNewTransactionId()
        {
            var transport = new ModbusIpTransport(StreamResourceMock, new ModbusFactory(),  NullModbusLogger.Instance);

            Assert.Equal(1, transport.GetNewTransactionId());
            Assert.Equal(2, transport.GetNewTransactionId());
        }

        [Fact]
        public void OnShouldRetryResponse_ReturnsTrue_IfWithinThreshold()
        {
            var transport = new ModbusIpTransport(StreamResourceMock, new ModbusFactory(),  NullModbusLogger.Instance);
            var request = new ReadCoilsInputsRequest(ModbusFunctionCodes.ReadCoils, 1, 1, 1);
            var response = new ReadCoilsInputsResponse(ModbusFunctionCodes.ReadCoils, 1, 1, null);

            request.TransactionId = 5;
            response.TransactionId = 4;
            transport.RetryOnOldResponseThreshold = 3;

            Assert.True(transport.OnShouldRetryResponse(request, response));
        }

        [Fact]
        public void OnShouldRetryResponse_ReturnsFalse_IfThresholdDisabled()
        {
            var transport = new ModbusIpTransport(StreamResourceMock, new ModbusFactory(),  NullModbusLogger.Instance);
            var request = new ReadCoilsInputsRequest(ModbusFunctionCodes.ReadCoils, 1, 1, 1);
            var response = new ReadCoilsInputsResponse(ModbusFunctionCodes.ReadCoils, 1, 1, null);

            request.TransactionId = 5;
            response.TransactionId = 4;
            transport.RetryOnOldResponseThreshold = 0;

            Assert.False(transport.OnShouldRetryResponse(request, response));
        }

        [Fact]
        public void OnShouldRetryResponse_ReturnsFalse_IfEqualTransactionId()
        {
            var transport = new ModbusIpTransport(StreamResourceMock, new ModbusFactory(), NullModbusLogger.Instance);
            var request = new ReadCoilsInputsRequest(ModbusFunctionCodes.ReadCoils, 1, 1, 1);
            var response = new ReadCoilsInputsResponse(ModbusFunctionCodes.ReadCoils, 1, 1, null);

            request.TransactionId = 5;
            response.TransactionId = 5;
            transport.RetryOnOldResponseThreshold = 3;

            Assert.False(transport.OnShouldRetryResponse(request, response));
        }

        [Fact]
        public void OnShouldRetryResponse_ReturnsFalse_IfOutsideThreshold()
        {
            var transport = new ModbusIpTransport(StreamResourceMock, new ModbusFactory(), NullModbusLogger.Instance);
            var request = new ReadCoilsInputsRequest(ModbusFunctionCodes.ReadCoils, 1, 1, 1);
            var response = new ReadCoilsInputsResponse(ModbusFunctionCodes.ReadCoils, 1, 1, null);

            request.TransactionId = 5;
            response.TransactionId = 2;
            transport.RetryOnOldResponseThreshold = 3;

            Assert.False(transport.OnShouldRetryResponse(request, response));
        }

        [Fact]
        public void ValidateResponse_MismatchingTransactionIds()
        {
            var transport = new ModbusIpTransport(StreamResourceMock, new ModbusFactory(), NullModbusLogger.Instance);

            var request = new ReadCoilsInputsRequest(ModbusFunctionCodes.ReadCoils, 1, 1, 1);
            request.TransactionId = 5;
            var response = new ReadCoilsInputsResponse(ModbusFunctionCodes.ReadCoils, 1, 1, null);
            response.TransactionId = 6;

            Assert.Throws<IOException>(() => transport.ValidateResponse(request, response));
        }

        [Fact]
        public void ValidateResponse()
        {
            var transport = new ModbusIpTransport(StreamResourceMock, new ModbusFactory(), NullModbusLogger.Instance);

            var request = new ReadCoilsInputsRequest(ModbusFunctionCodes.ReadCoils, 1, 1, 1);
            request.TransactionId = 5;
            var response = new ReadCoilsInputsResponse(ModbusFunctionCodes.ReadCoils, 1, 1, null);
            response.TransactionId = 5;

            // no exception is thrown
            transport.ValidateResponse(request, response);
        }
    }
}
