using Modbus4Net.Data;
using Modbus4Net.Message;
using Xunit;

namespace Modbus4Net.UnitTests.Message
{
    public class ReadCoilsInputsResponseFixture
    {
        [Fact]
        public void CreateReadCoilsResponse()
        {
            var response = new ReadCoilsInputsResponse(ModbusFunctionCodes.ReadCoils, 5, 2,
                new DiscreteCollection(true, true, true, true, true, true, false, false, true, true, false));
            Assert.Equal(ModbusFunctionCodes.ReadCoils, response.FunctionCode);
            Assert.Equal(5, response.SlaveAddress);
            Assert.Equal(2, response.ByteCount);
            var col = new DiscreteCollection(true, true, true, true, true, true, false, false, true, true,
                false);
            Assert.Equal(col.NetworkBytes, response.Data.NetworkBytes);
        }

        [Fact]
        public void CreateReadInputsResponse()
        {
            var response = new ReadCoilsInputsResponse(ModbusFunctionCodes.ReadInputs, 5, 2,
                new DiscreteCollection(true, true, true, true, true, true, false, false, true, true, false));
            Assert.Equal(ModbusFunctionCodes.ReadInputs, response.FunctionCode);
            Assert.Equal(5, response.SlaveAddress);
            Assert.Equal(2, response.ByteCount);
            var col = new DiscreteCollection(true, true, true, true, true, true, false, false, true, true,
                false);
            Assert.Equal(col.NetworkBytes, response.Data.NetworkBytes);
        }

        [Fact]
        public void ToString_Coils()
        {
            var response = new ReadCoilsInputsResponse(ModbusFunctionCodes.ReadCoils, 5, 2,
                new DiscreteCollection(true, true, true, true, true, true, false, false, true, true, false));

            Assert.Equal("Read 11 coils - {1, 1, 1, 1, 1, 1, 0, 0, 1, 1, 0}.", response.ToString());
        }

        [Fact]
        public void ToString_Inputs()
        {
            var response = new ReadCoilsInputsResponse(ModbusFunctionCodes.ReadInputs, 5, 2,
                new DiscreteCollection(true, true, true, true, true, true, false, false, true, true, false));

            Assert.Equal("Read 11 inputs - {1, 1, 1, 1, 1, 1, 0, 0, 1, 1, 0}.", response.ToString());
        }
    }
}