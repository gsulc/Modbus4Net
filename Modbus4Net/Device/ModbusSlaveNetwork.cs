﻿using Modbus4Net.Extensions;
using Modbus4Net.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Modbus4Net.Device
{
    internal abstract class ModbusSlaveNetwork : ModbusDevice, IModbusSlaveNetwork
    {
        private readonly IModbusLogger _logger;
        private readonly IDictionary<byte, IModbusSlave> _slaves = new ConcurrentDictionary<byte, IModbusSlave>();

        protected ModbusSlaveNetwork(IModbusTransport transport, IModbusFactory modbusFactory, IModbusLogger logger)
            : base(transport)
        {
            ModbusFactory = modbusFactory;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected IModbusFactory ModbusFactory { get; }

        protected IModbusLogger Logger => _logger;

        /// <summary>
        /// Start slave listening for requests.
        /// </summary>
        public abstract Task ListenAsync(CancellationToken cancellationToken = new CancellationToken());

        /// <summary>
        /// Apply the request.
        /// </summary>
        /// <param name="request"></param>
        protected IModbusMessage ApplyRequest(IModbusMessage request)
        {
            if (request.SlaveAddress == 0) // broadcast request
            {
                foreach (IModbusSlave slave in _slaves.Values)
                {
                    try
                    {
                        slave.ApplyRequest(request);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error($"Error applying request to slave {slave.UnitId}: {ex.Message}");
                    }
                }
            }
            else
            {
                IModbusSlave slave = GetSlave(request.SlaveAddress);

                if (slave == null)
                    Console.WriteLine($"NModbus Slave Network ignoring request intended for NModbus Slave {request.SlaveAddress}");
                else
                    return slave.ApplyRequest(request);
            }

            return null;
        }

        public void AddSlave(IModbusSlave slave)
        {
            if (slave == null)
                throw new ArgumentNullException(nameof(slave));

            _slaves.Add(slave.UnitId, slave);

            Logger.Information($"Slave {slave.UnitId} added to slave network.");
        }

        public void RemoveSlave(byte unitId)
        {
            _slaves.Remove(unitId);

            Logger.Information($"Slave {unitId} removed from slave network.");
        }

        public IModbusSlave GetSlave(byte unitId)
        {
            return _slaves.GetValueOrDefault(unitId);
        }
    }
}