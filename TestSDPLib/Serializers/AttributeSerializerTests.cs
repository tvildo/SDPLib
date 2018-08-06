using SDPLib.Serializers;
using System.Buffers;
using System.IO.Pipelines;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace TestSDPLib.Serializers
{
    public class AttributeSerializerTests
    {
        [Fact]
        public void CanDeSerialize()
        {
            var expected = "ssrc:102545776 label:45d68f81-63c5-45e1-8b2a-aacd80691824";
            var parsed = AttributeSerializer.Instance.ReadValue($"a={expected}".ToByteArray());
            Assert.Equal(expected, parsed);
        }

        [Fact]
        public async Task CanSerialize()
        {
            var value = "ssrc:102545776 label:45d68f81-63c5-45e1-8b2a-aacd80691824";
            var expected = $"a=ssrc:102545776 label:45d68f81-63c5-45e1-8b2a-aacd80691824{SDPLib.SDPSerializer.CRLF}".ToByteArray();
            var pipe = new Pipe();
            AttributeSerializer.Instance.WriteValue(pipe.Writer, value);
            pipe.Writer.Complete();
            var serialized = (await pipe.Reader.ReadAsync()).Buffer.ToArray();
            Assert.Equal(expected, serialized);
        }
    }
}
