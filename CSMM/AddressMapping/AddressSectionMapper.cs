using System;
using System.Collections.Generic;
using System.Text;

namespace CustomStreetManager
{
    internal class AddressSectionMapper
    {
        private List<AddressSection> sections;

        internal AddressSectionMapper(List<AddressSection> sections)
        {
            this.sections = sections;
        }

        internal void setSections(List<AddressSection> sections)
        {
            this.sections = sections;
        }

        internal bool sectionAvailable(string sectionName)
        {
            foreach (AddressSection section in sections)
            {
                if (String.Equals(section.sectionName, sectionName, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        internal AddressSection findSection(long address)
        {
            foreach (AddressSection section in sections)
            {
                if (section.containsVirtualAddress(address))
                {
                    return section;
                }
            }
            return null;
        }

        internal long map(long address)
        {
            var section = findSection(address);
            if (section != null)
            {
                return (int)section.toFileAddress(address);
            }
            throw new IndexOutOfRangeException("Address " + address + " can not be mapped");
        }

        internal long inverseMap(long address)
        {
            foreach (AddressSection section in sections)
            {
                if (section.containsFileAddress(address))
                {
                    return (int)section.toVirtualAddress(address);
                }
            }
            return -1;
        }
    }

}
