using System;
using System.Diagnostics;

namespace CustomStreetManager
{
    /**
     * Some info taken from:
     * - https://wiibrew.org/wiki/Assembler_Tutorial 
     * - https://mkwii.com/showthread.php?tid=940 
     * - https://smashboards.com/threads/assembly-guides-resources-q-a.397941/
     * - http://wiki.tockdom.com/wiki/Compiler#ABI
     */
    class PowerPcAsm
    {
        private static readonly UInt32 or_opcode = 0x7c000378;
        private static readonly UInt32 ori_opcode = 0x60000000;
        private static readonly UInt32 srw_opcode = 0x7c000430;
        private static readonly UInt32 andi_opcode = 0x70000000;
        private static readonly UInt32 rlwinm_opcode = 0x54000000;

        private static readonly UInt32 lwzx_opcode = 0x7c00002e;
        private static readonly UInt32 lwz_opcode = 0x80000000;
        private static readonly UInt32 lbz_opcode = 0x88000000;
        private static readonly UInt32 stbx_opcode = 0x7c0001ae;
        private static readonly UInt32 stw_opcode = 0x90000000;

        private static readonly UInt32 li_opcode = 0x38000000;
        private static readonly UInt32 lis_opcode = 0x3c000000;
        private static readonly UInt32 addi_opcode = li_opcode;
        private static readonly UInt32 addis_opcode = lis_opcode;
        private static readonly UInt32 mulli_opcode = 0x1c000000;
        private static readonly UInt32 add_opcode = 0x7c000214;

        private static readonly UInt32 cmpw_opcode = 0x7c000000;
        private static readonly UInt32 cmplw_opcode = 0x7c000040;
        private static readonly UInt32 cmpwi_opcode = 0x2c000000;
        private static readonly UInt32 cmplwi_opcode = 0x28000000;

        private static readonly UInt32 blt_opcode = 0x41800000;
        private static readonly UInt32 blr_opcode = 0x4e800020;
        private static readonly UInt32 beq_opcode = 0x41820000;
        private static readonly UInt32 bne_opcode = 0x40820000;
        private static readonly UInt32 bge_opcode = 0x40800000;
        private static readonly UInt32 b_opcode = 0x48000000;
        private static readonly UInt32 bl_opcode = 0x48000001;

