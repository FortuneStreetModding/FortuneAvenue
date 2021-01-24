using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MiscUtil.IO;
using System.ComponentModel;

namespace FSEditor.FSData {
	public class SquareData : INotifyPropertyChanged
    {
        #region Fields & Properties

        public const int Size = 0x20;

        private Boolean initialized = false;
		// ----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets the Square ID.
		/// </summary>
        public Byte Id
        {
            get
            {
                return _Id;
            }

            set
            {
                _Id = value;
                RaisePropertyChanged("Id");
            }
        }
        private Byte _Id;

		/// <summary>
		/// Gets or sets the ID of this SquareType.
		/// </summary>
        public UInt16 SquareTypeId
        {
            get
            {
                return _SquareTypeId;
            }

            set
            {
                _SquareTypeId = value;
                RaisePropertyChanged("SquareTypeId");
                RaisePropertyChanged("SquareType");
            }
        }
        private UInt16 _SquareTypeId;

        public SquareType SquareType 
        { 
            get 
            {
                return (FSData.SquareType)SquareTypeId; 
            }
            set
            {
                _SquareTypeId = (UInt16)value;
                RaisePropertyChanged("SquareTypeId");
                RaisePropertyChanged("SquareType");
            }
        }

		/// <summary>
		/// Gets or sets the position of this square.
		/// </summary>
		public Position Position { get; set; }

        /// <summary>
        /// Is unused by the game
        /// </summary>
        public UInt16 Unknown1 { get; set; }

		/// <summary>
		/// Gets or sets the WaypointData objects.
		/// </summary>
		public WaypointData Waypoint1 { get; set; }
        public WaypointData Waypoint2 { get; set; }
        public WaypointData Waypoint3 { get; set; }
        public WaypointData Waypoint4 { get; set; }

        public WaypointData[] Waypoints { get; set; }

		/// <summary>
		/// Gets or sets the District ID of this property square or the Destination ID of this Warp/Pipe/Door Backtracking square.
		/// </summary>
        public Byte DistrictDestinationId
        {
            get
            {
                return _DistrictDestinationId;
            }

            set
            {
                _DistrictDestinationId = value;
                RaisePropertyChanged("DistrictDestinationId");
            }
        }
        private Byte _DistrictDestinationId;

        /// <summary>
        /// If 1, the lift platform will not deactivate
        /// </summary>
        public Byte OneWayLift { get; set; }

		/// <summary>
		/// Gets or sets the value of the unowned/vacant property.
		/// </summary>
        public UInt16 Value
        {
            get
            {
                return _Value;
            }

            set
            {
                _Value = value;

                if(initialized) {
                    double yield = -0.15 * Math.Pow(0.2, 0.005 * value) + 0.2;
                    int price = (int)Math.Round(value * yield - 0.2);
                    Price = (ushort)price;
                }

                RaisePropertyChanged("Value");
            }
        }
        private UInt16 _Value;

		/// <summary>
		/// Gets or sets the price of the shop/property.
		/// </summary>
        public UInt16 Price
        {
            get
            {
                return _Price;
            }

            set
            {
                _Price = value;
                RaisePropertyChanged("Price");
            }
        }
        private UInt16 _Price;

		/// <summary>
		/// Is unused by the game
		/// </summary>
		public Byte Unknown2 { get; set; }

		/// <summary>
		/// Gets or sets the Shop Model ID.
		/// </summary>
		public Byte ShopModelId {
            get
            {
                return _ShopModelId;
            }
            set
            {
                _ShopModelId = value;
				if(initialized) {
	                Value = (ushort) (value * 10);
	                RaisePropertyChanged("ShopModelId");
				}
            }
        }
        private Byte _ShopModelId;
        // ----------------------------------------------------------------------------------------------------
        #endregion

        #region Custom Editor Values Not Written To FRB

        /// <summary>
        /// Surrounding tiles for the purpose of autopathing.
        /// </summary>
        public SquareData upper;
        public SquareData lower;
        public SquareData left;
        public SquareData right;
        public SquareData upperRight;
        public SquareData upperLeft;
        public SquareData lowerRight;
        public SquareData lowerLeft;

        /// <summary>
        /// Booleans to control autopathing logic.
        /// </summary>
        
        public bool fromSouthAllowSouthWest = true;
        public bool fromSouthAllowWest = true;
        public bool fromSouthAllowNorthWest = true;
        public bool fromSouthAllowNorth = true;
        public bool fromSouthAllowNorthEast = true;
        public bool fromSouthAllowEast = true;
        public bool fromSouthAllowSouthEast = true;

        public bool fromSouthWestAllowWest = true;
        public bool fromSouthWestAllowNorthWest = true;
        public bool fromSouthWestAllowNorth = true;
        public bool fromSouthWestAllowNorthEast = true;
        public bool fromSouthWestAllowEast = true;
        public bool fromSouthWestAllowSouthEast = true;
        public bool fromSouthWestAllowSouth = true;

        public bool fromWestAllowSouthWest = true;
        public bool fromWestAllowNorthWest = true;
        public bool fromWestAllowNorth = true;
        public bool fromWestAllowNorthEast = true;
        public bool fromWestAllowEast = true;
        public bool fromWestAllowSouthEast = true;
        public bool fromWestAllowSouth = true;

        public bool fromNorthWestAllowSouthWest = true;
        public bool fromNorthWestAllowWest = true;
        public bool fromNorthWestAllowNorth = true;
        public bool fromNorthWestAllowNorthEast = true;
        public bool fromNorthWestAllowEast = true;
        public bool fromNorthWestAllowSouthEast = true;
        public bool fromNorthWestAllowSouth = true;

