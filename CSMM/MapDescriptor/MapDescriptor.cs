﻿using FSEditor.FSData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using MiscUtil.IO;
using System.ComponentModel;
using MiscUtil.Conversion;
using System.Text.RegularExpressions;
using System.Threading;

namespace CustomStreetManager
{
    public class MapDescriptor
    {
        public bool Dirty { get; set; }
        public UInt32 ID { get; set; }
        public UInt32 InitialCash { get; set; }
        public UInt32 TargetAmount { get; set; }
        public BoardTheme Theme { get; set; }
        public RuleSet RuleSet { get; set; }
        public VAVAddr InternalNameAddr { get; set; }
        public string InternalName { get; set; }
        public VAVAddr FrbFile1Addr { get; set; }
        public string FrbFile1 { get; set; }
        public VAVAddr FrbFile2Addr { get; set; }
        public string FrbFile2 { get; set; }
        public VAVAddr FrbFile3Addr { get; set; }
        public string FrbFile3 { get; set; }
        public VAVAddr FrbFile4Addr { get; set; }
        public string FrbFile4 { get; set; }
        public VAVAddr BackgroundAddr { get; set; }
        public string Background { get; set; }
        public bool IsPracticeBoard { get; set; }

        private Optional<UInt32> bgmId = Optional<UInt32>.CreateEmpty();
        public UInt32 BGMID
        {
            get { return bgmId.OrElse(0); }
            set { bgmId = Optional<UInt32>.Create(value); }
        }
        private bool IsBgmIdInitialized()
        {
            return bgmId.Any();
        }
        public UInt32 Name_MSG_ID { get; set; }
        public Dictionary<string, string> Name { get; private set; }
        public string Name_EN
        {
            get { return Name[Locale.EN]; }
            set { }
        }
        public UInt32 Desc_MSG_ID { get; set; }
        public Dictionary<string, string> Desc { get; private set; }
        public byte[] VentureCard { get; set; }
        public int VentureCardActiveCount
        {
            get
            {
                var activeCount = 0;
                for (int i = 0; i < VentureCard.Length; i++)
                {
                    if (VentureCard[i] != 0)
                    {
                        activeCount++;
                    }
                }
                return activeCount;
            }
            private set { }
        }
        public VAVAddr MapSwitchParamAddr { get; set; }
        public Dictionary<int, OriginPoint> SwitchRotationOriginPoints { get; private set; }
        public LoopingMode LoopingMode { get; set; }
        public VAVAddr LoopingModeParamAddr { get; set; }
        public Single LoopingModeRadius { get; set; }
        public Single LoopingModeHorizontalPadding { get; set; }
        public Single LoopingModeVerticalSquareCount { get; set; }
        public UInt32 BaseSalary { get; set; }
        public UInt32 SalaryIncrement { get; set; }
        public UInt32 MaxDiceRoll { get; set; }
        public UInt32 TourBankruptcyLimit { get; set; } = 1;
        public UInt32 TourInitialCash { get; set; }
        public Character TourOpponent1 { get; set; } = Character.Mario;
        public Character TourOpponent2 { get; set; } = Character.Luigi;
        public Character TourOpponent3 { get; set; } = Character.Peach;
        public UInt32 TourClearRank { get; set; } = 2;
        public VAVAddr MapIconAddrAddr { get; set; }
        public string MapIcon { get; internal set; }

