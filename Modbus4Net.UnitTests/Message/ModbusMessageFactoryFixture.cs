﻿using Modbus4Net.Data;
using Modbus4Net.Message;
using System;
using System.Linq;
using Xunit;

namespace Modbus4Net.UnitTests.Message
{
    public class ModbusMessageFactoryFixture
    {
        [Fact]
        public void CreateModbusMessageReadCoilsRequest()
        {
            ReadCoilsInputsRequest request =
                ModbusMessageFactory.CreateModbusMessage<ReadCoilsInputsRequest>(new byte[]
                { 11, ModbusFunctionCodes.ReadCoils, 0, 19, 0, 37 });
            var expectedRequest = new ReadCoilsInputsRequest(ModbusFunctionCodes.ReadCoils, 11, 19, 37);
            ModbusMessageFixture.AssertModbusMessagePropertiesAreEqual(request, expectedRequest);
            Assert.Equal(expectedRequest.StartAddress, request.StartAddress);
            Assert.Equal(expectedRequest.NumberOfPoints, request.NumberOfPoints);
        }

        [Fact]
        public void CreateModbusMessageReadCoilsRequestWithInvalidFrameSize()
        {
            byte[] frame = { 11, ModbusFunctionCodes.ReadCoils, 4, 1, 2 };
            Assert.Throws<FormatException>(() => ModbusMessageFactory.CreateModbusMessage<ReadCoilsInputsRequest>(frame));
        }

        [Fact]
        public void CreateModbusMessageReadCoilsResponse()
        {
            ReadCoilsInputsResponse response =
                ModbusMessageFactory.CreateModbusMessage<ReadCoilsInputsResponse>(new byte[]
                { 11, ModbusFunctionCodes.ReadCoils, 1, 1 });
            var expectedResponse = new ReadCoilsInputsResponse(ModbusFunctionCodes.ReadCoils, 11, 1,
                new DiscreteCollection(true, false, false, false));
            ModbusMessageFixture.AssertModbusMessagePropertiesAreEqual(expectedResponse, response);
            Assert.Equal(expectedResponse.Data.NetworkBytes, response.Data.NetworkBytes);
        }

        [Fact]
        public void CreateModbusMessageReadCoilsResponseWithNoByteCount()
        {
            byte[] frame = { 11, ModbusFunctionCodes.ReadCoils };
            Assert.Throws<FormatException>(() => ModbusMessageFactory.CreateModbusMessage<ReadCoilsInputsResponse>(frame));
        }

        [Fact]
        public void CreateModbusMessageReadCoilsResponseWithInvalidDataSize()
        {
            byte[] frame = { 11, ModbusFunctionCodes.ReadCoils, 4, 1, 2, 3 };
            Assert.Throws<FormatException>(() => ModbusMessageFactory.CreateModbusMessage<ReadCoilsInputsResponse>(frame));
        }

        [Fact]
        public void CreateModbusMessageReadHoldingRegistersRequest()
        {
            ReadHoldingInputRegistersRequest request =
                ModbusMessageFactory.CreateModbusMessage<ReadHoldingInputRegistersRequest>(new byte[]
                { 17, ModbusFunctionCodes.ReadHoldingRegisters, 0, 107, 0, 3 });
            var expectedRequest =
                new ReadHoldingInputRegistersRequest(ModbusFunctionCodes.ReadHoldingRegisters, 17, 107, 3);
            ModbusMessageFixture.AssertModbusMessagePropertiesAreEqual(expectedRequest, request);
            Assert.Equal(expectedRequest.StartAddress, request.StartAddress);
            Assert.Equal(expectedRequest.NumberOfPoints, request.NumberOfPoints);
        }

        [Fact]
        public void CreateModbusMessageReadHoldingRegistersRequestWithInvalidFrameSize()
        {
            Assert.Throws<FormatException>(() =>
                ModbusMessageFactory.CreateModbusMessage<ReadHoldingInputRegistersRequest>(new byte[]
                { 11, ModbusFunctionCodes.ReadHoldingRegisters, 0, 0, 5 }));
        }

