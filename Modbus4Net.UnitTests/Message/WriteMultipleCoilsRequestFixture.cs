using Modbus4Net.Data;
using Modbus4Net.Message;
using System;
using Xunit;

namespace Modbus4Net.UnitTests.Message
{
    public class WriteMultipleCoilsRequestFixture
    {
        [Fact]
        public void CreateWriteMultipleCoilsRequest()
        {
            var col = new DiscreteCollection(true, false, true, false, true, true, true, false, false);
            var request = new WriteMultipleCoilsRequest(34, 45, col);
            Assert.Equal(ModbusFunctionCodes.WriteMultipleCoils, request.FunctionCode);
            Assert.Equal(34, request.SlaveAddress);
            Assert.Equal(45, request.StartAddress);
            Assert.Equal(9, request.NumberOfPoints);
            Assert.Equal(2, request.ByteCount);
            Assert.Equal(col.NetworkBytes, request.Data.NetworkBytes);
        }

        [Fact]
        public void CreateWriteMultipleCoilsRequestTooMuchData()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new WriteMultipleCoilsRequest(1, 2, MessageUtility.CreateDefaultCollection<DiscreteCollection, bool>(true, Modbus.MaximumDiscreteRequestResponseSize + 1)));
        }

        [Fact]
        public void CreateWriteMultipleCoilsRequestMaxSize()
        {
            var request = new WriteMultipleCoilsRequest(1, 2,
                MessageUtility.CreateDefaultCollection<DiscreteCollection, bool>(true, Modbus.MaximumDiscreteRequestResponseSize));
            Assert.Equal(Modbus.MaximumDiscreteRequestResponseSize, request.Data.Count);
        }

        [Fact]
        public void ToString_WriteMultipleCoilsRequest()
        {
            var col = new DiscreteCollection(true, false, true, false, true, true, true, false, false);
            var request = new WriteMultipleCoilsRequest(34, 45, col);

            Assert.Equal("Write 9 coils starting at address 45.", request.ToString());
        }
    }
}