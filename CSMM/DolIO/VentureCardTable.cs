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
        /// Write the compressed venture card table
        /// </summary>
        protected VAVAddr writeTable(List<MapDescriptor> mapDescriptors)
        {
            var ventureCardCompressedTable = new List<byte>();
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
                    ventureCardCompressedTable.Add(bitPackedVentureCardValue);
                }
            }
            return allocate(ventureCardCompressedTable, "VentureCardCompressedTable");
        }

        /// <summary>
        /// Hijack the LoadBoard() routine. Intercept the moment when the (now compressed) ventureCardTable is passed on to the InitChanceBoard() routine. 
        /// Call the decompressVentureCardTable routine and pass the resulting decompressed ventureCardTable (located at ventureCardDecompressedTableAddr) to the InitChanceBoard() routine instead.
        /// </summary>
        /// <param name="tableRowCount"></param>
        /// <param name="ventureCardCompressedTableAddr"></param>
        protected override void writeAsm(EndianBinaryWriter stream, AddressMapper addressMapper, List<MapDescriptor> mapDescriptors)
        {
            var tableRowCount = mapDescriptors.Count;
            var ventureCardCompressedTableAddr = writeTable(mapDescriptors);
            PowerPcAsm.Pair16Bit v = PowerPcAsm.make16bitValuePair((UInt32)ventureCardCompressedTableAddr);

            // Allocate working memory space for a single uncompressed venture card table which is passed on for the game to use. We will use it to store the result of decompressing a compressed venture card table
            var ventureCardDecompressedTableAddr = allocate(new byte[130], "VentureCardReservedMemoryForDecompressedTable");
            var ventureCardDecompressTableRoutine = allocate(writeSubroutine(ventureCardDecompressedTableAddr), "DecompressVentureCardSubroutine");

            // cmplwi r24,0x29                                     -> cmplwi r24,ventureCardTableCount-1
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x8007e104), SeekOrigin.Begin); stream.Write(PowerPcAsm.cmplwi(24, (UInt16)(tableRowCount - 1)));
            // mulli r0,r24,0x82                                   -> mulli r0,r24,0x10
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x8007e11c), SeekOrigin.Begin); stream.Write(PowerPcAsm.mulli(0, 24, 0x10));
            // r4 <- 0x80410648                                    -> r4 <- ventureCardTableAddr
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x8007e120), SeekOrigin.Begin); stream.Write(PowerPcAsm.lis(4, v.upper16Bit)); stream.Seek(4, SeekOrigin.Current); stream.Write(PowerPcAsm.addi(4, 4, v.lower16Bit));
            // li r5,0x0                                           -> bl ventureCardDecompressTableRoutine
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x8007e130), SeekOrigin.Begin); stream.Write(PowerPcAsm.bl((UInt32)addressMapper.toVersionAgnosticAddress((BSVAddr)0x8007e130), (UInt32)ventureCardDecompressTableRoutine));
        }

        /// <summary>
        /// Write the subroutine which takes a compressed venture card table as input and writes it into ventureCardDecompressedTableAddr
        /// </summary>
        /// <param name="ventureCardDecompressedTableAddr">The address for the reserved memory space to store the decompressed venture card table in</param>
        private List<UInt32> writeSubroutine(VAVAddr ventureCardDecompressedTableAddr)
        {
            var asm = new List<UInt32>();
            PowerPcAsm.Pair16Bit ventureCardTableAddrPair = PowerPcAsm.make16bitValuePair((UInt32)ventureCardDecompressedTableAddr);
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
            asm.Add(PowerPcAsm.li(4, 0));                                                           // ventureCardId = 0
            asm.Add(PowerPcAsm.mr(5, 6));                                                           // r6 is ventureCardCompressedTableAddr at this point. Copy it to r5 with which we will be working
            asm.Add(PowerPcAsm.lis(6, ventureCardTableAddrPair.upper16Bit));                        // \ load the ventureCardDecompressedTableAddr into r6. This address is
            asm.Add(PowerPcAsm.addi(6, 6, ventureCardTableAddrPair.lower16Bit));                    // /  where we will store the decompressed venture card table.
            int whileVentureCardIdSmaller128 = asm.Count;                                           // do {
            {                                                                                       //      
                asm.Add(PowerPcAsm.li(0, 0));                                                       //     \ load the next compressed word from ventureCardCompressedTableAddr 
                asm.Add(PowerPcAsm.lwzx(7, 5, 0));                                                  //     /  into r7. We will decompress the venture card table word by word.
                asm.Add(PowerPcAsm.li(8, 0));                                                       //     bitIndex = 0
                int whileBitIndexSmaller32 = asm.Count;                                             //     do 
                {                                                                                   //     {
                    asm.Add(PowerPcAsm.mr(0, 7));                                                   //         get the current compressed word
                    asm.Add(PowerPcAsm.srw(0, 0, 8));                                               //         shift it bitIndex times to the right
                    asm.Add(PowerPcAsm.andi(0, 0, 1));                                              //         retrieve the lowest bit of it -> r0 contains the decompressed venture card byte now.
                    asm.Add(PowerPcAsm.stbx(0, 4, 6));                                              //         store it into ventureCardDecompressedTableAddr[ventureCardId]
                    asm.Add(PowerPcAsm.addi(8, 8, 1));                                              //         bitIndex++
                    asm.Add(PowerPcAsm.addi(4, 4, 1));                                              //         ventureCardId++
                    asm.Add(PowerPcAsm.cmpwi(8, 32));                                               //      
                    asm.Add(PowerPcAsm.blt(asm.Count, whileBitIndexSmaller32));                     //     } while(bitIndex < 32)
                }                                                                                   //     
                asm.Add(PowerPcAsm.addi(5, 5, 4));                                                  //     ventureCardCompressedTableAddr += 4
                asm.Add(PowerPcAsm.cmpwi(4, 128));                                                  //     
                asm.Add(PowerPcAsm.blt(asm.Count, whileVentureCardIdSmaller128));                   // } while(ventureCardId < 128)
            }                                                                                       //
            asm.Add(PowerPcAsm.li(4, 0));                                                           // \ reset r4 = 0
            asm.Add(PowerPcAsm.li(5, 0));                                                           // / reset r5 = 0
            asm.Add(PowerPcAsm.blr());                                                              // return

            return asm;
        }
        /// <summary>
        /// In vanilla the venture card table uses a wasteful format of having an array of 130 bytes for each map: 
        /// 1 byte for each venture card id, the last two bytes are unused. 
        /// The hacked compressed venture card table uses only 16 bytes for each map
        /// </summary>
        /// <param name="s"></param>
        /// <param name="mapDescriptors"></param>
        /// <param name="isVanilla"></param>
        protected override void readTable(EndianBinaryReader s, List<MapDescriptor> mapDescriptors, AddressMapper addressMapper, bool isVanilla)
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
        protected override VAVAddr readTableAddr(EndianBinaryReader stream, AddressMapper addressMapper, bool isVanilla)
        {
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x8007e120), SeekOrigin.Begin);
            var lis_opcode = stream.ReadUInt32();
            stream.Seek(4, SeekOrigin.Current);
            var addi_opcode = stream.ReadUInt32();
            return (VAVAddr)PowerPcAsm.make32bitValueFromPair(lis_opcode, addi_opcode);
        }
        protected override Int16 readTableRowCount(EndianBinaryReader stream, AddressMapper addressMapper, bool isVanilla)
        {
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x8007e104), SeekOrigin.Begin);
            var opcode = stream.ReadUInt32();
            return (Int16)(PowerPcAsm.getOpcodeParameter(opcode) + 1);
        }
        protected override bool readIsVanilla(EndianBinaryReader stream, AddressMapper addressMapper)
        {
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x8007e130), SeekOrigin.Begin);
            var opcode = stream.ReadUInt32();
            return opcode == PowerPcAsm.li(5, 0);
        }
    }
}
