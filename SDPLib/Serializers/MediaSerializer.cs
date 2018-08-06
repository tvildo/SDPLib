using System;
using System.Buffers;
using System.Linq;
using System.Text;

namespace SDPLib.Serializers
{
    class MediaSerializer
    {
        public static readonly char[] ReservedChars = new[] { (char)SDPSerializer.ByteLF, (char)SDPSerializer.ByteSpace };
        private static byte[] HeaderBytes = new byte[] { (byte)'m', (byte)'=' };
        public const byte Identifier = (byte)'m';

        public static readonly MediaSerializer Instance = new MediaSerializer();

        public MediaDescription ReadValue(ReadOnlySpan<byte> data)
        {
            var remainingSlice = data;

            //header
            SerializationHelpers.ParseRequiredHeader("Media field", remainingSlice, HeaderBytes);
            remainingSlice = remainingSlice.Slice(HeaderBytes.Length);

            var mDescr = new MediaDescription();

            // Media
            mDescr.Media =
                 SerializationHelpers.ParseRequiredString("Media field: Media",
                 SerializationHelpers.NextRequiredDelimitedField("Media field: Media", SDPSerializer.ByteSpace, remainingSlice, out var consumed));
            remainingSlice = remainingSlice.Slice(consumed + 1);

            // port
            mDescr.Port =
                 SerializationHelpers.ParseRequiredString("Media field: Port",
                 SerializationHelpers.NextRequiredDelimitedField("Media field: Port", SDPSerializer.ByteSpace, remainingSlice, out consumed));
            remainingSlice = remainingSlice.Slice(consumed + 1);

            // Proto
            mDescr.Proto =
                SerializationHelpers.ParseRequiredString("Media field: Proto",
                SerializationHelpers.NextRequiredDelimitedField("Media field: Proto", SDPSerializer.ByteSpace, remainingSlice, out consumed));
            remainingSlice = remainingSlice.Slice(consumed + 1);

            // fmt
            if (remainingSlice.Length == 0)
            {
                throw new DeserializationException("Invalid Media field: fmt, expected required values");
            }
            else
            {
                mDescr.Fmts = Encoding.UTF8.GetString(remainingSlice).Split((char)SDPSerializer.ByteSpace);
            }

            return mDescr;
        }

        public void WriteValue(IBufferWriter<byte> writer, MediaDescription value)
        {
            if (value == null)
                throw new SerializationException("Media field must have value");

            SerializationHelpers.EnsureFieldIsPresent("Media field: Media", value.Media);
            SerializationHelpers.CheckForReserverdChars("Media field: Media", value.Media, ReservedChars);

            SerializationHelpers.EnsureFieldIsPresent("Media field: Port", value.Port);
            SerializationHelpers.CheckForReserverdChars("Media field: Port", value.Port, ReservedChars);

            SerializationHelpers.EnsureFieldIsPresent("Media field: Proto", value.Proto);
            SerializationHelpers.CheckForReserverdChars("Media field: Proto", value.Proto, ReservedChars);

            if(value.Fmts == null || !value.Fmts.Any())
            {
                throw new SerializationException("Invalid Media field: fmt, expected required values");
            }

            var field = $"m={value.Media} {value.Port} {value.Proto}";
            writer.WriteString(field);

            foreach(var fmt in value.Fmts)
            {
                SerializationHelpers.EnsureFieldIsPresent("Media field: fmt", fmt);
                SerializationHelpers.CheckForReserverdChars("Media field: fmt", fmt, ReservedChars);
                writer.WriteString($" {fmt}");
            }

            writer.WriteString(SDPSerializer.CRLF);
        }
    }
}
