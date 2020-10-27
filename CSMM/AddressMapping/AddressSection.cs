using System;
using System.Collections.Generic;
using System.Text;

namespace CustomStreetManager
{
    public class AddressSection
    {
        public long offsetBeg { get; set; }
        public long offsetEnd { get; set; }
        public long fileDelta { get; set; }
        public string sectionName { get; set; }

        public AddressSection(long offsetBeg, long offsetEnd, long fileDelta, string sectionName)
        {
            this.offsetBeg = offsetBeg;
            this.offsetEnd = offsetEnd;
            this.fileDelta = fileDelta;
            this.sectionName = sectionName;
        }

        public static AddressSection identity()
        {
            return new AddressSection(long.MinValue, long.MaxValue, 0, "Identity");
        }

        internal bool containsVirtualAddress(long virtualAddress)
        {
            if (offsetBeg <= virtualAddress && virtualAddress <= offsetEnd)
            {
                return true;
            }
            return false;
        }

        internal long toFileAddress(long virtualAddress)
        {
            return virtualAddress - fileDelta;
        }

        internal bool containsFileAddress(long fileAddress)
        {
            return containsVirtualAddress(toVirtualAddress(fileAddress));
        }

        internal long toVirtualAddress(long fileAddress)
        {
            return fileAddress + fileDelta;
        }
    }
}
