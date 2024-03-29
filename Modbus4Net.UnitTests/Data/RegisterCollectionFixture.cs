﻿using Modbus4Net.Data;
using Xunit;

namespace Modbus4Net.UnitTests.Data
{
    public class RegisterCollectionFixture
    {
        [Fact]
        public void ByteCount()
        {
            var col = new RegisterCollection(1, 2, 3);
            Assert.Equal(6, col.ByteCount);
        }

        [Fact]
        public void NewRegisterCollection()
        {
            var col = new RegisterCollection(5, 3, 4, 6);
            Assert.NotNull(col);
            Assert.Equal(4, col.Count);
            Assert.Equal(5, col[0]);
        }

        [Fact]
        public void NewRegisterCollectionFromBytes()
        {
            var col = new RegisterCollection(new byte[] { 0, 1, 0, 2, 0, 3 });
            Assert.NotNull(col);
            Assert.Equal(3, col.Count);
            Assert.Equal(1, col[0]);
            Assert.Equal(2, col[1]);
            Assert.Equal(3, col[2]);
        }

        [Fact]
        public void RegisterCollectionNetworkBytes()
        {
            var col = new RegisterCollection(5, 3, 4, 6);
            byte[] bytes = col.NetworkBytes;
            Assert.NotNull(bytes);
            Assert.Equal(8, bytes.Length);
            Assert.Equal(new byte[] { 0, 5, 0, 3, 0, 4, 0, 6 }, bytes);
        }

        [Fact]
        public void RegisterCollectionEmpty()
        {
            var col = new RegisterCollection();
            Assert.NotNull(col);
            Assert.Equal(0, col.NetworkBytes.Length);
        }

        [Fact]
        public void ModifyRegister()
        {
            var col = new RegisterCollection(1, 2, 3, 4);
            col[0] = 5;
        }

        [Fact]
        public void AddRegister()
        {
            var col = new RegisterCollection();
            Assert.Equal(0, col.Count);
            col.Add(45);
            Assert.Equal(1, col.Count);
        }

        [Fact]
        public void RemoveRegister()
        {
            var col = new RegisterCollection(3, 4, 5);
            Assert.Equal(3, col.Count);
            col.RemoveAt(2);
            Assert.Equal(2, col.Count);
        }
    }
}