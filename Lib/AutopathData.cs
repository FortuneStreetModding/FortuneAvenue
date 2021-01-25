using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FSEditor.FSData;
using MiscUtil.IO;

namespace Editor.FSData
{
    public class AutopathData {
        #region Fields & Properties

        #endregion

        #region Initialization
        // ----------------------------------------------------------------------------------------------------
        /// <summary>
        /// Default Constructor
        /// </summary>
        public AutopathData()
        {

        }
        // ----------------------------------------------------------------------------------------------------
        #endregion

        #region Loading & Writing Methods

        public static void LoadFromStream(EndianBinaryReader stream, ObservableCollection<SquareData> boardSquares)
        {
            foreach (var s in boardSquares)
            {
                s.fromSouthAllowSouthWest = stream.ReadBoolean();
                s.fromSouthAllowWest = stream.ReadBoolean();
                s.fromSouthAllowNorthWest = stream.ReadBoolean();
                s.fromSouthAllowNorth = stream.ReadBoolean();
                s.fromSouthAllowNorthEast = stream.ReadBoolean();
                s.fromSouthAllowEast = stream.ReadBoolean();
                s.fromSouthAllowSouthEast = stream.ReadBoolean();
                s.fromSouthWestAllowWest = stream.ReadBoolean();
                s.fromSouthWestAllowNorthWest = stream.ReadBoolean();
                s.fromSouthWestAllowNorth = stream.ReadBoolean();
                s.fromSouthWestAllowNorthEast = stream.ReadBoolean();
                s.fromSouthWestAllowEast = stream.ReadBoolean();
                s.fromSouthWestAllowSouthEast = stream.ReadBoolean();
                s.fromSouthWestAllowSouth = stream.ReadBoolean();
                s.fromWestAllowSouthWest = stream.ReadBoolean();
                s.fromWestAllowNorthWest = stream.ReadBoolean();
                s.fromWestAllowNorth = stream.ReadBoolean();
                s.fromWestAllowNorthEast = stream.ReadBoolean();
                s.fromWestAllowEast = stream.ReadBoolean();
                s.fromWestAllowSouthEast = stream.ReadBoolean();
                s.fromWestAllowSouth = stream.ReadBoolean();
                s.fromNorthWestAllowSouthWest = stream.ReadBoolean();
                s.fromNorthWestAllowWest = stream.ReadBoolean();
                s.fromNorthWestAllowNorth = stream.ReadBoolean();
                s.fromNorthWestAllowNorthEast = stream.ReadBoolean();
                s.fromNorthWestAllowEast = stream.ReadBoolean();
                s.fromNorthWestAllowSouthEast = stream.ReadBoolean();
                s.fromNorthWestAllowSouth = stream.ReadBoolean();
                s.fromNorthAllowSouthWest = stream.ReadBoolean();
                s.fromNorthAllowWest = stream.ReadBoolean();
                s.fromNorthAllowNorthWest = stream.ReadBoolean();
                s.fromNorthAllowNorthEast = stream.ReadBoolean();
                s.fromNorthAllowEast = stream.ReadBoolean();
                s.fromNorthAllowSouthEast = stream.ReadBoolean();
                s.fromNorthAllowSouth = stream.ReadBoolean();
                s.fromNorthEastAllowSouthWest = stream.ReadBoolean();
                s.fromNorthEastAllowWest = stream.ReadBoolean();
                s.fromNorthEastAllowNorthWest = stream.ReadBoolean();
                s.fromNorthEastAllowNorth = stream.ReadBoolean();
                s.fromNorthEastAllowEast = stream.ReadBoolean();
                s.fromNorthEastAllowSouthEast = stream.ReadBoolean();
                s.fromNorthEastAllowSouth = stream.ReadBoolean();
                s.fromEastAllowSouthWest = stream.ReadBoolean();
                s.fromEastAllowWest = stream.ReadBoolean();
                s.fromEastAllowNorthWest = stream.ReadBoolean();
                s.fromEastAllowNorth = stream.ReadBoolean();
                s.fromEastAllowNorthEast = stream.ReadBoolean();
                s.fromEastAllowSouthEast = stream.ReadBoolean();
                s.fromEastAllowSouth = stream.ReadBoolean();
                s.fromSouthEastAllowSouthWest = stream.ReadBoolean();
                s.fromSouthEastAllowWest = stream.ReadBoolean();
                s.fromSouthEastAllowNorthWest = stream.ReadBoolean();
                s.fromSouthEastAllowNorth = stream.ReadBoolean();
                s.fromSouthEastAllowNorthEast = stream.ReadBoolean();
                s.fromSouthEastAllowEast = stream.ReadBoolean();
                s.fromSouthEastAllowSouth = stream.ReadBoolean();
            }
        }
        // ----------------------------------------------------------------------------------------------------
        /// <summary>
        /// Loads a board from a stream.
        /// </summary>
        /// <param name="stream">The stream to read from.</param>
        /// <returns>A new BoardData object representing the contents of a FortuneStreet board.</returns>
        public static void WriteToStream(EndianBinaryWriter stream, ObservableCollection<SquareData> boardSquares)
        {
            foreach (var s in boardSquares)
            {
                stream.Write((bool)s.fromSouthAllowSouthWest);
                stream.Write((bool)s.fromSouthAllowWest);
                stream.Write((bool)s.fromSouthAllowNorthWest);
                stream.Write((bool)s.fromSouthAllowNorth);
                stream.Write((bool)s.fromSouthAllowNorthEast);
                stream.Write((bool)s.fromSouthAllowEast);
                stream.Write((bool)s.fromSouthAllowSouthEast);
                stream.Write((bool)s.fromSouthWestAllowWest);
                stream.Write((bool)s.fromSouthWestAllowNorthWest);
                stream.Write((bool)s.fromSouthWestAllowNorth);
                stream.Write((bool)s.fromSouthWestAllowNorthEast);
                stream.Write((bool)s.fromSouthWestAllowEast);
                stream.Write((bool)s.fromSouthWestAllowSouthEast);
                stream.Write((bool)s.fromSouthWestAllowSouth);
                stream.Write((bool)s.fromWestAllowSouthWest);
                stream.Write((bool)s.fromWestAllowNorthWest);
                stream.Write((bool)s.fromWestAllowNorth);
                stream.Write((bool)s.fromWestAllowNorthEast);
                stream.Write((bool)s.fromWestAllowEast);
                stream.Write((bool)s.fromWestAllowSouthEast);
                stream.Write((bool)s.fromWestAllowSouth);
                stream.Write((bool)s.fromNorthWestAllowSouthWest);
                stream.Write((bool)s.fromNorthWestAllowWest);
                stream.Write((bool)s.fromNorthWestAllowNorth);
                stream.Write((bool)s.fromNorthWestAllowNorthEast);
                stream.Write((bool)s.fromNorthWestAllowEast);
                stream.Write((bool)s.fromNorthWestAllowSouthEast);
                stream.Write((bool)s.fromNorthWestAllowSouth);
                stream.Write((bool)s.fromNorthAllowSouthWest);
                stream.Write((bool)s.fromNorthAllowWest);
                stream.Write((bool)s.fromNorthAllowNorthWest);
                stream.Write((bool)s.fromNorthAllowNorthEast);
                stream.Write((bool)s.fromNorthAllowEast);
                stream.Write((bool)s.fromNorthAllowSouthEast);
                stream.Write((bool)s.fromNorthAllowSouth);
                stream.Write((bool)s.fromNorthEastAllowSouthWest);
                stream.Write((bool)s.fromNorthEastAllowWest);
                stream.Write((bool)s.fromNorthEastAllowNorthWest);
                stream.Write((bool)s.fromNorthEastAllowNorth);
                stream.Write((bool)s.fromNorthEastAllowEast);
                stream.Write((bool)s.fromNorthEastAllowSouthEast);
                stream.Write((bool)s.fromNorthEastAllowSouth);
                stream.Write((bool)s.fromEastAllowSouthWest);
                stream.Write((bool)s.fromEastAllowWest);
                stream.Write((bool)s.fromEastAllowNorthWest);
                stream.Write((bool)s.fromEastAllowNorth);
                stream.Write((bool)s.fromEastAllowNorthEast);
                stream.Write((bool)s.fromEastAllowSouthEast);
                stream.Write((bool)s.fromEastAllowSouth);
                stream.Write((bool)s.fromSouthEastAllowSouthWest);
                stream.Write((bool)s.fromSouthEastAllowWest);
                stream.Write((bool)s.fromSouthEastAllowNorthWest);
                stream.Write((bool)s.fromSouthEastAllowNorth);
                stream.Write((bool)s.fromSouthEastAllowNorthEast);
                stream.Write((bool)s.fromSouthEastAllowEast);
                stream.Write((bool)s.fromSouthEastAllowSouth);
            }
        }
        #endregion
    }
}
