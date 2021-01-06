using System;
using System.Collections.Generic;
using System.Text;

namespace CustomStreetMapManager
{
    public class ConsoleProgress : IDisposable, IProgress<ProgressInfo>
    {
        private ASCIIProgressBar asciiProgressBar;
        private bool verbose;
        private bool silent;

        public ConsoleProgress(bool verbose, bool silent)
        {
            this.verbose = verbose;
            this.silent = silent;
            if(!silent)
            {
                asciiProgressBar = new ASCIIProgressBar();
            }
        }

        public void Dispose()
        {
            asciiProgressBar?.Dispose();
        }

        public void Report(ProgressInfo progressInfo)
        {
            if (silent)
                return;
            if (!string.IsNullOrEmpty(progressInfo.line))
            {
                if (!progressInfo.verbose || verbose)
                    Console.WriteLine(progressInfo.line);
            }
            asciiProgressBar?.Report(progressInfo.progress / 100.0);
        }
    }
}
