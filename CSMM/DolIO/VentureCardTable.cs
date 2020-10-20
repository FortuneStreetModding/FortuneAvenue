using MiscUtil.Conversion;
using MiscUtil.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomStreetManager
{
    public class VentureCardTable : DolIO
    {
        /// <summary>
        /// Write the subroutine which takes a compressed venture card table as input and writes it into ventureCardDecompressedTableAddr
        /// </summary>
        /// <param name="ventureCardDecompressedTableAddr">The address for the reserved memory space to store the decompressed venture card table in</param>
        protected override string writeSubroutine(EndianBinaryWriter s, UInt32 ventureCardDecompressedTableAddr)
        {
            PowerPcAsm.Pair16Bit ventureCardTableAddrPair = PowerPcAsm.make16bitValuePair(ventureCardDecompressedTableAddr);
            ///
            /// assume: 
            /// r6 = ventureCardCompressedTableAddr
            /// 
            /// variables:
            /// r4 = ventureCardId
            /// r5 = ventureCardCompressedTableAddr
            /// r6 = ventureCardDecompressedTableAddr
            /// r7 = ventureCardCompressedWord
            /// r8 = bitIndex
            /// r0 = tmp / currentByte
            ///
            /// return:
            /// r6 = ventureCardDecompressedTableAddr
            ///
            s.Write(PowerPcAsm.li(4, 0));                                                           // ventureCardId = 0
            s.Write(PowerPcAsm.mr(5, 6));                                                           // r6 is ventureCardCompressedTableAddr at this point. Copy it to r5 with which we will be working
            s.Write(PowerPcAsm.lis(6, ventureCardTableAddrPair.upper16Bit));                        // \ load the ventureCardDecompressedTableAddr into r6. This address is
            s.Write(PowerPcAsm.addi(6, 6, ventureCardTableAddrPair.lower16Bit));                    // /  where we will store the decompressed venture card table.
            uint whileVentureCardIdSmaller128 = (uint)s.BaseStream.Position;                        // do {
            {                                                                                       //      
                s.Write(PowerPcAsm.li(0, 0));                                                       //     \ load the next compressed word from ventureCardCompressedTableAddr 
                s.Write(PowerPcAsm.lwzx(7, 5, 0));                                                  //     /  into r7. We will decompress the venture card table word by word.
                s.Write(PowerPcAsm.li(8, 0));                                                       //     bitIndex = 0
                uint whileBitIndexSmaller32 = (uint)s.BaseStream.Position;                          //     do 
                {                                                                                   //     {
                    s.Write(PowerPcAsm.mr(0, 7));                                                   //         get the current compressed word
                    s.Write(PowerPcAsm.srw(0, 0, 8));                                               //         shift it bitIndex times to the right
                    s.Write(PowerPcAsm.andi(0, 0, 1));                                              //         retrieve the lowest bit of it -> r0 contains the decompressed venture card byte now.
                    s.Write(PowerPcAsm.stbx(0, 4, 6));                                              //         store it into ventureCardDecompressedTableAddr[ventureCardId]
                    s.Write(PowerPcAsm.addi(8, 8, 1));                                              //         bitIndex++
                    s.Write(PowerPcAsm.addi(4, 4, 1));                                              //         ventureCardId++
                    s.Write(PowerPcAsm.cmpwi(8, 32));                                               //      
                    s.Write(PowerPcAsm.blt((uint)s.BaseStream.Position, whileBitIndexSmaller32));   //     } while(bitIndex < 32)
                }                                                                                   //     
                s.Write(PowerPcAsm.addi(5, 5, 4));                                                  //     ventureCardCompressedTableAddr += 4
                s.Write(PowerPcAsm.cmpwi(4, 128));                                                  //     
                s.Write(PowerPcAsm.blt((uint)s.BaseStream.Position, whileVentureCardIdSmaller128)); // } while(ventureCardId < 128)
            }                                                                                       //
            s.Write(PowerPcAsm.li(4, 0));                                                           // \ reset r4 = 0
            s.Write(PowerPcAsm.li(5, 0));                                                           // / reset r5 = 0
            s.Write(PowerPcAsm.blr());                                                              // return

            return "DecompressVentureCardSubroutine";
        }
        /// <summary>
        /// Hijack the LoadBoard() routine. Intercept the moment when the (now compressed) ventureCardTable is passed on to the InitChanceBoard() routine. 
        /// Call the decompressVentureCardTable routine and pass the resulting decompressed ventureCardTable (located at ventureCardDecompressedTableAddr) to the InitChanceBoard() routine instead.
        /// </summary>
        /// <param name="tableRowCount"></param>
        /// <param name="ventureCardCompressedTableAddr"></param>
        /// <param name="dataAddr">unused</param>
        /// <param name="ventureCardDecompressTableRoutine"></param>
        protected override void writeTableRefs(EndianBinaryWriter stream, Func<uint, int> toFileAddress, Int16 tableRowCount, UInt32 ventureCardCompressedTableAddr, UInt32 dataAddr, UInt32 ventureCardDecompressTableRoutine)
        {
            // cmplwi r24,0x29                                     -> cmplwi r24,ventureCardTableCount-1
            stream.Seek(toFileAddress(0x8007e104), SeekOrigin.Begin); stream.Write(PowerPcAsm.cmplwi(24, (UInt16)(tableRowCount - 1)));
            // mulli r0,r24,0x82                                   -> mulli r0,r24,0x10
            stream.Seek(toFileAddress(0x8007e11c), SeekOrigin.Begin); stream.Write(PowerPcAsm.mulli(0, 24, 16));
            PowerPcAsm.Pair16Bit v = PowerPcAsm.make16bitValuePair(ventureCardCompressedTableAddr);
            // r4 <- 0x80410648                                    -> r4 <- ventureCardTableAddr
            stream.Seek(toFileAddress(0x8007e120), SeekOrigin.Begin); stream.Write(PowerPcAsm.lis(4, v.upper16Bit)); stream.Seek(4, SeekOrigin.Current); stream.Write(PowerPcAsm.addi(4, 4, v.lower16Bit));
            // li r5,0x0                                           -> bl ventureCardDecompressTableRoutine
            stream.Seek(toFileAddress(0x8007e130), SeekOrigin.Begin); stream.Write(PowerPcAsm.bl(0x8007e130, ventureCardDecompressTableRoutine));
        }
        /// <summary>
        /// Write the compressed venture card table
        /// </summary>
        protected override string writeTable(EndianBinaryWriter s, List<MapDescriptor> mapDescriptors)
        {
            foreach (var mapDescriptor in mapDescriptors)
            {
                // here we bitpack the venture card table so that each byte stores 8 venture cards. This results
                // in an array of just 16 bytes for each map.
                int i = 0;
                while (i < mapDescriptor.VentureCard.Count())
                {
                    byte bitPackedVentureCardValue = 0;
                    for (int j = 0; j < 8; j++, i++)
                    {
                        if (i < mapDescriptor.VentureCard.Count())
                        {
                            bitPackedVentureCardValue |= (byte)(mapDescriptor.VentureCard[i] << j);
                        }
                    }
                    s.Write(bitPackedVentureCardValue);
                }
            }
            return "VentureCardCompressedTable";
        }

        /// <summary>
        /// Allocate working memory space for a single uncompressed venture card table which is passed on for the game to use. We will use it to store the result of decompressing a compressed venture card table
        /// </summary>
        protected override string writeData(EndianBinaryWriter s)
        {
            s.Write(new byte[130]);
            return "VentureCardReservedMemoryForDecompressedTable";
        }
        private void readVanillaVentureCardTable(EndianBinaryReader s, List<MapDescriptor> mapDescriptors)
        {
            for (int i = 0; i < 42; i++)
            {
                for (int j = 0; j < 128; j++)
                {
                    mapDescriptors[i].VentureCard[j] = s.ReadByte();
                }
                // discard the last two bytes
                s.ReadByte();
                s.ReadByte();
            }
        }

        private void readCompressedVentureCardTable(EndianBinaryReader s, List<MapDescriptor> mapDescriptors)
        {
            foreach (var mapDescriptor in mapDescriptors)
            {
                int i = 0;
                while (i < mapDescriptor.VentureCard.Count())
                {
                    byte bitPackedVentureCardValue = s.ReadByte();
                    for (int j = 0; j < 8; j++, i++)
                    {
                        if (i < mapDescriptor.VentureCard.Count())
                        {
                            mapDescriptor.VentureCard[i] = (byte)((bitPackedVentureCardValue >> j) & 1);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// In vanilla the venture card table uses a wasteful format of having an array of 130 bytes for each map: 
        /// 1 byte for each venture card id, the last two bytes are unused. 
        /// The hacked compressed venture card table uses only 16 bytes for each map
        /// </summary>
        /// <param name="s"></param>
        /// <param name="mapDescriptors"></param>
        /// <param name="isVanilla"></param>
        protected override void readTable(EndianBinaryReader s, List<MapDescriptor> mapDescriptors, bool isVanilla)
        {
            if (isVanilla)
            {
                readVanillaVentureCardTable(s, mapDescriptors);
            }
            else
            {
                readCompressedVentureCardTable(s, mapDescriptors);
            }
        }
        protected override UInt32 readTableAddr(EndianBinaryReader stream, Func<uint, int> toFileAddress, bool isVanilla)
        {
            stream.Seek(toFileAddress(0x8007e120), SeekOrigin.Begin);
            var lis_opcode = stream.ReadUInt32();
            stream.Seek(4, SeekOrigin.Current);
            var addi_opcode = stream.ReadUInt32();
            return PowerPcAsm.make32bitValueFromPair(lis_opcode, addi_opcode);
        }
        protected override Int16 readTableRowCount(EndianBinaryReader stream, Func<uint, int> toFileAddress, bool isVanilla)
        {
            stream.Seek(toFileAddress(0x8007e104), SeekOrigin.Begin);
            var opcode = stream.ReadUInt32();
            return (Int16)(PowerPcAsm.getOpcodeParameter(opcode) + 1);
        }
        protected override bool readIsVanilla(EndianBinaryReader stream, Func<uint, int> toFileAddress)
        {
            stream.Seek(toFileAddress(0x8007e130), SeekOrigin.Begin);
            var opcode = stream.ReadUInt32();
            return opcode == PowerPcAsm.li(5, 0);
        }
    }
}
