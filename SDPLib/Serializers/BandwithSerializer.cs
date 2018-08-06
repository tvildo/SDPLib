﻿using System;
using System.Buffers;

namespace SDPLib.Serializers
{
    //Bandwidth ("b=")
    class BandwithSerializer
    {
        private static byte[] HeaderBytes = new byte[] { (byte)'b', (byte)'=' };
        public const byte Identifier = (byte)'b';
        public static readonly char[] ReservedChars = new[] { (char)SDPSerializer.ByteLF, (char)SDPSerializer.ByteSpace };
        public static readonly BandwithSerializer Instance = new BandwithSerializer();

        public Bandwidth ReadValue(ReadOnlySpan<byte> data)
        {
            var remainingSlice = data;

            // header
            SerializationHelpers.ParseRequiredHeader("Bandwith field", remainingSlice, HeaderBytes);
            remainingSlice = remainingSlice.Slice(HeaderBytes.Length);

            var bandwith = new Bandwidth();

            // type
            bandwith.Type =
                SerializationHelpers.ParseRequiredString("Bandwith field: Type",
                SerializationHelpers.NextRequiredDelimitedField("Bandwith field: Type", SDPSerializer.ByteColon, remainingSlice, out var consumed));
            remainingSlice = remainingSlice.Slice(consumed + 1);

            // value
            bandwith.Value =
                SerializationHelpers.ParseRequiredString("Bandwith field: value", remainingSlice);

            return bandwith;
        }

        public void WriteValue(IBufferWriter<byte> writer, Bandwidth value)
        {
            if (value == null)
                return;

            SerializationHelpers.EnsureFieldIsPresent("Bandwith field Type", value.Type);
            SerializationHelpers.CheckForReserverdChars("Connection Data nettype", value.Type, ReservedChars);

            SerializationHelpers.EnsureFieldIsPresent("Bandwith field value", value.Value);
            SerializationHelpers.CheckForReserverdChars("Bandwith field value", value.Value, ReservedChars);

            var field = $"b={value.Type}:{value.Value}{SDPSerializer.CRLF}";
            writer.WriteString(field);
        }
    }
}
