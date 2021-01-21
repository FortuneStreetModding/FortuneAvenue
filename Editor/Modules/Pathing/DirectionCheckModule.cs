using System.Collections.Generic;
using Editor;
using FSEditor.FSData;

namespace FortuneAvenue.Modules.Pathing
{
    public static class DirectionCheckModule
    {
        public static int directionCheckRange = 100;

        public static void AddSquareToTouchingSquaresList(SquareData square, List<SquareData> touchingSquares)
        {
            touchingSquares.Add(square);
        }
        /// 
        /// Cardinal Direction Checks
        ///
        /// 
        public static SquareData DoesSquareExistAboveThisOne(SquareData thisSquare, BoardFile board, List<SquareData> touchingSquares)
        {
            foreach (var otherSquare in board.BoardData.Squares)
            {
                if (otherSquare.Id == thisSquare.Id) continue;
                if (PositionValueComparisonModule.SquaresAreApproximatelyEvenOnTheXAxis(thisSquare, otherSquare) && 
                    PositionValueComparisonModule.OtherSquareIsWithinTheNegativeYDirectionCheckRange(thisSquare, otherSquare) && 
                    PositionValueComparisonModule.TheDifferenceInYIsLessThanZero(thisSquare, otherSquare))
                {
                    AddSquareToTouchingSquaresList(otherSquare, touchingSquares);
                    return otherSquare;
                }
            }
            return null;
        }

        public static SquareData DoesSquareExistBelowThisOne(SquareData thisSquare, BoardFile board, List<SquareData> touchingSquares)
        {
            foreach (var otherSquare in board.BoardData.Squares)
            {
                if (otherSquare.Id == thisSquare.Id) continue;
                if (PositionValueComparisonModule.SquaresAreApproximatelyEvenOnTheXAxis(thisSquare, otherSquare) && 
                    PositionValueComparisonModule.OtherSquareIsWithinPositiveYDirectionCheckRange(thisSquare, otherSquare) && 
                    PositionValueComparisonModule.TheDifferenceInYIsGreaterThanZero(thisSquare, otherSquare))
                {
                    AddSquareToTouchingSquaresList(otherSquare, touchingSquares);
                    return otherSquare;
                }
            }
            return null;
        }

        public static SquareData DoesSquareExistToTheRightOfThisOne(SquareData thisSquare, BoardFile board, List<SquareData> touchingSquares)
        {
            foreach (var otherSquare in board.BoardData.Squares)
            {
                if (otherSquare.Id == thisSquare.Id) continue;
                if (PositionValueComparisonModule.SquaresAreApproximatelyEvenOnTheYAxis(thisSquare, otherSquare) && 
                    PositionValueComparisonModule.OtherSquareIsWithinPositiveXDirectionCheckRange(thisSquare, otherSquare) && 
                    PositionValueComparisonModule.TheDifferenceInXIsGreaterThanZero(thisSquare, otherSquare))
                {
                    AddSquareToTouchingSquaresList(otherSquare, touchingSquares);
                    return otherSquare;
                }
            }
            return null;
        }

        public static SquareData DoesSquareExistToTheLeftOfThisOne(SquareData thisSquare, BoardFile board, List<SquareData> touchingSquares)
        {
            foreach (var otherSquare in board.BoardData.Squares)
            {
                if (otherSquare.Id == thisSquare.Id) continue;
                if (PositionValueComparisonModule.SquaresAreApproximatelyEvenOnTheYAxis(thisSquare, otherSquare) && 
                    PositionValueComparisonModule.OtherSquareIsWithinNegativeXDirectionCheckRange(thisSquare, otherSquare) && 
                    PositionValueComparisonModule.TheDifferenceInXIsLessThanZero(thisSquare, otherSquare))
                {
                    AddSquareToTouchingSquaresList(otherSquare, touchingSquares);
                    return otherSquare;
                }
            }
            return null;
        }

