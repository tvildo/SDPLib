using SDPLib;
using SDPLib.Serializers;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace TestSDPLib.Serializers
{
    public class ConnectionDataSerializerTests
    {

        [Fact]
        public async Task CanSerialize()
        {
            var expected = $"c=IN IP4 224.2.36.42/127{SDPLib.SDPSerializer.CRLF}".ToByteArray();

            var pipe = new Pipe();
            var value = new ConnectionData()
            {
                Nettype = "IN",
                AddrType = "IP4",
                ConnectionAddress = "224.2.36.42/127"
            };

            ConnectionDataSerializer.Instance.WriteValue(pipe.Writer, value);
            pipe.Writer.Complete();

            var serialized = (await pipe.Reader.ReadAsync()).Buffer.ToArray();

            Assert.Equal(expected, serialized);
        }

        [Fact]
        public void CanDeSerialize()
        {
            var field = $"c=IN IP4 224.2.36.42/127".ToByteArray();
            var result = ConnectionDataSerializer.Instance.ReadValue(field);

            var expected = new ConnectionData()
            {
                Nettype = "IN",
                AddrType = "IP4",
                ConnectionAddress = "224.2.36.42/127"
            };

            Assert.True(CheckIfConnectionDatasAreSame(expected, result));
        }

        private bool CheckIfConnectionDatasAreSame(ConnectionData a, ConnectionData b)
        {
            return a.AddrType == b.AddrType
                && a.ConnectionAddress == b.ConnectionAddress
                && a.Nettype == b.Nettype;
        }
    }
}
