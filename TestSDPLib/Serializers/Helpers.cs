using System.Text;

namespace TestSDPLib.Serializers
{
    public static class Helpers
    {
        public static byte[] ToByteArray(this string s)
        {
            return Encoding.UTF8.GetBytes(s);
        }
    }
}