# SDP: Session Description Protocol Library for .Net

library uses System.IO.Pipelines: High performance IO in .NET

# Nuget Package

https://www.nuget.org/packages/SDPLib

# Read SDP

```csharp
var expected = 
@"v=0
o=jdoe 2890844526 2890842807 IN IP4 10.47.16.5
s=SDP Seminar
i=A Seminar on the session description protocol
u=http://www.example.com/seminars/sdp.pdf
e=j.doe@example.com (Jane Doe)
c=IN IP4 224.2.17.12/127
t=2873397496 2873404696
a=recvonly
m=audio 49170 RTP/AVP 0
m=video 51372 RTP/AVP 99
a=rtpmap:99 h263-1998/90000
";

var value = SDPSerializer.ReadSDP(Encoding.UTF8.GetBytes(expected));

```

# Write SDP:

```csharp
var sdp = new SDP()
{
    Version = 1,
    Origin = new Origin()
    {
        UserName = "-",
        SessionId = 92389283923923,
        SessionVersion = 2,
        Nettype = "IN",
        AddrType = "IP4",
        UnicastAddress = "127.0.0.1"
    },
    SessionName = Encoding.UTF8.GetBytes("Live video session"),
    MediaDescriptions = new List<MediaDescription>()
    {
        new MediaDescription()
        {
            Media = "video",
            Port = "9",
            Proto = "UDP/TLS/RTP/SAVPF",
            Fmts = new List<string>(){ "96" , "97"}
        }
    }
};

byte[] serialized = SDPSerializer.WriteSDP(sdp);

```

# Limitations:

z=* (time zone adjustments) is not implemented


# License 

MIT
