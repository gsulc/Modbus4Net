using Modbus4Net.IO;
using Modbus4Net.Message;
using System;
using Xunit;

namespace Modbus4Net.UnitTests.IO
{
    public static class EmptyTransportFixture
    {
        [Fact]
        public static void Negative()
        {
            var transport = new EmptyTransport(new ModbusFactory());
            Assert.Throws<NotImplementedException>(() => transport.ReadRequest());
            Assert.Throws<NotImplementedException>(() => transport.ReadResponse<ReadCoilsInputsResponse>());
            Assert.Throws<NotImplementedException>(() => transport.BuildMessageFrame(null));
            Assert.Throws<NotImplementedException>(() => transport.Write(null));
            Assert.Throws<NotImplementedException>(() => transport.OnValidateResponse(null, null));
        }
    }
}
