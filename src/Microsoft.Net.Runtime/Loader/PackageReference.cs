﻿using System;
using NuGet;

namespace Microsoft.Net.Runtime.Loader
{
    public class PackageReference : IEquatable<PackageReference>
    {
        public string Name { get; set; }

        public SemanticVersion Version { get; set; }

        public override string ToString()
        {
            return Name + " " + Version;
        }

        public bool Equals(PackageReference other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(Name, other.Name) && Equals(Version, other.Version);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((PackageReference)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Name != null ? Name.GetHashCode() : 0) * 397) ^ (Version != null ? Version.GetHashCode() : 0);
            }
        }

        public static bool operator ==(PackageReference left, PackageReference right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(PackageReference left, PackageReference right)
        {
            return !Equals(left, right);
        }
    }
}