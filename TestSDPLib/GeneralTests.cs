using SDPLib;
using System.IO.Pipelines;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace TestSDPLib
{
    public class GeneralTests
    {
        [Fact]
        public void ShouldParseAndSerializeSameWay()
        {
            var expected = @"v=0
o=- 770984055657151438 2 IN IP4 127.0.0.1
s=-
i=Test title
u=http://www.example.com/seminars/sdp.pdf
a=charset:ISO-8859-1
a=group:BUNDLE audio video
a=msid-semantic: WMS 36deb484-3e8c-40da-a9af-381de5813df8 3dbb5da7-1894-4cd6-be71-f8e957274044 44850aa4-29e7-468a-855f-00b739a8fd7c 527d1bfc-d258-4115-988e-70c94ccc0f37 71706457-490a-47e8-b25a-77e025c40a2b aba4cac8-87de-4420-b087-8935eb8d4669 c41fe501-ca69-44cd-8ea4-3188e98759a7 e1392574-bd98-46b3-9e37-7a647eaf24ad
t=0 0
r=604800 3600 0 90000
m=audio 9 UDP/TLS/RTP/SAVPF 111 103 104 9 0 8 106 105 13 110 112 113 126
i=This is media title
a=Test
m=video 9 UDP/TLS/RTP/SAVPF 96 97 98 99 100 101 102 123 127 122 125 107 108 109 124
c=IN IP4 0.0.0.0
a=rtcp:9 IN IP4 0.0.0.0
a=extmap:2 urn:ietf:params:rtp-hdrext:toffset
";


            var serialized = Encoding.UTF8.GetString(
                SDPLib.SDPSerializer.WriteSDP(
                    SDPLib.SDPSerializer.ReadSDP(Encoding.UTF8.GetBytes(expected)
                    )));

            Assert.Equal(expected, serialized);
        }

        [Fact]
        public async Task ShouldParseAsyncAndSerializeSameWay()
        {
            var expected = @"v=0
o=- 770984055657151438 2 IN IP4 127.0.0.1
s=-
i=Test title
u=http://www.example.com/seminars/sdp.pdf
a=charset:ISO-8859-1
a=group:BUNDLE audio video
a=msid-semantic: WMS 36deb484-3e8c-40da-a9af-381de5813df8 3dbb5da7-1894-4cd6-be71-f8e957274044 44850aa4-29e7-468a-855f-00b739a8fd7c 527d1bfc-d258-4115-988e-70c94ccc0f37 71706457-490a-47e8-b25a-77e025c40a2b aba4cac8-87de-4420-b087-8935eb8d4669 c41fe501-ca69-44cd-8ea4-3188e98759a7 e1392574-bd98-46b3-9e37-7a647eaf24ad
t=0 0
r=604800 3600 0 90000
m=audio 9 UDP/TLS/RTP/SAVPF 111 103 104 9 0 8 106 105 13 110 112 113 126
i=This is media title
a=Test
m=video 9 UDP/TLS/RTP/SAVPF 96 97 98 99 100 101 102 123 127 122 125 107 108 109 124
c=IN IP4 0.0.0.0
a=rtcp:9 IN IP4 0.0.0.0
a=extmap:2 urn:ietf:params:rtp-hdrext:toffset
";
            var pipe = new Pipe();
            await pipe.Writer.WriteAsync(Encoding.UTF8.GetBytes(expected));
            pipe.Writer.Complete();

            var sdp = await SDPSerializer.ReadSDP(pipe.Reader);

            var serialized = Encoding.UTF8.GetString(
                SDPSerializer.WriteSDP(sdp));

            Assert.Equal(expected, serialized);
        }
    }
}
