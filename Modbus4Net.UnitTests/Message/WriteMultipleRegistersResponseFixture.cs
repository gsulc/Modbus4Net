﻿using Modbus4Net.Message;
using System;
using Xunit;

namespace Modbus4Net.UnitTests.Message
{
    public class WriteMultipleRegistersResponseFixture
    {
        [Fact]
        public void CreateWriteMultipleRegistersResponse()
        {
            var response = new WriteMultipleRegistersResponse(12, 39, 2);
            Assert.Equal(ModbusFunctionCodes.WriteMultipleRegisters, response.FunctionCode);
            Assert.Equal(12, response.SlaveAddress);
            Assert.Equal(39, response.StartAddress);
            Assert.Equal(2, response.NumberOfPoints);
        }

        [Fact]
        public void CreateWriteMultipleRegistersResponseTooMuchData()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new WriteMultipleRegistersResponse(1, 2, Modbus.MaximumRegisterRequestResponseSize + 1));
        }

        [Fact]
        public void CreateWriteMultipleRegistersResponseMaxSize()
        {
            var response = new WriteMultipleRegistersResponse(1, 2,
                Modbus.MaximumRegisterRequestResponseSize);
            Assert.Equal(Modbus.MaximumRegisterRequestResponseSize, response.NumberOfPoints);
        }

        [Fact]
        public void ToString_Test()
        {
            var response = new WriteMultipleRegistersResponse(1, 2, 3);

            Assert.Equal("Wrote 3 holding registers starting at address 2.", response.ToString());
        }
    }
}