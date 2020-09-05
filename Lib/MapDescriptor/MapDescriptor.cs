using FSEditor.FSData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using MiscUtil.IO;
using System.ComponentModel;
using MiscUtil.Conversion;

namespace FSEditor.MapDescriptor
{
    public class MapDescriptor
    {
        public bool Dirty { get; set; }
        public UInt32 ID { get; set; }
        public UInt32 InitialCash { get; set; }
        public UInt32 TargetAmount { get; set; }
        public BoardTheme Theme { get; set; }
        public RuleSet RuleSet { get; set; }
        public UInt32 InternalNameAddr { get; set; }
        public string InternalName { get; set; }
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
        public UInt32 BaseSalary { get; set; }
        public UInt32 SalaryIncrement { get; set; }
        public UInt32 MaxDiceRoll { get; set; }
        public LoopingMode LoopingMode { get; set; }
        public UInt32 TourBankruptcyLimit { get; set; }
        public UInt32 TourInitialCash { get; set; }
        public Character TourOpponent1 { get; set; }
        public Character TourOpponent2 { get; set; }
        public Character TourOpponent3 { get; set; }
        public UInt32 TourClearRank { get; set; }
        public UInt32 TourDifficulty { get; set; }
        public UInt32 TourGeneralPlayTime { get; set; }

        public MapDescriptor()
        {
            Venture_Cards = new byte[130];
        }

