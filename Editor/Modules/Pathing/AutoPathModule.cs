using System.Collections.Generic;
using FSEditor.FSData;

namespace FortuneAvenue.Modules.Pathing
{
    public static class AutoPathModule
    {
        public static void CheckSurroundingsForSquares(SquareData square, BoardFile board, List<SquareData> touchingSquares)
        {
            square.upper = DirectionCheckModule.DoesSquareExistAboveThisOne(square, board, touchingSquares);
            square.lower = DirectionCheckModule.DoesSquareExistBelowThisOne(square, board, touchingSquares);
            square.left = DirectionCheckModule.DoesSquareExistToTheLeftOfThisOne(square, board, touchingSquares);
            square.right = DirectionCheckModule.DoesSquareExistToTheRightOfThisOne(square, board, touchingSquares);

            if (square.upper == null && square.right == null) { square.upperRight = DirectionCheckModule.DoesSquareExistToTheUpperRightOfThisOne(square, board, touchingSquares); }
            if (square.upper == null && square.left == null) { square.upperLeft = DirectionCheckModule.DoesSquareExistToTheUpperLeftOfThisOne(square, board, touchingSquares); }
            if (square.lower == null && square.right == null) { square.lowerRight = DirectionCheckModule.DoesSquareExistToTheLowerRightOfThisOne(square, board, touchingSquares); }
            if (square.lower == null && square.left == null) { square.lowerLeft = DirectionCheckModule.DoesSquareExistToTheLowerLeftOfThisOne(square, board, touchingSquares); }
        }

        public static void EnumerateAutopathingRules(SquareData square, List<SquareData> startingTiles)
        {
            var destinationSquares1 = new List<SquareData>();
            var destinationSquares2 = new List<SquareData>();
            var destinationSquares3 = new List<SquareData>();
            var destinationSquares4 = new List<SquareData>();

            WaypointModule.PopulateExitLists(startingTiles, destinationSquares1, destinationSquares2, destinationSquares3, destinationSquares4);

            var waypointDestinationLists = new List<SquareData>[4]; //packaging the lists up for action in another method
            waypointDestinationLists[0] = destinationSquares1;
            waypointDestinationLists[1] = destinationSquares2;
            waypointDestinationLists[2] = destinationSquares3;
            waypointDestinationLists[3] = destinationSquares4;

            SetAllPathingBooleansToFalse(square);

            foreach (var entrySquare in startingTiles)
            {
                if (square.upper != null && entrySquare.Id == square.upper.Id) { WaypointModule.CheckPathsFromNorth(square, waypointDestinationLists); }
                if (square.lower != null && entrySquare.Id == square.lower.Id) { WaypointModule.CheckPathsFromSouth(square, waypointDestinationLists); }
                if (square.left != null && entrySquare.Id == square.left.Id) { WaypointModule.CheckPathsFromWest(square, waypointDestinationLists); }
                if (square.right != null && entrySquare.Id == square.right.Id) { WaypointModule.CheckPathsFromEast(square, waypointDestinationLists); }
                if (square.upperLeft != null && entrySquare.Id == square.upperLeft.Id) { WaypointModule.CheckPathsFromNorthWest(square, waypointDestinationLists); }
                if (square.upperRight != null && entrySquare.Id == square.upperRight.Id) { WaypointModule.CheckPathsFromNorthEast(square, waypointDestinationLists); }
                if (square.lowerLeft != null && entrySquare.Id == square.lowerLeft.Id) { WaypointModule.CheckPathsFromSouthWest(square, waypointDestinationLists); }
                if (square.lowerRight != null && entrySquare.Id == square.lowerRight.Id) { WaypointModule.CheckPathsFromSouthEast(square, waypointDestinationLists); }
            }
        }

        public static void PathSquare(SquareData square, List<SquareData> startingTiles)
        {
            var start1Exits = new List<SquareData>();
            var start2Exits = new List<SquareData>();
            var start3Exits = new List<SquareData>();
            var start4Exits = new List<SquareData>();

            if (startingTiles.Count > 4 || startingTiles.Count < 0) { return; }

            WaypointModule.PopulateExitLists(startingTiles, start1Exits, start2Exits, start3Exits, start4Exits);

            var waypointDestinationLists = new List<SquareData>[4]; //packaging the lists up for action in another method
            waypointDestinationLists[0] = start1Exits;
            waypointDestinationLists[1] = start2Exits;
            waypointDestinationLists[2] = start3Exits;
            waypointDestinationLists[3] = start4Exits;

            WaypointModule.ZeroOutAllWaypointValues(square);
            ApplyPathingRules(square, startingTiles, waypointDestinationLists);
            WaypointModule.UpdateWaypointValues(square, startingTiles, waypointDestinationLists);
        }

