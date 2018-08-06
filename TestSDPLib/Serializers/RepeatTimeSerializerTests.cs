using SDPLib;
using SDPLib.Serializers;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace TestSDPLib.Serializers
{
    public class RepeatTimeSerializerTests
    {
        [Fact]
        public void CanDeSerialize()
        {
            var testRepeatTime = $"r=604800 3600 0 90000";
            var expected = new RepeatTime()
            {
                RepeatInterval = TimeSpan.FromSeconds(604800),
                ActiveDuration = TimeSpan.FromSeconds(3600),
                OffsetsFromStartTime = new List<TimeSpan>()
                {
                    TimeSpan.FromSeconds(0),
                    TimeSpan.FromSeconds(90000),
                }
            };
            var value = RepeatTimeSerializer.Instance.ReadValue(testRepeatTime.ToByteArray());
            Assert.True(CheckIfOriginSareSame(expected, value));
        }

        [Fact]
        public async Task CanSerialize()
        {
            var expected = $"r=604800 3600 0 90000{SDPLib.SDPSerializer.CRLF}".ToByteArray();

            var pipe = new Pipe();
            var value = new RepeatTime()
            {
                RepeatInterval = TimeSpan.FromSeconds(604800),
                ActiveDuration = TimeSpan.FromSeconds(3600),
                OffsetsFromStartTime = new List<TimeSpan>()
                {
                    TimeSpan.FromSeconds(0),
                    TimeSpan.FromSeconds(90000),
                }
            };

            RepeatTimeSerializer.Instance.WriteValue(pipe.Writer, value);
            pipe.Writer.Complete();

            var serialized = (await pipe.Reader.ReadAsync()).Buffer.ToArray();

            Assert.Equal(expected, serialized);
        }

        private bool CheckIfOriginSareSame(RepeatTime expected, RepeatTime value)
        {
            return expected.ActiveDuration == value.ActiveDuration
                && expected.RepeatInterval == value.RepeatInterval
                && expected.OffsetsFromStartTime.SequenceEqual(value.OffsetsFromStartTime);
        }
    }
}
