namespace Modbus4Net.Data
{
    internal class DefaultSlaveDataStore : ISlaveDataStore
    {
        public IPointSource<ushort> HoldingRegisters { get; } = new DefaultPointSource<ushort>();

        public IPointSource<ushort> InputRegisters { get; } = new DefaultPointSource<ushort>();

        public IPointSource<bool> CoilDiscretes { get; } = new DefaultPointSource<bool>();

        public IPointSource<bool> CoilInputs { get; } = new DefaultPointSource<bool>();
    }
}