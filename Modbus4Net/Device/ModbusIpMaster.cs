using System.Diagnostics.CodeAnalysis;

namespace Modbus4Net.Device
{
    /// <summary>
    /// Modbus IP master device.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1706:ShortAcronymsShouldBeUppercase", Justification = "Breaking change.")]
    internal class ModbusIpMaster : ModbusMaster
    {
        /// <summary>
        /// Modbus IP master device.
        /// </summary>
        /// <param name="transport">Transport used by this master.</param>
        public ModbusIpMaster(IModbusTransport transport)
            : base(transport)
        {
        }
    }
}
