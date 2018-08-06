using SDPLib;
using SDPLib.Serializers;
using System;
using System.Buffers;
using System.IO.Pipelines;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace TestSDPLib.Serializers
{
    public class EncriptionKeySerializerTests
    {
        [Fact]
        public async Task CanSerialize()
        {
            var expected = $"k=clear:secretkey{SDPLib.SDPSerializer.CRLF}".ToByteArray();

            var pipe = new Pipe();
            var value = new EncriptionKey()
            {
                Method = EncriptionKey.ClearMethod,
                Value = "secretkey"
            };

            EncriptionKeySerializer.Instance.WriteValue(pipe.Writer, value);
            pipe.Writer.Complete();

            var serialized = (await pipe.Reader.ReadAsync()).Buffer.ToArray();

            Assert.Equal(expected, serialized);
        }

        [Fact]
        public void CanDeSerialize()
        {
            var field = "k=clear:secretkey".ToByteArray();
            var result = EncriptionKeySerializer.Instance.ReadValue(field);

            var expected = new EncriptionKey()
            {
                Method = EncriptionKey.ClearMethod,
                Value = "secretkey"
            };

            Assert.True(CheckIfConnectionDatasAreSame(expected, result));
        }

        private bool CheckIfConnectionDatasAreSame(EncriptionKey expected, EncriptionKey result)
        {
            return expected.Method == result.Method
                && expected.Value == result.Value;
        }
    }
}
