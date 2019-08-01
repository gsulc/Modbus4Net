using Modbus4Net.Message;
using System;
using Xunit;

namespace Modbus4Net.UnitTests.Message
{
    public class ReadHoldingInputRegistersRequestFixture
    {
        [Fact]
        public void CreateReadHoldingRegistersRequest()
        {
            var request = new ReadHoldingInputRegistersRequest(
                ModbusFunctionCodes.ReadHoldingRegisters, 5, 1, 10);
            Assert.Equal(ModbusFunctionCodes.ReadHoldingRegisters, request.FunctionCode);
            Assert.Equal(5, request.SlaveAddress);
            Assert.Equal(1, request.StartAddress);
            Assert.Equal(10, request.NumberOfPoints);
        }

        [Fact]
        public void CreateReadInputRegistersRequest()
        {
            var request = new ReadHoldingInputRegistersRequest(ModbusFunctionCodes.ReadInputRegisters, 5,
                1, 10);
            Assert.Equal(ModbusFunctionCodes.ReadInputRegisters, request.FunctionCode);
            Assert.Equal(5, request.SlaveAddress);
            Assert.Equal(1, request.StartAddress);
            Assert.Equal(10, request.NumberOfPoints);
        }

        [Fact]
        public void CreateReadHoldingInputRegistersRequestTooMuchData()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new ReadHoldingInputRegistersRequest(ModbusFunctionCodes.ReadHoldingRegisters, 1, 2,
                Modbus.MaximumRegisterRequestResponseSize + 1));
        }

        [Fact]
        public void CreateReadHoldingInputRegistersRequestMaxSize()
        {
            var response = new ReadHoldingInputRegistersRequest(
                ModbusFunctionCodes.ReadHoldingRegisters, 1, 2, Modbus.MaximumRegisterRequestResponseSize);
            Assert.Equal(Modbus.MaximumRegisterRequestResponseSize, response.NumberOfPoints);
        }

        [Fact]
        public void ToString_ReadHoldingRegistersRequest()
        {
            var request = new ReadHoldingInputRegistersRequest(
                ModbusFunctionCodes.ReadHoldingRegisters, 5, 1, 10);

            Assert.Equal("Read 10 holding registers starting at address 1.", request.ToString());
        }

        [Fact]
        public void ToString_ReadInputRegistersRequest()
        {
            var request = new ReadHoldingInputRegistersRequest(ModbusFunctionCodes.ReadInputRegisters, 5,
                1, 10);

            Assert.Equal("Read 10 input registers starting at address 1.", request.ToString());
        }
    }
}