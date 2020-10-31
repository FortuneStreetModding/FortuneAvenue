using MiscUtil.Conversion;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CustomStreetManager
{
    public class ST7P01toST7E01SymbolMapDeltaCalculator
    {
        public void createMapping()
        {
            var st7p01 = readMapFile(@"ST7P01.map");
            var st7e01 = readMapFile(@"ST7E01.map");
            var sections = new List<AddressSection>();
            // make a reverse sorted list -> we will be making the sections from the end of the virtual address map to the beginning
            List<long> bsvAddrList = new List<long>(st7p01.Keys);
            bsvAddrList.Sort();
            bsvAddrList.Reverse();
            long previousBsvAddr = -1;
            List<long> fsvAddrList = new List<long>(st7e01.Keys);
            fsvAddrList.Sort();
            fsvAddrList.Reverse();
            foreach (var bsvAddr in bsvAddrList)
            {
                // we need a previous bsvaddr, otherwise we cant find out the size of the section
                if (previousBsvAddr != -1)
                {
                    var symbolName = st7p01[bsvAddr];
                    // we only make a mapping with symbols which are known (symbols which start with "zz_" are unknown)
                    if (!symbolName.StartsWith("zz_"))
                    {
                        var size = previousBsvAddr - bsvAddr - 1;

                        // find a symbol with the same name in st7e01 (the range should be around the same +/- 200
                        foreach (var fsvAddr in fsvAddrList)
                        {
                            if (st7e01[fsvAddr] == symbolName && bsvAddr - 200 < fsvAddr && fsvAddr < bsvAddr + 200)
                            {
                                // delete the addres we found so that we do not use it again (there are several addresses which share a symbol map)
                                fsvAddrList.Remove(fsvAddr);
                                var delta = bsvAddr - fsvAddr;
                                var section = new AddressSection(bsvAddr, bsvAddr + size, delta, symbolName);
                                sections.Add(section);
                                break;
                            }
                        }
                    }
                }
                previousBsvAddr = bsvAddr;
            }
            // write result to text file
            using (var writer = new StreamWriter(@"ST7P01_to_ST7E01_delta.txt"))
            {
                foreach (var section in sections)
                {
                    var line = String.Format(" {0,10} .. {1,10} : {2,10} : {3,-10}", section.offsetBeg.ToString("X"), section.offsetEnd.ToString("X"), section.fileDelta.ToString("X"), section.sectionName);
                    writer.WriteLine(line);
                }
            }
        }

        private SortedDictionary<long, string> readMapFile(string mapFile)
        {
            using (var reader = new StreamReader(mapFile))
            {
                var symbolMap = new SortedDictionary<long, string>();
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(' ');
                    if (values.Length != 5)
                        continue;
                    long offsetBeg = EndianBitConverter.Big.ToUInt32(HexUtil.hexStringToByteArray(values[0]), 0);
                    symbolMap.Add(offsetBeg, values[4].Trim());
                }
                return symbolMap;
            }
        }
    }
}
