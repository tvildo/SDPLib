﻿using System;
using System.Buffers;

namespace SDPLib.Serializers
{
    class OriginSerializer
    {
        public static readonly OriginSerializer Instance = new OriginSerializer();
        public static readonly byte[] HeaderBytes = new byte[] { (byte)'o', (byte)'=' };
        public static readonly char[] ReservedChars = new[] { (char) SDPSerializer.ByteLF, (char) SDPSerializer.ByteSpace};

    public DeserializationState ReadValue(ReadOnlySpan<byte> data, DeserializationSession session)
        {
            var remainingSlice = data;

            //header
            SerializationHelpers.ParseRequiredHeader("Origin field", remainingSlice, HeaderBytes);
            remainingSlice = remainingSlice.Slice(HeaderBytes.Length);

            session.ParsedValue.Origin = new Origin();

            //username
            session.ParsedValue.Origin.UserName =
                SerializationHelpers.ParseRequiredString("Origin field: username",
                SerializationHelpers.NextRequiredDelimitedField("Origin field: username", SDPSerializer.ByteSpace, remainingSlice, out var consumed));
            remainingSlice = remainingSlice.Slice(consumed + 1);

            //sess-id
            session.ParsedValue.Origin.SessionId = SerializationHelpers.ParseLong("Origin field: sess-id",
                SerializationHelpers.NextRequiredDelimitedField("Origin field: sess-id", SDPSerializer.ByteSpace, remainingSlice, out consumed));
            remainingSlice = remainingSlice.Slice(consumed + 1);

            //sess-version
            session.ParsedValue.Origin.SessionVersion = SerializationHelpers.ParseLong("Origin field: sess-version",
                SerializationHelpers.NextRequiredDelimitedField("Origin field: sess-version", SDPSerializer.ByteSpace, remainingSlice, out consumed));
            remainingSlice = remainingSlice.Slice(consumed + 1);

            //nettype
            session.ParsedValue.Origin.Nettype =
                SerializationHelpers.ParseRequiredString("Origin field: nettype",
                SerializationHelpers.NextRequiredDelimitedField("Origin field: nettype", SDPSerializer.ByteSpace, remainingSlice, out consumed));
            remainingSlice = remainingSlice.Slice(consumed + 1);

            //addrtype
            session.ParsedValue.Origin.AddrType =
                SerializationHelpers.ParseRequiredString("Origin field: addrtype",
                SerializationHelpers.NextRequiredDelimitedField("Origin field: addrtype", SDPSerializer.ByteSpace, remainingSlice, out consumed));
            remainingSlice = remainingSlice.Slice(consumed + 1);

            // unicast-address
            session.ParsedValue.Origin.UnicastAddress =
                SerializationHelpers.ParseRequiredString("Origin field: unicast-address", remainingSlice);

            return SessionNameSerializer.Instance.ReadValue;
        }

        public void WriteValue(IBufferWriter<byte> writer, Origin value)
        {
            if (value == null)
                throw new SerializationException("Origin must have value");

            var userName = value.UserName;
            //it is "-" if the originating host does not support the concept of user IDs
            if (value.UserName == null)
                userName = "-";
            else
            {
                SerializationHelpers.CheckForReserverdChars("Origin userame", value.UserName, ReservedChars);
            }

            SerializationHelpers.EnsureFieldIsPresent("Origin nettype", value.Nettype);
            SerializationHelpers.CheckForReserverdChars("Origin nettype", value.Nettype, ReservedChars);

            SerializationHelpers.EnsureFieldIsPresent("Origin addrtype", value.AddrType);
            SerializationHelpers.CheckForReserverdChars("Origin addrtype", value.AddrType, ReservedChars);

            SerializationHelpers.EnsureFieldIsPresent("Origin unicast address", value.UnicastAddress);
            SerializationHelpers.CheckForReserverdChars("Origin unicast address", value.UnicastAddress, ReservedChars);

            var field = $"o={userName} {value.SessionId} {value.SessionVersion} {value.Nettype} {value.AddrType} {value.UnicastAddress}{SDPSerializer.CRLF}";
            writer.WriteString(field);
        }
    }
}
