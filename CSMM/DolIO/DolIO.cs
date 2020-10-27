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
        private FreeSpaceManager _fsm;
        private IProgress<ProgressInfo> _p;
        private EndianBinaryWriter _w;
        private AddressMapper _am;
        public void write(EndianBinaryWriter stream, AddressMapper addressMapper, List<MapDescriptor> mapDescriptors, FreeSpaceManager freeSpaceManager, IProgress<ProgressInfo> progress)
        {
            this._fsm = freeSpaceManager;
            this._p = progress;
            this._w = stream;
            this._am = addressMapper;
            writeAsm(stream, addressMapper, mapDescriptors);
            this._fsm = null;
            this._p = null;
            this._w = null;
            this._am = null;
        }
        protected abstract void writeAsm(EndianBinaryWriter writer, AddressMapper addressMapper, List<MapDescriptor> mapDescriptors);
        public void read(EndianBinaryReader stream, AddressMapper addressMapper, List<MapDescriptor> mapDescriptors, IProgress<ProgressInfo> progress)
        {
            var isVanilla = readIsVanilla(stream, addressMapper);
            var rowCount = readTableRowCount(stream, addressMapper, isVanilla);
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
            var addr = readTableAddr(stream, addressMapper, isVanilla);
            stream.Seek(addressMapper.toFileAddress(addr), SeekOrigin.Begin);
            readTable(stream, mapDescriptors, addressMapper, isVanilla);
        }
        protected abstract VAVAddr readTableAddr(EndianBinaryReader reader, AddressMapper addressMapper, bool isVanilla);
        protected abstract Int16 readTableRowCount(EndianBinaryReader reader, AddressMapper addressMapper, bool isVanilla);
        protected abstract void readTable(EndianBinaryReader reader, List<MapDescriptor> mapDescriptors, AddressMapper addressMapper, bool isVanilla);
        protected abstract bool readIsVanilla(EndianBinaryReader reader, AddressMapper addressMapper);

        protected VAVAddr allocate(string str, string purpose = null)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                EndianBinaryWriter s = new EndianBinaryWriter(EndianBitConverter.Big, memoryStream);
                s.Write(str);
                s.Write((byte)0);
                return _fsm.allocateUnusedSpace(memoryStream.ToArray(), _w, _am, _p, purpose);
            }
        }
        protected VAVAddr allocate(byte[] bytes, string purpose)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                EndianBinaryWriter s = new EndianBinaryWriter(EndianBitConverter.Big, memoryStream);
                s.Write(bytes);
                return _fsm.allocateUnusedSpace(memoryStream.ToArray(), _w, _am, _p, purpose);
            }
        }
        protected VAVAddr allocate(List<byte> bytes, string purpose)
        {
            return allocate(bytes.ToArray(), purpose);
        }
        protected VAVAddr allocate(List<UInt32> opcodes, string purpose)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                EndianBinaryWriter s = new EndianBinaryWriter(EndianBitConverter.Big, memoryStream);
                foreach (UInt32 opcode in opcodes)
                    s.Write(opcode);
                return _fsm.allocateUnusedSpace(memoryStream.ToArray(), _w, _am, _p, purpose);
            }
        }
        protected VAVAddr allocate(List<VAVAddr> addressTable, string purpose)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                EndianBinaryWriter s = new EndianBinaryWriter(EndianBitConverter.Big, memoryStream);
                foreach (VAVAddr address in addressTable)
                    s.Write((UInt32)address);
                return _fsm.allocateUnusedSpace(memoryStream.ToArray(), _w, _am, _p, purpose);
            }
        }
        protected string resolveAddressToString(VAVAddr virtualAddress, EndianBinaryReader stream, AddressMapper addressMapper)
        {
            if (virtualAddress == VAVAddr.NullAddress)
                return null;
            int fileAddress = addressMapper.toFileAddress(virtualAddress);
            stream.Seek(fileAddress, SeekOrigin.Begin);
            byte[] buff = stream.ReadBytes(64);
            return HexUtil.byteArrayToString(buff);
        }
    }
}
