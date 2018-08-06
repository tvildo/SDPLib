using SDPLib;
using SDPLib.Serializers;
using System.Buffers;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Threading.Tasks;
using Xunit;
using System.Linq;

namespace TestSDPLib.Serializers
{
    public class MediaSerializerTests
    {
        [Fact]
        public async Task CanSerialize()
        {
            var expected = $"m=video 49170/2 RTP/AVP 31 35{SDPLib.SDPSerializer.CRLF}".ToByteArray();

            var pipe = new Pipe();
            var value = new MediaDescription()
            {
                Media = "video",
                Port = "49170/2",
                Proto = "RTP/AVP",
                Fmts = new List<string>()
                {
                    "31", "35"
                }
            };

            MediaSerializer.Instance.WriteValue(pipe.Writer, value);
            pipe.Writer.Complete();

            var serialized = (await pipe.Reader.ReadAsync()).Buffer.ToArray();

            Assert.Equal(expected, serialized);
        }

        [Fact]
        public void CanDeSerialize()
        {
            var field = $"m=video 49170/2 RTP/AVP 31 35".ToByteArray();
            var value = MediaSerializer.Instance.ReadValue(field);

            var expected = new MediaDescription()
            {
                Media = "video",
                Port = "49170/2",
                Proto = "RTP/AVP",
                Fmts = new List<string>()
                {
                    "31", "35"
                }
            };

            Assert.True(CheckIfOriginSareSame(expected, value));
        }

        private bool CheckIfOriginSareSame(MediaDescription a, MediaDescription b)
        {
            return a.Media == b.Media
                 && a.Port == b.Port
                 && a.Proto == b.Proto
                 && a.Fmts.SequenceEqual(b.Fmts);
        }
    }
}
