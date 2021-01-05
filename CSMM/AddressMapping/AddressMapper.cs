using System;
using System.Collections.Generic;
using System.Text;

namespace CustomStreetMapManager
{

    public class AddressMapper
    {
        AddressSectionMapper fileMapper;
        AddressSectionMapper versionMapper;
        public AddressMapper(List<AddressSection> fileMappingSections) {
            this.fileMapper = new AddressSectionMapper(fileMappingSections);
        }

        public void setVersionMappingSections(List<AddressSection> versionMappingSections)
        {
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

        internal BSVAddr fileAddressToBoomStreetVirtualAddress(long address)
        {
            return (BSVAddr)versionMapper.inverseMap(fileMapper.inverseMap(address));
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