        public static void ApplyPathingRules(SquareData square, List<SquareData> startSquaresList, List<SquareData>[] exitsList)
        {
            foreach (var s in startSquaresList)
            {
                if (square.lower != null && s.Id == square.lower.Id) { WaypointModule.CleanPathsFromSouth(square, exitsList); }
                if (square.lowerLeft != null && s.Id == square.lowerLeft.Id) { WaypointModule.CleanPathsFromSouthWest(square, exitsList); }
                if (square.left != null && s.Id == square.left.Id) { WaypointModule.CleanPathsFromWest(square, exitsList); }
                if (square.upperLeft != null && s.Id == square.upperLeft.Id) { WaypointModule.CleanPathsFromNorthWest(square, exitsList); }
                if (square.upper != null && s.Id == square.upper.Id) { WaypointModule.CleanPathsFromNorth(square, exitsList); }
                if (square.upperRight != null && s.Id == square.upperRight.Id) { WaypointModule.CleanPathsFromNorthEast(square, exitsList); }
                if (square.right != null && s.Id == square.right.Id) { WaypointModule.CleanPathsFromEast(square, exitsList); }
                if (square.lowerRight != null && s.Id == square.lowerRight.Id) { WaypointModule.CleanPathsFromSouthEast(square, exitsList); }
            }
        }

        public static void SetAllPathingBooleansToFalse(SquareData square)
        {
            square.fromSouthAllowSouthWest = false;
            square.fromSouthAllowWest = false;
            square.fromSouthAllowNorthWest = false;
            square.fromSouthAllowNorth = false;
            square.fromSouthAllowNorthEast = false;
            square.fromSouthAllowEast = false;
            square.fromSouthAllowSouthEast = false;

            square.fromSouthWestAllowWest = false;
            square.fromSouthWestAllowNorthWest = false;
            square.fromSouthWestAllowNorth = false;
            square.fromSouthWestAllowNorthEast = false;
            square.fromSouthWestAllowEast = false;
            square.fromSouthWestAllowSouthEast = false;
            square.fromSouthWestAllowSouth = false;

            square.fromWestAllowSouthWest = false;
            square.fromWestAllowNorthWest = false;
            square.fromWestAllowNorth = false;
            square.fromWestAllowNorthEast = false;
            square.fromWestAllowEast = false;
            square.fromWestAllowSouthEast = false;
            square.fromWestAllowSouth = false;

            square.fromNorthWestAllowSouthWest = false;
            square.fromNorthWestAllowWest = false;
            square.fromNorthWestAllowNorth = false;
            square.fromNorthWestAllowNorthEast = false;
            square.fromNorthWestAllowEast = false;
            square.fromNorthWestAllowSouthEast = false;
            square.fromNorthWestAllowSouth = false;

            square.fromNorthAllowSouthWest = false;
            square.fromNorthAllowWest = false;
            square.fromNorthAllowNorthWest = false;
            square.fromNorthAllowNorthEast = false;
            square.fromNorthAllowEast = false;
            square.fromNorthAllowSouthEast = false;
            square.fromNorthAllowSouth = false;

            square.fromNorthEastAllowSouthWest = false;
            square.fromNorthEastAllowWest = false;
            square.fromNorthEastAllowNorthWest = false;
            square.fromNorthEastAllowNorth = false;
            square.fromNorthEastAllowEast = false;
            square.fromNorthEastAllowSouthEast = false;
            square.fromNorthEastAllowSouth = false;

            square.fromEastAllowSouthWest = false;
            square.fromEastAllowWest = false;
            square.fromEastAllowNorthWest = false;
            square.fromEastAllowNorth = false;
            square.fromEastAllowNorthEast = false;
            square.fromEastAllowSouthEast = false;
            square.fromEastAllowSouth = false;

            square.fromSouthEastAllowSouthWest = false;
            square.fromSouthEastAllowWest = false;
            square.fromSouthEastAllowNorthWest = false;
            square.fromSouthEastAllowNorth = false;
            square.fromSouthEastAllowNorthEast = false;
            square.fromSouthEastAllowEast = false;
            square.fromSouthEastAllowSouth = false;
        }
    }
}
