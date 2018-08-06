using SDPLib.Serializers;
using System;
using System.Buffers;
using System.IO.Pipelines;
using System.Threading.Tasks;
using Xunit;

namespace TestSDPLib.Serializers
{
    public class UriSerializerTests
    {
        [Fact]
        public void CanDeSerialize()
        {
            var testUrl = "https://github.com/tvildo";
            var session = new DeserializationSession() { ParsedValue = new SDPLib.SDP() };
            var version = UriSerializer.Instance.ReadValue($"u={testUrl}".ToByteArray(), session);
            Assert.Equal(testUrl, session.ParsedValue.Uri.AbsoluteUri);
        }

        [Fact]
        public async Task CanSerialize()
        {
            var testUrl = "https://github.com/tvildo";
            var expected = $"u={testUrl}{SDPLib.SDPSerializer.CRLF}".ToByteArray();
            var pipe = new Pipe();
            UriSerializer.Instance.WriteValue(pipe.Writer, new Uri(testUrl));
            pipe.Writer.Complete();
            var serialized = (await pipe.Reader.ReadAsync()).Buffer.ToArray();
            Assert.Equal(expected, serialized);
        }
    }
}
