﻿using System;
using System.Threading.Tasks;

namespace Modbus4Net.IO
{
    /// <summary>
    /// Represents a serial resource.
    /// Implementor - http://en.wikipedia.org/wiki/Bridge_Pattern
    /// </summary>
    public interface IStreamResource : IDisposable
    {
        /// <summary>
        /// Forms a connection.
        /// </summary>
        void Connect();

        /// <summary>
        /// Removes the connection.
        /// </summary>
        void Disconnect();

        /// <summary>
        /// Connection state.
        /// </summary>
        bool Connected { get; }

        /// <summary>
        /// Indicates that no timeout should occur.
        /// </summary>
        int InfiniteTimeout { get; }

        /// <summary>
        /// Gets or sets the number of milliseconds before a timeout occurs when a read operation does not finish.
        /// </summary>
        int ReadTimeout { get; set; }

        /// <summary>
        /// Gets or sets the number of milliseconds before a timeout occurs when a write operation does not finish.
        /// </summary>
        int WriteTimeout { get; set; }

        /// <summary>
        /// Purges the receive buffer.
        /// </summary>
        void DiscardInBuffer();

        /// <summary>
        /// Reads a number of bytes from the input buffer and writes those bytes into a byte array at the specified offset.
        /// </summary>
        /// <param name="buffer">The byte array to write the input to.</param>
        /// <param name="offset">The offset in the buffer array to begin writing.</param>
        /// <param name="count">The number of bytes to read.</param>
        /// <returns>The number of bytes read.</returns>
        int Read(byte[] buffer, int offset, int count);

        Task<int> ReadAsync(byte[] buffer, int offset, int count);

        /// <summary>
        /// Writes a specified number of bytes to the port from an output buffer, starting at the specified offset.
        /// </summary>
        /// <param name="buffer">The byte array that contains the data to write to the port.</param>
        /// <param name="offset">The offset in the buffer array to begin writing.</param>
        /// <param name="count">The number of bytes to write.</param>
        void Write(byte[] buffer, int offset, int count);

        Task WriteAsync(byte[] buffer, int offset, int count);
    }
}
