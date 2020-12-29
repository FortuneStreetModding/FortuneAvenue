using FSEditor.FSData;
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
using System.Globalization;

namespace CustomStreetManager
{
    public class MapDescriptor
    {
        [Browsable(false)]
        public bool Dirty { get; set; }
        // --------------------------------------
        // --- MapSet Zone Order and Practice ---
        // --------------------------------------
        /// <summary>
        /// 0 = Easy Mode, 1 = Standard Mode
        /// </summary>
        public sbyte MapSet { get; set; } = -1;
        public sbyte Zone { get; set; } = -1;
        public sbyte Order { get; set; } = -1;
        public bool IsPracticeBoard { get; set; }
        // ------------------
        // --- Basic Info ---
        // ------------------
        [ReadOnly(true)]
        public string Name_En
        {
            get
            {
                return Name.GetValueOrDefault(Locale.EN, "");
            }
            private set { }
        }
        [ReadOnly(true)]
        public RuleSet RuleSet { get; set; }

        [ReadOnly(true)]
        public UInt32 InitialCash { get; set; }
        [ReadOnly(true)]
        public UInt32 TargetAmount { get; set; }
        [ReadOnly(true)]
        public UInt32 BaseSalary { get; set; }
        [ReadOnly(true)]
        public UInt32 SalaryIncrement { get; set; }
        [ReadOnly(true)]
        public UInt32 MaxDiceRoll { get; set; }
        [Browsable(false)]
        public byte[] VentureCard { get; set; }
        [ReadOnly(true)]
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
        // ------------------------------------------
        // --- Frb Files and Switch Configuration ---
        // ------------------------------------------
        [ReadOnly(true)]
        public string FrbFiles
        {
            get
            {
                var str = FrbFile1 + ", ";
                if (!string.IsNullOrEmpty(FrbFile2))
                    str += FrbFile2 + ", ";
                if (!string.IsNullOrEmpty(FrbFile3))
                    str += FrbFile3 + ", ";
                if (!string.IsNullOrEmpty(FrbFile4))
                    str += FrbFile4 + ", ";
                return str.Remove(str.Length - 2);
            }
            private set { }
        }
        [Browsable(false)]
        public string FrbFile1 { get; set; }
        [Browsable(false)]
        public string FrbFile2 { get; set; }
        [Browsable(false)]
        public string FrbFile3 { get; set; }
        [Browsable(false)]
        public string FrbFile4 { get; set; }
        [ReadOnly(true)]
        [DisplayName("SwitchRotationOriginPoints")]
        public string SwitchRotationOriginPoints_
        {
            get
            {
                var str = "";
                foreach (var item in SwitchRotationOriginPoints)
                {
                    str += item.Value;
                }
                return str;
            }
            private set { }
        }
        [Browsable(false)]
        public Dictionary<int, OriginPoint> SwitchRotationOriginPoints { get; private set; }
        // ------------------
        // --- Background ---
        // ------------------
        [ReadOnly(true)]
        public BoardTheme Theme { get; set; }
        [ReadOnly(true)]
        public string Background { get; set; }

