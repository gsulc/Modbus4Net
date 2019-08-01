namespace Modbus4Net.Message
{
    /// <summary>
    /// Modbus message factory.
    /// </summary>
    public static class ModbusMessageFactory
    {
        /// <summary>
        /// Create a Modbus message.
        /// </summary>
        /// <typeparam name="T">Modbus message type.</typeparam>
        /// <param name="frame">Bytes of Modbus frame.</param>
        /// <returns>New Modbus message based on type and frame bytes.</returns>
        public static T CreateModbusMessage<T>(byte[] frame)
            where T : IModbusMessage, new()
        {
            var message = new T();
            message.Initialize(frame);
            return message;
        }
    }
}