using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MiscUtil.IO;
using System.IO;
using Editor.FSData;

namespace FSEditor.FSData {
	public class BoardFile : Header {

        public new const uint Size = Header.Size + BoardInfo.Size + BoardData.Size;

		#region Fields & Properties
        /// <summary>
        /// Occupies 8 bytes
        /// Seems to be always 0
        /// </summary>
        public UInt64 Unknown { get; set; }

		// ----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets the board information header.
		/// </summary>
		public BoardInfo BoardInfo { get; set; }

		/// <summary>
		/// Gets or sets the board data header holding information about the board's squares.
		/// </summary>
		public BoardData BoardData { get; set; }

        // ----------------------------------------------------------------------------------------------------
		#endregion

		#region Initialization
		// ----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Default Constructor.
		/// </summary>
		public BoardFile() : base("I4DT") {} 
		// ----------------------------------------------------------------------------------------------------
		#endregion

		#region Loading & Writing Methods
        public static BoardFile LoadDefault()
        {
            BoardFile boardFile = new BoardFile();
            boardFile.BoardInfo = new BoardInfo();
            boardFile.BoardData = new BoardData();
 
            SquareData bankSquare = SquareData.LoadDefault((byte)0);
            bankSquare.SquareTypeId = 1;
            boardFile.BoardData.Squares.Add(bankSquare);
            return boardFile;
        }
		// ----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Loads file information from a stream.
		/// </summary>
		/// <param name="stream">The stream to read from.</param>
		/// <returns>A new FileInfo object holding information about the file.</returns>
		public static BoardFile LoadFromStream(EndianBinaryReader stream)
        {
			// Seek to the beginning of the header.
			stream.Seek(0x00, SeekOrigin.Begin);
			BoardFile boardFile = new BoardFile();

			// Verify Header & Read FileSize
			boardFile.ReadMagicNumberAndHeaderSize(stream);

            // Load Unknown
            boardFile.Unknown = stream.ReadUInt64();

			// Load BoardInfo
			boardFile.BoardInfo = BoardInfo.LoadFromStream(stream);

			// Load BoardData
			boardFile.BoardData = BoardData.LoadFromStream(stream);

            // Load Autopath Data
            if (boardFile.BoardInfo.VersionFlag > 0)
            {
                AutopathData.LoadFromStream(stream, boardFile.BoardData.Squares);
            }

            return boardFile;
		}

        public void WriteToStream(EndianBinaryWriter stream)
        {
            // Seek to the beginning of the header.
            stream.Seek(0x00, SeekOrigin.Begin);

            HeaderSize = (uint)(BoardFile.Size + (BoardData.Squares.Count * SquareData.Size));

            // Verify Header & Read FileSize
            WriteMagicNumberAndHeaderSize(stream);

            // Write Unknown
            stream.Write(Unknown);

            // Write BoardInfo
            BoardInfo.WriteToStream(stream);

            // Write BoardData
            BoardData.WriteToStream(stream);

			// Write Autopath Data
            AutopathData.WriteToStream(stream, BoardData.Squares);
        }
		// ----------------------------------------------------------------------------------------------------
		#endregion
	}
}
