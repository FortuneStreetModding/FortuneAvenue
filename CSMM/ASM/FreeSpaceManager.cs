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
        private Dictionary<UInt32, UInt32> remainingFreeSpaceBlocks = new Dictionary<UInt32, UInt32>();
        private readonly Dictionary<UInt32, UInt32> totalFreeSpaceBlocks = new Dictionary<UInt32, UInt32>();
        private readonly Dictionary<byte[], UInt32> reuseValues = new Dictionary<byte[], UInt32>();
        private bool startedAllocating = false;

        public void addFreeSpace(UInt32 start, UInt32 end)
        {
            if (startedAllocating)
                throw new InvalidOperationException("Can't add more free space after calling allocateUnusedSpace()");
            totalFreeSpaceBlocks.Add(end, start);
            remainingFreeSpaceBlocks.Add(end, start);
        }

        private UInt32 findSuitableFreeSpaceBlock(int requiredSize)
        {
            // search for a suitable free space where it fits best (fill smallest free space blocks first which still hold the space)
            int smallestFreeSpaceBlockSize = int.MaxValue;
            var smallestFreeSpaceBlockEnd = uint.MaxValue;
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
            if (smallestFreeSpaceBlockEnd == uint.MaxValue)
            {
                throw new InsufficientMemoryException("Could not find a suitable free space block in the main.dol to allocate " + requiredSize + " bytes.");
            }
            return smallestFreeSpaceBlockEnd;
        }

        private int calculateFreeSpace(Dictionary<UInt32, UInt32> freeSpaceBlocks)
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

        private UInt32 findLargestFreeSpaceBlock(Dictionary<UInt32, UInt32> freeSpaceBlocks)
        {
            int largestFreeSpaceBlockSize = -1;
            UInt32 largestFreeSpaceBlockEnd = uint.MaxValue;
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
            if (largestFreeSpaceBlockEnd == uint.MaxValue)
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

        public UInt32 allocateUnusedSpace(byte[] bytes, EndianBinaryWriter stream, Func<UInt32, int> toFileAddress, IProgress<ProgressInfo> progress)
        {
            return allocateUnusedSpace(bytes, stream, toFileAddress, progress, "");
        }

        public UInt32 allocateUnusedSpace(byte[] bytes, EndianBinaryWriter stream, Func<UInt32, int> toFileAddress, IProgress<ProgressInfo> progress, string purpose)
        {
            if (!string.IsNullOrEmpty(purpose))
            {
                purpose = " for " + purpose;
            }
            startedAllocating = true;
            string str = HexUtil.byteArrayToStringOrHex(bytes);
            if (reuseValues.ContainsKey(bytes))
            {
                progress?.Report(new ProgressInfo("Reuse " + str + " at " + reuseValues[bytes].ToString("X") + purpose, true));
                return reuseValues[bytes];
            }
            else
            {
                UInt32 end = findSuitableFreeSpaceBlock(bytes.Length);
                UInt32 start = remainingFreeSpaceBlocks[end];
                UInt32 virtualAddr = start;

                UInt32 newStartPos = start + (UInt32)bytes.Length;
                while (newStartPos % 4 != 0)
                    newStartPos++;
                if (newStartPos > end)
                    newStartPos = end;
                remainingFreeSpaceBlocks[end] = newStartPos;

                stream.Seek(toFileAddress(virtualAddr), SeekOrigin.Begin);
                stream.Write(bytes);
                byte[] fillUpToAlign = new byte[newStartPos - virtualAddr - bytes.Length];
                stream.Write(fillUpToAlign);

                reuseValues.Add(bytes, virtualAddr);
                progress?.Report(new ProgressInfo("Allocate " + str + " (" + bytes.Length + " bytes) at " + virtualAddr.ToString("X") + purpose, true));
                return virtualAddr;
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
        /// <param name="toFileAddress"></param>
        public void nullTheFreeSpace(EndianBinaryWriter stream, Func<UInt32, int> toFileAddress)
        {
            byte[] nullBytes;
            foreach (var entry in remainingFreeSpaceBlocks)
            {
                var start = entry.Value;
                var end = entry.Key;
                var freeSpaceSize = end - start;
                stream.Seek(toFileAddress(start), SeekOrigin.Begin);
                nullBytes = new byte[freeSpaceSize];
                stream.Write(nullBytes);
            }
        }

        public void reset()
        {
            this.remainingFreeSpaceBlocks = new Dictionary<UInt32, UInt32>(totalFreeSpaceBlocks);
            reuseValues.Clear();
            startedAllocating = false;
        }
    }
}
