﻿using Modbus4Net.Device;
using System;
using Xunit;

namespace Modbus4Net.UnitTests.Device
{
    public class TcpConnectionEventArgsFixture
    {
        [Fact]
        public void TcpConnectionEventArgs_NullEndPoint()
        {
            Assert.Throws<ArgumentNullException>(() => new TcpConnectionEventArgs(null));
        }

        [Fact]
        public void TcpConnectionEventArgs_EmptyEndPoint()
        {
            Assert.Throws<ArgumentException>(() => new TcpConnectionEventArgs(string.Empty));
        }

        [Fact]
        public void TcpConnectionEventArgs()
        {
            var args = new TcpConnectionEventArgs("foo");

            Assert.Equal("foo", args.EndPoint);
        }
    }
}