using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomStreetManager
{
    public struct ProgressInfo
    {
        public int progress;
        public string stdLine;
        public string errLine;

        public static implicit operator ProgressInfo(int value)
        {
            return new ProgressInfo() { progress = value, stdLine = null, errLine = null };
        }

        public static implicit operator ProgressInfo(string value)
        {
            return new ProgressInfo() { progress = -1, stdLine = value, errLine = null };
        }

        public static int lerp(float v0, float v1, float t)
        {
            return (int)((1 - t) * v0 + t * v1);
        }
    }
}