        ///
        /// Diagonal Direction Checks
        ///
        public static SquareData DoesSquareExistToTheUpperRightOfThisOne(SquareData thisSquare, BoardFile board, List<SquareData> touchingSquares)
        {
            foreach (var otherSquare in board.BoardData.Squares)
            {
                if (otherSquare.Id == thisSquare.Id) continue;
                if (PositionValueComparisonModule.OtherSquareIsWithinTheNegativeYDirectionCheckRange(thisSquare, otherSquare) &&
                    PositionValueComparisonModule.TheDifferenceInYIsLessThanZero(thisSquare, otherSquare) &&
                    PositionValueComparisonModule.OtherSquareIsWithinPositiveXDirectionCheckRange(thisSquare, otherSquare) &&
                    PositionValueComparisonModule.TheDifferenceInXIsGreaterThanZero(thisSquare, otherSquare))
                {
                    AddSquareToTouchingSquaresList(otherSquare, touchingSquares);
                    return otherSquare;
                }
            }
            return null;
        }

        public static SquareData DoesSquareExistToTheUpperLeftOfThisOne(SquareData thisSquare, BoardFile board, List<SquareData> touchingSquares)
        {
            foreach (var otherSquare in board.BoardData.Squares)
            {
                if (otherSquare.Id == thisSquare.Id) continue;
                if (PositionValueComparisonModule.OtherSquareIsWithinTheNegativeYDirectionCheckRange(thisSquare, otherSquare) && 
                    PositionValueComparisonModule.TheDifferenceInYIsLessThanZero(thisSquare, otherSquare) && 
                    PositionValueComparisonModule.OtherSquareIsWithinNegativeXDirectionCheckRange(thisSquare, otherSquare) && 
                    PositionValueComparisonModule.TheDifferenceInXIsLessThanZero(thisSquare, otherSquare))
                {
                    AddSquareToTouchingSquaresList(otherSquare, touchingSquares);
                    return otherSquare;
                }
            }
            return null;
        }

        public static SquareData DoesSquareExistToTheLowerRightOfThisOne(SquareData thisSquare, BoardFile board, List<SquareData> touchingSquares)
        {
            foreach (var otherSquare in board.BoardData.Squares)
            {
                if (otherSquare.Id == thisSquare.Id) continue;
                if (PositionValueComparisonModule.OtherSquareIsWithinPositiveYDirectionCheckRange(thisSquare, otherSquare) && 
                    PositionValueComparisonModule.TheDifferenceInYIsGreaterThanZero(thisSquare, otherSquare) && 
                    PositionValueComparisonModule.OtherSquareIsWithinPositiveXDirectionCheckRange(thisSquare, otherSquare) && 
                    PositionValueComparisonModule.TheDifferenceInXIsGreaterThanZero(thisSquare, otherSquare))
                {
                    AddSquareToTouchingSquaresList(otherSquare, touchingSquares);
                    return otherSquare;
                }
            }
            return null;
        }

        public static SquareData DoesSquareExistToTheLowerLeftOfThisOne(SquareData thisSquare, BoardFile board, List<SquareData> touchingSquares)
        {
            foreach (var otherSquare in board.BoardData.Squares)
            {
                if (otherSquare.Id == thisSquare.Id) continue;
                if (PositionValueComparisonModule.OtherSquareIsWithinPositiveYDirectionCheckRange(thisSquare, otherSquare) && 
                    PositionValueComparisonModule.TheDifferenceInYIsGreaterThanZero(thisSquare, otherSquare) && 
                    PositionValueComparisonModule.OtherSquareIsWithinNegativeXDirectionCheckRange(thisSquare, otherSquare) && 
                    PositionValueComparisonModule.TheDifferenceInXIsLessThanZero(thisSquare, otherSquare))
                {
                    AddSquareToTouchingSquaresList(otherSquare, touchingSquares);
                    return otherSquare;
                }
            }
            return null;
        }
    }
}