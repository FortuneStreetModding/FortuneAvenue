using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomStreetManager
{
    public class OriginPoint
    {
        public Single X { get; set; }
        public Single Y { get; set; }

        public OriginPoint()
        {
        }

        public OriginPoint(Single x, Single y)
        {
            this.X = x;
            this.Y = y;
        }

        public override string ToString()
        {
            
            return "(" + X.ToString(CultureInfo.InvariantCulture) + "," + Y.ToString(CultureInfo.InvariantCulture) + ")";
        }
    }
}
