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
            readAsm(stream, mapDescriptors, addressMapper);
        }
        protected abstract void readAsm(EndianBinaryReader reader, List<MapDescriptor> mapDescriptors, AddressMapper addressMapper);

        protected VAVAddr allocate(string str, string purpose = null)
        {
            if (string.IsNullOrEmpty(str))
                return VAVAddr.NullAddress;
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
                s.Write(opcodes);
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
            var pos = stream.BaseStream.Position;
            stream.Seek(fileAddress, SeekOrigin.Begin);
            byte[] buff = stream.ReadBytes(64);
            stream.BaseStream.Seek(pos, SeekOrigin.Begin);
            return HexUtil.byteArrayToString(buff);
        }
    }
}
