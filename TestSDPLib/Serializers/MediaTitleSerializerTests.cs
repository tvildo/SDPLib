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
    public class MediaTitleSerializerTests
    {
        [Fact]
        public void CanDeSerialize()
        {
            var info = MediaTitleSerializer.Instance.ReadValue("i=Gamarjoba Sakartvelo".ToByteArray());
            Assert.Equal("Gamarjoba Sakartvelo".ToByteArray(), info);
        }


        [Fact]
        public async Task CanSerialize()
        {
            var expected = $"i=Gamarjoba Sakartvelo{SDPLib.SDPSerializer.CRLF}".ToByteArray();
            var value = "Gamarjoba Sakartvelo".ToByteArray();
            var pipe = new Pipe();
            MediaTitleSerializer.Instance.WriteValue(pipe.Writer, value);
            pipe.Writer.Complete();
            var serialized = (await pipe.Reader.ReadAsync()).Buffer.ToArray();
            Assert.Equal(expected, serialized);
        }
    }
}
