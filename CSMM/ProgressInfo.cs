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

        public ProgressInfo(int progress, string text)
        {
            this.progress = progress;
            this.stdLine = text;
            errLine = null;
        }

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
        public static int lerp(int min, int max, int value)
        {
            return lerp(min / 100f, max / 100f, value / 100f);
        }
        public static IProgress<ProgressInfo> makeSubProgress(IProgress<ProgressInfo> progress, int minProgress, int maxProgress)
        {
            return new Progress<ProgressInfo>(progressInfo =>
            {
                progressInfo.progress = ProgressInfo.lerp(minProgress, maxProgress, progressInfo.progress);
                progress.Report(progressInfo);
            });
        }
        public static IProgress<ProgressInfo> makeNoProgress(IProgress<ProgressInfo> progress)
        {
            return new Progress<ProgressInfo>(progressInfo =>
            {
                progressInfo.progress = -1;
                progress.Report(progressInfo);
            });
        }
    }
}
