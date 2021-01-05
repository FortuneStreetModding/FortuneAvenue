using System;
using System.Collections.Generic;
using System.Text;

namespace CustomStreetMapManager
{
    /// <summary>
    /// Boom Street Virtual Address
    /// </summary>
    public struct BSVAddr
    {
        private UInt32 value;
        private BSVAddr(UInt32 value) : this()
        {
            this.value = value;
        }
        public static explicit operator BSVAddr(uint v)
        {
            return new BSVAddr(v);
        }
        public static explicit operator long(BSVAddr v)
        {
            return (long)v.value;
        }
        public override string ToString()
        {
            return value.ToString("X");
        }
    }
}
