using Modbus4Net.Message;
using Xunit;

namespace Modbus4Net.UnitTests.Message
{
    public class SlaveExceptionResponseFixture
    {
        [Fact]
        public void CreateSlaveExceptionResponse()
        {
            var response = new SlaveExceptionResponse(11, ModbusFunctionCodes.ReadCoils + Modbus.ExceptionOffset,
                2);
            Assert.Equal(11, response.SlaveAddress);
            Assert.Equal(ModbusFunctionCodes.ReadCoils + Modbus.ExceptionOffset, response.FunctionCode);
            Assert.Equal(2, response.SlaveExceptionCode);
        }

        [Fact]
        public void SlaveExceptionResponsePDU()
        {
            var response = new SlaveExceptionResponse(11, ModbusFunctionCodes.ReadCoils + Modbus.ExceptionOffset,
                2);
            Assert.Equal(new byte[] { response.FunctionCode, response.SlaveExceptionCode }, response.ProtocolDataUnit);
        }
    }
}