        private Optional<UInt32> bgmId = Optional<UInt32>.CreateEmpty();
        [ReadOnly(true)]
        public UInt32 BGMID
        {
            get { return bgmId.OrElse(0); }
            set { bgmId = Optional<UInt32>.Create(value); }
        }
        private bool IsBgmIdInitialized()
        {
            return bgmId.Any();
        }
        [ReadOnly(true)]
        public string MapIcon { get; internal set; }
        // --------------------
        // --- Looping Mode ---
        // --------------------
        [ReadOnly(true)]
        public LoopingMode LoopingMode { get; set; }
        [ReadOnly(true)]
        public Single LoopingModeRadius { get; set; }
        [ReadOnly(true)]
        public Single LoopingModeHorizontalPadding { get; set; }
        [ReadOnly(true)]
        public Single LoopingModeVerticalSquareCount { get; set; }
        // --------------------------
        // --- Tour Configuration ---
        // --------------------------
        [ReadOnly(true)]
        public UInt32 TourBankruptcyLimit { get; set; } = 1;
        [ReadOnly(true)]
        public UInt32 TourInitialCash { get; set; }
        [ReadOnly(true)]
        public Character TourOpponent1 { get; set; } = Character.Mario;
        [ReadOnly(true)]
        public Character TourOpponent2 { get; set; } = Character.Luigi;
        [ReadOnly(true)]
        public Character TourOpponent3 { get; set; } = Character.Peach;
        [ReadOnly(true)]
        public UInt32 TourClearRank { get; set; } = 2;
        [ReadOnly(true)]
        public UInt32 UnlockID { get; set; } = 0;
        // --------------------
        // --- Localization ---
        // --------------------
        [ReadOnly(true)]
        public UInt32 Name_MSG_ID { get; set; }
        [Browsable(false)]
        public Dictionary<string, string> Name { get; private set; }
        [ReadOnly(true)]
        public string Name_DE { get { return Name.GetValueOrDefault(Locale.DE, ""); } private set { } }
        [ReadOnly(true)]
        public string Name_FR { get { return Name.GetValueOrDefault(Locale.FR, ""); } private set { } }
        [ReadOnly(true)]
        public string Name_IT { get { return Name.GetValueOrDefault(Locale.IT, ""); } private set { } }
        [ReadOnly(true)]
        public string Name_JP { get { return Name.GetValueOrDefault(Locale.JP, ""); } private set { } }
        [ReadOnly(true)]
        public string Name_ES { get { return Name.GetValueOrDefault(Locale.ES, ""); } private set { } }
        [ReadOnly(true)]
        public UInt32 Desc_MSG_ID { get; set; }
        [Browsable(false)]
        public Dictionary<string, string> Desc { get; private set; }
        [ReadOnly(true)]
        public string Desc_EN { get { return Desc.GetValueOrDefault(Locale.EN, ""); } private set { } }
        [ReadOnly(true)]
        public string Desc_DE { get { return Desc.GetValueOrDefault(Locale.DE, ""); } private set { } }
        [ReadOnly(true)]
        public string Desc_FR { get { return Desc.GetValueOrDefault(Locale.FR, ""); } private set { } }
        [ReadOnly(true)]
        public string Desc_IT { get { return Desc.GetValueOrDefault(Locale.IT, ""); } private set { } }
        [ReadOnly(true)]
        public string Desc_JP { get { return Desc.GetValueOrDefault(Locale.JP, ""); } private set { } }
        [ReadOnly(true)]
        public string Desc_ES { get { return Desc.GetValueOrDefault(Locale.ES, ""); } private set { } }
        [ReadOnly(true)]
        public string InternalName { get; set; }
        [Browsable(false)]
        public string MapDescriptorFilePath { get; private set; }

        public MapDescriptor()
        {
            Name = new Dictionary<string, string>();
            Desc = new Dictionary<string, string>();
            VentureCard = new byte[128];
            SwitchRotationOriginPoints = new Dictionary<int, OriginPoint>();
        }

