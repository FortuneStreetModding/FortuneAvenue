using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FSEditor.FSData {
	public enum SquareType : byte
    {
		Property = 0x00,
		Bank = 0x01,
		VentureSquare = 0x02,

		SuitSquareSpade = 0x03,
		SuitSquareHeart = 0x04,
		SuitSquareDiamond = 0x05,
		SuitSquareClub = 0x06,
		ChangeOfSuitSquareSpade = 0x07,
		ChangeOfSuitSquareHeart = 0x08,
		ChangeOfSuitSquareDiamond = 0x09,
		ChangeOfSuitSquareClub = 0x0A,

		TakeABreakSquare = 0x0B,
		BoonSquare = 0x0C,
		BoomSquare = 0x0D,
		StockBrokerSquare = 0x0E,
		RollOnSquare = 0x10,
		ArcadeSquare = 0x11,
		SwitchSquare = 0x12,
		CannonSquare = 0x13,

		BackStreetSquareA = 0x14,
		BackStreetSquareB = 0x15,
		BackStreetSquareC = 0x16,
		BackStreetSquareD = 0x17,
		BackStreetSquareE = 0x18,

		OneWayAlleyDoorA = 0x1C,
		OneWayAlleyDoorB = 0x1D,
		OneWayAlleyDoorC = 0x1E,
		OneWayAlleyDoorD = 0x1F,

		LiftMagmaliceSquareStart = 0x20,
		MagmaliceSquare = 0x21,
		OneWayAlleySquare = 0x22,
		LiftSquareEnd = 0x23,

        unknown0x24 = 0x24,
        unknown0x25 = 0x25,
        unknown0x26 = 0x26,
        unknown0x27 = 0x27,
        unknown0x28 = 0x28,
        unknown0x29 = 0x29,
        unknown0x2A = 0x2A,
        unknown0x2B = 0x2B,
        unknown0x2C = 0x2C,
        unknown0x2D = 0x2D,

		/// <summary>
		/// A custom square implemented with an ASM Hack. See CustomStreetManager/*/EventSquare.cs
		/// </summary>
        EventSquare = 0x2E,
        unknown0x2F = 0x2F,

        VacantPlot = 0x30
	}
}
