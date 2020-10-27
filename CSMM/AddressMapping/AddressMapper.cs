using System;
using System.Collections.Generic;
using System.Text;

namespace CustomStreetManager
{

    public class AddressMapper
    {
        AddressSectionMapper fileMapper;
        AddressSectionMapper versionMapper;
        public AddressMapper(List<AddressSection> fileMappingSections, List<AddressSection> versionMappingSections) {
            this.fileMapper = new AddressSectionMapper(fileMappingSections);
            this.versionMapper = new AddressSectionMapper(versionMappingSections);
        }
        public bool canConvertToFileAddress(VAVAddr versionAddress)
        {
            return fileMapper.findSection((long)versionAddress) != null;
        }
        public int toFileAddress(VAVAddr versionAddress)
        {
            return (int)fileMapper.map((long)versionAddress);
        }

        public bool canConvertToFileAddress(BSVAddr boomStreetVirtualAddress)
        {
            if(canConvertToVersionAgnosticAddress(boomStreetVirtualAddress))
            {
                return canConvertToFileAddress(toVersionAgnosticAddress(boomStreetVirtualAddress));
            }
            return false;
        }
        public int toFileAddress(BSVAddr boomStreetVirtualAddress)
        {
            return (int)fileMapper.map((long)toVersionAgnosticAddress(boomStreetVirtualAddress));
        }

        public bool canConvertToVersionAgnosticAddress(BSVAddr boomStreetVirtualAddress)
        {
            return versionMapper.findSection((long)boomStreetVirtualAddress) != null;
        }
        public VAVAddr toVersionAgnosticAddress(BSVAddr boomStreetVirtualAddress)
        {
            return (VAVAddr)versionMapper.map((long)boomStreetVirtualAddress);
        }
    }
}
