using Modbus4Net.Data;
using Modbus4Net.Message;
using Xunit;

namespace Modbus4Net.UnitTests.Message
{
    public class DiagnosticsRequestResponseFixture
    {
        [Fact]
        public void ToString_Test()
        {
            DiagnosticsRequestResponse response;

            response = new DiagnosticsRequestResponse(ModbusFunctionCodes.DiagnosticsReturnQueryData, 3, new RegisterCollection(5));
            Assert.Equal("Diagnostics message, sub-function return query data - {5}.", response.ToString());
        }
    }
}