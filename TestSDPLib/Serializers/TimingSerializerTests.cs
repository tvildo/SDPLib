using SDPLib;
using SDPLib.Serializers;
using System.Buffers;
using System.IO.Pipelines;
using System.Threading.Tasks;
using Xunit;

namespace TestSDPLib.Serializers
{
    public class TimingSerializerTests
    {
        [Fact]
        public async Task CanSerialize()
        {
            var expected = $"t=3034423619 3042462419{SDPLib.SDPSerializer.CRLF}".ToByteArray();

            var pipe = new Pipe();
            var value = new TimingInfo()
            {
                StartTime = 3034423619,
                StopTime = 3042462419
            };

            TimingSerializer.Instance.WriteValue(pipe.Writer, value);
            pipe.Writer.Complete();

            var serialized = (await pipe.Reader.ReadAsync()).Buffer.ToArray();

            Assert.Equal(expected, serialized);
        }

        [Fact]
        public void CanDeSerialize()
        {
            var field = $"t=3034423619 3042462419".ToByteArray();
            var result = TimingSerializer.Instance.ReadValue(field);

            var expected = new TimingInfo()
            {
                StartTime = 3034423619,
                StopTime = 3042462419
            };

            Assert.True(CheckIfAreSame(expected, result));
        }

        private bool CheckIfAreSame(TimingInfo a, TimingInfo b)
        {
            return a.StartTime == b.StartTime
                && a.StopTime == b.StopTime;
        }
    }
}
