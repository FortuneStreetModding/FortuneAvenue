using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Data;
using FSEditor.FSData;

namespace Editor
{
    class SquareConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            SquareType sq = (SquareType)value;
            switch (sq)
            {
                case SquareType.ArcadeSquare: return "/Editor;component/Images/GroundArcade.png";
                case SquareType.BackStreetSquareA: return "/Editor;component/Images/GroundWarpA.png";
                case SquareType.BackStreetSquareB: return "/Editor;component/Images/GroundWarpB.png";
                case SquareType.BackStreetSquareC: return "/Editor;component/Images/GroundWarpC.png";
                case SquareType.BackStreetSquareD: return "/Editor;component/Images/GroundWarpD.png";
                case SquareType.BackStreetSquareE: return "/Editor;component/Images/GroundWarpE.png";
                case SquareType.Bank: return "/Editor;component/Images/GroundBank.png";
                case SquareType.BoomSquare: return "/Editor;component/Images/GroundBoom.png";
                case SquareType.BoonSquare: return "/Editor;component/Images/GroundBoon.png";
                case SquareType.CannonSquare: return "/Editor;component/Images/GroundCannon.png";
                case SquareType.ChangeOfSuitSquareClub: return "/Editor;component/Images/GroundCSuit04.png";
                case SquareType.ChangeOfSuitSquareDiamond: return "/Editor;component/Images/GroundCSuit03.png";
                case SquareType.ChangeOfSuitSquareHeart: return "/Editor;component/Images/GroundCSuit02.png";
                case SquareType.ChangeOfSuitSquareSpade: return "/Editor;component/Images/GroundCSuit01.png";
                case SquareType.LiftMagmaliceSquareStart: return "/Editor;component/Images/GroundLiftMagmaliceStart.png";
                case SquareType.LiftSquareEnd: return "/Editor;component/Images/GroundLift.png";
                case SquareType.MagmaliceSquare: return "/Editor;component/Images/GroundMagmalice.png";
                case SquareType.OneWayAlleyDoorA: return "/Editor;component/Images/GroundDoorBlue.png";
                case SquareType.OneWayAlleyDoorB: return "/Editor;component/Images/GroundDoorRed.png";
                case SquareType.OneWayAlleyDoorC: return "/Editor;component/Images/GroundDoorYellow.png";
                case SquareType.OneWayAlleyDoorD: return "/Editor;component/Images/GroundDoorGreen.png";
                case SquareType.OneWayAlleySquare: return "/Editor;component/Images/GroundDoorMario.png";
                case SquareType.Property: return "/Editor;component/Images/GroundProperty.png";
                case SquareType.RollOnSquare: return "/Editor;component/Images/GroundRollOn.png";
                case SquareType.StockBrokerSquare: return "/Editor;component/Images/GroundStockBroker.png";
                case SquareType.SuitSquareClub: return "/Editor;component/Images/GroundSuit04.png";
                case SquareType.SuitSquareDiamond: return "/Editor;component/Images/GroundSuit03.png";
                case SquareType.SuitSquareHeart: return "/Editor;component/Images/GroundSuit02.png";
                case SquareType.SuitSquareSpade: return "/Editor;component/Images/GroundSuit01.png";
                case SquareType.SwitchSquare: return "/Editor;component/Images/GroundSwitch.png";
                case SquareType.TakeABreakSquare: return "/Editor;component/Images/GroundTakeABreak.png";
                case SquareType.VacantPlot: return "/Editor;component/Images/GroundVacant.png";
                case SquareType.VentureSquare: return "/Editor;component/Images/GroundVenture.png";
            }
            return "/Editor;component/Images/GroundDefault.png";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
