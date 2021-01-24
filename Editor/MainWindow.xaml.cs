using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using FSEditor.FSData;
using System.IO;
using System.Linq;
using CustomStreetMapManager;
using FortuneAvenue.Modules.Pathing;

namespace Editor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Lists
        public static readonly Dictionary<SquareType, String> SquareTypeList = new Dictionary<SquareType, String>()
        {
            {SquareType.Bank, "Bank"},

            {SquareType.Property, "Property"},
            {SquareType.VacantPlot, "Vacant Plot"},

            {SquareType.VentureSquare, "Venture"},

            {SquareType.SuitSquareSpade, "Suit: Spade"},
            {SquareType.SuitSquareHeart, "Suit: Heart"},
            {SquareType.SuitSquareDiamond, "Suit: Diamond"},
            {SquareType.SuitSquareClub, "Suit: Club"},

            {SquareType.ChangeOfSuitSquareSpade, "Change-of-suit: Spade"},
            {SquareType.ChangeOfSuitSquareHeart, "Change-of-suit: Heart"},
            {SquareType.ChangeOfSuitSquareDiamond, "Change-of-suit: Diamond"},
            {SquareType.ChangeOfSuitSquareClub, "Change-of-suit: Club"},

            {SquareType.TakeABreakSquare, "Take-a-Break"},
            {SquareType.BoonSquare, "Boon"},
            {SquareType.BoomSquare, "Boom"},
            {SquareType.StockBrokerSquare, "Stockbroker"},
            {SquareType.RollOnSquare, "Roll On"},
            {SquareType.ArcadeSquare, "Arcade"},
            {SquareType.CannonSquare, "Cannon"},

            {SquareType.BackStreetSquareA, "Backstreet: A / Green Pipe"},
            {SquareType.BackStreetSquareB, "Backstreet: B / Blue Pipe"},
            {SquareType.BackStreetSquareC, "Backstreet: C / Red Pipe"},
            {SquareType.BackStreetSquareD, "Backstreet: D / Yellow Pipe"},
            {SquareType.BackStreetSquareE, "Backstreet: E / Green Pipe"},

            {SquareType.OneWayAlleySquare, "One-Way Alley"},
            {SquareType.OneWayAlleyDoorA, "One-Way Alley Door: A / Blue Warp"},
            {SquareType.OneWayAlleyDoorB, "One-Way Alley Door: B / Red Warp"},
            {SquareType.OneWayAlleyDoorC, "One-Way Alley Door: C / Yellow Warp"},
            {SquareType.OneWayAlleyDoorD, "One-Way Alley Door: D / Green Warp"},

            {SquareType.SwitchSquare, "Switch"},
            {SquareType.LiftMagmaliceSquareStart, "Lift / Magmalice Start"},
            {SquareType.MagmaliceSquare, "Magmalice"},
            {SquareType.LiftSquareEnd, "Lift End"},

            {SquareType.EventSquare, "Event Square"},
        };
        public static readonly Dictionary<Byte, String> ShopTypeList = new Dictionary<Byte, String>()
        {
            {0, "(empty)"},
            {5, "(50g) Scrap-paper shop"},
            {6, "(60g) Wool shop"},
            {7, "(70g) Bottle store"},
            {8, "(80g) Secondhand book shop"},
            {9, "(90g) Scrap-metal supplier"},
            {10,"(100g) Stationery shop"},
            {11,"(110g) General store"},
            {12,"(120g) Florist's"},
            {13,"(130g) Ice-cream shop"},
            {14,"(140g) Comic-book shop"},
            {15,"(150g) Dairy"},
            {16,"(160g) Doughnut shop"},
            {17,"(170g) Pizza shack"},
            {18,"(180g) Bakery"},
            {19,"(190g) Grocery store"},
            {20,"(200g) Pharmacy"},
            {21,"(210g) Fish market"},
            {22,"(220g) Toy shop"},
            {23,"(230g) Bookshop"},
            {24,"(240g) Cosmetics boutique"},
            {25,"(250g) T-shirt shop"},
            {26,"(260g) Fruit stall"},
            {27,"(270g) Photography studio"},
            {28,"(280g) Coffee shop"},
            {29,"(290g) Butcher shop"},
            {30,"(300g) Restaurant"},
            {31,"(310g) Barbershop"},
            {32,"(320g) Hat boutique"},
            {33,"(330g) Hardware store"},
            {34,"(340g) Gift shop"},
            {35,"(350g) Launderette"},
            {36,"(360g) Shoe shop"},
            {37,"(370g) Clothing store"},
            {38,"(380g) Optician's"},
            {39,"(390g) Clockmaker's"},
            {40,"(400g) Furniture shop"},
            {41,"(410g) Sports shop"},
            {42,"(420g) Locksmith's"},
            {43,"(430g) Glassmaker's"},
            {44,"(440g) Sushi restaurant"},
            {45,"(450g) Art gallery"},
            {46,"(460g) Leatherware boutique"},
            {47,"(470g) Pet shop"},
            {48,"(480g) Nail salon"},
            {49,"(490g) Spice shop"},
            {50,"(500g) Music shop"},
            {51,"(510g) Surf shop"},
            {52,"(520g) Boating shop"},
            {53,"(530g) Cartographer's"},
            {54,"(540g) Alloy rims shop"},
            {55,"(550g) Fashion boutique"},
            {56,"(560g) Waxworks"},
            {57,"(570g) Lens shop"},
            {58,"(580g) Kaleidoscope shop"},
            {59,"(590g) Crystal ball shop"},
            {61,"(610g) Gemstone supplier"},
            {62,"(620g) Taxidermy studio"},
            {65,"(650g) Antiques dealer's"},
            {68,"(680g) Goldsmith's"},
            {70,"(700g) Fossil shop"},
            {72,"(720g) Music-box shop"},
            {75,"(750g) Marionette workshop"},
            {76,"(760g) Health shop"},
            {80,"(800g) Organic food shop"},
            {81,"(810g) Bridal boutique"},
            {85,"(850g) Autograph shop"},
            {90,"(900g) Meteorite shop"},
            {98,"(980g) Department store"},
        };
        #endregion

        public static short Snap { get; private set; }

        private string _currentFileName = null; // Includes position, used for File/Save
        private string _loneFileName = null; // File name only, used for titlebar
        private static bool backgroundAxesOn = false;
        private static Brush backgroundBrush = Brushes.Transparent;

        public MainWindow()
        {
            InitializeComponent();
            TypeComboBox.DisplayMemberPath = "Value";
            TypeComboBox.SelectedValuePath = "Key";
            TypeComboBox.ItemsSource = SquareTypeList;
            ShopComboBox.DisplayMemberPath = "Value";
            ShopComboBox.SelectedValuePath = "Key";
            ShopComboBox.ItemsSource = ShopTypeList;
        }

        private void UpdateTitle()
        {
            if (_loneFileName == null)
                this.Title = "Fortune Avenue - [New Board]";
            else
                this.Title = "Fortune Avenue - [" + _loneFileName + "]";
        }

        private void UpdateGalaxyRadio()
        {
            var board = ((BoardFile)this.DataContext);

            switch (board.BoardInfo.GalaxyStatus)
            {
                case LoopingMode.None:
                    this.GalaxyNLoop.IsChecked = true;
                    break;
                case LoopingMode.Both:
                    this.GalaxyVHLoop.IsChecked = true;
                    break;
                case LoopingMode.Vertical:
                    this.GalaxyVLoop.IsChecked = true;
                    break;
                default:
                    this.GalaxyNLoop.IsChecked = false;
                    this.GalaxyVLoop.IsChecked = false;
                    this.GalaxyVHLoop.IsChecked = false;
                    break;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CenterScrollViewer();
        }

        private void CenterScrollViewer()
        {
            var border = (Border)VisualTreeHelper.GetChild(PART_Squares, 0);

            // Get scrollviewer
            ScrollViewer scrollViewer = border.Child as ScrollViewer;
            if (scrollViewer == null) return;
            // center the Scroll Viewer...

            var cx = scrollViewer.ScrollableWidth / 2.0;
            scrollViewer.ScrollToHorizontalOffset(cx);

            var cy = scrollViewer.ScrollableHeight / 2.0;
            scrollViewer.ScrollToVerticalOffset(cy);
        }

        #region Menu Items

        #region File Menu

        // Corresponds to "File/New"
        private void New_Click(object sender, RoutedEventArgs e)
        {
            _currentFileName = null;
            _loneFileName = null;
            var Board = BoardFile.LoadDefault();
            this.DataContext = Board;
            UpdateTitle();
            UpdateGalaxyRadio();
            CenterScrollViewer();
        }

        // Corresponds to "File/Open"
        private void Open_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.AddExtension = true;
            openFileDialog.CheckFileExists = true;
            openFileDialog.DefaultExt = "frb";
            openFileDialog.Filter = "Fortune Street Board (.frb) | *.frb";
            openFileDialog.Multiselect = false;
            if (openFileDialog.ShowDialog() != true)
                return;

            _currentFileName = openFileDialog.FileName;
            _loneFileName = openFileDialog.SafeFileName;

            using (var stream = openFileDialog.OpenFile())
            {
                MiscUtil.IO.EndianBinaryReader binReader = new MiscUtil.IO.EndianBinaryReader(MiscUtil.Conversion.EndianBitConverter.Big, stream);
                var Board = BoardFile.LoadFromStream(binReader);
                this.DataContext = Board;
            }
            UpdateTitle();
            UpdateGalaxyRadio();
            CenterScrollViewer();
        }

        // Corresponds to "File/Save"
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            var board = ((BoardFile)this.DataContext);

            if (board == null)
                return;

            if (_currentFileName == null)
            {
                SaveAs_Click(sender, e);
                return;
            }

            using (var stream = new FileStream(_currentFileName, System.IO.FileMode.Create, System.IO.FileAccess.ReadWrite, System.IO.FileShare.None))
            {
                MiscUtil.IO.EndianBinaryWriter binWriter = new MiscUtil.IO.EndianBinaryWriter(MiscUtil.Conversion.EndianBitConverter.Big, stream);
                board.WriteToStream(binWriter);
            }
            UpdateTitle();
        }

        // Corresponds to "File/Save As..."
        private void SaveAs_Click(object sender, RoutedEventArgs e)
        {
            var board = ((BoardFile)this.DataContext);

            if (board == null)
                return;

            var saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Fortune Street Board (.frb) | *.frb";
            saveFileDialog.DefaultExt = "frb";
            if (saveFileDialog.ShowDialog() != true)
                return;

            _currentFileName = saveFileDialog.FileName;
            _loneFileName = saveFileDialog.SafeFileName;

            using (var stream = saveFileDialog.OpenFile())
            {
                MiscUtil.IO.EndianBinaryWriter binWriter = new MiscUtil.IO.EndianBinaryWriter(MiscUtil.Conversion.EndianBitConverter.Big, stream);
                board.WriteToStream(binWriter);
            }
            UpdateTitle();
        }

        // Corresponds to "File/Exit"
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            System.Environment.Exit(0);
        }

        #endregion //File Menu

        #region Tools Menu

        // Corresponds to "Tools/Stock Prices"
        private void StockPrices_Click(object sender, RoutedEventArgs e)
        {
            var board = ((BoardFile)this.DataContext);
            if (board == null)
                return;

            var districtCount = new int[12];
            var districtSum = new int[12];
            var highestDistrict = -1;

            foreach (var square in board.BoardData.Squares)
            {
                if (square.DistrictDestinationId >= 12)
                    continue; // ignore invalid districts
                if (square.SquareType == SquareType.VacantPlot)
                    districtSum[square.DistrictDestinationId] += 200; // Vacant lots are always 200g by default
                else if (square.SquareType == SquareType.Property)
                    districtSum[square.DistrictDestinationId] += square.Value;
                else
                    continue; // not a property

                ++districtCount[square.DistrictDestinationId];
                if (highestDistrict < square.DistrictDestinationId)
                    highestDistrict = square.DistrictDestinationId;
            }

            StringBuilder stockssb = new StringBuilder();
            for (var i = 0; i <= highestDistrict; ++i)
            {
                if (districtCount[i] == 0)
                    continue;

                // The base stock value is just the average value of shops in that district
                var stpri = districtSum[i] / districtCount[i];

                // Initial stock has a base multiplier of 0xB00 in 16.16 fixed point,
                // so we simulate that
                stpri *= 0x00000B00;
                stpri >>= 16;

                stockssb.AppendFormat("District {0}: {1}g\n", ((char)('A' + (char)i)).ToString(), stpri);
            }
            MessageBox.Show(stockssb.ToString(), "Stock Prices", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // Corresponds to "Tools/Create Paths"
        // This is virtually unchanged from the original code base.
        private void Autopath_Click(object sender, RoutedEventArgs e)
        {
            var board = ((BoardFile)this.DataContext);

            if (board == null)
                return;

            foreach (var square in board.BoardData.Squares)
            {
                if (square.SquareType == SquareType.OneWayAlleySquare || 
                    square.SquareType == SquareType.OneWayAlleyDoorB || 
                    square.SquareType == SquareType.OneWayAlleyDoorC || 
                    square.SquareType == SquareType.OneWayAlleyDoorD)
                    continue;

                var touchingSquares = new List<SquareData>();
                CheckSurroundingsForSquares(square, board, touchingSquares);
                WaypointModule.PopulateWaypoints(square, touchingSquares);
            }

            MessageBox.Show("Successfully created paths!");
        }

        public void CheckSurroundingsForSquares(SquareData square, BoardFile board, List<SquareData> touchingSquares)
        {
            square.upper = DirectionCheckModule.DoesSquareExistAboveThisOne(square, board, touchingSquares);
            square.lower = DirectionCheckModule.DoesSquareExistBelowThisOne(square, board, touchingSquares);
            square.left = DirectionCheckModule.DoesSquareExistToTheLeftOfThisOne(square, board, touchingSquares);
            square.right = DirectionCheckModule.DoesSquareExistToTheRightOfThisOne(square, board, touchingSquares);

            if (square.upper == null && square.right == null) { square.upperRight = DirectionCheckModule.DoesSquareExistToTheUpperRightOfThisOne(square, board, touchingSquares); }
            if (square.upper == null && square.left == null) { square.upperLeft = DirectionCheckModule.DoesSquareExistToTheUpperLeftOfThisOne(square, board, touchingSquares); }
            if (square.lower == null && square.right == null) { square.lowerRight = DirectionCheckModule.DoesSquareExistToTheLowerRightOfThisOne(square, board, touchingSquares); }
            if (square.lower == null && square.left == null) { square.lowerLeft = DirectionCheckModule.DoesSquareExistToTheLowerLeftOfThisOne(square, board, touchingSquares); }
        }

        // Corresponds to "Tools/Verify Board"
        private void Verify_Click(object sender, RoutedEventArgs e)
        {
            var board = ((BoardFile)this.DataContext);

            if (board == null)
                return;

            var errsb = new StringBuilder();
            var warnsb = new StringBuilder();

            short errors = 0;
            short warnings = 0;

            var districts = new int[12];
            var highestDistrict = -1;

            if (board.BoardData.Squares.Count(t => t.SquareType == SquareType.Bank) == 0)
                warnsb.AppendFormat("W{0}: There should be at least one Bank.\n", ++warnings);

            if (board.BoardData.Squares.Count > 0 && board.BoardData.Squares[0].SquareType != SquareType.Bank)
                warnsb.AppendFormat("W{0}: The starting square (ID 0) should be a Bank.\n", ++warnings);

            if (board.BoardData.Squares.Count < 3)
                errsb.AppendFormat("E{0}: Board must have at least 3 spaces to allow players to move.\n", ++errors);

            if (board.BoardInfo.MaxDiceRoll < 1 || board.BoardInfo.MaxDiceRoll > 9)
                errsb.AppendFormat("E{0}: Max Dice Roll must be between 1 and 9.\n", ++errors);

            foreach (var square in board.BoardData.Squares)
            {
                if (square.SquareType == SquareType.Property || square.SquareType == SquareType.VacantPlot)
                {
                    if (square.DistrictDestinationId > 11)
                        warnsb.AppendFormat("E{0}: Square {1} uses district ID {2}. The maximum is 11.\n", ++errors, square.Id, square.DistrictDestinationId);
                    else
                    {
                        ++districts[square.DistrictDestinationId];
                        if (highestDistrict < square.DistrictDestinationId)
                            highestDistrict = square.DistrictDestinationId;
                    }
                }

                // Ignore waypoints for doors / one way alleys.
                if (square.SquareType == SquareType.OneWayAlleySquare
                    || square.SquareType == SquareType.OneWayAlleyDoorA
                    || square.SquareType == SquareType.OneWayAlleyDoorB
                    || square.SquareType == SquareType.OneWayAlleyDoorC
                    || square.SquareType == SquareType.OneWayAlleyDoorD)
                    continue;

                // Check if too many different destinations
                var destinations = new HashSet<byte>();
                foreach (var waypointData in square.Waypoints)
                {
                    destinations.Add(waypointData.Destination1);
                    destinations.Add(waypointData.Destination2);
                    destinations.Add(waypointData.Destination3);
                }

                if(destinations.Contains(255)) { destinations.Remove(255); }

                if (destinations.Count() > 4) 
                {
                    errsb.AppendFormat("E{2}: Square {0}, has {1} different destinations. The max amount is 4. \n", square.Id, destinations.Count(), ++errors);
                }

                for (var i = 0; i < 4; ++i)
                {
                    //Entry
                    if (square.Waypoints[i].EntryId >= board.BoardData.Squares.Count)
                    {
                        if (square.Waypoints[i].EntryId != 255)
                            errsb.AppendFormat("E{3}: Square {0}, Waypoint {1}, EntryId references square {2} which does not exist\n", square.Id, i + 1, square.Waypoints[i].EntryId, ++errors);
                    }
                    else
                    {
                        var other = board.BoardData.Squares[square.Waypoints[i].EntryId];
                        if (Math.Abs(square.Position.X - other.Position.X) > 96
                            || Math.Abs(square.Position.Y - other.Position.Y) > 96)
                        {
                            warnsb.AppendFormat("W{3}: Square {0}, Waypoint {1}, EntryId references square {2} which is too far away\n", square.Id, i + 1, square.Waypoints[i].EntryId, ++warnings);
                        }
                    }

                    //Destination1
                    if (square.Waypoints[i].Destination1 >= board.BoardData.Squares.Count)
                    {
                        if (square.Waypoints[i].Destination1 != 255)
                            errsb.AppendFormat("E{3}: Square {0}, Waypoint {1}, Destination1 references square {2} which does not exist\n", square.Id, i + 1, square.Waypoints[i].Destination1, ++errors);
                    }
                    else
                    {
                        var other = board.BoardData.Squares[square.Waypoints[i].Destination1];
                        if (Math.Abs(square.Position.X - other.Position.X) > 96
                            || Math.Abs(square.Position.Y - other.Position.Y) > 96)
                        {
                            warnsb.AppendFormat("W{3}: Square {0}, Waypoint {1}, Destination1 references square {2} which is too far away\n", square.Id, i + 1, square.Waypoints[i].Destination1, ++warnings);
                        }
                    }

                    //Destination2
                    if (square.Waypoints[i].Destination2 >= board.BoardData.Squares.Count)
                    {
                        if (square.Waypoints[i].Destination2 != 255)
                            errsb.AppendFormat("E{3}: Square {0}, Waypoint {1}, Destination2 references square {2} which does not exist\n", square.Id, i + 1, square.Waypoints[i].Destination2, ++errors);
                    }
                    else
                    {
                        var other = board.BoardData.Squares[square.Waypoints[i].Destination2];
                        if (Math.Abs(square.Position.X - other.Position.X) > 96
                            || Math.Abs(square.Position.Y - other.Position.Y) > 96)
                        {
                            warnsb.AppendFormat("W{3}: Square {0}, Waypoint {1}, Destination2 references square {2} which is too far away\n", square.Id, i + 1, square.Waypoints[i].Destination2, ++warnings);
                        }
                    }

                    //Destination3
                    if (square.Waypoints[i].Destination3 >= board.BoardData.Squares.Count)
                    {
                        if (square.Waypoints[i].Destination3 != 255)
                            errsb.AppendFormat("E{3}: Square {0}, Waypoint {1}, Destination3 references square {2} which does not exist\n", square.Id, i + 1, square.Waypoints[i].Destination3, ++errors);
                    }
                    else
                    {
                        var other = board.BoardData.Squares[square.Waypoints[i].Destination3];
                        if (Math.Abs(square.Position.X - other.Position.X) > 96
                            || Math.Abs(square.Position.Y - other.Position.Y) > 96)
                        {
                            warnsb.AppendFormat("W{3}: Square {0}, Waypoint {1}, Destination3 references square {2} which is too far away\n", square.Id, i + 1, square.Waypoints[i].Destination3, ++warnings);
                        }
                    }
                }
            }

            for (int i = 0; i <= highestDistrict; ++i)
            {
                if (districts[i] == 0)
                    errsb.AppendFormat("E{0}: District {1} has no plots!  Did you skip a number when assigning districts?\n", ++errors, i);
                else if (districts[i] < 3)
                    warnsb.AppendFormat("W{0}: District {1} has {2} plots available; this makes Max Capital act weirdly. Use a minimum of 3.\n", ++warnings, i, districts[i]);
                else if (districts[i] > 6)
                    errsb.AppendFormat("E{0}: District {1} has {2} plots available; this makes the stock menu crash! Maximum is 6.\n", ++errors, i, districts[i]);
            }

            if (errors == 0 && warnings == 0)
                MessageBox.Show("Board passed verification.", "Board Verification", MessageBoxButton.OK, MessageBoxImage.Information);
            else
            {
                StringBuilder fsb = new StringBuilder();
                fsb.AppendFormat("{0} error(s), {1} warning(s).\n", errors, warnings);
                fsb.Append(errsb.ToString());
                fsb.Append(warnsb.ToString());
                if (errors > 0)
                    MessageBox.Show(fsb.ToString(), "Board Verification", MessageBoxButton.OK, MessageBoxImage.Error);
                else
                    MessageBox.Show(fsb.ToString(), "Board Verification", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        #endregion //Tools Menu

        #region Help Menu

        // Corresponds to "Help/Show Help Dialog"
        private void Help_Click(object sender, RoutedEventArgs e)
        {
            var helpWin = new HelpWindow();
            helpWin.ShowDialog();
        }

        // Corresponds to "Help/About"
        private void About_Click(object sender, RoutedEventArgs e)
        {
            var aboutBox = new AboutBox();
            aboutBox.ShowDialog();
        }

        #endregion //Help Menu

        #endregion //Menu Items

        #region Buttons

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            var board = ((BoardFile)this.DataContext);

            if (board == null)
                return;

            board.BoardData.Squares.Add(SquareData.LoadDefault((byte)board.BoardData.Squares.Count));
        }

        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            var board = ((BoardFile)this.DataContext);
            var selected = (SquareData)PART_Squares.SelectedItem;

            if (board == null || selected == null)
                return;

            board.BoardData.Squares.Remove(selected);

            for (byte i = 0; i < board.BoardData.Squares.Count; ++i)
            {
                var square = board.BoardData.Squares[i];
                square.Id = i;

                for (int j = 0; j < 4; ++j)
                {
                    if (square.Waypoints[j].EntryId == selected.Id)
                        square.Waypoints[j].EntryId = 255;
                    else if (square.Waypoints[j].EntryId > selected.Id && square.Waypoints[j].EntryId != 255)
                        square.Waypoints[j].EntryId--;

                    if (square.Waypoints[j].Destination1 == selected.Id)
                        square.Waypoints[j].Destination1 = 255;
                    else if (square.Waypoints[j].Destination1 > selected.Id && square.Waypoints[j].Destination1 != 255)
                        square.Waypoints[j].Destination1--;

                    if (square.Waypoints[j].Destination2 == selected.Id)
                        square.Waypoints[j].Destination2 = 255;
                    else if (square.Waypoints[j].Destination2 > selected.Id && square.Waypoints[j].Destination2 != 255)
                        square.Waypoints[j].Destination2--;

                    if (square.Waypoints[j].Destination3 == selected.Id)
                        square.Waypoints[j].Destination3 = 255;
                    else if (square.Waypoints[j].Destination3 > selected.Id && square.Waypoints[j].Destination3 != 255)
                        square.Waypoints[j].Destination3--;
                }
            }
        }

        private void SnapAll_Click(object sender, RoutedEventArgs e)
        {
            var board = ((BoardFile)this.DataContext);

            if (board == null)
                return;

            foreach (var square in board.BoardData.Squares)
            {
                square.Position.X = RoundBy(square.Position.X, MainWindow.Snap);
                square.Position.Y = RoundBy(square.Position.Y, MainWindow.Snap);
            }
        }

        // Not technically a button but close enough.
        private void PART_ResetZoom(object sender, MouseButtonEventArgs e)
        {
            PART_Zoom.Value = 1;
        }

        #endregion //Buttons

        // Checks contents of the Snap text box for validity and
        // sets the Snap variable if appropriate.
        // Run when the snap checkbox is checked/unchecked or
        // when the snap textbox value is changed.
        private void SnapCheck(object sender, RoutedEventArgs e)
        {
            CheckBox cb = this.SnapCheckBox;
            TextBox stb = this.SnapTextBox;
            Button sab = this.SnapAllButton;

            Int16 tempSnap;

            try
            {
                tempSnap = System.Convert.ToInt16(stb.Text);
            }
            catch (FormatException)
            {
                // Assume no snap
                tempSnap = 0;
            }

            if (tempSnap < 0)
                stb.Text = "0";
            else if (tempSnap > 64)
                stb.Text = "64";
            else // No auto updating, so apply
            {
                if (cb != null && stb != null && (bool)cb.IsChecked && tempSnap > 1)
                    Snap = tempSnap;
                else
                    Snap = 0;
            }

            if (sab != null)
                sab.IsEnabled = (Snap > 1);
        }

        private void DrawAxesCheck(object sender, RoutedEventArgs e)
        {
            this.Resources["CanvasGrid"] = CanvasGrid(new Rect(0, 0, 2048, 2048), new Size(64, 64));
        }

        private void DrawAxesUncheck(object sender, RoutedEventArgs e)
        {
            this.Resources["CanvasGrid"] = Brushes.Transparent;
        }

        // Rounds x to a multiple of y.
        public static Int16 RoundBy(Int16 x, Int16 y)
        {
            if (x < 0)
                return (short)(((x - (y / 2)) / y) * y);
            return (short)(((x + (y / 2)) / y) * y);
        }

        private void Galaxy_Click(object sender, RoutedEventArgs e)
        {
            var board = ((BoardFile)this.DataContext);

            if ((bool)this.GalaxyNLoop.IsChecked)
            {
                board.BoardInfo.GalaxyStatus = LoopingMode.None;
            }
            else if ((bool)this.GalaxyVLoop.IsChecked)
            {
                board.BoardInfo.GalaxyStatus = LoopingMode.Vertical;
            }
            else if ((bool)this.GalaxyVHLoop.IsChecked)
            {
                board.BoardInfo.GalaxyStatus = LoopingMode.Both;
            }
        }

        private static Brush CanvasGrid(Rect bounds, Size tileSize)
        {
            var brushColor = Brushes.SlateGray;
            var brushThickness = 2.0;
            var tileRect = new Rect(tileSize);


            var gridDots = new DrawingBrush
            {
                Stretch = Stretch.None,
                TileMode = TileMode.Tile,
                Viewport = tileRect,
                ViewportUnits = BrushMappingMode.Absolute,
                Drawing = new GeometryDrawing
                {
                    Geometry = new GeometryGroup
                    {
                        Children = new GeometryCollection
                        {
                            new RectangleGeometry(new Rect(tileRect.TopLeft, new Size(tileRect.Width / 2, tileRect.Height / 2))),
                            new RectangleGeometry(new Rect(new Point(tileRect.Width / 2, tileRect.Height / 2), new Size(tileRect.Width / 2, tileRect.Height / 2)))
                        }
                    }
                }
            };

            var axesGrid = new DrawingBrush
            {
                Stretch = Stretch.None,
                AlignmentX = AlignmentX.Center,
                AlignmentY = AlignmentY.Center,
                Transform = new TranslateTransform(bounds.Left, bounds.Top),
                Drawing = new GeometryDrawing
                {
                    Pen = new Pen(brushColor, brushThickness),
                    Geometry = new GeometryGroup
                    {
                        Children = new GeometryCollection
                        {
                            new RectangleGeometry(new Rect(bounds.Size)),
                            new LineGeometry(new Point((bounds.Left + bounds.Right) / 2, bounds.Top), new Point((bounds.Left + bounds.Right) / 2, bounds.Bottom)),
                            new LineGeometry(new Point(bounds.Left, (bounds.Top + bounds.Bottom) / 2), new Point(bounds.Right, (bounds.Top + bounds.Bottom) / 2))
                        }
                    },
                    Brush = gridDots
                }
            };

            return axesGrid;
        }

        private void calculatePrice(object sender, TextChangedEventArgs e)
        {
            PriceRatio.Text = "";
            int value = 0, price = 0;
            Int32.TryParse(Value.Text, out value);
            Int32.TryParse(Price.Text, out price);
            if (value != 0)
            {
                double yield = (double)price / (double)value;
                PriceRatio.Text = yield.ToString("0.00");
            }
        }

        private void openCSMM(object sender, RoutedEventArgs e)
        {
            CSMM f2 = new CSMM();
            f2.Show();
        }
    }
}
