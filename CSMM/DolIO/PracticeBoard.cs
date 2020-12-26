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
            var validation = MapDescriptor.getPracticeBoards(mapDescriptors, out easyPracticeBoard, out standardPracticeBoard);
            if(!validation.Passed)
            {
                throw new ArgumentException(validation.GetMessage("\n"));
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
            mapDescriptors[easyPracticeBoard].MapSet = 0;
            mapDescriptors[standardPracticeBoard].IsPracticeBoard = true;
            mapDescriptors[standardPracticeBoard].MapSet = 1;
        }
    }
}
