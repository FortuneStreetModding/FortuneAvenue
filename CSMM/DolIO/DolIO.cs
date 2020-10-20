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
    public abstract class DolIO
    {
        public void write(EndianBinaryWriter stream, Func<uint, int> toFileAddress, List<MapDescriptor> mapDescriptors, FreeSpaceManager freeSpaceManager, IProgress<ProgressInfo> progress)
        {
            UInt32 tableAddr;
            string purpose = "";
            using (MemoryStream memoryStream = new MemoryStream())
            {
                EndianBinaryWriter s = new EndianBinaryWriter(EndianBitConverter.Big, memoryStream);
                purpose = writeTable(s, mapDescriptors);

                tableAddr = freeSpaceManager.allocateUnusedSpace(memoryStream.ToArray(), stream, toFileAddress, progress, purpose);
            }
            UInt32 dataAddr = 0;
            using (MemoryStream memoryStream = new MemoryStream())
            {
                EndianBinaryWriter s = new EndianBinaryWriter(EndianBitConverter.Big, memoryStream);
                purpose = writeData(s);
                var bytes = memoryStream.ToArray();
                if (bytes.Length > 0)
                {
                    dataAddr = freeSpaceManager.allocateUnusedSpace(bytes, stream, toFileAddress, progress, purpose);
                }
            }
            UInt32 subroutineAddr = 0;
            using (MemoryStream memoryStream = new MemoryStream())
            {
                EndianBinaryWriter s = new EndianBinaryWriter(EndianBitConverter.Big, memoryStream);
                purpose = writeSubroutine(s, dataAddr);
                var bytes = memoryStream.ToArray();
                if (bytes.Length > 0)
                {
                    subroutineAddr = freeSpaceManager.allocateUnusedSpace(bytes, stream, toFileAddress, progress, purpose);
                }
            }
            writeTableRefs(stream, toFileAddress, (short)mapDescriptors.Count, tableAddr, dataAddr, subroutineAddr);
        }
        protected abstract string writeTable(EndianBinaryWriter s, List<MapDescriptor> mapDescriptors);
        protected virtual string writeData(EndianBinaryWriter s) { return null; }
        protected virtual string writeSubroutine(EndianBinaryWriter s, UInt32 dataAddr) { return null; }
        protected abstract void writeTableRefs(EndianBinaryWriter stream, Func<uint, int> toFileAddress, Int16 tableRowCount, UInt32 tableAddr, UInt32 dataAddr, UInt32 subroutineAddr);
        public void read(EndianBinaryReader stream, Func<uint, int> toFileAddress, List<MapDescriptor> mapDescriptors, IProgress<ProgressInfo> progress)
        {
            var isVanilla = readIsVanilla(stream, toFileAddress);
            var rowCount = readTableRowCount(stream, toFileAddress, isVanilla);
            if (rowCount != mapDescriptors.Count)
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
            var addr = readTableAddr(stream, toFileAddress, isVanilla);
            stream.Seek(toFileAddress(addr), SeekOrigin.Begin);
            readTable(stream, mapDescriptors, isVanilla);
        }
        protected abstract UInt32 readTableAddr(EndianBinaryReader stream, Func<uint, int> toFileAddress, bool isVanilla);
        protected abstract Int16 readTableRowCount(EndianBinaryReader stream, Func<uint, int> toFileAddress, bool isVanilla);
        protected abstract void readTable(EndianBinaryReader s, List<MapDescriptor> mapDescriptors, bool isVanilla);
        protected abstract bool readIsVanilla(EndianBinaryReader stream, Func<uint, int> toFileAddress);
    }
}
