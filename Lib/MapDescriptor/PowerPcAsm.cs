﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSEditor.MapDescriptor
{
    /**
     * Some info taken from:
     * - https://wiibrew.org/wiki/Assembler_Tutorial 
     */
    class PowerPcAsm
    {
        public static readonly UInt32 nop = 0x60000000;
        private static readonly UInt32 lis_r4_zero = 0x3c800000;
        private static readonly UInt32 addi_r4_zero = 0x38840000;
        private static readonly UInt32 lis_r29_zero = 0x3fa00000;
        private static readonly UInt32 addi_r29_zero = 0x3bbd0000;
        private static readonly UInt32 lis_r30_zero = 0x3fc00000;
        private static readonly UInt32 addi_r30_zero = 0x3bde0000;
        public static readonly UInt32 cmpw_r29_r30 = 0x7c1df000;
        public static readonly UInt32 cmpw_r30_r28 = 0x7c1ee000;
        public static readonly UInt32 cmplwi_r24_zero = 0x28180000;
        public static readonly UInt32 cmplwi_r28_zero = 0x281c0000;
        public static readonly UInt32 cmplwi_r29_zero = 0x281d0000;

        public struct Pair16Bit
        {
            public UInt16 upper16Bit;
            public Int16 lower16Bit;
        }
        public static UInt32 make32bitValueFromPair(UInt32 opcode1, UInt32 opcode2)
        {
            // e.g. 0x3c808041 and 0x38840648

            var upper16Bit = opcode1 & 0x0000FFFF;
            var lower16Bit = (Int16)(opcode2 & 0x0000FFFF);
            if (lower16Bit < 0)
            {
                upper16Bit -= 1;
            }
            return (upper16Bit << 0x10) + ((UInt32) lower16Bit & 0x0000FFFF);
        }
        public static Pair16Bit make16bitValuePair(UInt32 addr)
        {
            Pair16Bit addrPair = new Pair16Bit();
            addrPair.upper16Bit = (UInt16)((addr & 0xFFFF0000) >> 0x10);
            addrPair.lower16Bit = (Int16)(addr & 0x0000FFFF);
            if (addrPair.lower16Bit < 0)
            {
                addrPair.upper16Bit += 1;
            }
            return addrPair;
        }
        public static Int16 getOpcodeParameter(UInt32 opcode)
        {
            return (Int16)(opcode & 0x0000FFFF);
        }
        public static UInt32 lis_r4(ushort addr)
        {
            return lis_r4_zero + ((UInt32)addr & 0x0000FFFF);
        }
        internal static UInt32 addi_r4(short addr)
        {
            return addi_r4_zero + ((UInt32)addr & 0x0000FFFF);
        }
        public static UInt32 lis_r29(UInt16 addr)
        {
            return lis_r29_zero + ((UInt32)addr & 0x0000FFFF);
        }
        public static UInt32 addi_r29(Int16 addr)
        {
            return addi_r29_zero + ((UInt32)addr & 0x0000FFFF);
        }
        public static UInt32 lis_r30(UInt16 addr)
        {
            return lis_r30_zero + ((UInt32)addr & 0x0000FFFF);
        }
        public static UInt32 addi_r30(Int16 addr)
        {
            return addi_r30_zero + ((UInt32)addr & 0x0000FFFF);
        }
        public static UInt32 cmplwi_r24(Int16 addr)
        {
            return cmplwi_r24_zero + ((UInt32)addr & 0x0000FFFF);
        }
        public static UInt32 cmplwi_r28(Int16 addr)
        {
            return cmplwi_r28_zero + ((UInt32)addr & 0x0000FFFF);
        }
        public static UInt32 cmplwi_r29(Int16 addr)
        {
            return cmplwi_r29_zero + ((UInt32)addr & 0x0000FFFF);
        }
    }
}