        [Fact]
        public void CreateModbusMessageReadHoldingRegistersResponse()
        {
            ReadHoldingInputRegistersResponse response =
                ModbusMessageFactory.CreateModbusMessage<ReadHoldingInputRegistersResponse>(new byte[]
                { 11, ModbusFunctionCodes.ReadHoldingRegisters, 4, 0, 3, 0, 4 });
            var expectedResponse =
                new ReadHoldingInputRegistersResponse(ModbusFunctionCodes.ReadHoldingRegisters, 11, new RegisterCollection(3, 4));
            ModbusMessageFixture.AssertModbusMessagePropertiesAreEqual(expectedResponse, response);
        }

        [Fact]
        public void CreateModbusMessageReadHoldingRegistersResponseWithInvalidFrameSize()
        {
            Assert.Throws<FormatException>(() => ModbusMessageFactory.CreateModbusMessage<ReadHoldingInputRegistersResponse>(new byte[]
                { 11, ModbusFunctionCodes.ReadHoldingRegisters }));
        }

        [Fact]
        public void CreateModbusMessageSlaveExceptionResponse()
        {
            SlaveExceptionResponse response =
                ModbusMessageFactory.CreateModbusMessage<SlaveExceptionResponse>(new byte[] { 11, 129, 2 });
            var expectedException = new SlaveExceptionResponse(11,
                ModbusFunctionCodes.ReadCoils + Modbus.ExceptionOffset, 2);
            Assert.Equal(expectedException.FunctionCode, response.FunctionCode);
            Assert.Equal(expectedException.SlaveAddress, response.SlaveAddress);
            Assert.Equal(expectedException.MessageFrame, response.MessageFrame);
            Assert.Equal(expectedException.ProtocolDataUnit, response.ProtocolDataUnit);
        }

        [Fact]
        public void CreateModbusMessageSlaveExceptionResponseWithInvalidFunctionCode()
        {
            Assert.Throws<FormatException>(() =>
                ModbusMessageFactory.CreateModbusMessage<SlaveExceptionResponse>(new byte[] { 11, 128, 2 }));
        }

        [Fact]
        public void CreateModbusMessageSlaveExceptionResponseWithInvalidFrameSize()
        {
            Assert.Throws<FormatException>(() => ModbusMessageFactory.CreateModbusMessage<SlaveExceptionResponse>(new byte[] { 11, 128 }));
        }

        [Fact]
        public void CreateModbusMessageWriteSingleCoilRequestResponse()
        {
            WriteSingleCoilRequestResponse request =
                ModbusMessageFactory.CreateModbusMessage<WriteSingleCoilRequestResponse>(new byte[]
                { 17, ModbusFunctionCodes.WriteSingleCoil, 0, 172, byte.MaxValue, 0 });
            var expectedRequest = new WriteSingleCoilRequestResponse(17, 172, true);
            ModbusMessageFixture.AssertModbusMessagePropertiesAreEqual(expectedRequest, request);
            Assert.Equal(expectedRequest.StartAddress, request.StartAddress);
            Assert.Equal(expectedRequest.Data.NetworkBytes, request.Data.NetworkBytes);
        }

        [Fact]
        public void CreateModbusMessageWriteSingleCoilRequestResponseWithInvalidFrameSize()
        {
            Assert.Throws<FormatException>(() =>
                ModbusMessageFactory.CreateModbusMessage<WriteSingleCoilRequestResponse>(new byte[]
                { 11, ModbusFunctionCodes.WriteSingleCoil, 0, 105, byte.MaxValue }));
        }

        [Fact]
        public void CreateModbusMessageWriteSingleRegisterRequestResponse()
        {
            WriteSingleRegisterRequestResponse request =
                ModbusMessageFactory.CreateModbusMessage<WriteSingleRegisterRequestResponse>(new byte[]
                { 17, ModbusFunctionCodes.WriteSingleRegister, 0, 1, 0, 3 });
            var expectedRequest = new WriteSingleRegisterRequestResponse(17, 1, 3);
            ModbusMessageFixture.AssertModbusMessagePropertiesAreEqual(expectedRequest, request);
            Assert.Equal(expectedRequest.StartAddress, request.StartAddress);
            Assert.Equal(expectedRequest.Data.NetworkBytes, request.Data.NetworkBytes);
        }

        [Fact]
        public void CreateModbusMessageWriteSingleRegisterRequestResponseWithInvalidFrameSize()
        {
            Assert.Throws<FormatException>(() =>
                ModbusMessageFactory.CreateModbusMessage<WriteSingleRegisterRequestResponse>(new byte[]
                { 11, ModbusFunctionCodes.WriteSingleRegister, 0, 1, 0 }));
        }

