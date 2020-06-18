using FSEditor.FSData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using MiscUtil.IO;
using System.ComponentModel;

namespace FSEditor.MapDescriptor
{
    public class MapDescriptor
    {
        public UInt32 ID { get; set; }
        public UInt32 InitialCash { get; set; }
        public UInt32 TargetAmount { get; set; }
        public BoardTheme Theme { get; set; }
        public RuleSet RuleSet { get; set; }
        public UInt32 FrbFile1Addr { get; set; }
        public string FrbFile1 { get; set; }
        public UInt32 FrbFile2Addr { get; set; }
        public string FrbFile2 { get; set; }
        public UInt32 FrbFile3Addr { get; set; }
        public string FrbFile3 { get; set; }
        public UInt32 FrbFile4Addr { get; set; }
        public string FrbFile4 { get; set; }
        public string Background { get; set; }
        public UInt32 BGMID { get; set; }
        public UInt32 Name_ID { get; set; }
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
            Venture_Cards = new byte[130];
        }

        public void ReadMapDataFromStream(EndianBinaryReader stream)
        {
            Name_ID = stream.ReadUInt32();
            BGMID = stream.ReadUInt32();
            UInt32 InternalNameAddr = stream.ReadUInt32();
            UInt32 BackgroundAddr = stream.ReadUInt32();
            RuleSet = (RuleSet)stream.ReadUInt32();
            Theme = (BoardTheme)stream.ReadUInt32();
            FrbFile1Addr = stream.ReadUInt32();
            FrbFile2Addr = stream.ReadUInt32();
            FrbFile3Addr = stream.ReadUInt32();
            FrbFile4Addr = stream.ReadUInt32();
            UInt32 MapSwitchParamAddr = stream.ReadUInt32();
            UInt32 MapGalaxyParamAddr = stream.ReadUInt32();
            ID = stream.ReadUInt32();
            UInt32 BGSequenceAddr = stream.ReadUInt32();
        }

        public void ReadMapDefaultsFromStream(EndianBinaryReader stream)
        {
            TargetAmount = stream.ReadUInt32();
            UInt32 DefaultHasanPlayer = stream.ReadUInt32();
            InitialCash = stream.ReadUInt32();
            UInt32 TourOpponent1 = stream.ReadUInt32();
            UInt32 TourOpponent2 = stream.ReadUInt32();
            UInt32 TourOpponent3 = stream.ReadUInt32();
            UInt32 TourClearRank = stream.ReadUInt32();
            UInt32 TourDifficulty = stream.ReadUInt32();
            UInt32 GeneralPlayTime = stream.ReadUInt32();
        }

        public void ReadVentureCardTableFromStream(EndianBinaryReader stream)
        {
            for (int i = 0; i < Venture_Cards.Length; i++)
            {
                Venture_Cards[i] = stream.ReadByte();
            }
        }
    }
}
