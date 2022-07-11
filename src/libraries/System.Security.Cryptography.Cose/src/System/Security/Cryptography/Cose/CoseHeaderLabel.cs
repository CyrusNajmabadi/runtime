﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Versioning;

namespace System.Security.Cryptography.Cose
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public readonly struct CoseHeaderLabel : IEquatable<CoseHeaderLabel>
    {
        internal string LabelName => LabelAsString != null ? $"\"{LabelAsString}\"" : LabelAsInt32.ToString();
        private string DebuggerDisplay => $"Label = {LabelName}, Type = {(LabelAsString != null ? typeof(string) : typeof(int))}";

        // https://www.iana.org/assignments/cose/cose.xhtml#header-parameters
        public static CoseHeaderLabel Algorithm => new CoseHeaderLabel(KnownHeaders.Alg);
        public static CoseHeaderLabel CriticalHeaders => new CoseHeaderLabel(KnownHeaders.Crit);
        public static CoseHeaderLabel ContentType => new CoseHeaderLabel(KnownHeaders.ContentType);
        public static CoseHeaderLabel KeyIdentifier => new CoseHeaderLabel(KnownHeaders.Kid);

        internal int LabelAsInt32 { get; }
        internal string? LabelAsString { get; }
        internal int EncodedSize { get; }

        public CoseHeaderLabel(int label)
        {
            this = default;
            LabelAsInt32 = label;
            EncodedSize = CoseHelpers.GetIntegerEncodedSize(label);
        }

        public CoseHeaderLabel(string label)
        {
            if (label is null)
            {
                throw new ArgumentException(null, nameof(label));
            }

            this = default;
            LabelAsString = label;
            EncodedSize = CoseHelpers.GetTextStringEncodedSize(label);
        }

        public bool Equals(CoseHeaderLabel other)
        {
            return LabelAsString == other.LabelAsString && LabelAsInt32 == other.LabelAsInt32;
        }

        public override bool Equals([NotNullWhen(true)] object? obj) => obj is CoseHeaderLabel otherObj && Equals(otherObj);

        public override int GetHashCode()
        {
            // Since this type is used as a key in a dictionary (see CoseHeaderMap)
            // and since the label is potentially adversary-provided, we'll need
            // to randomize the hash code.

            if (LabelAsString != null)
            {
                return LabelAsString.GetRandomizedOrdinalHashCode();
            }

            return LabelAsInt32.GetRandomizedHashCode();
        }

        public static bool operator ==(CoseHeaderLabel left, CoseHeaderLabel right) => left.Equals(right);

        public static bool operator !=(CoseHeaderLabel left, CoseHeaderLabel right) => !left.Equals(right);
    }
}
