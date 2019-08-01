using Modbus4Net.Message;
using System;
using Xunit;

namespace Modbus4Net.UnitTests.Message
{
    public class ReadCoilsInputsRequestFixture
    {
        [Fact]
        public void CreateReadCoilsRequest()
        {
            var request = new ReadCoilsInputsRequest(ModbusFunctionCodes.ReadCoils, 5, 1, 10);
            Assert.Equal(ModbusFunctionCodes.ReadCoils, request.FunctionCode);
            Assert.Equal(5, request.SlaveAddress);
            Assert.Equal(1, request.StartAddress);
            Assert.Equal(10, request.NumberOfPoints);
        }

        [Fact]
        public void CreateReadInputsRequest()
        {
            var request = new ReadCoilsInputsRequest(ModbusFunctionCodes.ReadInputs, 5, 1, 10);
            Assert.Equal(ModbusFunctionCodes.ReadInputs, request.FunctionCode);
            Assert.Equal(5, request.SlaveAddress);
            Assert.Equal(1, request.StartAddress);
            Assert.Equal(10, request.NumberOfPoints);
        }

        [Fact]
        public void CreateReadCoilsInputsRequestTooMuchData()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new ReadCoilsInputsRequest(ModbusFunctionCodes.ReadCoils, 1, 2, Modbus.MaximumDiscreteRequestResponseSize + 1));
        }

        [Fact]
        public void CreateReadCoilsInputsRequestMaxSize()
        {
            var response = new ReadCoilsInputsRequest(ModbusFunctionCodes.ReadCoils, 1, 2,
                Modbus.MaximumDiscreteRequestResponseSize);
            Assert.Equal(Modbus.MaximumDiscreteRequestResponseSize, response.NumberOfPoints);
        }

        [Fact]
        public void ToString_ReadCoilsRequest()
        {
            var request = new ReadCoilsInputsRequest(ModbusFunctionCodes.ReadCoils, 5, 1, 10);

            Assert.Equal("Read 10 coils starting at address 1.", request.ToString());
        }

        [Fact]
        public void ToString_ReadInputsRequest()
        {
            var request = new ReadCoilsInputsRequest(ModbusFunctionCodes.ReadInputs, 5, 1, 10);

            Assert.Equal("Read 10 inputs starting at address 1.", request.ToString());
        }
    }
}