        public void ReadMapDataFromStream(EndianBinaryReader stream)
        {
            Name_MSG_ID = stream.ReadUInt32();
            BGMID = stream.ReadUInt32();
            InternalNameAddr = stream.ReadUInt32();
            BackgroundAddr = stream.ReadUInt32();
            RuleSet = (RuleSet)stream.ReadUInt32();
            Theme = (BoardTheme)stream.ReadUInt32();
            FrbFile1Addr = stream.ReadUInt32();
            FrbFile2Addr = stream.ReadUInt32();
            FrbFile3Addr = stream.ReadUInt32();
            FrbFile4Addr = stream.ReadUInt32();
            // todo 
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
            TourBankruptcyLimit = stream.ReadUInt32();
            TourInitialCash = stream.ReadUInt32();
            TourOpponent1 = (Character)stream.ReadUInt32();
            TourOpponent2 = (Character)stream.ReadUInt32();
            TourOpponent3 = (Character)stream.ReadUInt32();
            TourClearRank = stream.ReadUInt32();
            TourDifficulty = stream.ReadUInt32();
            TourGeneralPlayTime = stream.ReadUInt32();
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

        public string ReadFrbFileInfo(string param_folder)
        {
            string warning = "";
            var file = Path.Combine(param_folder, FrbFile1 + ".frb");
            using (var stream = File.OpenRead(file))
            {
                EndianBinaryReader binReader = new EndianBinaryReader(EndianBitConverter.Big, stream);
                BoardFile board = BoardFile.LoadFromStream(binReader);
                // TODO: What is unused and what is used? check InitialCash and TargetAmount. Fixate on only the real one
                MaxDiceRoll = board.BoardInfo.MaxDiceRoll;
                BaseSalary = board.BoardInfo.BaseSalary;
                SalaryIncrement = board.BoardInfo.SalaryIncrement;
                InitialCash = board.BoardInfo.InitialCash;
                if (TargetAmount != board.BoardInfo.TargetAmount)
                {
                    warning += "[" + ID + "]: FRB target amount is " + board.BoardInfo.TargetAmount + " but DOL target amount is " + TargetAmount + Environment.NewLine;
                    warning += "The DOL target amount will be used." + Environment.NewLine;
                }
                LoopingMode = board.BoardInfo.GalaxyStatus;
                // TODO: Check conformity with MapGalaxyParamAddr
                if (LoopingMode != 0)
                {

                }
            }
            // check the other frb files for validity
            if (!string.IsNullOrWhiteSpace(FrbFile2))
            {
                file = Path.Combine(param_folder, FrbFile2 + ".frb");
                using (var stream = File.OpenRead(file))
                {
                    EndianBinaryReader binReader = new EndianBinaryReader(EndianBitConverter.Big, stream);
                    BoardFile.LoadFromStream(binReader);
                }
            }
            if (!string.IsNullOrWhiteSpace(FrbFile3))
            {
                file = Path.Combine(param_folder, FrbFile3 + ".frb");
                using (var stream = File.OpenRead(file))
                {
                    EndianBinaryReader binReader = new EndianBinaryReader(EndianBitConverter.Big, stream);
                    BoardFile.LoadFromStream(binReader);
                }
            }
            if (!string.IsNullOrWhiteSpace(FrbFile4))
            {
                file = Path.Combine(param_folder, FrbFile4 + ".frb");
                using (var stream = File.OpenRead(file))
                {
                    EndianBinaryReader binReader = new EndianBinaryReader(EndianBitConverter.Big, stream);
                    BoardFile.LoadFromStream(binReader);
                }
            }
            return warning;
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
            mapDescriptor.InternalName = Path.GetFileNameWithoutExtension(fileName);
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
                        if (flaggedValueOrNull != null)
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
            switch (keyValuePair.Key.Replace(" ", "").Replace("(", "").Replace(")", "").ToLower())
            {
                case "tourbankruptcylimit":
                    TourBankruptcyLimit = uint.Parse(keyValuePair.Value);
                    break;
                case "tourinitialcash":
                    TourInitialCash = uint.Parse(keyValuePair.Value);
                    break;
                case "touropponent1":
                    TourOpponent1 = (Character)Enum.Parse(typeof(Character), keyValuePair.Value.Replace(" ", ""), true);
                    break;
                case "touropponent2":
                    TourOpponent2 = (Character)Enum.Parse(typeof(Character), keyValuePair.Value.Replace(" ", ""), true);
                    break;
                case "touropponent3":
                    TourOpponent3 = (Character)Enum.Parse(typeof(Character), keyValuePair.Value.Replace(" ", ""), true);
                    break;
                case "tourclearrank":
                    TourClearRank = uint.Parse(keyValuePair.Value);
                    break;
                case "tourdifficulty":
                    TourDifficulty = uint.Parse(keyValuePair.Value);
                    break;
                case "tourgeneralplaytime":
                    TourGeneralPlayTime = uint.Parse(keyValuePair.Value);
                    break;
                case "initialcash":
                    InitialCash = uint.Parse(keyValuePair.Value);
                    break;
                case "targetamount":
                    TargetAmount = uint.Parse(keyValuePair.Value);
                    break;
                case "basesalary":
                    BaseSalary = uint.Parse(keyValuePair.Value);
                    break;
                case "salaryincrement":
                    SalaryIncrement = uint.Parse(keyValuePair.Value);
                    break;
                case "maximumdiceroll":
                    MaxDiceRoll = uint.Parse(keyValuePair.Value);
                    break;
                case "loopingmode":
                    LoopingMode = (LoopingMode)Enum.Parse(typeof(LoopingMode), keyValuePair.Value.Replace(" ", ""), true);
                    break;
                case "rules":
                    RuleSet = (RuleSet)Enum.Parse(typeof(RuleSet), keyValuePair.Value.Replace(" ", ""), true);
                    break;
                case "theme":
                    Theme = (BoardTheme)Enum.Parse(typeof(BoardTheme), keyValuePair.Value.Replace(" ", ""), true);
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
            if (firstValue.Equals(":o:"))
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
                string headingText = line.Replace("#", "").ToLower().Trim();
                if (headingText.Contains("properties") || headingText.Contains("configuration") || headingText.Contains("localization"))
                {
                    return MDParserState.KeyValueTable;
                }
                else if (headingText.Contains("music"))
                {
                    return MDParserState.BGMTable;
                }
                else if (headingText.Contains("background"))
                {
                    return MDParserState.BackgroundTable;
                }
                else if (headingText.Contains("card"))
                {
                    return MDParserState.VentureCardTable;
                }
            }
            return MDParserState.NoHeading;
        }

        public void set(MapDescriptor mapDescriptor)
        {
            InitialCash = mapDescriptor.InitialCash;
            TargetAmount = mapDescriptor.TargetAmount;
            Theme = mapDescriptor.Theme;
            RuleSet = mapDescriptor.RuleSet;
            InternalName = mapDescriptor.InternalName;
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

            BaseSalary = mapDescriptor.BaseSalary;
            SalaryIncrement = mapDescriptor.SalaryIncrement;
            MaxDiceRoll = mapDescriptor.MaxDiceRoll;
            LoopingMode = mapDescriptor.LoopingMode;

            TourBankruptcyLimit = mapDescriptor.TourBankruptcyLimit;
            TourInitialCash = mapDescriptor.TourInitialCash;
            TourOpponent1 = mapDescriptor.TourOpponent1;
            TourOpponent2 = mapDescriptor.TourOpponent2;
            TourOpponent3 = mapDescriptor.TourOpponent3;
            TourClearRank = mapDescriptor.TourClearRank;
            TourDifficulty = mapDescriptor.TourDifficulty;
            TourGeneralPlayTime = mapDescriptor.TourGeneralPlayTime;
        }

        public string generateMapDescriptorFileContent()
        {
            MapDescriptorTemplate t = new MapDescriptorTemplate(this);
            return t.TransformText().TrimStart();
        }
    }
}
