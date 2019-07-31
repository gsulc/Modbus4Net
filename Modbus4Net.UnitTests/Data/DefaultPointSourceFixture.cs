using Modbus4Net.Data;
using Xunit;

namespace Modbus4Net.UnitTests.Data
{

    public class DefaultPointSourceFixture
    {
        [Theory]
        [InlineData(0, 42)]
        [InlineData(ushort.MaxValue - 1, 45)]
        [InlineData(77, 456)]
        public void AddValues(ushort startAddress, int value)
        {
            IPointSource<int> points = new DefaultPointSource<int>();

            points.WritePoints(startAddress, new []{ value });

            Assert.Equal(value, points.ReadPoints(startAddress, 1)[0]);
        }

    }
}