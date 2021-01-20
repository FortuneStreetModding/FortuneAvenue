using System.Collections.Generic;
using FSEditor.FSData;

namespace Editor
{
    public class DirectionCheckModule
    {
        private MainWindow _mainWindow;

        public int directionCheckRange = 100;

        public DirectionCheckModule(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
        }

        private static bool SquaresAreApproximatelyEvenOnTheXAxis(SquareData thisSquare, SquareData otherSquare)
        {
            var thisXPos = thisSquare.Position.X;
            var otherXPos = otherSquare.Position.X;
            var range = 20;

            if (otherXPos > thisXPos)
            {
                return (otherXPos - thisXPos <= range && otherXPos - thisXPos >= -range);
            }
            if (otherXPos < thisXPos)
            {
                return (thisXPos - otherXPos <= range && thisXPos - otherXPos >= -range);
            }
            if (otherXPos == thisXPos)
            {
                return true;
            }

            return false;
        }

        private static bool SquaresAreApproximatelyEvenOnTheYAxis(SquareData thisSquare, SquareData otherSquare)
        {
            var thisYPos = thisSquare.Position.Y;
            var otherYPos = otherSquare.Position.Y;
            var range = 20;

            if (otherYPos > thisYPos)
            {
                return (otherYPos - thisYPos <= range && otherYPos - thisYPos >= -range);
            }
            else if (otherYPos < thisYPos)
            {
                return (thisYPos - otherYPos <= range && thisYPos - otherYPos >= -range);
            }
            else if (otherYPos == thisYPos)
            {
                return true;
            }

            return false;
        }

        public bool DoesSquareExistAboveThisOne(SquareData thisSquare, BoardFile board,
            List<SquareData> touchingSquares)
        {
            foreach (var otherSquare in board.BoardData.Squares)
            {
                if (otherSquare.Id == thisSquare.Id) continue;
                if (SquaresAreApproximatelyEvenOnTheXAxis(thisSquare, otherSquare) &&
                    otherSquare.Position.Y - thisSquare.Position.Y >= -directionCheckRange &&
                    otherSquare.Position.Y - thisSquare.Position.Y < 0)
                {
                    touchingSquares.Add(otherSquare);
                    return true;
                }
            }
            return false;
        }

        public bool DoesSquareExistBelowThisOne(SquareData thisSquare, BoardFile board, List<SquareData> touchingSquares)
        {
            foreach (var otherSquare in board.BoardData.Squares)
            {
                if (otherSquare.Id == thisSquare.Id) continue;
                if (SquaresAreApproximatelyEvenOnTheXAxis(thisSquare, otherSquare) &&
                    otherSquare.Position.Y - thisSquare.Position.Y <= directionCheckRange &&
                    otherSquare.Position.Y - thisSquare.Position.Y > 0)
                {
                    touchingSquares.Add(otherSquare);
                    return true;
                }
            }
            return false;
        }

        public bool DoesSquareExistToTheRightOfThisOne(SquareData thisSquare, BoardFile board, List<SquareData> touchingSquares)
        {
            foreach (var otherSquare in board.BoardData.Squares)
            {
                if (otherSquare.Id == thisSquare.Id) continue;
                if (SquaresAreApproximatelyEvenOnTheYAxis(thisSquare, otherSquare) &&
                    otherSquare.Position.X - thisSquare.Position.X <= directionCheckRange &&
                    otherSquare.Position.X - thisSquare.Position.X > 0)
                {
                    touchingSquares.Add(otherSquare);
                    return true;
                }
            }
            return false;
        }

        public bool DoesSquareExistToTheLeftOfThisOne(SquareData thisSquare, BoardFile board, List<SquareData> touchingSquares)
        {
            foreach (var otherSquare in board.BoardData.Squares)
            {
                if (otherSquare.Id == thisSquare.Id) continue;
                if (SquaresAreApproximatelyEvenOnTheYAxis(thisSquare, otherSquare) &&
                    otherSquare.Position.X - thisSquare.Position.X >= -directionCheckRange &&
                    otherSquare.Position.X - thisSquare.Position.X < 0)
                {
                    touchingSquares.Add(otherSquare);
                    return true;
                }
            }
            return false;
        }