        [Fact]
        public void CreateModbusMessageWriteMultipleRegistersRequest()
        {
            WriteMultipleRegistersRequest request =
                ModbusMessageFactory.CreateModbusMessage<WriteMultipleRegistersRequest>(new byte[]
                { 11, ModbusFunctionCodes.WriteMultipleRegisters, 0, 5, 0, 1, 2, 255, 255 });
            var expectedRequest = new WriteMultipleRegistersRequest(11, 5,
                new RegisterCollection(ushort.MaxValue));
            ModbusMessageFixture.AssertModbusMessagePropertiesAreEqual(expectedRequest, request);
            Assert.Equal(expectedRequest.StartAddress, request.StartAddress);
            Assert.Equal(expectedRequest.NumberOfPoints, request.NumberOfPoints);
            Assert.Equal(expectedRequest.ByteCount, request.ByteCount);
            Assert.Equal(expectedRequest.Data.NetworkBytes, request.Data.NetworkBytes);
        }

        [Fact]
        public void CreateModbusMessageWriteMultipleRegistersRequestWithInvalidFrameSize()
        {
            Assert.Throws<FormatException>(() =>
                ModbusMessageFactory.CreateModbusMessage<WriteMultipleRegistersRequest>(new byte[]
                { 11, ModbusFunctionCodes.WriteMultipleRegisters, 0, 5, 0, 1, 2 }));
        }

        [Fact]
        public void CreateModbusMessageWriteMultipleRegistersResponse()
        {
            WriteMultipleRegistersResponse response =
                ModbusMessageFactory.CreateModbusMessage<WriteMultipleRegistersResponse>(new byte[]
                { 17, ModbusFunctionCodes.WriteMultipleRegisters, 0, 1, 0, 2 });
            var expectedResponse = new WriteMultipleRegistersResponse(17, 1, 2);
            ModbusMessageFixture.AssertModbusMessagePropertiesAreEqual(expectedResponse, response);
            Assert.Equal(expectedResponse.StartAddress, response.StartAddress);
            Assert.Equal(expectedResponse.NumberOfPoints, response.NumberOfPoints);
        }

        [Fact]
        public void CreateModbusMessageWriteMultipleCoilsRequest()
        {
            WriteMultipleCoilsRequest request =
                ModbusMessageFactory.CreateModbusMessage<WriteMultipleCoilsRequest>(new byte[]
                { 17, ModbusFunctionCodes.WriteMultipleCoils, 0, 19, 0, 10, 2, 205, 1 });
            var expectedRequest = new WriteMultipleCoilsRequest(17, 19,
                new DiscreteCollection(true, false, true, true, false, false, true, true, true, false));
            ModbusMessageFixture.AssertModbusMessagePropertiesAreEqual(expectedRequest, request);
            Assert.Equal(expectedRequest.StartAddress, request.StartAddress);
            Assert.Equal(expectedRequest.NumberOfPoints, request.NumberOfPoints);
            Assert.Equal(expectedRequest.ByteCount, request.ByteCount);
            Assert.Equal(expectedRequest.Data.NetworkBytes, request.Data.NetworkBytes);
        }

        [Fact]
        public void CreateModbusMessageWriteMultipleCoilsRequestWithInvalidFrameSize()
        {
            Assert.Throws<FormatException>(() =>
                ModbusMessageFactory.CreateModbusMessage<WriteMultipleCoilsRequest>(new byte[]
                { 17, ModbusFunctionCodes.WriteMultipleCoils, 0, 19, 0, 10, 2 }));
        }

        [Fact]
        public void CreateModbusMessageWriteMultipleCoilsResponse()
        {
            WriteMultipleCoilsResponse response =
                ModbusMessageFactory.CreateModbusMessage<WriteMultipleCoilsResponse>(new byte[]
                { 17, ModbusFunctionCodes.WriteMultipleCoils, 0, 19, 0, 10 });
            var expectedResponse = new WriteMultipleCoilsResponse(17, 19, 10);
            ModbusMessageFixture.AssertModbusMessagePropertiesAreEqual(expectedResponse, response);
            Assert.Equal(expectedResponse.StartAddress, response.StartAddress);
            Assert.Equal(expectedResponse.NumberOfPoints, response.NumberOfPoints);
        }

