using SDPLib;
using SDPLib.Serializers;
using System.Buffers;
using System.IO.Pipelines;
using System.Threading.Tasks;
using Xunit;

namespace TestSDPLib.Serializers
{
    public class OriginSerializerTests
    {
        [Fact]
        public async Task CanSerialize()
        {
            var expected = $"o=jdoe 2890844526 2890842807 IN IP4 10.47.16.5{SDPLib.SDPSerializer.CRLF}".ToByteArray();

            var pipe = new Pipe();
            var value = new Origin()
            {
                UserName = "jdoe",
                SessionId = 2890844526,
                SessionVersion = 2890842807,
                Nettype = "IN",
                AddrType = "IP4",
                UnicastAddress = "10.47.16.5"
            };

            OriginSerializer.Instance.WriteValue(pipe.Writer, value);
            pipe.Writer.Complete();

            var serialized = (await pipe.Reader.ReadAsync()).Buffer.ToArray();

            Assert.Equal(expected, serialized);
        }

        [Fact]
        public async Task CanSerializeEmptyUsername()
        {
            var expected = $"o=- 2890844526 2890842807 IN IP4 10.47.16.5{SDPLib.SDPSerializer.CRLF}".ToByteArray();

            var pipe = new Pipe();
            var value = new Origin()
            {
                SessionId = 2890844526,
                SessionVersion = 2890842807,
                Nettype = "IN",
                AddrType = "IP4",
                UnicastAddress = "10.47.16.5"
            };

            OriginSerializer.Instance.WriteValue(pipe.Writer, value);
            pipe.Writer.Complete();

            var serialized = (await pipe.Reader.ReadAsync()).Buffer.ToArray();

            Assert.Equal(expected, serialized);
        }

        [Fact]
        public void CanDeSerialize()
        {
            var field = $"o=jdoe 2890844526 2890842807 IN IP4 10.47.16.5".ToByteArray();
            var session = new DeserializationSession() { ParsedValue = new SDP() };
            var stateFn = OriginSerializer.Instance.ReadValue(field, session );

            var value = new Origin()
            {
                UserName = "jdoe",
                SessionId = 2890844526,
                SessionVersion = 2890842807,
                Nettype = "IN",
                AddrType = "IP4",
                UnicastAddress = "10.47.16.5"
            };

            DeserializationState nextStateFn = SessionNameSerializer.Instance.ReadValue;
            Assert.Equal(stateFn, nextStateFn);
            Assert.True(CheckIfOriginSareSame(session.ParsedValue.Origin, value));
        }

        private bool CheckIfOriginSareSame(Origin a, Origin b)
        {
            var areEqual = a.UserName == b.UserName
                && a.SessionId == b.SessionId
                && a.SessionVersion == b.SessionVersion
                && a.Nettype == b.Nettype
                && a.AddrType == b.AddrType
                && a.UnicastAddress == b.UnicastAddress;

            return areEqual;
        }
    }
}
