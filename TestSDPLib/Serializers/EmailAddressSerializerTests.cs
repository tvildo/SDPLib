using SDPLib.Serializers;
using System.Buffers;
using System.IO.Pipelines;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace TestSDPLib.Serializers
{
    public class EmailAddressSerializerTests
    {
        [Fact]
        public void CanDeSerialize()
        {
            var testEmail = "j.doe@example.com (Jane Doe)";
            var session = new DeserializationSession() { ParsedValue = new SDPLib.SDP() };
            var version = EmailAddressSerializer.Instance.ReadValue($"e={testEmail}".ToByteArray(), session);
            Assert.Equal(testEmail, session.ParsedValue.EmailNumbers.First());
        }

        [Fact]
        public async Task CanSerialize()
        {
            var testEmail = "j.doe@example.com (Jane Doe)";
            var expected = $"e={testEmail}{SDPLib.SDPSerializer.CRLF}".ToByteArray();
            var pipe = new Pipe();
            EmailAddressSerializer.Instance.WriteValue(pipe.Writer, testEmail);
            pipe.Writer.Complete();
            var serialized = (await pipe.Reader.ReadAsync()).Buffer.ToArray();
            Assert.Equal(expected, serialized);
        }
    }
}
