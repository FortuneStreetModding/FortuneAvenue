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
    public class FreeSpaceManager
    {
        /// <summary>
        /// Maps the end of a free space region to its start.
        /// </summary>
        private Dictionary<VAVAddr, VAVAddr> remainingFreeSpaceBlocks = new Dictionary<VAVAddr, VAVAddr>();
        private readonly Dictionary<VAVAddr, VAVAddr> totalFreeSpaceBlocks = new Dictionary<VAVAddr, VAVAddr>();
        private readonly Dictionary<byte[], VAVAddr> reuseValues = new Dictionary<byte[], VAVAddr>();
        private bool startedAllocating = false;

        public void addFreeSpace(VAVAddr start, VAVAddr end)
        {
            if (startedAllocating)
                throw new InvalidOperationException("Can't add more free space after calling allocateUnusedSpace()");
            totalFreeSpaceBlocks.Add(end, start);
            remainingFreeSpaceBlocks.Add(end, start);
        }

        private VAVAddr findSuitableFreeSpaceBlock(int requiredSize)
        {
            // search for a suitable free space where it fits best (fill smallest free space blocks first which still hold the space)
            int smallestFreeSpaceBlockSize = int.MaxValue;
            VAVAddr smallestFreeSpaceBlockEnd = VAVAddr.MaxValue;
            foreach (var entry in remainingFreeSpaceBlocks)
            {
                var start = entry.Value;
                var end = entry.Key;
                int freeSpaceBlockSize = (int)(end - start);
                if (freeSpaceBlockSize < smallestFreeSpaceBlockSize && freeSpaceBlockSize >= requiredSize)
                {
                    smallestFreeSpaceBlockSize = freeSpaceBlockSize;
                    smallestFreeSpaceBlockEnd = end;
                }
            }
            if (smallestFreeSpaceBlockEnd == VAVAddr.MaxValue)
            {
                throw new InsufficientMemoryException("Could not find a suitable free space block in the main.dol to allocate " + requiredSize + " bytes.");
            }
            return smallestFreeSpaceBlockEnd;
        }

        private int calculateFreeSpace(Dictionary<VAVAddr, VAVAddr> freeSpaceBlocks)
        {
            int freeSpaceLeft = 0;
            foreach (var entry in freeSpaceBlocks)
            {
                var start = entry.Value;
                var end = entry.Key;
                var freeSpaceSize = end - start;
                freeSpaceLeft += (int)freeSpaceSize;
            }
            return freeSpaceLeft;
        }

        public int calculateTotalRemainingFreeSpace()
        {
            return calculateFreeSpace(remainingFreeSpaceBlocks);
        }

        public int calculateTotalFreeSpace()
        {
            return calculateFreeSpace(totalFreeSpaceBlocks);
        }

        private VAVAddr findLargestFreeSpaceBlock(Dictionary<VAVAddr, VAVAddr> freeSpaceBlocks)
        {
            int largestFreeSpaceBlockSize = -1;
            VAVAddr largestFreeSpaceBlockEnd = (VAVAddr)uint.MaxValue;
            foreach (var entry in remainingFreeSpaceBlocks)
            {
                var start = entry.Value;
                var end = entry.Key;
                int freeSpaceBlockSize = (int)(end - start);
                if (freeSpaceBlockSize > largestFreeSpaceBlockSize)
                {
                    largestFreeSpaceBlockSize = freeSpaceBlockSize;
                    largestFreeSpaceBlockEnd = end;
                }
            }
            if (largestFreeSpaceBlockEnd == (VAVAddr)uint.MaxValue)
            {
                throw new InsufficientMemoryException("Could not determine the largest free space block.");
            }
            return largestFreeSpaceBlockEnd;
        }

        public int calculateLargestFreeSpaceBlockSize()
        {
            var end = findLargestFreeSpaceBlock(totalFreeSpaceBlocks);
            var start = totalFreeSpaceBlocks[end];
            return (int)(end - start);
        }

        public int calculateLargestRemainingFreeSpaceBlockSize()
        {
            var end = findLargestFreeSpaceBlock(remainingFreeSpaceBlocks);
            var start = totalFreeSpaceBlocks[end];
            return (int)(end - start);
        }

        public VAVAddr allocateUnusedSpace(byte[] bytes, EndianBinaryWriter stream, AddressMapper fileMapper, IProgress<ProgressInfo> progress)
        {
            return allocateUnusedSpace(bytes, stream, fileMapper, progress, "");
        }

        public VAVAddr allocateUnusedSpace(byte[] bytes, EndianBinaryWriter stream, AddressMapper fileMapper, IProgress<ProgressInfo> progress, string purpose)
        {
            if (!string.IsNullOrEmpty(purpose))
            {
                purpose = " for " + purpose;
            }
            startedAllocating = true;
            string str = HexUtil.byteArrayToStringOrHex(bytes);
            if (reuseValues.ContainsKey(bytes))
            {
                progress?.Report(new ProgressInfo("Reuse " + str + " at " + reuseValues[bytes].ToString() + purpose, true));
                return reuseValues[bytes];
            }
            else
            {
                VAVAddr end = findSuitableFreeSpaceBlock(bytes.Length);
                VAVAddr start = remainingFreeSpaceBlocks[end];
                VAVAddr addr = start;

                VAVAddr newStartPos = start + bytes.Length;
                while (newStartPos % 4 != 0)
                    newStartPos++;
                if (newStartPos > end)
                    newStartPos = end;
                remainingFreeSpaceBlocks[end] = newStartPos;

                stream.Seek(fileMapper.toFileAddress(addr), SeekOrigin.Begin);
                stream.Write(bytes);
                byte[] fillUpToAlign = new byte[(int)(newStartPos - addr - bytes.Length)];
                stream.Write(fillUpToAlign);

                reuseValues.Add(bytes, addr);
                progress?.Report(new ProgressInfo("Allocate " + str + " (" + bytes.Length + " bytes) at " + addr.ToString() + purpose, true));
                return addr;
            }
        }
        /// <summary>
        /// This method writes 0x00 to all remaining free space in the main.dol. The reason we do this 
        /// is so that we can fail-fast if one of the determined free-space locations turn out to be used 
        /// after all (the main.dol will probably crash in that case or have some other strange behavior).
        /// If we do not do this, then this might turn into a heisenbug where some rare users experience this
        /// bug and others do not, making it harder to find the root cause.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="addressMapper"></param>
        public void nullTheFreeSpace(EndianBinaryWriter stream, AddressMapper addressMapper)
        {
            byte[] nullBytes;
            foreach (var entry in remainingFreeSpaceBlocks)
            {
                var start = entry.Value;
                var end = entry.Key;
                var freeSpaceSize = end - start;
                stream.Seek(addressMapper.toFileAddress(start), SeekOrigin.Begin);
                nullBytes = new byte[freeSpaceSize];
                stream.Write(nullBytes);
            }
        }

        public void reset()
        {
            this.remainingFreeSpaceBlocks = new Dictionary<VAVAddr, VAVAddr>(totalFreeSpaceBlocks);
            reuseValues.Clear();
            startedAllocating = false;
        }
    }
}
