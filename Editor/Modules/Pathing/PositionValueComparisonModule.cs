using FortuneAvenue.Modules.Pathing;
using FSEditor.FSData;

namespace Editor
{
    public static class PositionValueComparisonModule
    {
        public static int varianceAllowedWhenCheckingIfEqualOnAnAxis = 20;

        public static bool SquaresAreApproximatelyEvenOnTheXAxis(SquareData thisSquare, SquareData otherSquare)
        {
            var thisXPos = thisSquare.Position.X;
            var otherXPos = otherSquare.Position.X;

            if (otherXPos > thisXPos)
            {
                return (otherXPos - thisXPos <= varianceAllowedWhenCheckingIfEqualOnAnAxis &&
                        otherXPos - thisXPos >= -varianceAllowedWhenCheckingIfEqualOnAnAxis);
            }
            if (otherXPos < thisXPos)
            {
                return (thisXPos - otherXPos <= varianceAllowedWhenCheckingIfEqualOnAnAxis &&
                        thisXPos - otherXPos >= -varianceAllowedWhenCheckingIfEqualOnAnAxis);
            }
            if (otherXPos == thisXPos)
            {
                return true;
            }

            return false;
        }

        public static bool SquaresAreApproximatelyEvenOnTheYAxis(SquareData thisSquare, SquareData otherSquare)
        {
            var thisYPos = thisSquare.Position.Y;
            var otherYPos = otherSquare.Position.Y;

            if (otherYPos > thisYPos)
            {
                return (otherYPos - thisYPos <= varianceAllowedWhenCheckingIfEqualOnAnAxis &&
                        otherYPos - thisYPos >= -varianceAllowedWhenCheckingIfEqualOnAnAxis);
            }
            else if (otherYPos < thisYPos)
            {
                return (thisYPos - otherYPos <= varianceAllowedWhenCheckingIfEqualOnAnAxis &&
                        thisYPos - otherYPos >= -varianceAllowedWhenCheckingIfEqualOnAnAxis);
            }
            else if (otherYPos == thisYPos)
            {
                return true;
            }

            return false;
        }
        public static bool TheDifferenceInXIsGreaterThanZero(SquareData thisSquare, SquareData otherSquare)
        {
            return otherSquare.Position.X - thisSquare.Position.X > 0;
        }

        public static bool TheDifferenceInXIsLessThanZero(SquareData thisSquare, SquareData otherSquare)
        {
            return otherSquare.Position.X - thisSquare.Position.X < 0;
        }

        public static bool OtherSquareIsWithinNegativeXDirectionCheckRange(SquareData thisSquare, SquareData otherSquare)
        {
            return otherSquare.Position.X - thisSquare.Position.X >= -DirectionCheckModule.directionCheckRange;
        }

        public static bool OtherSquareIsWithinPositiveXDirectionCheckRange(SquareData thisSquare, SquareData otherSquare)
        {
            return otherSquare.Position.X - thisSquare.Position.X <= DirectionCheckModule.directionCheckRange;
        }

        public static bool OtherSquareIsWithinPositiveYDirectionCheckRange(SquareData thisSquare, SquareData otherSquare)
        {
            return otherSquare.Position.Y - thisSquare.Position.Y <= DirectionCheckModule.directionCheckRange;
        }

        public static bool OtherSquareIsWithinTheNegativeYDirectionCheckRange(SquareData thisSquare, SquareData otherSquare)
        {
            return otherSquare.Position.Y - thisSquare.Position.Y >= -DirectionCheckModule.directionCheckRange;
        }

        public static bool TheDifferenceInYIsGreaterThanZero(SquareData thisSquare, SquareData otherSquare)
        {
            return otherSquare.Position.Y - thisSquare.Position.Y > 0;
        }

        public static bool TheDifferenceInYIsLessThanZero(SquareData thisSquare, SquareData otherSquare)
        {
            return otherSquare.Position.Y - thisSquare.Position.Y < 0;
        }
    }
}