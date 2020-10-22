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
        private FreeSpaceManager freeSpaceManager;
        private IProgress<ProgressInfo> progress;
        private EndianBinaryWriter stream;
        private Func<uint, int> toFileAddress;
        public void write(EndianBinaryWriter stream, Func<uint, int> toFileAddress, List<MapDescriptor> mapDescriptors, FreeSpaceManager freeSpaceManager, IProgress<ProgressInfo> progress)
        {
            this.freeSpaceManager = freeSpaceManager;
            this.progress = progress;
            this.stream = stream;
            this.toFileAddress = toFileAddress;
            try
            {
                UInt32 tableAddr;
                string purpose = "";
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    EndianBinaryWriter s = new EndianBinaryWriter(EndianBitConverter.Big, memoryStream);
                    purpose = writeTable(s, mapDescriptors);

                    tableAddr = freeSpaceManager.allocateUnusedSpace(memoryStream.ToArray(), stream, toFileAddress, progress, purpose);
                }
                writeAsm(stream, toFileAddress, (short)mapDescriptors.Count, tableAddr);
            }
            finally
            {
                this.freeSpaceManager = null;
                this.progress = null;
                this.stream = null;
                this.toFileAddress = null;
            }
        }
        protected abstract string writeTable(EndianBinaryWriter s, List<MapDescriptor> mapDescriptors);
        protected abstract void writeAsm(EndianBinaryWriter stream, Func<uint, int> toFileAddress, Int16 tableRowCount, UInt32 tableAddr);
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
            readTable(stream, mapDescriptors, toFileAddress, isVanilla);
        }
        protected abstract UInt32 readTableAddr(EndianBinaryReader stream, Func<uint, int> toFileAddress, bool isVanilla);
        protected abstract Int16 readTableRowCount(EndianBinaryReader stream, Func<uint, int> toFileAddress, bool isVanilla);
        protected abstract void readTable(EndianBinaryReader s, List<MapDescriptor> mapDescriptors, Func<uint, int> toFileAddress, bool isVanilla);
        protected abstract bool readIsVanilla(EndianBinaryReader stream, Func<uint, int> toFileAddress);

        protected UInt32 allocateString(string str, string purpose)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                EndianBinaryWriter s = new EndianBinaryWriter(EndianBitConverter.Big, memoryStream);
                s.Write(str);
                s.Write((byte)0);
                return freeSpaceManager.allocateUnusedSpace(memoryStream.ToArray(), stream, toFileAddress, progress);
            }
        }
        protected UInt32 allocateData(byte[] bytes, string purpose)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                EndianBinaryWriter s = new EndianBinaryWriter(EndianBitConverter.Big, memoryStream);
                s.Write(bytes);
                return freeSpaceManager.allocateUnusedSpace(memoryStream.ToArray(), stream, toFileAddress, progress, purpose);
            }
        }
        protected UInt32 allocateSubroutine(List<UInt32> opcodes, string purpose)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                EndianBinaryWriter s = new EndianBinaryWriter(EndianBitConverter.Big, memoryStream);
                foreach (UInt32 opcode in opcodes)
                    s.Write(opcode);
                return freeSpaceManager.allocateUnusedSpace(memoryStream.ToArray(), stream, toFileAddress, progress, purpose);
            }
        }
        protected string resolveAddressToString(uint virtualAddress, EndianBinaryReader stream, Func<uint, int> toFileAddress)
        {
            int fileAddress = toFileAddress(virtualAddress);
            if (fileAddress >= 0)
            {
                stream.Seek(fileAddress, SeekOrigin.Begin);
                byte[] buff = stream.ReadBytes(64);
                return HexUtil.byteArrayToString(buff);
            }
            else
            {
                return null;
            }
        }
    }
}
