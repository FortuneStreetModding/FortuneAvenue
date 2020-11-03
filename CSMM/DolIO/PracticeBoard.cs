using MiscUtil.Conversion;
using MiscUtil.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CustomStreetManager
{
    public class PracticeBoard : DolIO
    {
        protected override void writeAsm(EndianBinaryWriter stream, AddressMapper addressMapper, List<MapDescriptor> mapDescriptors)
        {
            short easyPracticeBoard = -1;
            short standardPracticeBoard = -1;
            for (short i = 0; i < mapDescriptors.Count; i++)
            {
                var mapDescriptor = mapDescriptors[i];
                if (mapDescriptor.IsPracticeBoard)
                {
                    if (mapDescriptor.RuleSet == RuleSet.Easy)
                    {
                        if (easyPracticeBoard == -1)
                        {
                            easyPracticeBoard = i;
                        }
                        else
                        {
                            throw new ArgumentException("Only one stage with easy rule set can be set as practice board.");
                        }
                    }
                    else
                    {
                        if (standardPracticeBoard == -1)
                        {
                            standardPracticeBoard = i;
                        }
                        else
                        {
                            throw new ArgumentException("Only one stage with standard rule set can be set as practice board.");
                        }
                    }
                }
            }
            // li r0,0x29                                                                 -> li r0,easyPracticeBoard
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x80173bf8), SeekOrigin.Begin); stream.Write(PowerPcAsm.li(0, easyPracticeBoard));
            // li r0,0x14                                                                 -> li r0,standardPracticeBoard
            stream.Seek(addressMapper.toFileAddress((BSVAddr)0x80173c04), SeekOrigin.Begin); stream.Write(PowerPcAsm.li(0, standardPracticeBoard));
        }
        protected override void readAsm(EndianBinaryReader stream, List<MapDescriptor> mapDescriptors, AddressMapper addressMapper)
        {
            stream.Seek(addressMapper.toFileAddress((BSVAddr)(0x80173bf8 + 2)), SeekOrigin.Begin);
            var easyPracticeBoard = stream.ReadInt16();
            stream.Seek(addressMapper.toFileAddress((BSVAddr)(0x80173c04 + 2)), SeekOrigin.Begin);
            var standardPracticeBoard = stream.ReadInt16();

            mapDescriptors[easyPracticeBoard].IsPracticeBoard = true;
            mapDescriptors[standardPracticeBoard].IsPracticeBoard = true;
        }
    }
}
