using System;
using System.Collections.Generic;
using System.Text;

namespace CustomStreetMapManager
{
    /// <summary>
    /// Version Agnostic Virtual Address
    /// This represents an address which has been converted to the current version of the DOL that has been loaded
    /// </summary>
    public struct VAVAddr
    {
        public static readonly VAVAddr MaxValue = new VAVAddr(UInt32.MaxValue);
        public static readonly VAVAddr NullAddress = new VAVAddr(0);
        private UInt32 value;
        private VAVAddr(UInt32 value) : this()
        {
            this.value = value;
        }
        public static explicit operator VAVAddr(uint v)
        {
            return new VAVAddr(v);
        }
        public static explicit operator int(VAVAddr v)
        {
            return (int)v.value;
        }
        public static explicit operator uint(VAVAddr v)
        {
            return v.value;
        }
        public static explicit operator long(VAVAddr v)
        {
            return (long)v.value;
        }
        public static VAVAddr operator +(VAVAddr x, int y)
        {
            return new VAVAddr(x.value + (uint)y);
        }
        public static VAVAddr operator +(int x, VAVAddr y)
        {
            return new VAVAddr((uint)x + y.value);
        }
        public static VAVAddr operator ++(VAVAddr x)
        {
            return new VAVAddr(x.value + 1);
        }
        public static VAVAddr operator -(VAVAddr x, int y)
        {
            return new VAVAddr(x.value - (uint)y);
        }
        public static VAVAddr operator -(int x, VAVAddr y)
        {
            return new VAVAddr((uint)x - y.value);
        }
        public static UInt32 operator -(VAVAddr x, VAVAddr y)
        {
            return x.value - y.value;
        }
        public static int operator %(VAVAddr x, int y)
        {
            return (int)(x.value % y);
        }
        public static bool operator ==(VAVAddr x, VAVAddr y)
        {
            return x.value == y.value;
        }
        public static bool operator !=(VAVAddr x, VAVAddr y)
        {
            return x.value != y.value;
        }
        public static bool operator >=(VAVAddr x, VAVAddr y)
        {
            return x.value >= y.value;
        }
        public static bool operator <=(VAVAddr x, VAVAddr y)
        {
            return x.value <= y.value;
        }
        public static bool operator >(VAVAddr x, VAVAddr y)
        {
            return x.value > y.value;
        }
        public static bool operator <(VAVAddr x, VAVAddr y)
        {
            return x.value < y.value;
        }
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return value == ((VAVAddr)obj).value;
        }
        public override int GetHashCode()
        {
            unchecked
            {
                return EqualityComparer<UInt32>.Default.GetHashCode(value);
            }
        }
        public override string ToString()
        {
            return value.ToString("X");
        }
    }
}
