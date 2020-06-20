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
        public UInt32 BackgroundAddr { get; set; }
        public string Background { get; set; }
        public UInt32 BGMID { get; set; }
        public UInt32 Name_MSG_ID { get; set; }
        public string Name_DE { get; set; }
        public string Name_EN { get; set; }
        public string Name_FR { get; set; }
        public string Name_IT { get; set; }
        public string Name_JP { get; set; }
        public string Name_SU { get; set; }
        public string Name_UK { get; set; }
        public UInt32 Desc_MSG_ID { get; set; }
        public string Desc_DE { get; set; }
        public string Desc_EN { get; set; }
        public string Desc_FR { get; set; }
        public string Desc_IT { get; set; }
        public string Desc_JP { get; set; }
        public string Desc_SU { get; set; }
        public string Desc_UK { get; set; }
        public byte[] Venture_Cards { get; private set; }
        public int Venture_Card_Count { get; private set; }

        public MapDescriptor()
        {
            Venture_Cards = new byte[130];
        }

        public void ReadMapDataFromStream(EndianBinaryReader stream)
        {
            Name_MSG_ID = stream.ReadUInt32();
            BGMID = stream.ReadUInt32();
            UInt32 InternalNameAddr = stream.ReadUInt32();
            BackgroundAddr = stream.ReadUInt32();
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

            if (ID >= 0 && ID < 18)
            {
                Desc_MSG_ID = 4416 + ID;
            }
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
            Venture_Card_Count = 0;
            for (int i = 0; i < Venture_Cards.Length; i++)
            {
                if (Venture_Cards[i] != 0)
                {
                    Venture_Card_Count++;
                }
            }
        }
        enum MDParserState
        {
            NoHeading,
            Description,
            KeyValueTable,
            BackgroundTable,
            BGMTable,
            VentureCardTable
        }
        public void ReadMapDescriptorFromFile(string fileName)
        {
            string[] lines = File.ReadAllLines(fileName);
            MDParserState state = MDParserState.NoHeading;
            MapDescriptor mapDescriptor = new MapDescriptor();
            mapDescriptor.Desc_EN = "";
            for (int i = 0; i < lines.Length; i += 1)
            {
                string line = lines[i];
                MDParserState newState = detectHeading(state, line);
                if(newState != MDParserState.NoHeading)
                {
                    if(newState == MDParserState.Description)
                    {
                        mapDescriptor.Name_EN = line.Replace("#", "").Trim();
                    }
                    state = newState;
                } else
                {
                    parseContent(state, line);
                }
            }
        }

        private void parseContent(MDParserState state, string line)
        {
            switch (state)
            {
                case MDParserState.Description:
                    Desc_EN += line;
                    break;
                case MDParserState.KeyValueTable:
                    string[] columns = line.Split('|');
                    if(columns.Length > 0)
                    {
                        parseKeyValuePair(columns);
                    }
                    break;
            }
        }

        private KeyValuePair<string, string> parseKeyValuePair(string[] columns)
        {
            bool isValue = false;
            KeyValuePair<string, string> keyValuePair = new KeyValuePair<string, string>();
            for (int i = 0; i < columns.Length; i++)
            {
                if (columns[i].Trim().Length > 0)
                {
                    if (isValue)
                    {
                        keyValuePair.Value = columns[i].Trim();
                    }
                    else
                    {
                        keyValuePair.Key = columns[i].Trim();
                    }
                    isValue = true;
                }
            }
        }

        private MDParserState detectHeading(MDParserState state, string line)
        {
            if (line.StartsWith("#"))
            {
                if (state == MDParserState.NoHeading)
                {
                    return MDParserState.Description;
                }
                string headingText = line.Replace("#", "").Trim();
                if (headingText.Contains("Properties") || headingText.Contains("Configuration") || headingText.Contains("Localization"))
                {
                    return MDParserState.KeyValueTable;
                }
                if (headingText.Contains("Music"))
                {
                    return MDParserState.BGMTable;
                }
                if (headingText.Contains("Card"))
                {
                    return MDParserState.VentureCardTable;
                }
            }
            return MDParserState.NoHeading;
        }

        private void set(MapDescriptor mapDescriptor)
        {
            InitialCash = mapDescriptor.InitialCash;
            TargetAmount = mapDescriptor.TargetAmount;
            Theme = mapDescriptor.Theme;
            RuleSet = mapDescriptor.RuleSet;
            FrbFile1 = mapDescriptor.FrbFile1;
            FrbFile2 = mapDescriptor.FrbFile2;
            FrbFile3 = mapDescriptor.FrbFile3;
            FrbFile4 = mapDescriptor.FrbFile4;
            Background = mapDescriptor.Background;
            BGMID = mapDescriptor.BGMID;
            Name_DE = mapDescriptor.Name_DE;
            Name_EN = mapDescriptor.Name_EN;
            Name_FR = mapDescriptor.Name_FR;
            Name_IT = mapDescriptor.Name_IT;
            Name_JP = mapDescriptor.Name_JP;
            Name_SU = mapDescriptor.Name_SU;
            Name_UK = mapDescriptor.Name_UK;

            Desc_DE = mapDescriptor.Desc_DE;
            Desc_EN = mapDescriptor.Desc_EN;
            Desc_FR = mapDescriptor.Desc_FR;
            Desc_IT = mapDescriptor.Desc_IT;
            Desc_JP = mapDescriptor.Desc_JP;
            Desc_SU = mapDescriptor.Desc_SU;
            Desc_UK = mapDescriptor.Desc_UK;

            for (int i = 0; i < Venture_Cards.Length; i++)
            {
                Venture_Cards[i] = mapDescriptor.Venture_Cards[i];
            }
        }
    }
}
