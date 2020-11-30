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
                case SquareType.ArcadeSquare: return "/FortuneAvenue;component/Images/GroundArcade.png";
                case SquareType.BackStreetSquareA: return "/FortuneAvenue;component/Images/GroundWarpA.png";
                case SquareType.BackStreetSquareB: return "/FortuneAvenue;component/Images/GroundWarpB.png";
                case SquareType.BackStreetSquareC: return "/FortuneAvenue;component/Images/GroundWarpC.png";
                case SquareType.BackStreetSquareD: return "/FortuneAvenue;component/Images/GroundWarpD.png";
                case SquareType.BackStreetSquareE: return "/FortuneAvenue;component/Images/GroundWarpE.png";
                case SquareType.Bank: return "/FortuneAvenue;component/Images/GroundBank.png";
                case SquareType.BoomSquare: return "/FortuneAvenue;component/Images/GroundBoom.png";
                case SquareType.BoonSquare: return "/FortuneAvenue;component/Images/GroundBoon.png";
                case SquareType.CannonSquare: return "/FortuneAvenue;component/Images/GroundCannon.png";
                case SquareType.ChangeOfSuitSquareClub: return "/FortuneAvenue;component/Images/GroundCSuit04.png";
                case SquareType.ChangeOfSuitSquareDiamond: return "/FortuneAvenue;component/Images/GroundCSuit03.png";
                case SquareType.ChangeOfSuitSquareHeart: return "/FortuneAvenue;component/Images/GroundCSuit02.png";
                case SquareType.ChangeOfSuitSquareSpade: return "/FortuneAvenue;component/Images/GroundCSuit01.png";
                case SquareType.LiftMagmaliceSquareStart: return "/FortuneAvenue;component/Images/GroundLiftMagmaliceStart.png";
                case SquareType.LiftSquareEnd: return "/FortuneAvenue;component/Images/GroundLift.png";
                case SquareType.MagmaliceSquare: return "/FortuneAvenue;component/Images/GroundMagmalice.png";
                case SquareType.OneWayAlleyDoorA: return "/FortuneAvenue;component/Images/GroundDoorBlue.png";
                case SquareType.OneWayAlleyDoorB: return "/FortuneAvenue;component/Images/GroundDoorRed.png";
                case SquareType.OneWayAlleyDoorC: return "/FortuneAvenue;component/Images/GroundDoorYellow.png";
                case SquareType.OneWayAlleyDoorD: return "/FortuneAvenue;component/Images/GroundDoorGreen.png";
                case SquareType.OneWayAlleySquare: return "/FortuneAvenue;component/Images/GroundDoorMario.png";
                case SquareType.Property: return "/FortuneAvenue;component/Images/GroundProperty.png";
                case SquareType.RollOnSquare: return "/FortuneAvenue;component/Images/GroundRollOn.png";
                case SquareType.StockBrokerSquare: return "/FortuneAvenue;component/Images/GroundStockBroker.png";
                case SquareType.SuitSquareClub: return "/FortuneAvenue;component/Images/GroundSuit04.png";
                case SquareType.SuitSquareDiamond: return "/FortuneAvenue;component/Images/GroundSuit03.png";
                case SquareType.SuitSquareHeart: return "/FortuneAvenue;component/Images/GroundSuit02.png";
                case SquareType.SuitSquareSpade: return "/FortuneAvenue;component/Images/GroundSuit01.png";
                case SquareType.SwitchSquare: return "/FortuneAvenue;component/Images/GroundSwitch.png";
                case SquareType.TakeABreakSquare: return "/FortuneAvenue;component/Images/GroundTakeABreak.png";
                case SquareType.VacantPlot: return "/FortuneAvenue;component/Images/GroundVacant.png";
                case SquareType.VentureSquare: return "/FortuneAvenue;component/Images/GroundVenture.png";
                case SquareType.EventSquare: return "/FortuneAvenue;component/Images/EventSquare.png";
            }
            return "/FortuneAvenue;component/Images/GroundDefault.png";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