        public static MapDescriptorValidation getPracticeBoards(List<MapDescriptor> mapDescriptors, out short easyPracticeBoard, out short standardPracticeBoard)
        {
            var validation = new MapDescriptorValidation();
            easyPracticeBoard = -1;
            standardPracticeBoard = -1;
            for (short i = 0; i < mapDescriptors.Count; i++)
            {
                var mapDescriptor = mapDescriptors[i];
                if (mapDescriptor.IsPracticeBoard)
                {
                    validation.AddProblem(i, typeof(MapDescriptor).GetProperty("IsPracticeBoard"), "Only one map for each MapSet can be set as practice board");
                    if (mapDescriptor.MapSet == 0)
                    {
                        if (easyPracticeBoard == -1)
                            easyPracticeBoard = i;
                        else
                            validation.Passed = false;
                    }
                    else if (mapDescriptor.MapSet == 1)
                    {
                        if (standardPracticeBoard == -1)
                            standardPracticeBoard = i;
                        else
                            validation.Passed = false;
                    }
                }
            }
            if (easyPracticeBoard == -1 || standardPracticeBoard == -1)
            {
                validation.Passed = false;
                validation.Clear();
                if (easyPracticeBoard == -1)
                {
                    for (short i = 0; i < mapDescriptors.Count; i++)
                    {
                        if (mapDescriptors[i].MapSet == 0)
                            validation.AddProblem(i, typeof(MapDescriptor).GetProperty("IsPracticeBoard"), "At least one map for each MapSet must be set as practice board");
                    }
                }
                if (standardPracticeBoard == -1)
                {
                    for (short i = 0; i < mapDescriptors.Count; i++)
                    {
                        if (mapDescriptors[i].MapSet == 1)
                            validation.AddProblem(i, typeof(MapDescriptor).GetProperty("IsPracticeBoard"), "At least one map for each MapSet must be set as practice board");
                    }
                }
            }
            if (validation.Passed)
                validation.Clear();
            return validation;
        }

        public static MapDescriptorValidation getMapSets(List<MapDescriptor> mapDescriptors, out Dictionary<int, int> mapSets)
        {
            var validation = new MapDescriptorValidation();
            mapSets = new Dictionary<int, int>();
            for (short i = 0; i < mapDescriptors.Count; i++)
            {
                var mapDescriptor = mapDescriptors[i];
                if (mapDescriptor.MapSet == 0 || mapDescriptor.MapSet == 1)
                {
                    mapSets.Add(i, mapDescriptor.MapSet);
                }
                else if (mapDescriptor.MapSet != -1)
                {
                    validation.AddProblem(i, typeof(MapDescriptor).GetProperty("MapSet"), "MapSet must be either 0 or 1 or -1");
                    validation.Passed = false;
                }
            }
            if (!mapSets.Values.Contains(0))
            {
                validation.AddProblem(typeof(MapDescriptor).GetProperty("MapSet"), "At least one map must be available for MapSet 0");
                validation.Passed = false;
            }
            if (!mapSets.Values.Contains(1))
            {
                validation.AddProblem(typeof(MapDescriptor).GetProperty("MapSet"), "At least one map must be available for MapSet 1");
                validation.Passed = false;
            }
            return validation;
        }