        public bool fromNorthAllowSouthWest = true;
        public bool fromNorthAllowWest = true;
        public bool fromNorthAllowNorthWest = true;
        public bool fromNorthAllowNorthEast = true;
        public bool fromNorthAllowEast = true;
        public bool fromNorthAllowSouthEast = true;
        public bool fromNorthAllowSouth = true;

        public bool fromNorthEastAllowSouthWest = true;
        public bool fromNorthEastAllowWest = true;
        public bool fromNorthEastAllowNorthWest = true;
        public bool fromNorthEastAllowNorth = true;
        public bool fromNorthEastAllowEast = true;
        public bool fromNorthEastAllowSouthEast = true;
        public bool fromNorthEastAllowSouth = true;

        public bool fromEastAllowSouthWest = true;
        public bool fromEastAllowWest = true;
        public bool fromEastAllowNorthWest = true;
        public bool fromEastAllowNorth = true;
        public bool fromEastAllowNorthEast = true;
        public bool fromEastAllowSouthEast = true;
        public bool fromEastAllowSouth = true;

        public bool fromSouthEastAllowSouthWest = true;
        public bool fromSouthEastAllowWest = true;
        public bool fromSouthEastAllowNorthWest = true;
        public bool fromSouthEastAllowNorth = true;
        public bool fromSouthEastAllowNorthEast = true;
        public bool fromSouthEastAllowEast = true;
        public bool fromSouthEastAllowSouth = true;

        #endregion

        #region Loading & Writing Methods
        public static SquareData LoadDefault(Byte squareId)
        {
            var newSquare = new SquareData()
            {
                Id = squareId,
                Position = new Position(0, 0),
                Waypoint1 = new WaypointData()
                {
                    EntryId = 255,
                    Destination1 = 255,
                    Destination2 = 255,
                    Destination3 = 255,
                },
                Waypoint2 = new WaypointData()
                {
                    EntryId = 255,
                    Destination1 = 255,
                    Destination2 = 255,
                    Destination3 = 255,
                },
                Waypoint3 = new WaypointData()
                {
                    EntryId = 255,
                    Destination1 = 255,
                    Destination2 = 255,
                    Destination3 = 255,
                },
                Waypoint4 = new WaypointData()
                {
                    EntryId = 255,
                    Destination1 = 255,
                    Destination2 = 255,
                    Destination3 = 255,
                },
            };

            newSquare.Waypoints = new WaypointData[] { newSquare.Waypoint1, newSquare.Waypoint2, newSquare.Waypoint3, newSquare.Waypoint4, };
            newSquare.initialized = true;
            return newSquare;
        }

		// ----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Loads a new SquareData from a stream.
		/// </summary>
		/// <param name="stream">The stream to load from.</param>
		/// <returns>A new SquareData representing a square.</returns>
		public static SquareData LoadFromStream(EndianBinaryReader stream, Byte squareId) {
			SquareData data = new SquareData();
			data.Id = squareId;

			// Read SquareType ID
			data.SquareTypeId = stream.ReadUInt16();

			// Read Position
			data.Position = new Position(stream.ReadInt16(), stream.ReadInt16());

			// Read Waypoints
            data.Unknown1 = stream.ReadUInt16();
			data.Waypoint1 = WaypointData.LoadFromStream(stream);
			data.Waypoint2 = WaypointData.LoadFromStream(stream);
			data.Waypoint3 = WaypointData.LoadFromStream(stream);
			data.Waypoint4 = WaypointData.LoadFromStream(stream);
            data.Waypoints = new WaypointData[] { data.Waypoint1, data.Waypoint2, data.Waypoint3, data.Waypoint4, }; 

			// Read District ID / Destination ID
			data.DistrictDestinationId = stream.ReadByte();

            data.OneWayLift = stream.ReadByte();

			// Read Property Value
			data.Value = stream.ReadUInt16();

			// Read Shop Price
			data.Price = stream.ReadUInt16();

			// Read Unknown Value
			data.Unknown2 = stream.ReadByte();

			// Read Shop Model ID
			data.ShopModelId = stream.ReadByte();

            data.initialized = true;

            return data;
		}

        public void WriteToStream(EndianBinaryWriter stream)
        {
            stream.Write(SquareTypeId);

            stream.Write(Position.X);
            stream.Write(Position.Y);

            stream.Write(Unknown1);
            Waypoint1.WriteToStream(stream);
            Waypoint2.WriteToStream(stream);
            Waypoint3.WriteToStream(stream);
            Waypoint4.WriteToStream(stream);

            stream.Write(DistrictDestinationId);

            stream.Write(OneWayLift);

            stream.Write(Value);

            stream.Write(Price);

            stream.Write(Unknown2);

            stream.Write(ShopModelId);
        }
		// ----------------------------------------------------------------------------------------------------
		#endregion

        #region INotifyPropertyChanged Members
        [NonSerialized]
        protected System.ComponentModel.PropertyChangedEventHandler _PropertyChanged;
        public virtual event System.ComponentModel.PropertyChangedEventHandler PropertyChanged
        {
            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.Synchronized)]
            add
            {
                _PropertyChanged += value;
            }

            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.Synchronized)]
            remove
            {
                _PropertyChanged -= value;
            }
        }

        protected virtual void RaisePropertyChanged(string name)
        {
            System.ComponentModel.PropertyChangedEventHandler handler = _PropertyChanged;
            if (handler != null)
            {
                handler(this, new System.ComponentModel.PropertyChangedEventArgs(name));
            }
        }
        #endregion
	}
}
