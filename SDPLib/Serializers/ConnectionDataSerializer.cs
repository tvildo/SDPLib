﻿using System;
using System.Buffers;

namespace SDPLib.Serializers
{
    //Connection Data ("c=")
    class ConnectionDataSerializer
    {
        private static byte[] HeaderBytes = new byte[] { (byte)'c', (byte)'=' };
        public const byte Identifier = (byte)'c';
        public static readonly ConnectionDataSerializer Instance = new ConnectionDataSerializer();
        public static readonly char[] ReservedChars = new[] { (char)SDPSerializer.ByteLF, (char)SDPSerializer.ByteSpace };

        public ConnectionData ReadValue(ReadOnlySpan<byte> data)
        {
            var remainingSlice = data;

            // header
            SerializationHelpers.ParseRequiredHeader("Connection Data field", remainingSlice, HeaderBytes);
            remainingSlice = remainingSlice.Slice(HeaderBytes.Length);

            var connData = new ConnectionData();

            // nettype
            connData.Nettype =
                SerializationHelpers.ParseRequiredString("Connection Data field: nettype",
                SerializationHelpers.NextRequiredDelimitedField("Connection Data field: nettype", SDPSerializer.ByteSpace, remainingSlice, out var consumed));
            remainingSlice = remainingSlice.Slice(consumed + 1);

            // addrtype
            connData.AddrType =
                SerializationHelpers.ParseRequiredString("Connection Data field: addrtype",
                SerializationHelpers.NextRequiredDelimitedField("Connection Data field: addrtype", SDPSerializer.ByteSpace, remainingSlice, out consumed));
            remainingSlice = remainingSlice.Slice(consumed + 1);

            // connection-address
            connData.ConnectionAddress =
                SerializationHelpers.ParseRequiredString("Connection Data field: connection-address", remainingSlice);

            return connData;
        }

        public void WriteValue(IBufferWriter<byte> writer, ConnectionData value)
        {
            if (value == null)
                return;

            SerializationHelpers.EnsureFieldIsPresent("Connection Data nettype", value.Nettype);
            SerializationHelpers.CheckForReserverdChars("Connection Data nettype", value.Nettype, ReservedChars);

            SerializationHelpers.EnsureFieldIsPresent("Connection Data addrtype", value.AddrType);
            SerializationHelpers.CheckForReserverdChars("Connection Data addrtype", value.AddrType, ReservedChars);

            SerializationHelpers.EnsureFieldIsPresent("Connection Data unicast address", value.ConnectionAddress);
            SerializationHelpers.CheckForReserverdChars("Connection Data unicast address", value.ConnectionAddress, ReservedChars);

            var field = $"c={value.Nettype} {value.AddrType} {value.ConnectionAddress}{SDPSerializer.CRLF}";
            writer.WriteString(field);
        }
    }
}
