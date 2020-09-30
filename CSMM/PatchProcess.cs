using FSEditor.Exceptions;
using FSEditor.FSData;
using FSEditor.MapDescriptor;
using MiscUtil.Conversion;
using MiscUtil.IO;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CustomStreetManager
{
    public partial class PatchProcess
    {
        public List<MapDescriptor> mapDescriptors;
        private MainDol mainDol;
        // Key = locale, Value = file contents
        private Dictionary<string, UI_Message> ui_messages = new Dictionary<string, UI_Message>();
        private string inputFile;
        // The cache directory is the directory for the extraced wbfs/iso file. It should not be modified and can be reused later 
        // to speed up the next patch process. Or if the user wishes to, can also be cleaned up at the end of the patch process. 
        // The directory which contains the final patched and new content to be inserted into the wbfs/iso. It contains only the delta to the cache directory.
        private readonly DataFileSet riivFileSet = new DataFileSet(Path.Combine(Directory.GetCurrentDirectory(), "fortunestreet"));
        // The directory to store intermediate files which are created and deleted again during the path process
        private readonly DataFileSet tmpFileSet = new DataFileSet(Path.Combine(Directory.GetCurrentDirectory(), "tmp"));
        private DataFileSet cacheFileSet;

        public void cleanUp(bool cleanCache, bool cleanRiivolution)
        {
            if (Directory.Exists(tmpFileSet.rootDir))
            {
                Directory.Delete(tmpFileSet.rootDir, true);
            }
            if (cleanCache)
            {
                if (Directory.Exists(tmpFileSet.rootDir))
                {
                    Directory.Delete(tmpFileSet.rootDir, true);
                }
            }
            if (cleanRiivolution)
            {
                if (Directory.Exists(tmpFileSet.rootDir))
                {
                    Directory.Delete(tmpFileSet.rootDir, true);
                }
            }
        }
    }
}