        public MapDescriptor()
        {
            Name = new Dictionary<string, string>();
            Desc = new Dictionary<string, string>();
            VentureCard = new byte[128];
            SwitchRotationOriginPoints = new Dictionary<int, OriginPoint>();
        }
        public void readRotationOriginPoints(EndianBinaryReader stream, AddressMapper addressMapper)
        {
            SwitchRotationOriginPoints.Clear();
            // Special case handling: in the original game these values are initialized at run time only. So we need to hardcode them:
            if (MapSwitchParamAddr == addressMapper.toVersionAgnosticAddress((BSVAddr) 0x806b8df0)) // magmageddon
            {
                // no points
            }
            else if (MapSwitchParamAddr == addressMapper.toVersionAgnosticAddress((BSVAddr) 0x8047d598)) // collosus
            {
                SwitchRotationOriginPoints[0] = new OriginPoint(-288, -32);
                SwitchRotationOriginPoints[1] = new OriginPoint(288, -32);
            }
            else if (MapSwitchParamAddr == addressMapper.toVersionAgnosticAddress((BSVAddr) 0x8047d5b4)) // observatory
            {
                SwitchRotationOriginPoints[0] = new OriginPoint(0, 0);
            }
            else if (addressMapper.canConvertToFileAddress(MapSwitchParamAddr))
            {
                stream.Seek(addressMapper.toFileAddress(MapSwitchParamAddr), SeekOrigin.Begin);
                var originPointCount = stream.ReadUInt32();
                for (int i = 0; i < originPointCount; i++)
                {
                    OriginPoint point = new OriginPoint();
                    point.X = stream.ReadSingle();
                    var z = stream.ReadSingle(); // ignore Z value
                    point.Y = stream.ReadSingle();
                    SwitchRotationOriginPoints[i] = point;
                }
            }
        }
        public void writeSwitchRotationOriginPoints(EndianBinaryWriter stream)
        {
            stream.Write((UInt32)SwitchRotationOriginPoints.Count);
            for (int i = 0; i < SwitchRotationOriginPoints.Count; i++)
            {
                stream.Write(SwitchRotationOriginPoints[i].X);
                stream.Write((UInt32)0);
                stream.Write(SwitchRotationOriginPoints[i].Y);
            }
        }
        public void readLoopingModeParams(EndianBinaryReader stream, AddressMapper addressMapper)
        {
            if (addressMapper.canConvertToFileAddress(LoopingModeParamAddr))
            {
                stream.Seek(addressMapper.toFileAddress(LoopingModeParamAddr), SeekOrigin.Begin);
                LoopingModeRadius = stream.ReadSingle();
                LoopingModeHorizontalPadding = stream.ReadSingle();
                LoopingModeVerticalSquareCount = stream.ReadSingle();
            }
        }
        public void writeLoopingModeParams(EndianBinaryWriter stream)
        {
            stream.Write(LoopingModeRadius);
            stream.Write(LoopingModeHorizontalPadding);
            stream.Write(LoopingModeVerticalSquareCount);
        }
        public void readMapDataFromStream(EndianBinaryReader stream)
        {
            Name_MSG_ID = stream.ReadUInt32();
            BGMID = stream.ReadUInt32();
            InternalNameAddr = (VAVAddr)stream.ReadUInt32();
            BackgroundAddr = (VAVAddr)stream.ReadUInt32();
            RuleSet = (RuleSet)stream.ReadUInt32();
            Theme = (BoardTheme)stream.ReadUInt32();
            FrbFile1Addr = (VAVAddr)stream.ReadUInt32();
            FrbFile2Addr = (VAVAddr)stream.ReadUInt32();
            FrbFile3Addr = (VAVAddr)stream.ReadUInt32();
            FrbFile4Addr = (VAVAddr)stream.ReadUInt32();
            MapSwitchParamAddr = (VAVAddr)stream.ReadUInt32();
            LoopingModeParamAddr = (VAVAddr)stream.ReadUInt32();
            ID = stream.ReadUInt32();
            // ignore BG Sequence
            UInt32 BGSequenceAddr = stream.ReadUInt32();
        }
        public void writeMapData(EndianBinaryWriter stream, VAVAddr internalNameAddr, VAVAddr backgroundAddr, VAVAddr frbFile1Addr, VAVAddr frbFile2Addr, VAVAddr frbFile3Addr, VAVAddr frbFile4Addr, VAVAddr mapSwitchParamAddr, VAVAddr loopingModeParamAddr, VAVAddr bgSequenceMarioStadium)
        {
            stream.Write(Name_MSG_ID);
            stream.Write(BGMID);
            stream.Write((UInt32)internalNameAddr);
            stream.Write((UInt32)backgroundAddr);
            stream.Write((UInt32)RuleSet);
            stream.Write((UInt32)Theme);
            stream.Write((UInt32)frbFile1Addr);
            stream.Write((UInt32)frbFile2Addr);
            stream.Write((UInt32)frbFile3Addr);
            stream.Write((UInt32)frbFile4Addr);
            stream.Write((UInt32)mapSwitchParamAddr);
            stream.Write((UInt32)loopingModeParamAddr);
            stream.Seek(4, SeekOrigin.Current); // skip MapOriginID
            // the BGSequence is only used for mario stadium to animate the Miis playing baseball in the background. As such this will be hardcoded whenever bg004 is selected.
            stream.Write(Background == "bg004" ? (UInt32)bgSequenceMarioStadium : (UInt32)0);
        }
        public HashSet<SquareType> readFrbFileInfo(string param_folder, IProgress<ProgressInfo> progress, CancellationToken ct)
        {
            var usedSquareTypes = new HashSet<SquareType>();
            var file = Path.Combine(param_folder, FrbFile1 + ".frb");
            using (var stream = File.OpenRead(file))
            {
                EndianBinaryReader binReader = new EndianBinaryReader(EndianBitConverter.Big, stream);
                BoardFile board = BoardFile.LoadFromStream(binReader);
                MaxDiceRoll = board.BoardInfo.MaxDiceRoll;
                BaseSalary = board.BoardInfo.BaseSalary;
                SalaryIncrement = board.BoardInfo.SalaryIncrement;
                InitialCash = board.BoardInfo.InitialCash;
                if (TourInitialCash == 0)
                    TourInitialCash = InitialCash;
                if (TargetAmount != board.BoardInfo.TargetAmount)
                {
                    progress.Report("[" + ID + "] " + Name[Locale.EN] + ": frb target amount is " + board.BoardInfo.TargetAmount + " but md target amount is " + TargetAmount + ". The frb target amount has no effect.");
                }
                LoopingMode = board.BoardInfo.GalaxyStatus;
                if (LoopingMode != LoopingMode.None)
                {
                    if (LoopingModeRadius == 0 || LoopingModeVerticalSquareCount == 0)
                    {
                        progress.Report("[" + ID + "] " + Name[Locale.EN] + ": frb has looping enabled, but looping parameters are missing in the md.");
                    }
                }
                else if (LoopingMode == LoopingMode.None)
                {
                    if (LoopingModeRadius != 0 || LoopingModeHorizontalPadding != 0 || LoopingModeVerticalSquareCount != 0)
                    {
                        progress.Report("[" + ID + "] " + Name[Locale.EN] + ": frb has looping disabled. The looping parameters defined in the md will have no effect.");
                    }
                }
                foreach(var square in board.BoardData.Squares)
                {
                    usedSquareTypes.Add(square.SquareType);
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
            return usedSquareTypes;
        }

        enum MDParserState
        {
            NoHeading,
            Description,
            KeyValueTable,
            BackgroundTable,
            IconTable,
            BGMTable,
            VentureCardTable
        }
        public void readMapDescriptorFromFile(string fileName, string internalName)
        {
            string[] lines = File.ReadAllLines(fileName);
            MapDescriptor mapDescriptor = new MapDescriptor();
            mapDescriptor.InternalName = internalName;
            if (lines[0].StartsWith("# "))
            {
                mapDescriptor.Name[Locale.EN] = lines[0].Replace("#", "").Trim();
            }
            else
            {
                throw new Exception("Could not parse " + fileName + " because the first line does not start with a '# '.");
            }
            mapDescriptor.Desc[Locale.EN] = "";
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
                    Desc[Locale.EN] += " ";
                    Desc[Locale.EN] += line.Trim();
                    Desc[Locale.EN] = Desc[Locale.EN].Trim();
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
                            if (string.IsNullOrEmpty(MapIcon))
                                VanillaDatabase.getMapIconFromVanillaBackground(Background).IfPresent(value => MapIcon = value);
                            if (!IsBgmIdInitialized())
                                VanillaDatabase.getBgmIdFromVanillaBackground(Background).IfPresent(value => BGMID = value);
                        }
                    }
                    break;
                case MDParserState.IconTable:
                    columns = line.Split('|');
                    if (columns.Length > 1)
                    {
                        string flaggedValueOrNull = parseFlaggedValueOrReturnNull(columns);
                        if (flaggedValueOrNull != null)
                        {
                            MapIcon = flaggedValueOrNull;
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
                            var index = uint.Parse(flaggedValueOrNull) - 1;
                            VentureCard[index] = 1;
                        }
                    }
                    break;
            }
        }

