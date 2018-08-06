﻿using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;

namespace SDPLib.Serializers
{
    //Email Address ("e=")
    class EmailAddressSerializer
    {
        private static byte[] HeaderBytes = new byte[] { (byte)'e', (byte)'=' };
        public const byte Identifier = (byte)'e';
        public static char[] ReservedChars = new char[] { (char)SDPSerializer.ByteLF };

        public static readonly EmailAddressSerializer Instance = new EmailAddressSerializer();

        public DeserializationState ReadValue(ReadOnlySpan<byte> data, DeserializationSession session)
        {
            if (session.ParsedValue.MediaDescriptions != null && session.ParsedValue.MediaDescriptions.Any())
            {
                throw new DeserializationException("Email address MUST be specified before the first media field");
            }

            var remainingSlice = data;

            //header
            SerializationHelpers.ParseRequiredHeader("Email field", remainingSlice, HeaderBytes);
            remainingSlice = remainingSlice.Slice(HeaderBytes.Length);

            //uri
            var emailString =
                SerializationHelpers.ParseRequiredString("Email field",
                SerializationHelpers.NextRequiredField("Email field", remainingSlice));

            session.ParsedValue.EmailNumbers = session.ParsedValue.EmailNumbers ?? new List<string>();
            session.ParsedValue.EmailNumbers.Add(emailString);
            return OptionalValueDeSerializer.Instance.ReadValue;
        }

        public void WriteValue(IBufferWriter<byte> writer, string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return;

            SerializationHelpers.CheckForReserverdChars("Email field", value, ReservedChars);

            var field = $"e={value}{SDPSerializer.CRLF}";
            writer.WriteString(field);
        }
    }
}
