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
                if (square.lower != null && s.Id == square.lower.Id) { WaypointModule.CheckPathsFromSouth(square, exitsList); }
                if (square.lowerLeft != null && s.Id == square.lowerLeft.Id) { WaypointModule.CheckPathsFromSouthWest(square, exitsList); }
                if (square.left != null && s.Id == square.left.Id) { WaypointModule.CheckPathsFromWest(square, exitsList); }
                if (square.upperLeft != null && s.Id == square.upperLeft.Id) { WaypointModule.CheckPathsFromNorthWest(square, exitsList); }
                if (square.upper != null && s.Id == square.upper.Id) { WaypointModule.CheckPathsFromNorth(square, exitsList); }
                if (square.upperRight != null && s.Id == square.upperRight.Id) { WaypointModule.CheckPathsFromNorthEast(square, exitsList); }
                if (square.right != null && s.Id == square.right.Id) { WaypointModule.CheckPathsFromEast(square, exitsList); }
                if (square.lowerRight != null && s.Id == square.lowerRight.Id) { WaypointModule.CheckPathsFromSouthEast(square, exitsList); }
            }
        }
    }
}
