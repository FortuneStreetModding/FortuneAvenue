using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomStreetManager
{
    public class MainDolSection
    {
        public string sectionName { get; set; }
        public long offsetBeg { get; set; }
        public long offsetEnd { get; set; }
        public long fileDelta { get; set; }

        internal bool containsVirtualAddress(long virtualAddress)
        {
            if(offsetBeg <= virtualAddress && virtualAddress <= offsetEnd)
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
