using SDPLib.Serializers;
using System;
using System.Buffers;
using System.IO.Pipelines;
using System.Threading.Tasks;
using Xunit;

namespace TestSDPLib.Serializers
{
    public class SessionNameSerializerTests
    {
        [Fact]
        public void CanDeSerialize()
        {
            var session = new DeserializationSession() { ParsedValue = new SDPLib.SDP() };
            var version = SessionNameSerializer.Instance.ReadValue("s=Gamarjoba".ToByteArray(), session);
            Assert.Equal("Gamarjoba".ToByteArray(), session.ParsedValue.SessionName);
        }


        [Fact]
        public async Task CanSerialize()
        {
            var expected = $"s=Gamarjoba{SDPLib.SDPSerializer.CRLF}".ToByteArray();
            var value = "Gamarjoba".ToByteArray();

            var pipe = new Pipe();

            SessionNameSerializer.Instance.WriteValue(pipe.Writer, value);
            pipe.Writer.Complete();

            var serialized = (await pipe.Reader.ReadAsync()).Buffer.ToArray();

            Assert.Equal(expected, serialized);
        }

        [Fact]
        public async Task CanSerializeEmptySessionName()
        {
            var expected = $"s= {SDPLib.SDPSerializer.CRLF}".ToByteArray();
            var value = "Gamarjoba".ToByteArray();

            var pipe = new Pipe();

            SessionNameSerializer.Instance.WriteValue(pipe.Writer, ReadOnlySpan<byte>.Empty);
            pipe.Writer.Complete();

            var serialized = (await pipe.Reader.ReadAsync()).Buffer.ToArray();

            Assert.Equal(expected, serialized);
        }
    }
}
