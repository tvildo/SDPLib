using SDPLib.Serializers;
using System.Buffers;
using System.IO.Pipelines;
using System.Threading.Tasks;
using Xunit;

namespace TestSDPLib.Serializers
{
    public class SessionInformationSerializerTests
    {
        [Fact]
        public void CanDeSerialize()
        {
            var session = new DeserializationSession() { ParsedValue = new SDPLib.SDP() };
            var version = SessionInformationSerializer.Instance.ReadValue("i=Gamarjoba Sakartvelo".ToByteArray(), session);
            Assert.Equal("Gamarjoba Sakartvelo".ToByteArray(), session.ParsedValue.SessionInformation);
        }


        [Fact]
        public async Task CanSerialize()
        {
            var expected = $"i=Gamarjoba Sakartvelo{SDPLib.SDPSerializer.CRLF}".ToByteArray();
            var value = "Gamarjoba Sakartvelo".ToByteArray();
            var pipe = new Pipe();
            SessionInformationSerializer.Instance.WriteValue(pipe.Writer, value);
            pipe.Writer.Complete();
            var serialized = (await pipe.Reader.ReadAsync()).Buffer.ToArray();
            Assert.Equal(expected, serialized);
        }
    }
}
