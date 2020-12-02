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
    public abstract class DolIOTable : DolIO
    {
        protected override void readAsm(EndianBinaryReader stream, List<MapDescriptor> mapDescriptors, AddressMapper addressMapper)
        {
            var isVanilla = readIsVanilla(stream, addressMapper);
            var rowCount = readTableRowCount(stream, addressMapper, isVanilla);
            if (rowCount != -1 && rowCount != mapDescriptors.Count)
            {
                if (isVanilla)
                {
                    // in vanilla all kinds of strange stuff is there. E.g. 
                    // - there are 42 venture card tables but 48 maps. 
                    // - there are 48 maps but the ids get mapped to different values (e.g. easy map yoshi island index is 21 but mapped to 18 in some tables and in other tables mapped to 0)
                    //     so we cant really figure out the real amount of maps unless doing some complex logic
                }
                else
                {
                    // should not happen as with the hacks that we apply we streamline the tables and total map count so that they should always map
                    throw new ApplicationException("The amount of rows of the table in the main.dol is " + rowCount + " but the mapDescriptor count is " + mapDescriptors.Count);
                }
            }
            var addr = readTableAddr(stream, addressMapper, isVanilla);
            stream.Seek(addressMapper.toFileAddress(addr), SeekOrigin.Begin);
            readAsm(stream, mapDescriptors, addressMapper, isVanilla);
        }
        protected abstract void readAsm(EndianBinaryReader reader, List<MapDescriptor> mapDescriptors, AddressMapper addressMapper, bool isVanilla);
        protected abstract bool readIsVanilla(EndianBinaryReader reader, AddressMapper addressMapper);
        protected abstract Int16 readTableRowCount(EndianBinaryReader reader, AddressMapper addressMapper, bool isVanilla);
        protected abstract VAVAddr readTableAddr(EndianBinaryReader reader, AddressMapper addressMapper, bool isVanilla);
    }
}