        public static MapDescriptorValidation getZones(List<MapDescriptor> mapDescriptors, int mapSet, out Dictionary<int, int> zones)
        {
            // one of the worst spaghetti codes ever (yes I feel bad for it)
            var validation = new MapDescriptorValidation();
            zones = new Dictionary<int, int>();
            for (short i = 0; i < mapDescriptors.Count; i++)
            {
                var mapDescriptor = mapDescriptors[i];
                if (mapDescriptor.MapSet == mapSet)
                {
                    if (mapDescriptor.Zone >= 0 && mapDescriptor.Zone <= 2)
                    {
                        zones.Add(i, mapDescriptor.Zone);
                    }
                    else if (mapDescriptor.Zone != -1)
                    {
                        validation.AddProblem(i, typeof(MapDescriptor).GetProperty("Zone"), "The zone must be either 0, 1, 2 or -1");
                        validation.Passed = false;
                    }
                }
            }
            var distinct = zones.Values.Distinct().ToList();
            if (distinct.Any())
            {
                for (short i = 0; i < mapDescriptors.Count; i++)
                {
                    var mapDescriptor = mapDescriptors[i];
                    if (mapDescriptor.MapSet == mapSet)
                    {
                        if (zones.Values.Contains(0))
                        {

                            // count for each mapSet how many maps have been assigned to it
                            var a = from m in mapDescriptors where m.MapSet == mapSet && m.Zone == 0 select m;
                            var b = from m in mapDescriptors where m.MapSet != mapSet && m.Zone == 0 select m;
                            if (a.Count() != b.Count() && mapDescriptor.Zone == 0)
                            {
                                validation.AddProblem(i, typeof(MapDescriptor).GetProperty("Zone"), "The amount of maps in zone 0 must be the same in all MapSets.");
                                validation.Passed = false;
                            }
                            if (a.Count() < 6 && mapDescriptor.Zone == 0)
                            {
                                validation.AddProblem(i, typeof(MapDescriptor).GetProperty("Zone"), "The amount of maps in each zone must be at least 6.");
                                validation.Passed = false;
                            }
                            if (a.Count() > 16 && mapDescriptor.Zone == 0)
                            {
                                validation.AddProblem(i, typeof(MapDescriptor).GetProperty("Zone"), "The amount of maps in each zone must be less than 16.");
                                validation.Passed = false;
                            }
                        }
                        else
                        {
                            validation.AddProblem(i, typeof(MapDescriptor).GetProperty("Zone"), "At least one map must be available for zone 0 in MapSet " + mapSet);
                            validation.Passed = false;
                        }
                        if (zones.Values.Contains(1))
                        {
                            // count for each mapSet how many maps have been assigned to it
                            var a = from m in mapDescriptors where m.MapSet == mapSet && m.Zone == 1 select m;
                            var b = from m in mapDescriptors where m.MapSet != mapSet && m.Zone == 1 select m;
                            if (a.Count() != b.Count() && mapDescriptor.Zone == 1)
                            {
                                validation.AddProblem(i, typeof(MapDescriptor).GetProperty("Zone"), "The amount of maps in zone 1 must be the same in all MapSets.");
                                validation.Passed = false;
                            }
                            if (a.Count() < 6 && mapDescriptor.Zone == 1)
                            {
                                validation.AddProblem(i, typeof(MapDescriptor).GetProperty("Zone"), "The amount of maps in each zone must be at least 6.");
                                validation.Passed = false;
                            }
                            if (a.Count() > 16 && mapDescriptor.Zone == 1)
                            {
                                validation.AddProblem(i, typeof(MapDescriptor).GetProperty("Zone"), "The amount of maps in each zone must be less than 16.");
                                validation.Passed = false;
                            }
                        }
                        else
                        {
                            validation.AddProblem(i, typeof(MapDescriptor).GetProperty("Zone"), "At least one map must be available for zone 1 in MapSet " + mapSet);
                            validation.Passed = false;
                        }
                        if (zones.Values.Contains(2))
                        {
                            // count for each mapSet how many maps have been assigned to it
                            var a = from m in mapDescriptors where m.MapSet == mapSet && m.Zone == 2 select m;
                            var b = from m in mapDescriptors where m.MapSet != mapSet && m.Zone == 2 select m;
                            if (a.Count() != b.Count() && mapDescriptor.Zone == 2)
                            {
                                validation.AddProblem(i, typeof(MapDescriptor).GetProperty("Zone"), "The amount of maps in zone 2 must be the same in all MapSets.");
                                validation.Passed = false;
                            }
                            if (a.Count() < 6 && mapDescriptor.Zone == 2)
                            {
                                validation.AddProblem(i, typeof(MapDescriptor).GetProperty("Zone"), "The amount of maps in each zone must be at least 6.");
                                validation.Passed = false;
                            }
                            if (a.Count() > 16 && mapDescriptor.Zone == 2)
                            {
                                validation.AddProblem(i, typeof(MapDescriptor).GetProperty("Zone"), "The amount of maps in each zone must be less than 16.");
                                validation.Passed = false;
                            }
                        }
                        else
                        {
                            validation.AddProblem(i, typeof(MapDescriptor).GetProperty("Zone"), "At least one map must be available for zone 2 in MapSet " + mapSet);
                            validation.Passed = false;
                        }
                    }
                }
            }
            else
            {
                for (short i = 0; i < mapDescriptors.Count; i++)
                {
                    var mapDescriptor = mapDescriptors[i];
                    if (mapDescriptor.MapSet != mapSet)
                    {
                        validation.AddProblem(i, typeof(MapDescriptor).GetProperty("Zone"), "No map has been assigned to category " + mapSet);
                        validation.Passed = false;
                    }
                }
            }
            return validation;
        }

