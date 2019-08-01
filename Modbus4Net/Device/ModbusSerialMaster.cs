using Modbus4Net.Data;
using Modbus4Net.Message;
using System.Diagnostics.CodeAnalysis;

namespace Modbus4Net.Device
{
    /// <summary>
    /// Modbus serial master device.
    /// </summary>
    internal class ModbusSerialMaster : ModbusMaster, IModbusSerialMaster
    {
        internal ModbusSerialMaster(IModbusSerialTransport transport)
            : base(transport)
        {
        }

        /// <summary>
        /// Gets the Modbus Transport.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        IModbusSerialTransport IModbusSerialMaster.Transport => (IModbusSerialTransport)Transport;

        /// <summary>
        /// Serial Line only.
        /// Diagnostic function which loops back the original data.
        /// NModbus only supports looping back one ushort value, this is a limitation of the "Best Effort" implementation of
        /// the RTU protocol.
        /// </summary>
        /// <param name="slaveAddress">Address of device to test.</param>
        /// <param name="data">Data to return.</param>
        /// <returns>Return true if slave device echoed data.</returns>
        public bool ReturnQueryData(byte slaveAddress, ushort data)
        {
            var request = new DiagnosticsRequestResponse(
                ModbusFunctionCodes.DiagnosticsReturnQueryData,
                slaveAddress,
                new RegisterCollection(data));

            DiagnosticsRequestResponse response = Transport.UnicastMessage<DiagnosticsRequestResponse>(request);

            return response.Data[0] == data;
        }
    }
}
