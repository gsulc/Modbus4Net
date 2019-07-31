using Modbus4Net.Data;

namespace Modbus4Net.Message
{
    internal abstract class AbstractModbusMessageWithData<TData> : AbstractModbusMessage
        where TData : IModbusMessageDataCollection
    {
        internal AbstractModbusMessageWithData()
        {
        }

        internal AbstractModbusMessageWithData(byte slaveAddress, byte functionCode)
            : base(slaveAddress, functionCode)
        {
        }

        public TData Data
        {
            get => (TData)MessageImpl.Data;
            set => MessageImpl.Data = value;
        }
    }
}