        public struct Pair16Bit
        {
            public Int16 upper16Bit;
            public Int16 lower16Bit;
        }
        public static UInt32 make32bitValueFromPair(UInt32 lis_opcode, UInt32 addi_opcode)
        {
            // e.g. 0x3c808041 and 0x38840648

            var upper16Bit = lis_opcode & 0x0000FFFF;
            var lower16Bit = (Int16)(addi_opcode & 0x0000FFFF);
            if (lower16Bit < 0)
            {
                upper16Bit -= 1;
            }
            return (upper16Bit << 0x10) + ((UInt32)lower16Bit & 0x0000FFFF);
        }
        public static Pair16Bit make16bitValuePair(UInt32 addr)
        {
            Pair16Bit addrPair = new Pair16Bit();
            addrPair.upper16Bit = (Int16)((addr & 0xFFFF0000) >> 0x10);
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
        public static UInt32 li(byte register, short value)
        {
            if (register > 31)
                throw new ArgumentException("the register value must be 31 or below");
            return li_opcode + ((UInt32)register << 21) + ((UInt32)value & 0x0000FFFF);
        }
        public static UInt32 lis(byte register, short value)
        {
            if (register > 31)
                throw new ArgumentException("the register value must be 31 or below");
            return lis_opcode + ((UInt32)register << 21) + ((UInt32)value & 0x0000FFFF);
        }
        public static UInt32 subi(byte register1, byte register2, short value)
        {
            return addi(register1, register2, (short)-value);
        }
        public static UInt32 addi(byte register1, byte register2, short value)
        {
            if (register1 > 31 || register2 > 31)
                throw new ArgumentException("the register value must be 31 or below");
            if (register2 == 0)
                throw new ArgumentException("the register2 cannot be r0");
            return addi_opcode + ((UInt32)register1 << 21) + ((UInt32)register2 << 16) + ((UInt32)value & 0x0000FFFF);
        }
        public static UInt32 addis(byte register1, byte register2, short value)
        {
            if (register1 > 31 || register2 > 31)
                throw new ArgumentException("the register value must be 31 or below");
            if (register2 == 0)
                throw new ArgumentException("the register2 cannot be r0");
            return addis_opcode + ((UInt32)register1 << 21) + ((UInt32)register2 << 16) + ((UInt32)value & 0x0000FFFF);
        }
        public static UInt32 add(byte register1, byte register2, byte register3)
        {
            if (register1 > 31 || register2 > 31 || register3 > 31)
                throw new ArgumentException("the register value must be 31 or below");
            return add_opcode + ((UInt32)register1 << 21) + ((UInt32)register2 << 16) + ((UInt32)register3 << 11);
        }
        public static void test()
        {
            Debug.WriteLine(li(31, 0x28).ToString("X"));
            Debug.WriteLine(addi(0, 1, 1).ToString("X"));
            Debug.WriteLine(addi(0, 31, 1).ToString("X"));
            Debug.WriteLine(addi(1, 1, 1).ToString("X"));
            Debug.WriteLine(addi(31, 1, 1).ToString("X"));
            Debug.WriteLine(cmplw(1, 1).ToString("X"));
            Debug.WriteLine(cmplw(1, 3).ToString("X"));
            Debug.WriteLine(cmplwi(1, 3).ToString("X"));
            Debug.WriteLine(cmplwi(31, 0xFFFF).ToString("X"));
            Debug.WriteLine(ori(31, 31, -1).ToString("X"));
            Debug.WriteLine(ori(1, 1, 1).ToString("X"));
            // 7c000378     or         r0,r0,r0
            // 7c210b78     or         r1,r1,r1
            // 7ffffb78     or         r31,r31,r31
            // 7c411b78     or         r1,r2,r3
            Debug.WriteLine(or(0, 0, 0).ToString("X"));
            Debug.WriteLine(or(1, 1, 1).ToString("X"));
            Debug.WriteLine(or(31, 31, 31).ToString("X"));
            Debug.WriteLine(or(1, 2, 3).ToString("X"));
            // 7c01002e     lwzx       r0,r1,r0
            // 7c22182e     lwzx       r1,r2,r3
            // 7ffff82e     lwzx       r31,r31,r31
            // 7c85302e     lwzx       r4,r5,r6
            // 7c21082e     lwzx       r1,r1,r1
            Debug.WriteLine(lwzx(0, 1, 0).ToString("X"));
            Debug.WriteLine(lwzx(1, 2, 3).ToString("X"));
            Debug.WriteLine(lwzx(31, 31, 31).ToString("X"));
            Debug.WriteLine(lwzx(4, 5, 6).ToString("X"));
            Debug.WriteLine(lwzx(1, 1, 1).ToString("X"));
            // 80230002     lwz       r1,0x2,r3
            // 83ffffff     lwz       r31,-0x1,r31
            // 8063001C     lwz       r3,0x1c,r3
            Debug.WriteLine(lwz(1, 2, 3).ToString("X"));
            Debug.WriteLine(lwz(31, -1, 31).ToString("X"));
            Debug.WriteLine(lwz(3, 0x1c, 3).ToString("X"));
            // 7c0101ae     stbx       r0,r1,r0
            // 7c2219ae     stbx       r1,r2,r3
            // 7ffff9ae     stbx       r31,r31,r31
            Debug.WriteLine(stbx(0, 1, 0).ToString("X"));
            Debug.WriteLine(stbx(1, 2, 3).ToString("X"));
            Debug.WriteLine(stbx(31, 31, 31).ToString("X"));
            // 70210001     andi.      r1,r1,0x1
            // 73ff00ff     andi.      r31,r31,0xff
            // 70410003     andi.      r1,r2,0x3
            Debug.WriteLine(andi(1, 1, 1).ToString("X"));
            Debug.WriteLine(andi(31, 31, 0xFF).ToString("X"));
            Debug.WriteLine(andi(1, 2, 3).ToString("X"));
            // 7c000430     srw        r0,r0,r0
            // 7c201430     srw        r0,r1,r2
            // 7ffffc30     srw        r31,r31,r31
            Debug.WriteLine(srw(0, 0, 0).ToString("X"));
            Debug.WriteLine(srw(0, 1, 2).ToString("X"));
            Debug.WriteLine(srw(31, 31, 31).ToString("X"));
            // 1c000000     mulli      r0, r0,0x0
            // 1c210001     mulli      r1, r1,0x1
            // 1c220003     mulli      r1, r2,0x3
            // 1fff00ff     mulli      r31,r31,0xff
            Debug.WriteLine(mulli(0, 0, 0).ToString("X"));
            Debug.WriteLine(mulli(1, 1, 1).ToString("X"));
            Debug.WriteLine(mulli(1, 2, 3).ToString("X"));
            Debug.WriteLine(mulli(31, 31, 0xff).ToString("X"));
            // 8007e130:  483adf31     bl 0x8042c060
            Debug.WriteLine(bl(0x8007e130, 0x8042c060).ToString("X"));
            // 8042c0a8:  4180ffe4     blt        LAB_8042c08c
            // 8042c0b4:  4180ffc4     blt        LAB_8042c078
            Debug.WriteLine(blt(0x8042c0a8, 0x8042c08c).ToString("X"));
            Debug.WriteLine(blt(0x8042c0b4, 0x8042c078).ToString("X"));
            // 800feb34 8b ff 00 ff     lbz r31,0xff(r31)
            // 800feb38 88 02 00 01     lbz r0,0x1(r2)
            // 800feb3c 88 21 00 01     lbz r1,0x1(r1)
            // 800feb40 90 01 00 00     stw r0,0x0(r1)
            // 800feb44 90 21 00 01     stw r1,0x1(r1)
            // 800feb48 90 23 00 02     stw r1,0x2(r3)
            Debug.WriteLine(lbz(31,0xff,31).ToString("X"));
            Debug.WriteLine(lbz(0, 0x1, 2).ToString("X"));
            Debug.WriteLine(lbz(1, 0x1, 1).ToString("X"));
            Debug.WriteLine(stw(0, 0x0, 1).ToString("X"));
            Debug.WriteLine(stw(1, 0x1, 1).ToString("X"));
            Debug.WriteLine(stw(1, 0x2, 3).ToString("X"));
            // 7c000214 add r0,r0,r0
            // 7c210a14 add r1,r1,r1
            // 7c221a14 add r1,r2,r3
            Debug.WriteLine(add(0, 0, 0).ToString("X"));
            Debug.WriteLine(add(1, 1, 1).ToString("X"));
            Debug.WriteLine(add(1, 2, 3).ToString("X"));
        }

        public static UInt32 ori(byte register1, byte register2, short value)
        {
            if (register1 > 31 || register2 > 31)
                throw new ArgumentException("the register value must be 31 or below");
            return ori_opcode + ((UInt32)register1 << 21) + ((UInt32)register2 << 16) + ((UInt32)value & 0x0000FFFF);
        }
        public static UInt32 or(byte register1, byte register2, byte register3)
        {
            if (register1 > 31 || register2 > 31 || register3 > 31)
                throw new ArgumentException("the register value must be 31 or below");
            return or_opcode + ((UInt32)register1 << 16) + ((UInt32)register2 << 21) + ((UInt32)register3 << 11);
        }
        public static UInt32 mr(byte register1, byte register2)
        {
            return or(register1, register2, register2);
        }
        public static UInt32 cmpw(byte register1, byte register2)
        {
            if (register1 > 31 || register2 > 31)
                throw new ArgumentException("the register value must be 31 or below");
            return cmpw_opcode + ((UInt32)register1 << 16) + ((UInt32)register2 << 11);
        }
        public static UInt32 mulli(byte register1, byte register2, short value)
        {
            if (register1 > 31 || register2 > 31)
                throw new ArgumentException("the register value must be 31 or below");
            return mulli_opcode + ((UInt32)register1 << 21) + ((UInt32)register2 << 16) + ((UInt32)value & 0x0000FFFF);
        }
        public static UInt32 cmplw(byte register1, byte register2)
        {
            if (register1 > 31 || register2 > 31)
                throw new ArgumentException("the register value must be 31 or below");
            return cmplw_opcode + ((UInt32)register1 << 16) + ((UInt32)register2 << 11);
        }
        public static UInt32 bl(VAVAddr startPos, VAVAddr targetPos)
        {
            return bl(startPos, 0, targetPos);
        }
        public static UInt32 bl(VAVAddr startPos, int offset, VAVAddr targetPos)
        {
            return bl((uint)(startPos + offset * 4), (uint) targetPos);
        }
        public static UInt32 bl(uint currentPos, uint targetPos)
        {
            UInt32 offset = ((targetPos - currentPos) >> 2) & 0x00FFFFFF;
            return bl_opcode + (offset << 2);
        }
        public static UInt32 b(VAVAddr startPos, VAVAddr targetPos)
        {
            return b(startPos, 0, targetPos);
        }
        public static UInt32 b(VAVAddr startPos, int offset, VAVAddr targetPos)
        {
            return b((uint)(startPos + offset * 4), (uint)targetPos);
        }
        public static UInt32 b(uint currentPos, uint targetPos)
        {
            UInt32 offset = ((targetPos - currentPos) >> 2) & 0x00FFFFFF;
            return b_opcode + (offset << 2);
        }
        public static uint blt(uint currentPos, uint targetPos)
        {
            return blt_opcode + ((targetPos - currentPos) & 0x0000FFFF);
        }
        public static uint blt(int currentPos, int targetPos)
        {
            return (uint)(blt_opcode + ((4 * (targetPos - currentPos)) & 0x0000FFFF));
        }
        public static uint beq(uint currentPos, uint targetPos)
        {
            return beq_opcode + ((targetPos - currentPos) & 0x0000FFFF);
        }
        public static uint beq(int currentPos, int targetPos)
        {
            return beq(targetPos - currentPos);
        }
        public static uint beq(int offset)
        {
            return (uint)(beq_opcode + ((4 * offset) & 0x0000FFFF));
        }
        public static uint bge(int currentPos, int targetPos)
        {
            return bge(targetPos - currentPos);
        }
        public static uint bge(int offset)
        {
            return (uint)(bge_opcode + ((4 * offset) & 0x0000FFFF));
        }
        public static uint bne(int currentPos, int targetPos)
        {
            return bne(targetPos - currentPos);
        }
        public static uint bne(int offset)
        {
            return (uint)(bne_opcode + ((4 * offset) & 0x0000FFFF));
        }
        public static UInt32 cmpwi(byte register, short value)
        {
            if (register > 31)
                throw new ArgumentException("the register value must be 31 or below");
            return cmpwi_opcode + ((UInt32)register << 16) + ((UInt32)value & 0x0000FFFF);
        }
        public static UInt32 cmplwi(byte register, ushort value)
        {
            if (register > 31)
                throw new ArgumentException("the register value must be 31 or below");
            return cmplwi_opcode + ((UInt32)register << 16) + ((UInt32)value & 0x0000FFFF);
        }
        public static UInt32 blr()
        {
            return blr_opcode;
        }
        public static UInt32 nop()
        {
            return ori(0, 0, 0);
        }
        public static uint lwzx(byte register1, byte register2, byte register3)
        {
            if (register1 > 31 || register2 > 31 || register3 > 31)
                throw new ArgumentException("the register value must be 31 or below");
            if (register2 == 0)
                throw new ArgumentException("the register2 cannot be r0");
            return lwzx_opcode + ((UInt32)register1 << 21) + ((UInt32)register2 << 16) + ((UInt32)register3 << 11);
        }
        public static uint lwz(byte register1, short value, byte register2)
        {
            if (register1 > 31 || register2 > 31)
                throw new ArgumentException("the register value must be 31 or below");
            if (register2 == 0)
                throw new ArgumentException("the register2 cannot be r0");
            return lwz_opcode + ((UInt32)register1 << 21) + ((UInt32)register2 << 16) + ((UInt32)value & 0x0000FFFF);
        }
        public static uint lbz(byte register1, short value, byte register2)
        {
            if (register1 > 31 || register2 > 31)
                throw new ArgumentException("the register value must be 31 or below");
            if (register2 == 0)
                throw new ArgumentException("the register2 cannot be r0");
            return lbz_opcode + ((UInt32)register1 << 21) + ((UInt32)register2 << 16) + ((UInt32)value & 0x0000FFFF);
        }
        public static uint srw(byte register1, byte register2, byte register3)
        {
            if (register1 > 31 || register2 > 31 || register3 > 31)
                throw new ArgumentException("the register value must be 31 or below");
            return srw_opcode + ((UInt32)register1 << 16) + ((UInt32)register2 << 21) + ((UInt32)register3 << 11);
        }
        public static uint rlwinm(byte register1, byte register2, byte value1, byte value2, byte value3)
        {
            if (register1 > 31 || register2 > 31)
                throw new ArgumentException("the register value must be 31 or below");
            if (value1 > 31 || value2 > 31 || value3 > 31)
                throw new ArgumentException("the scalar value must be 31 or below");
            return rlwinm_opcode + ((UInt32)register1 << 16) + ((UInt32)register2 << 21) + ((UInt32)value1 << 11) + ((UInt32)value2 << 6) + ((UInt32)value3 << 1);
        }
        public static uint stw(byte register1, short value, byte register2)
        {
            if (register1 > 31 || register2 > 31)
                throw new ArgumentException("the register value must be 31 or below");
            if (register2 == 0)
                throw new ArgumentException("the register2 cannot be r0");
            return stw_opcode + ((UInt32)register1 << 21) + ((UInt32)register2 << 16) + ((UInt32)value & 0x0000FFFF);
        }
        public static uint andi(byte register1, byte register2, short value)
        {
            if (register1 > 31 || register2 > 31)
                throw new ArgumentException("the register value must be 31 or below");
            return andi_opcode + ((UInt32)register1 << 16) + ((UInt32)register2 << 21) + ((UInt32)value & 0x0000FFFF);
        }
        public static uint stbx(byte register1, byte register2, byte register3)
        {
            if (register1 > 31 || register2 > 31 || register3 > 31)
                throw new ArgumentException("the register value must be 31 or below");
            if (register2 == 0)
                throw new ArgumentException("the register2 cannot be r0");
            return stbx_opcode + ((UInt32)register1 << 21) + ((UInt32)register2 << 16) + ((UInt32)register3 << 11);
        }
    }
}
