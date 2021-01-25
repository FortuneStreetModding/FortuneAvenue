using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MiscUtil.IO;

namespace FSEditor.FSData {
	public abstract class Header {
        public const uint Size = 0x10;

		#region Fields & Properties
		// ----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the MagicNumber of this header.
		/// </summary>
		protected String MagicNumber { get; private set; }

		/// <summary>
		/// Gets or sets the size of this header.
		/// </summary>
		public UInt32 HeaderSize { get; set; }
		// ----------------------------------------------------------------------------------------------------
		#endregion

		#region Initialization
		// ----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Default Constructor.
		/// </summary>
		/// <param name="magicNumber">The MagicNumber of this header.</param>
		protected Header(String magicNumber) {
			this.MagicNumber = magicNumber;
		} 
		// ----------------------------------------------------------------------------------------------------
		#endregion

		#region Protected Methods
		// ----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Verifies the MagicNumber at the beginning and reads the file size afterwards.
		/// </summary>
		protected void ReadMagicNumberAndHeaderSize(EndianBinaryReader stream) {
			if (!stream.ReadString(4).ToUpper().Equals(this.MagicNumber)) {
				throw new Exceptions.InvalidFileException();
			}
			this.HeaderSize = stream.ReadUInt32();
		}

        protected void WriteMagicNumberAndHeaderSize(EndianBinaryWriter stream)
        {
            var magic = Encoding.ASCII.GetBytes(MagicNumber);
            stream.Write(magic[0]);
            stream.Write(magic[1]);
            stream.Write(magic[2]);
            stream.Write(magic[3]);
            stream.Write(HeaderSize);
        }
		// ----------------------------------------------------------------------------------------------------

		#endregion
	}
}
