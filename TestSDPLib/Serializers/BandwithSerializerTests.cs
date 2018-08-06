using SDPLib;
using SDPLib.Serializers;
using System.Buffers;
using System.IO.Pipelines;
using System.Threading.Tasks;
using Xunit;

namespace TestSDPLib.Serializers
{
    public class BandwithSerializerTests
    {
        [Fact]
        public async Task CanSerialize()
        {
            var expected = $"b=X-YZ:128{SDPLib.SDPSerializer.CRLF}".ToByteArray();

            var pipe = new Pipe();
            var value = new Bandwidth()
            {
                Type = "X-YZ",
                Value = "128",
            };

            BandwithSerializer.Instance.WriteValue(pipe.Writer, value);
            pipe.Writer.Complete();

            var serialized = (await pipe.Reader.ReadAsync()).Buffer.ToArray();

            Assert.Equal(expected, serialized);
        }

        [Fact]
        public void CanDeSerialize()
        {
            var field = $"b=X-YZ:128".ToByteArray();
            var result = BandwithSerializer.Instance.ReadValue(field);

            var expected = new Bandwidth()
            {
                Type = "X-YZ",
                Value = "128",
            };

            Assert.True(CheckIfAreSame(expected, result));
        }

        private bool CheckIfAreSame(Bandwidth a, Bandwidth b)
        {
            return a.Type == b.Type
                && a.Value == b.Value;
        }
    }
}
