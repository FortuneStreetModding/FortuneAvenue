using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MiscUtil.IO;
using System.IO;

namespace FSEditor.FSData {
	public class BoardInfo : Header {

        public new const uint Size = 0x20;

		#region Fields & Properties
		/// <summary>
		/// Gets or sets the initial amount of cash all players start with.
		/// </summary>
		public UInt16 InitialCash { get; set; }

		/// <summary>
		/// Gets or sets the target amount of money.
		/// </summary>
		public UInt16 TargetAmount { get; set; }

		/// <summary>
		/// Gets or sets the base salary value.
		/// Salary is calculated: Salary = Base * (Level * Increment)
		/// </summary>
		public UInt16 BaseSalary { get; set; }

		/// <summary>
		/// Gets or sets the salary increment value.
		/// Salary is calculated: Salary = Base * (Level * Increment)
		/// </summary>
		public UInt16 SalaryIncrement { get; set; }

		/// <summary>
		/// Gets or sets the size of the dice.
		/// </summary>
		public UInt16 MaxDiceRoll { get; set; }

        /// <summary>
        /// Determines "galaxy" status -- boards wrapping around on themselves
        /// </summary>
        public LoopingMode GalaxyStatus { get; set; }

        // ----------------------------------------------------------------------------------------------------
		#endregion

		#region Initialization
		// ----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Default Constructor.
		/// </summary>
		public BoardInfo()  : base("I4DT") {
            this.InitialCash = 1000;
            this.TargetAmount = 10000;
            this.BaseSalary = 100;
            this.SalaryIncrement = 100;
            this.MaxDiceRoll = 6;
            this.GalaxyStatus = 0;
        } 
		// ----------------------------------------------------------------------------------------------------
		#endregion

		#region Loading & Writing Methods
		// ----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Loads board information from a stream.
		/// </summary>
		/// <param name="stream">The stream to read from.</param>
		/// <returns>A new BoardInfo object holding information about the board.</returns>
		public static BoardInfo LoadFromStream(EndianBinaryReader stream) {
			BoardInfo boardInfo = new BoardInfo();

			// Verify Header & Read FileSize
			boardInfo.ReadMagicNumberAndHeaderSize(stream);

            stream.ReadUInt64(); // Read padding data.
			boardInfo.InitialCash = stream.ReadUInt16();
			boardInfo.TargetAmount = stream.ReadUInt16();
			boardInfo.BaseSalary = stream.ReadUInt16();
			boardInfo.SalaryIncrement = stream.ReadUInt16();
            boardInfo.MaxDiceRoll = stream.ReadUInt16();
            boardInfo.GalaxyStatus = (LoopingMode) stream.ReadUInt16();
            stream.ReadUInt32(); // Read padding data.

			return boardInfo;
		}

        public void WriteToStream(EndianBinaryWriter stream)
        {
            HeaderSize = Size;

            // Verify Header & Read FileSize
            WriteMagicNumberAndHeaderSize(stream);

            stream.Write((Int64)0); // Data padding.
            stream.Write(InitialCash);
            stream.Write(TargetAmount);
            stream.Write(BaseSalary);
            stream.Write(SalaryIncrement);
            stream.Write(MaxDiceRoll);
            stream.Write((UInt16)GalaxyStatus);
            stream.Write((Int32)0); // Data padding.
        }
		// ----------------------------------------------------------------------------------------------------
		#endregion
	}
}
