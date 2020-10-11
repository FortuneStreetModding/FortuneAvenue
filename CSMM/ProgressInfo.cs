using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CustomStreetManager
{
    public struct ProgressInfo
    {
        public int progress;
        public string line;
        public bool verbose;

        public ProgressInfo(int progress, string text)
        {
            this.progress = progress;
            this.line = text;
            this.verbose = false;
        }
        public ProgressInfo(int progress, string text, bool verbose)
        {
            this.progress = progress;
            this.line = text;
            this.verbose = true;
        }

        public ProgressInfo(string text, bool verbose)
        {
            this.progress = -1;
            this.line = text;
            this.verbose = true;
        }

        public static implicit operator ProgressInfo(int value)
        {
            return new ProgressInfo() { progress = value, line = null, verbose = false };
        }

        public static implicit operator ProgressInfo(string value)
        {
            return new ProgressInfo() { progress = -1, line = value, verbose = false };
        }

        public static int lerp(float v0, float v1, float t)
        {
            return (int)((1 - t) * v0 + t * v1);
        }
        public static int lerp(int min, int max, int value)
        {
            if (value < 0)
                return value;
            return lerp(min, max, value / 100f);
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

        /**
         * Useful for when a command line job does not report progress but we still want to show the user that work is being done.
         */
        public static async Task<bool> makeFakeProgress(IProgress<ProgressInfo> progress, CancellationToken ct)
        {
            for (int i = 1; i <= 100; i++)
            {
                try
                {
                    await Task.Delay(i * i / 8 + i / 2 + 15, ct).ConfigureAwait(false);
                }
                catch (TaskCanceledException)
                {
                    return true;
                }
                if (ct.IsCancellationRequested)
                    return true;
                progress?.Report(i);
            }
            return true;
        }
    }
}
