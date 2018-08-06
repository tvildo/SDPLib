using SDPLib.Serializers;
using System.Buffers;
using System.IO.Pipelines;
using System.Threading.Tasks;
using Xunit;

namespace TestSDPLib.Serializers
{
    public class VersionSerializerTests
    {
        [Fact]
        public void CanDeSerialize()
        {
            var version = VersionSerializer.Instance.ReadValue("v=0".ToByteArray());
            Assert.Equal(0, version);
        }

        [Fact]
        public void EnsureVersionIsInteger()
        {
            Assert.Throws<DeserializationException>(() => VersionSerializer.Instance.ReadValue("v=a".ToByteArray()));
        }

        [Fact]
        public async Task CanSerialize()
        {
            var pipe = new Pipe();
            VersionSerializer.Instance.WriteValue(pipe.Writer, 0);
            pipe.Writer.Complete();

            var result = (await pipe.Reader.ReadAsync()).Buffer;
            Assert.Equal($"v=0{SDPLib.SDPSerializer.CRLF}".ToByteArray(), result.ToArray());
        }
    }
}
