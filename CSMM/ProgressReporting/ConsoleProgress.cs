using System;
using System.Collections.Generic;
using System.Text;

namespace CustomStreetMapManager
{
    public class ConsoleProgress : IDisposable, IProgress<ProgressInfo>
    {
        private ASCIIProgressBar asciiProgressBar;
        private bool verbose;

        public ConsoleProgress(bool verbose)
        {
            asciiProgressBar = new ASCIIProgressBar();
            this.verbose = verbose;
        }

        public void Dispose()
        {
            asciiProgressBar.Dispose();
        }

        public void Report(ProgressInfo progressInfo)
        {
            if (!string.IsNullOrEmpty(progressInfo.line))
            {
                if (!progressInfo.verbose || verbose)
                    Console.WriteLine(progressInfo.line);
            }
            asciiProgressBar?.Report(progressInfo.progress / 100.0);
        }
    }
}
