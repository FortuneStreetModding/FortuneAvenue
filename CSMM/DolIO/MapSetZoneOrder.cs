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
    public class MapSetZoneOrder : DolIO
    {
        protected override void writeAsm(EndianBinaryWriter stream, AddressMapper addressMapper, List<MapDescriptor> mapDescriptors)
        {
            // --- Game::GameSequenceDataAdapter::GetNumMapsInZone ---
            var addr = addressMapper.toVersionAgnosticAddress((BSVAddr)0x8020f3ac);
            stream.Seek(addressMapper.toFileAddress(addr), SeekOrigin.Begin);
            // li r3,0x6 ->  b yes
            stream.Write(PowerPcAsm.b(addr, (VAVAddr)0));
            // --- Game::GameSequenceDataAdapter::GetMapsInZone ---

        }
        protected override void readAsm(EndianBinaryReader stream, List<MapDescriptor> mapDescriptors, AddressMapper addressMapper)
        {
            var isVanilla = readIsVanilla(stream, addressMapper);
            if(isVanilla)
            {
                setVanillaMapSetZoneOrder(mapDescriptors);
            }
        }

        private void setVanillaMapSetZoneOrder(List<MapDescriptor> mapDescriptors)
        {
            for(int i=0;i<mapDescriptors.Count;i++)
            {
                var mapDescriptor = mapDescriptors[i];
                mapDescriptor.MapSet = VanillaDatabase.getVanillaMapSet(i);
                mapDescriptor.Zone = VanillaDatabase.getVanillaZone(i);
                mapDescriptor.Order = VanillaDatabase.getVanillaOrder(i);
            }
        }

        protected bool readIsVanilla(EndianBinaryReader stream, AddressMapper addressMapper)
        {
            return true;
        }
    }
}
