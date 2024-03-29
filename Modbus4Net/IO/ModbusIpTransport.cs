﻿using Modbus4Net.Logging;
using Modbus4Net.Unme.Common;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Modbus4Net.IO
{
    /// <summary>
    /// Transport for Internet protocols.
    /// </summary>
    public class ModbusIpTransport : ModbusTransport
    {
        private static readonly object _transactionIdLock = new object();
        private ushort _transactionId;

        public ModbusIpTransport(IStreamResource streamResource, IModbusFactory modbusFactory, IModbusLogger logger)
            : base(streamResource, modbusFactory, logger)
        {
            if (streamResource == null)
                throw new ArgumentNullException(nameof(streamResource));
        }

        public static byte[] ReadRequestResponse(IStreamResource streamResource, IModbusLogger logger)
        {
            if (streamResource == null)
                throw new ArgumentNullException(nameof(streamResource));
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            // read header
            byte[] mbapHeader = new byte[6];
            int numBytesRead = 0;

            while (numBytesRead != 6)
            {
                int bRead = streamResource.Read(mbapHeader, numBytesRead, 6 - numBytesRead);

                if (bRead == 0)
                    throw new IOException("Read resulted in 0 bytes returned.");

                numBytesRead += bRead;
            }

            logger.Debug($"MBAP header: {string.Join(", ", mbapHeader)}");
            ushort frameLength = (ushort)IPAddress.HostToNetworkOrder(BitConverter.ToInt16(mbapHeader, 4));
            logger.Debug($"{frameLength} bytes in PDU.");

            // read message
            byte[] messageFrame = new byte[frameLength];
            numBytesRead = 0;

            while (numBytesRead != frameLength)
            {
                int bRead = streamResource.Read(messageFrame, numBytesRead, frameLength - numBytesRead);

                if (bRead == 0)
                    throw new IOException("Read resulted in 0 bytes returned.");

                numBytesRead += bRead;
            }

            logger.Debug($"PDU: {frameLength}");
            byte[] frame = mbapHeader.Concat(messageFrame).ToArray();
            logger.Debug($"RX: {string.Join(", ", frame)}");

            return frame;
        }

        public static async Task<byte[]> ReadRequestResponseAsync(IStreamResource streamResource, IModbusLogger logger)
        {
            if (streamResource == null)
                throw new ArgumentNullException(nameof(streamResource));
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));
            
            byte[] mbapHeader = await ReadHeaderAsync(streamResource);
            logger.Debug($"MBAP header: {string.Join(", ", mbapHeader)}");

            ushort frameLength = (ushort)IPAddress.HostToNetworkOrder(BitConverter.ToInt16(mbapHeader, 4));
            logger.Debug($"{frameLength} bytes in PDU.");
            
            byte[] messageFrame = await ReadMessageAsync(streamResource, frameLength);
            logger.Debug($"PDU: {frameLength}");

            byte[] frame = mbapHeader.Concat(messageFrame).ToArray();
            logger.Debug($"RX: {string.Join(", ", frame)}");

            return frame;
        }

        private static async Task<byte[]> ReadHeaderAsync(IStreamResource streamResource)
        {
            byte[] mbapHeader = new byte[6];
            int numBytesRead = 0;

            while (numBytesRead != 6)
            {
                int bRead = await streamResource.ReadAsync(mbapHeader, numBytesRead, 6 - numBytesRead);

                if (bRead == 0)
                    throw new IOException("Read resulted in 0 bytes returned.");

                numBytesRead += bRead;
            }

            return mbapHeader;
        }

        private static async Task<byte[]> ReadMessageAsync(IStreamResource streamResource, ushort frameLength)
        {
            byte[] messageFrame = new byte[frameLength];
            int numBytesRead = 0;

            while (numBytesRead != frameLength)
            {
                int bRead = await streamResource.ReadAsync(messageFrame, numBytesRead, frameLength - numBytesRead);

                if (bRead == 0)
                    throw new IOException("Read resulted in 0 bytes returned.");

                numBytesRead += bRead;
            }

            return messageFrame;
        }

        public static byte[] GetMbapHeader(IModbusMessage message)
        {
            byte[] transactionId = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)message.TransactionId));
            byte[] length = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)(message.ProtocolDataUnit.Length + 1)));

            var stream = new MemoryStream(7);
            stream.Write(transactionId, 0, transactionId.Length);
            stream.WriteByte(0);
            stream.WriteByte(0);
            stream.Write(length, 0, length.Length);
            stream.WriteByte(message.SlaveAddress);

            return stream.ToArray();
        }

        /// <summary>
        /// Create a new transaction ID.
        /// </summary>
        public virtual ushort GetNewTransactionId()
        {
            lock (_transactionIdLock)
            {
                _transactionId = _transactionId == ushort.MaxValue ? (ushort)1 : ++_transactionId;
            }

            return _transactionId;
        }

        public IModbusMessage CreateMessageAndInitializeTransactionId<T>(byte[] fullFrame)
            where T : IModbusMessage, new()
        {
            byte[] mbapHeader = fullFrame.Slice(0, 6).ToArray();
            byte[] messageFrame = fullFrame.Slice(6, fullFrame.Length - 6).ToArray();

            IModbusMessage response = CreateResponse<T>(messageFrame);
            response.TransactionId = (ushort)IPAddress.NetworkToHostOrder(BitConverter.ToInt16(mbapHeader, 0));

            return response;
        }

        public override byte[] BuildMessageFrame(IModbusMessage message)
        {
            byte[] header = GetMbapHeader(message);
            byte[] pdu = message.ProtocolDataUnit;
            var messageBody = new MemoryStream(header.Length + pdu.Length);

            messageBody.Write(header, 0, header.Length);
            messageBody.Write(pdu, 0, pdu.Length);

            return messageBody.ToArray();
        }

        public override void Write(IModbusMessage message)
        {
            message.TransactionId = GetNewTransactionId();
            byte[] frame = BuildMessageFrame(message); // what is the point of this being abstract?

            Logger.LogFrameTx(frame);

            StreamResource.Write(frame, 0, frame.Length);
        }

        public override Task WriteAsync(IModbusMessage message)
        {
            message.TransactionId = GetNewTransactionId();
            byte[] frame = BuildMessageFrame(message);

            Logger.LogFrameTx(frame);

            return StreamResource.WriteAsync(frame, 0, frame.Length);
        }

        public override byte[] ReadRequest()
        {
            return ReadRequestResponse(StreamResource, Logger);
        }

        public override Task<byte[]> ReadRequestAsync()
        {
            return ReadRequestResponseAsync(StreamResource, Logger);
        }

        public override IModbusMessage ReadResponse<T>()
        {
            return CreateMessageAndInitializeTransactionId<T>(ReadRequestResponse(StreamResource, Logger));
        }

        public override async Task<IModbusMessage> ReadResponseAsync<T>()
        {
            byte[] response = await ReadRequestResponseAsync(StreamResource, Logger);
            return CreateMessageAndInitializeTransactionId<T>(response);
        }

        public override void OnValidateResponse(IModbusMessage request, IModbusMessage response)
        {
            if (request.TransactionId != response.TransactionId)
            {
                string msg = $"Response was not of expected transaction ID. Expected {request.TransactionId}, received {response.TransactionId}.";
                throw new IOException(msg);
            }
        }

        public override bool OnShouldRetryResponse(IModbusMessage request, IModbusMessage response)
        {
            if (request.TransactionId > response.TransactionId && request.TransactionId - response.TransactionId < RetryOnOldResponseThreshold)
            {
                // This response was from a previous request
                return true;
            }

            return base.OnShouldRetryResponse(request, response);
        }
    }
}