        [Fact]
        public void CreateModbusMessageWriteMultipleCoilsResponseWithInvalidFrameSize()
        {
            Assert.Throws<FormatException>(() =>
                ModbusMessageFactory.CreateModbusMessage<WriteMultipleCoilsResponse>(new byte[]
                { 17, ModbusFunctionCodes.WriteMultipleCoils, 0, 19, 0 }));
        }

        [Fact]
        public void CreateModbusMessageReadWriteMultipleRegistersRequest()
        {
            ReadWriteMultipleRegistersRequest request = ModbusMessageFactory.CreateModbusMessage<ReadWriteMultipleRegistersRequest>(
                new byte[] { 0x05, 0x17, 0x00, 0x03, 0x00, 0x06, 0x00, 0x0e, 0x00, 0x03, 0x06, 0x00, 0xff, 0x00, 0xff, 0x00, 0xff });
            var writeCollection = new RegisterCollection(255, 255, 255);
            var expectedRequest = new ReadWriteMultipleRegistersRequest(5, 3, 6, 14, writeCollection);
            ModbusMessageFixture.AssertModbusMessagePropertiesAreEqual(expectedRequest, request);
        }

        [Fact]
        public void CreateModbusMessageReadWriteMultipleRegistersRequestWithInvalidFrameSize()
        {
            byte[] frame = { 17, ModbusFunctionCodes.ReadWriteMultipleRegisters, 1, 2, 3 };
            Assert.Throws<FormatException>(() =>
                ModbusMessageFactory.CreateModbusMessage<ReadWriteMultipleRegistersRequest>(frame));
        }

        [Fact]
        public void CreateModbusMessageReturnQueryDataRequestResponse()
        {
            const byte slaveAddress = 5;
            var data = new RegisterCollection(50);
            byte[] frame = new byte[] { slaveAddress, 8, 0, 0 }.Concat(data.NetworkBytes).ToArray();
            DiagnosticsRequestResponse message =
                ModbusMessageFactory.CreateModbusMessage<DiagnosticsRequestResponse>(frame);
            var expectedMessage =
                new DiagnosticsRequestResponse(ModbusFunctionCodes.DiagnosticsReturnQueryData, slaveAddress, data);

            Assert.Equal(expectedMessage.SubFunctionCode, message.SubFunctionCode);
            ModbusMessageFixture.AssertModbusMessagePropertiesAreEqual(expectedMessage, message);
        }

        [Fact]
        public void CreateModbusMessageReturnQueryDataRequestResponseTooSmall()
        {
            byte[] frame = new byte[] { 5, 8, 0, 0, 5 };
            Assert.Throws<FormatException>(() =>
                ModbusMessageFactory.CreateModbusMessage<DiagnosticsRequestResponse>(frame));
        }

        //[Fact]
        //public void CreateModbusRequestWithInvalidMessageFrame()
        //{
        //    Assert.Throws<FormatException>(() => ModbusMessageFactory.CreateModbusRequest(new byte[] { 0, 1 }));
        //}

        //[Fact]
        //public void CreateModbusRequestWithInvalidFunctionCode()
        //{
        //    Assert.Throws<ArgumentException>(() => ModbusMessageFactory.CreateModbusRequest(new byte[] { 1, 99, 0, 0, 0, 1, 23 }));
        //}

        //[Fact]
        //public void CreateModbusRequestForReadCoils()
        //{
        //    ReadCoilsInputsRequest req = new ReadCoilsInputsRequest(1, 2, 1, 10);
        //    IModbusMessage request = ModbusMessageFactory.CreateModbusRequest(req.MessageFrame);
        //    Assert.Equal(typeof(ReadCoilsInputsRequest), request.GetType());
        //}

        //[Fact]
        //public void CreateModbusRequestForDiagnostics()
        //{
        //    DiagnosticsRequestResponse diagnosticsRequest = new DiagnosticsRequestResponse(0, 2,
        //        new RegisterCollection(45));
        //    IModbusMessage request = ModbusMessageFactory.CreateModbusRequest(diagnosticsRequest.MessageFrame);
        //    Assert.Equal(typeof(DiagnosticsRequestResponse), request.GetType());
        //}
    }
}