        public bool DoesSquareExistToTheUpperRightOfThisOne(SquareData thisSquare, BoardFile board, List<SquareData> touchingSquares)
        {
            foreach (var otherSquare in board.BoardData.Squares)
            {
                if (otherSquare.Id == thisSquare.Id) continue;
                if (otherSquare.Position.Y - thisSquare.Position.Y >= -directionCheckRange &&
                    otherSquare.Position.Y - thisSquare.Position.Y < 0 &&
                    otherSquare.Position.X - thisSquare.Position.X <= directionCheckRange &&
                    otherSquare.Position.X - thisSquare.Position.X > 0)
                {
                    touchingSquares.Add(otherSquare);
                    return true;
                }
            }
            return false;
        }

        public bool DoesSquareExistToTheUpperLeftOfThisOne(SquareData thisSquare, BoardFile board, List<SquareData> touchingSquares)
        {
            foreach (var otherSquare in board.BoardData.Squares)
            {
                if (otherSquare.Id == thisSquare.Id) continue;
                if (otherSquare.Position.Y - thisSquare.Position.Y >= -directionCheckRange &&
                    otherSquare.Position.Y - thisSquare.Position.Y < 0 &&
                    otherSquare.Position.X - thisSquare.Position.X >= -directionCheckRange &&
                    otherSquare.Position.X - thisSquare.Position.X < 0)
                {
                    touchingSquares.Add(otherSquare);
                    return true;
                }
            }
            return false;
        }

        public bool DoesSquareExistToTheLowerRightOfThisOne(SquareData thisSquare, BoardFile board, List<SquareData> touchingSquares)
        {
            foreach (var otherSquare in board.BoardData.Squares)
            {
                if (otherSquare.Id == thisSquare.Id) continue;
                if (otherSquare.Position.Y - thisSquare.Position.Y <= directionCheckRange &&
                    otherSquare.Position.Y - thisSquare.Position.Y > 0 &&
                    otherSquare.Position.X - thisSquare.Position.X <= directionCheckRange &&
                    otherSquare.Position.X - thisSquare.Position.X > 0)
                {
                    touchingSquares.Add(otherSquare);
                    return true;
                }
            }
            return false;
        }

        public bool DoesSquareExistToTheLowerLeftOfThisOne(SquareData thisSquare, BoardFile board, List<SquareData> touchingSquares)
        {
            foreach (var otherSquare in board.BoardData.Squares)
            {
                if (otherSquare.Id == thisSquare.Id) continue;
                if (otherSquare.Position.Y - thisSquare.Position.Y <= directionCheckRange &&
                    otherSquare.Position.Y - thisSquare.Position.Y > 0 &&
                    otherSquare.Position.X - thisSquare.Position.X >= -directionCheckRange &&
                    otherSquare.Position.X - thisSquare.Position.X < 0)
                {
                    touchingSquares.Add(otherSquare);
                    return true;
                }
            }
            return false;
        }

        public void CheckSurroundingsForSquares(SquareData square, BoardFile board, List<SquareData> touchingSquares)
        {
            var upper = this.DoesSquareExistAboveThisOne(square, board, touchingSquares);
            var lower = this.DoesSquareExistBelowThisOne(square, board, touchingSquares);
            var left = this.DoesSquareExistToTheLeftOfThisOne(square, board, touchingSquares);
            var right = this.DoesSquareExistToTheRightOfThisOne(square, board, touchingSquares);

            if (!upper && !right)
            {
                var upperRight = this.DoesSquareExistToTheUpperRightOfThisOne(square, board, touchingSquares);
            }

            if (!upper && !left)
            {
                var upperLeft = this.DoesSquareExistToTheUpperLeftOfThisOne(square, board, touchingSquares);
            }

            if (!lower && !right)
            {
                var lowerRight = this.DoesSquareExistToTheLowerRightOfThisOne(square, board, touchingSquares);
            }

            if (!lower && !left)
            {
                var lowerLeft = this.DoesSquareExistToTheLowerLeftOfThisOne(square, board, touchingSquares);
            }
        }
    }
}