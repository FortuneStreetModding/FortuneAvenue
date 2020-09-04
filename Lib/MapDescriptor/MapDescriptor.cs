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
            MapDescriptor mapDescriptor = new MapDescriptor();
            if (lines[0].StartsWith("# "))
            {
                mapDescriptor.Name_EN = lines[0].Replace("#", "").Trim();
            }
            else
            {
                throw new Exception("Could not parse " + fileName + " because the first line does not start with a '# '.");
            }
            mapDescriptor.Desc_EN = "";
            MDParserState state = MDParserState.Description;
            for (int i = 1; i < lines.Length; i += 1)
            {
                string line = lines[i];
                MDParserState newState = detectHeading(state, line);
                if (newState == MDParserState.NoHeading)
                {
                    mapDescriptor.parseContent(state, line);
                }
                else
                {
                    state = newState;
                }
            }
            // if everything went well, set the parsed map descriptor values
            set(mapDescriptor);
        }

        private void parseContent(MDParserState state, string line)
        {
            string[] columns;
            switch (state)
            {
                case MDParserState.Description:
                    Desc_EN += " ";
                    Desc_EN += line.Trim();
                    Desc_EN = Desc_EN.Trim();
                    break;
                case MDParserState.KeyValueTable:
                    columns = line.Split('|');
                    if (columns.Length > 1)
                    {
                        KeyValuePair<string, string> keyValuePair = parseKeyValuePair(columns);
                        setProperty(keyValuePair);
                    }
                    break;
                case MDParserState.BackgroundTable:
                    columns = line.Split('|');
                    if (columns.Length > 1)
                    {
                        string flaggedValueOrNull = parseFlaggedValueOrReturnNull(columns);
                        if(flaggedValueOrNull != null)
                        {
                            Background = flaggedValueOrNull;
                        }
                    }
                    break;
                case MDParserState.BGMTable:
                    columns = line.Split('|');
                    if (columns.Length > 1)
                    {
                        string flaggedValueOrNull = parseFlaggedValueOrReturnNull(columns);
                        if (flaggedValueOrNull != null)
                        {
                            BGMID = uint.Parse(flaggedValueOrNull);
                        }
                    }
                    break;
                case MDParserState.VentureCardTable:
                    columns = line.Split('|');
                    if (columns.Length > 1)
                    {
                        string flaggedValueOrNull = parseFlaggedValueOrReturnNull(columns);
                        if (flaggedValueOrNull != null)
                        {
                            var index = uint.Parse(flaggedValueOrNull);
                            Venture_Cards[index] = 1;
                        }
                    }
                    break;
            }
        }

        private void setProperty(KeyValuePair<string, string> keyValuePair)
        {
            switch (keyValuePair.Key.Replace(" ","").Replace("(","").Replace(")","").ToLower())
            {
                case "initialcash":
                    InitialCash = uint.Parse(keyValuePair.Value);
                    break;
                case "targetamount":
                    TargetAmount = uint.Parse(keyValuePair.Value);
                    break;
                case "rules":
                    switch (keyValuePair.Value.Replace(" ", "").ToLower())
                    {
                        case "standard":
                            RuleSet = RuleSet.Standard;
                            break;
                        case "basic":
                            RuleSet = RuleSet.Basic;
                            break;
                    }
                    break;
                case "theme":
                    switch (keyValuePair.Value.Replace(" ", "").ToLower())
                    {
                        case "mario":
                            Theme = BoardTheme.Mario;
                            break;
                        case "dragonquest":
                            Theme = BoardTheme.DragonQuest;
                            break;
                    }
                    break;
                case "frbfilename1":
                    FrbFile1 = keyValuePair.Value;
                    break;
                case "frbfilename2":
                    FrbFile2 = keyValuePair.Value;
                    break;
                case "frbfilename3":
                    FrbFile3 = keyValuePair.Value;
                    break;
                case "frbfilename4":
                    FrbFile4 = keyValuePair.Value;
                    break;
                case "namede":
                    Name_DE = keyValuePair.Value;
                    break;
                case "namees":
                    Name_SU = keyValuePair.Value;
                    break;
                case "namefr":
                    Name_FR = keyValuePair.Value;
                    break;
                case "nameit":
                    Name_IT = keyValuePair.Value;
                    break;
                case "namejp":
                    Name_JP = keyValuePair.Value;
                    break;
                case "descde":
                    Desc_DE = keyValuePair.Value;
                    break;
                case "desces":
                    Desc_SU = keyValuePair.Value;
                    break;
                case "descfr":
                    Desc_FR = keyValuePair.Value;
                    break;
                case "descit":
                    Desc_IT = keyValuePair.Value;
                    break;
                case "descjp":
                    Desc_JP = keyValuePair.Value;
                    break;
            }
        }

        private KeyValuePair<string, string> parseKeyValuePair(string[] columns)
        {
            bool isValue = false;
            string key = null;
            string value = null;
            for (int i = 0; i < columns.Length; i++)
            {
                if (columns[i].Trim().Length > 0)
                {
                    if (isValue)
                    {
                        value = columns[i].Trim();
                        break;
                    }
                    else
                    {
                        key = columns[i].Trim();
                    }
                    isValue = true;
                }
            }
            return new KeyValuePair<string, string>(key, value);
        }

        private string parseFlaggedValueOrReturnNull(string[] columns)
        {
            bool firstValueFound = false;
            string firstValue = null;
            string secondValue = null;
            // go through each column and find the first two entries with content
            for (int i = 0; i < columns.Length; i++)
            {
                if (columns[i].Trim().Length > 0)
                {
                    if (firstValueFound)
                    {
                        secondValue = columns[i].Trim();
                        break;
                    }
                    else
                    {
                        firstValue = columns[i].Trim();
                    }
                    firstValueFound = true;
                }
            }
            // if one of the column is marked with ":o:" return the other one
            if(firstValue.Equals(":o:"))
            {
                return secondValue;
            }
            if (secondValue.Equals(":o:"))
            {
                return firstValue;
            }
            // otherwise return null
            return null;
        }

        private MDParserState detectHeading(MDParserState state, string line)
        {
            if (line.StartsWith("#"))
            {
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
