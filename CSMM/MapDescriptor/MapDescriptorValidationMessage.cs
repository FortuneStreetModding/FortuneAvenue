using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;

namespace CustomStreetMapManager
{
    public class MapDescriptorValidationMessage
    {
        public readonly int index;
        public readonly PropertyInfo prop;
        public readonly string reason;
        public MapDescriptorValidationMessage(int i, PropertyInfo prop, string reason)
        {
            this.index = i;
            this.prop = prop;
            this.reason = reason;
        }

        public MapDescriptorValidationMessage(PropertyInfo prop, string reason)
        {
            this.index = -1;
            this.prop = prop;
            this.reason = reason;
        }

        public override string ToString()
        {
            if (index == -1)
            {
                return prop.Name + " is invalid: " + reason;
            }
            else
            {
                return "At index " + index + " " + prop.Name + " is invalid: " + reason;
            }
        }
    }
}
