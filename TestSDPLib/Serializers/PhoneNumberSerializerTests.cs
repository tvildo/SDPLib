using SDPLib.Serializers;
using System.Buffers;
using System.IO.Pipelines;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace TestSDPLib.Serializers
{
    public class PhoneNumberSerializerTests
    {
        [Fact]
        public void CanDeSerialize()
        {
            var testPhone = "+1 617 555-6011";
            var session = new DeserializationSession() { ParsedValue = new SDPLib.SDP() };
            var version = PhoneNumberSerializer.Instance.ReadValue($"p={testPhone}".ToByteArray(), session);
            Assert.Equal(testPhone, session.ParsedValue.PhoneNumbers.First());
        }

        [Fact]
        public async Task CanSerialize()
        {
            var testPhone = "+1 617 555-6011";
            var expected = $"p={testPhone}{SDPLib.SDPSerializer.CRLF}".ToByteArray();
            var pipe = new Pipe();
            PhoneNumberSerializer.Instance.WriteValue(pipe.Writer, testPhone);
            pipe.Writer.Complete();
            var serialized = (await pipe.Reader.ReadAsync()).Buffer.ToArray();
            Assert.Equal(expected, serialized);
        }
    }
}
