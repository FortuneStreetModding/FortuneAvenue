using MiscUtil.Conversion;
using MiscUtil.IO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomStreetMapManager
{
    public class RuleSetTable : DolIOTable
    {
        protected VAVAddr writeTable(List<MapDescriptor> mapDescriptors)
        {
            var ruleSetTable = new List<UInt32>();
            foreach (var mapDescriptor in mapDescriptors)
            {
                ruleSetTable.Add((UInt32)mapDescriptor.RuleSet);
            }
            return allocate(ruleSetTable, "RuleSetTable");
        }
        protected override void writeAsm(EndianBinaryWriter stream, AddressMapper addressMapper, List<MapDescriptor> mapDescriptors)
        {
            var tableAddr = writeTable(mapDescriptors);
            PowerPcAsm.Pair16Bit v = PowerPcAsm.make16bitValuePair((UInt32)tableAddr);

            // --- Update Table Addr ---
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x801cca98), SeekOrigin.Begin);
            // mulli r0,r3,0x38 ->  mulli r0,r3,0x04
            stream.Write(PowerPcAsm.mulli(0, 3, 0x04));
            // r3 <- 0x80428e50 ->  r3 <- tableAddr
            stream.Write(PowerPcAsm.lis(3, v.upper16Bit)); stream.Write(PowerPcAsm.addi(3, 3, v.lower16Bit));
            stream.Seek(0x4, SeekOrigin.Current);
            // lwz r3,0x10(r3)  ->  lwz r3,0x0(r3)
            stream.Write(PowerPcAsm.lwz(3, 0, 3));

            // --- ASM hack: Use the rule set from map instead of from global setting ---
            var ruleSetFromMapRoutine = allocate(writeRuleSetFromMapRoutine(addressMapper, (VAVAddr)0), "writeRuleSetFromMapRoutine");
            stream.Seek(addressMapper.toFileAddress(ruleSetFromMapRoutine), SeekOrigin.Begin);
            stream.Write(writeRuleSetFromMapRoutine(addressMapper, ruleSetFromMapRoutine)); // re-write the routine again since now we know where it is located in the main dol

            var virtualPos = addressMapper.toVersionAgnosticAddress((BSVAddr)0x8007e13c);
            stream.Seek(addressMapper.toFileAddress(virtualPos), SeekOrigin.Begin);
            // lha r3,0x3c(r30)  -> bl ruleSetFromMapRoutine
            stream.Write(PowerPcAsm.bl(virtualPos, ruleSetFromMapRoutine));
            // cmpwi r23,0x0     -> lha r3,0x3c(r30)
            stream.Write(PowerPcAsm.lha(3, 0x3c, 30));
            // lha r0,0x28(r30)  -> cmpwi r23,0x0
            stream.Write(PowerPcAsm.cmpwi(23, 0x0));
            // stw r25,0x53f4(r29) -> lha r0,0x28(r30)
            stream.Write(PowerPcAsm.lha(0, 0x28, 30));
        }

        private List<UInt32> writeRuleSetFromMapRoutine(AddressMapper addressMapper, VAVAddr routineStartAddress)
        {
            var Game_GetRuleFlag = addressMapper.toVersionAgnosticAddress((BSVAddr)0x801cca98);

            // precondition: r24 is mapId
            // precondition: r25 is global rule set which we are gonna use to store the linkreturn
            var asm = new List<UInt32>();
            asm.Add(PowerPcAsm.mflr(25));
            asm.Add(PowerPcAsm.mr(3, 24));                                             // r3 <- r24
            asm.Add(PowerPcAsm.bl(routineStartAddress, asm.Count, Game_GetRuleFlag));  // r3 <- bl Game_GetRuleFlag(r3)
            asm.Add(PowerPcAsm.stw(3, 0x53f4, 29));                                    // gameRule <- r3
            asm.Add(PowerPcAsm.mtlr(25));
            asm.Add(PowerPcAsm.blr());                                                 // return
            return asm;
        }

        protected override void readAsm(EndianBinaryReader s, List<MapDescriptor> mapDescriptors, AddressMapper addressMapper, bool isVanilla)
        {
            if (isVanilla) {
                s.Seek(0x10, SeekOrigin.Current); // offset
            }
            foreach (var mapDescriptor in mapDescriptors)
            {
                mapDescriptor.RuleSet = (RuleSet)s.ReadUInt32();
                if (isVanilla)
                {
                    // in vanilla main.dol the table has other stuff in it like bgm id, map frb files, etc.
                    // this we need to skip to go the next target amount in the table
                    s.Seek(0x38 - 0x04, SeekOrigin.Current);
                }
            }
        }
        protected override VAVAddr readTableAddr(EndianBinaryReader stream, AddressMapper addressMapper, bool isVanilla)
        {
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x801cca9c), SeekOrigin.Begin);
            var lis_opcode = stream.ReadUInt32();
            var addi_opcode = stream.ReadUInt32();
            return (VAVAddr)PowerPcAsm.make32bitValueFromPair(lis_opcode, addi_opcode);
        }
        protected override Int16 readTableRowCount(EndianBinaryReader stream, AddressMapper addressMapper, bool isVanilla)
        {
            return -1;
        }
        protected override bool readIsVanilla(EndianBinaryReader stream, AddressMapper addressMapper)
        {
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x801cca98), SeekOrigin.Begin);
            var opcode = stream.ReadUInt32();
            // mulli r0,r3,0x38
            return opcode == PowerPcAsm.mulli(0, 3, 0x38);
        }
    }
}
