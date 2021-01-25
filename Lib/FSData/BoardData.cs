using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MiscUtil.IO;
using System.IO;
using System.Drawing;
using System.Collections.ObjectModel;
using Editor.FSData;

namespace FSEditor.FSData {
	public class BoardData : Header {
		#region Fields & Properties

        public new const uint Size = 0x10;

        /// <summary>
		/// Gets or sets the squares in this board.
		/// </summary>
		public ObservableCollection<SquareData> Squares { get; set; }
		// ----------------------------------------------------------------------------------------------------
		#endregion

		#region Initialization
		// ----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Default Constructor
		/// </summary>
		public BoardData() : base("I4PL") {
            this.Squares = new ObservableCollection<SquareData>();
        }
		// ----------------------------------------------------------------------------------------------------
		#endregion

		#region Loading & Writing Methods
		// ----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Loads a board from a stream.
		/// </summary>
		/// <param name="stream">The stream to read from.</param>
		/// <returns>A new BoardData object representing the contents of a FortuneStreet board.</returns>
		public static BoardData LoadFromStream(EndianBinaryReader stream) {
			var board = new BoardData();
            ushort squareCount;

			// Verify Header & Read FileSize
			board.ReadMagicNumberAndHeaderSize(stream);

            stream.ReadUInt32(); // Padding
            squareCount = stream.ReadUInt16();
            stream.ReadUInt16(); // Padding

			// Loading Square Data
			for (byte i = 0; i < squareCount; i++) {
				board.Squares.Add(SquareData.LoadFromStream(stream, i));
			}

			return board;
		}

        public void WriteToStream(EndianBinaryWriter stream)
        {
            HeaderSize = (UInt32)(Size + (Squares.Count * SquareData.Size));

			// Verify Header & Read FileSize
            WriteMagicNumberAndHeaderSize(stream);

			// Read Square Count
            stream.Write((Int32)0);
            stream.Write((UInt16)Squares.Count);
            stream.Write((Int16)0);

			// Loading Square Data
            for (Byte i = 0; i < Squares.Count; i++)
            {
                Squares[i].WriteToStream(stream);
			}
        }
		// ----------------------------------------------------------------------------------------------------
		#endregion
	}
}