        private void setProperty(KeyValuePair<string, string> keyValuePair)
        {
            var key = keyValuePair.Key.Replace(" ", "").Replace("(", "").Replace(")", "").ToLower();
            switch (key)
            {
                case "radius":
                    LoopingModeRadius = Single.Parse(keyValuePair.Value);
                    break;
                case "horizontalpadding":
                    LoopingModeHorizontalPadding = Single.Parse(keyValuePair.Value);
                    break;
                case "verticalsquarecount":
                    LoopingModeVerticalSquareCount = Single.Parse(keyValuePair.Value);
                    break;
                case "tourbankruptcylimit":
                    TourBankruptcyLimit = UInt32.Parse(keyValuePair.Value);
                    break;
                case "tourinitialcash":
                    TourInitialCash = UInt32.Parse(keyValuePair.Value);
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
                    TourClearRank = UInt32.Parse(keyValuePair.Value);
                    break;
                case "initialcash":
                    InitialCash = UInt32.Parse(keyValuePair.Value);
                    break;
                case "targetamount":
                    TargetAmount = UInt32.Parse(keyValuePair.Value);
                    break;
                case "basesalary":
                    BaseSalary = UInt32.Parse(keyValuePair.Value);
                    break;
                case "salaryincrement":
                    SalaryIncrement = UInt32.Parse(keyValuePair.Value);
                    break;
                case "maximumdiceroll":
                    MaxDiceRoll = UInt32.Parse(keyValuePair.Value);
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
                    Name[Locale.DE] = keyValuePair.Value;
                    break;
                case "namees":
                    Name[Locale.ES] = keyValuePair.Value;
                    break;
                case "namefr":
                    Name[Locale.FR] = keyValuePair.Value;
                    break;
                case "nameit":
                    Name[Locale.IT] = keyValuePair.Value;
                    break;
                case "namejp":
                    Name[Locale.JP] = keyValuePair.Value;
                    break;
                case "descde":
                    Desc[Locale.DE] = keyValuePair.Value;
                    break;
                case "desces":
                    Desc[Locale.ES] = keyValuePair.Value;
                    break;
                case "descfr":
                    Desc[Locale.FR] = keyValuePair.Value;
                    break;
                case "descit":
                    Desc[Locale.IT] = keyValuePair.Value;
                    break;
                case "descjp":
                    Desc[Locale.JP] = keyValuePair.Value;
                    break;
            }
            if (key.StartsWith("rotationoriginpoint"))
            {
                var number = Regex.Match(key, @"\d+").Value;
                int i = Int32.Parse(number) - 1;
                Single value = Single.Parse(keyValuePair.Value);
                if (!SwitchRotationOriginPoints.ContainsKey(i))
                {
                    SwitchRotationOriginPoints[i] = new OriginPoint();
                }
                if (key.Contains("x"))
                {
                    SwitchRotationOriginPoints[i].X = value;
                }
                else if (key.Contains("y"))
                {
                    SwitchRotationOriginPoints[i].Y = value;
                }
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
                else if (headingText.Contains("icon"))
                {
                    return MDParserState.IconTable;
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
            foreach (string locale in Locale.ALL)
            {
                if (mapDescriptor.Name.ContainsKey(locale))
                {
                    Name[locale] = mapDescriptor.Name[locale];
                }
                else
                {
                    Name[locale] = mapDescriptor.Name[Locale.EN];
                }
                if (mapDescriptor.Desc.ContainsKey(locale))
                {
                    Desc[locale] = mapDescriptor.Desc[locale];
                }
                else
                {
                    Desc[locale] = mapDescriptor.Desc[Locale.EN];
                }
            }

            for (int i = 0; i < VentureCard.Length; i++)
            {
                VentureCard[i] = mapDescriptor.VentureCard[i];
            }

            BaseSalary = mapDescriptor.BaseSalary;
            SalaryIncrement = mapDescriptor.SalaryIncrement;
            MaxDiceRoll = mapDescriptor.MaxDiceRoll;
            LoopingMode = mapDescriptor.LoopingMode;
            LoopingModeRadius = mapDescriptor.LoopingModeRadius;
            LoopingModeHorizontalPadding = mapDescriptor.LoopingModeHorizontalPadding;
            LoopingModeVerticalSquareCount = mapDescriptor.LoopingModeVerticalSquareCount;

            SwitchRotationOriginPoints = mapDescriptor.SwitchRotationOriginPoints;

            TourBankruptcyLimit = mapDescriptor.TourBankruptcyLimit;
            TourInitialCash = mapDescriptor.TourInitialCash;
            TourOpponent1 = mapDescriptor.TourOpponent1;
            TourOpponent2 = mapDescriptor.TourOpponent2;
            TourOpponent3 = mapDescriptor.TourOpponent3;
            TourClearRank = mapDescriptor.TourClearRank;

            MapIcon = mapDescriptor.MapIcon;
        }

        public string generateMapDescriptorFileContent()
        {
            MapDescriptorTemplate t = new MapDescriptorTemplate(this);
            return t.TransformText().TrimStart();
        }
    }
}
