﻿using Modbus4Net.Device;
using Modbus4Net.IO;

namespace Modbus4Net
{
    /// <summary>
    /// Extension methods for the IModbusFactory interface.
    /// </summary>
    public static class FactoryExtensions
    {
        /// <summary>
        /// Creates an RTU master.
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="streamResource"></param>
        /// <returns></returns>
        public static IModbusSerialMaster CreateRtuMaster(this IModbusFactory factory, IStreamResource streamResource)
        {
            IModbusRtuTransport transport = factory.CreateRtuTransport(streamResource);
            return new ModbusSerialMaster(transport);
        }

        /// <summary>
        /// Creates an ASCII master.
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="streamResource"></param>
        /// <returns></returns>
        public static IModbusSerialMaster CreateAsciiMaster(this IModbusFactory factory, IStreamResource streamResource)
        {
            IModbusAsciiTransport transport = factory.CreateAsciiTransport(streamResource);
            return new ModbusSerialMaster(transport);
        }

        /// <summary>
        /// Creates an RTU slave network.
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="streamResource"></param>
        /// <returns></returns>
        public static IModbusSlaveNetwork CreateRtuSlaveNetwork(this IModbusFactory factory,
            IStreamResource streamResource)
        {
            IModbusRtuTransport transport = factory.CreateRtuTransport(streamResource);
            return factory.CreateSlaveNetwork(transport);
        }

        /// <summary>
        /// Creates an ASCII slave network.
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="streamResource"></param>
        /// <returns></returns>
        public static IModbusSlaveNetwork CreateAsciiSlaveNetwork(this IModbusFactory factory,
            IStreamResource streamResource)
        {
            IModbusAsciiTransport transport = factory.CreateAsciiTransport(streamResource);
            return factory.CreateSlaveNetwork(transport);
        }
    }
}