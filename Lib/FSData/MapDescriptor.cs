using FSEditor.FSData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FSEditor.FSData
{
    class MapDescriptor
    {
        public UInt32 InitialCash { get; set; }
        public UInt32 TargetAmount { get; set; }
        public BoardTheme Theme { get; set; }
        public RuleSet RuleSet { get; set; }
        public string FrbFile1 { get; set; }
        public string FrbFile2 { get; set; }
        public string FrbFile3 { get; set; }
        public string FrbFile4 { get; set; }
        public string Background { get; set; }
        public UInt32 BGM { get; set; }
        public string Name_DE { get; set; }
        public string Name_EN { get; set; }
        public string Name_FR { get; set; }
        public string Name_IT { get; set; }
        public string Name_JP { get; set; }
        public string Name_SU { get; set; }
        public string Name_UK { get; set; }
        public string Desc_DE { get; set; }
        public string Desc_EN { get; set; }
        public string Desc_FR { get; set; }
        public string Desc_IT { get; set; }
        public string Desc_JP { get; set; }
        public string Desc_SU { get; set; }
        public string Desc_UK { get; set; }
        public byte[] Venture_Cards { get; private set; }

        public MapDescriptor()
        {
            Venture_Cards = new byte[128];
        }

    }
}