        public static MapDescriptorValidation getOrdering(List<MapDescriptor> mapDescriptors, int mapSet, int zone, out Dictionary<int, int> ordering)
        {
            var validation = new MapDescriptorValidation();
            ordering = new Dictionary<int, int>();
            for (short i = 0; i < mapDescriptors.Count; i++)
            {
                var mapDescriptor = mapDescriptors[i];
                if (mapDescriptor.MapSet == mapSet && mapDescriptor.Zone == zone)
                {
                    if (mapDescriptor.Order >= 0)
                    {
                        ordering.Add(i, mapDescriptor.Order);
                    }
                    else
                    {
                        validation.AddProblem(i, typeof(MapDescriptor).GetProperty("Order"), "The lowest order within a zone must be a positive value");
                        validation.Passed = false;
                    }
                }
            }
            var distinct = ordering.Values.Distinct().ToList();
            if (distinct.Any())
            {
                for (short i = 0; i < mapDescriptors.Count; i++)
                {
                    var mapDescriptor = mapDescriptors[i];
                    if (mapDescriptor.MapSet == mapSet && mapDescriptor.Zone == zone)
                    {
                        if (distinct.Count != ordering.Count)
                        {
                            validation.AddProblem(i, typeof(MapDescriptor).GetProperty("Order"), "The order value within a zone must be unique");
                            validation.Passed = false;
                        }
                        if (distinct.Min() != 0)
                        {
                            validation.AddProblem(i, typeof(MapDescriptor).GetProperty("Order"), "The lowest order within a zone must be 0");
                            validation.Passed = false;
                        }
                        if (distinct.Max() != distinct.Count - 1)
                        {
                            validation.AddProblem(i, typeof(MapDescriptor).GetProperty("Order"), "There must be no gaps in the ordering");
                            validation.Passed = false;
                        }
                    }
                }
            }
            else
            {
                for (short i = 0; i < mapDescriptors.Count; i++)
                {
                    var mapDescriptor = mapDescriptors[i];
                    if (mapDescriptor.MapSet != mapSet || mapDescriptor.Zone != zone)
                    {
                        validation.AddProblem(i, typeof(MapDescriptor).GetProperty("Order"), "No map has been assigned to zone " + zone + " in MapSet " + mapSet);
                        validation.Passed = false;
                    }
                }
            }
            return validation;
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
                    progress.Report("[" + UnlockID + "] " + Name[Locale.EN] + ": frb target amount is " + board.BoardInfo.TargetAmount + " but md target amount is " + TargetAmount + ". The frb target amount has no effect.");
                }
                LoopingMode = board.BoardInfo.GalaxyStatus;
                if (LoopingMode != LoopingMode.None)
                {
                    if (LoopingModeRadius == 0 || LoopingModeVerticalSquareCount == 0)
                    {
                        progress.Report("[" + UnlockID + "] " + Name[Locale.EN] + ": frb has looping enabled, but looping parameters are missing in the md.");
                    }
                }
                else if (LoopingMode == LoopingMode.None)
                {
                    if (LoopingModeRadius != 0 || LoopingModeHorizontalPadding != 0 || LoopingModeVerticalSquareCount != 0)
                    {
                        progress.Report("[" + UnlockID + "] " + Name[Locale.EN] + ": frb has looping disabled. The looping parameters defined in the md will have no effect.");
                    }
                }
                foreach (var square in board.BoardData.Squares)
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
            Content,
            Description,
            HeadingIgnore,
            HeadingKeyValueTable,
            HeadingBackgroundTable,
            HeadingIconTable,
            HeadingBGMTable,
            HeadingVentureCardTable
        }
        public void readMapDescriptorFromFile(string fileName, string internalName)
        {
            string[] lines = File.ReadAllLines(fileName);
            MapDescriptor mapDescriptor = new MapDescriptor();
            mapDescriptor.MapDescriptorFilePath = fileName;
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
                if (newState == MDParserState.Content)
                {
                    mapDescriptor.parseContent(state, line);
                }
                else
                {
                    state = newState;
                }
            }
            // if a locale is missing, use the english as default
            foreach (string locale in Locale.ALL)
            {
                if (!mapDescriptor.Name.ContainsKey(locale))
                {
                    mapDescriptor.Name[locale] = mapDescriptor.Name[Locale.EN];
                }
                if (!mapDescriptor.Desc.ContainsKey(locale))
                {
                    mapDescriptor.Desc[locale] = mapDescriptor.Desc[Locale.EN];
                }
            }
            // if everything went well, set the parsed map descriptor values
            setFromImport(mapDescriptor);
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
                case MDParserState.HeadingKeyValueTable:
                    columns = line.Split('|');
                    if (columns.Length > 1)
                    {
                        KeyValuePair<string, string> keyValuePair = parseKeyValuePair(columns);
                        setProperty(keyValuePair);
                    }
                    break;
                case MDParserState.HeadingBackgroundTable:
                    columns = line.Split('|');
                    if (columns.Length > 1)
                    {
                        string flaggedValueOrNull = parseFlaggedValueOrReturnNull(columns);
                        if (flaggedValueOrNull != null)
                        {
                            Background = flaggedValueOrNull;
                            if (string.IsNullOrEmpty(MapIcon))
                            {
                                if (Background == "bg901")
                                    MapIcon = "p_bg_901";
                                else
                                    VanillaDatabase.getMapIconFromVanillaBackground(Background).IfPresent(value => MapIcon = value);
                            }
                            if (!IsBgmIdInitialized())
                                VanillaDatabase.getBgmIdFromVanillaBackground(Background).IfPresent(value => BGMID = value);
                        }
                    }
                    break;
                case MDParserState.HeadingIconTable:
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
                case MDParserState.HeadingBGMTable:
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
                case MDParserState.HeadingVentureCardTable:
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
                case MDParserState.HeadingIgnore:
                    // ignore contents
                    break;
            }
        }

        private void setProperty(KeyValuePair<string, string> keyValuePair)
        {
            var key = keyValuePair.Key.Replace(" ", "").Replace("(", "").Replace(")", "").ToLower();
            switch (key)
            {
                case "radius":
                    LoopingModeRadius = Single.Parse(keyValuePair.Value, CultureInfo.InvariantCulture);
                    break;
                case "horizontalpadding":
                    LoopingModeHorizontalPadding = Single.Parse(keyValuePair.Value, CultureInfo.InvariantCulture);
                    break;
                case "verticalsquarecount":
                    LoopingModeVerticalSquareCount = Single.Parse(keyValuePair.Value, CultureInfo.InvariantCulture);
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
                Single value = Single.Parse(keyValuePair.Value, CultureInfo.InvariantCulture);
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
                    return MDParserState.HeadingKeyValueTable;
                }
                else if (headingText.Contains("music"))
                {
                    return MDParserState.HeadingBGMTable;
                }
                else if (headingText.Contains("background"))
                {
                    return MDParserState.HeadingBackgroundTable;
                }
                else if (headingText.Contains("icon"))
                {
                    return MDParserState.HeadingIconTable;
                }
                else if (headingText.Contains("card"))
                {
                    return MDParserState.HeadingVentureCardTable;
                }
                else
                {
                    return MDParserState.HeadingIgnore;
                }
            }
            return MDParserState.Content;
        }

        public void set(MapDescriptor mapDescriptor)
        {
            MapSet = mapDescriptor.MapSet;
            Zone = mapDescriptor.Zone;
            Order = mapDescriptor.Order;
            IsPracticeBoard = mapDescriptor.IsPracticeBoard;
            UnlockID = mapDescriptor.UnlockID;
            Name_MSG_ID = mapDescriptor.Name_MSG_ID;
            Desc_MSG_ID = mapDescriptor.Desc_MSG_ID;
            setFromImport(mapDescriptor);
        }

        public void setFromImport(MapDescriptor mapDescriptor)
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
                    Name[locale] = mapDescriptor.Name[locale];
                if (mapDescriptor.Desc.ContainsKey(locale))
                    Desc[locale] = mapDescriptor.Desc[locale];
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
            Dirty = mapDescriptor.Dirty;
            MapDescriptorFilePath = mapDescriptor.MapDescriptorFilePath;
        }

        public string generateMapDescriptorFileContent()
        {
            MapDescriptorTemplate t = new MapDescriptorTemplate(this);
            return t.TransformText().TrimStart();
        }

        public override bool Equals(object x)
        {
            if (!(x is MapDescriptor)) return false;
            var y = x as MapDescriptor;
            if (MapSet != y.MapSet) return false;
            if (Zone != y.Zone) return false;
            if (Order != y.Order) return false;
            if (IsPracticeBoard != y.IsPracticeBoard) return false;
            if (Name_En != y.Name_En) return false;
            if (RuleSet != y.RuleSet) return false;
            if (InitialCash != y.InitialCash) return false;
            if (TargetAmount != y.TargetAmount) return false;
            if (BaseSalary != y.BaseSalary) return false;
            if (SalaryIncrement != y.SalaryIncrement) return false;
            if (MaxDiceRoll != y.MaxDiceRoll) return false;
            if (!VentureCard.SequenceEqual(y.VentureCard)) return false;
            if (MaxDiceRoll != y.MaxDiceRoll) return false;
            if (FrbFile1 != y.FrbFile1) return false;
            if (FrbFile2 != y.FrbFile2) return false;
            if (FrbFile3 != y.FrbFile3) return false;
            if (FrbFile4 != y.FrbFile4) return false;
            if (SwitchRotationOriginPoints_ != y.SwitchRotationOriginPoints_) return false;
            if (Theme != y.Theme) return false;
            if (Background != y.Background) return false;
            if (BGMID != y.BGMID) return false;
            if (MapIcon != y.MapIcon) return false;
            if (LoopingMode != y.LoopingMode) return false;
            if (LoopingModeRadius != y.LoopingModeRadius) return false;
            if (LoopingModeHorizontalPadding != y.LoopingModeHorizontalPadding) return false;
            if (LoopingModeVerticalSquareCount != y.LoopingModeVerticalSquareCount) return false;
            if (TourBankruptcyLimit != y.TourBankruptcyLimit) return false;
            if (TourInitialCash != y.TourInitialCash) return false;
            if (TourOpponent1 != y.TourOpponent1) return false;
            if (TourOpponent2 != y.TourOpponent2) return false;
            if (TourOpponent3 != y.TourOpponent3) return false;
            if (TourClearRank != y.TourClearRank) return false;
            if (UnlockID != y.UnlockID) return false;
            if (Name_MSG_ID != y.Name_MSG_ID) return false;
            if (Name_DE != y.Name_DE) return false;
            if (Name_FR != y.Name_FR) return false;
            if (Name_IT != y.Name_IT) return false;
            if (Name_JP != y.Name_JP) return false;
            if (Name_ES != y.Name_ES) return false;
            if (Desc_MSG_ID != y.Desc_MSG_ID) return false;
            if (Desc_EN != y.Desc_EN) return false;
            if (Desc_DE != y.Desc_DE) return false;
            if (Desc_FR != y.Desc_FR) return false;
            if (Desc_IT != y.Desc_IT) return false;
            if (Desc_JP != y.Desc_JP) return false;
            if (Desc_ES != y.Desc_ES) return false;
            if (InternalName != y.InternalName) return false;
            return true;
        }

        public override int GetHashCode()
        {
            return InternalName.GetHashCode() ^ RuleSet.GetHashCode();
        }
    }
}
