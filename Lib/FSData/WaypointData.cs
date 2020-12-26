using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MiscUtil.IO;

namespace FSEditor.FSData {
	public class WaypointData {
		#region Fields & Properties
		// ----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets the ID of the square to enter from.
		/// </summary>
		public Byte EntryId { get; set; }

        public Byte Destination1 { get; set; }

        public Byte Destination2 { get; set; }

        public Byte Destination3 { get; set; }


		// ----------------------------------------------------------------------------------------------------
		#endregion

		#region Loading & Writing Methods
		// ----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Loads a new SquareData from a stream.
		/// </summary>
		/// <param name="stream">The stream to load from.</param>
		/// <returns>A new SquareData representing a square.</returns>
		public static WaypointData LoadFromStream(EndianBinaryReader stream) {
			WaypointData data = new WaypointData();
			data.EntryId = stream.ReadByte();
			data.Destination1 = stream.ReadByte();
			data.Destination2 = stream.ReadByte();
			data.Destination3 = stream.ReadByte();
			return data;
		}

		/// <summary>
		/// Writes a WaypointData object into a specified stream.
		/// </summary>
		/// <param name="stream">The stream to write into.</param>
		public void WriteToStream(EndianBinaryWriter stream) {
			stream.Write(this.EntryId);
			stream.Write(this.Destination1);
			stream.Write(this.Destination2);
			stream.Write(this.Destination3);
		}
		// ----------------------------------------------------------------------------------------------------
		#endregion
	}
}
