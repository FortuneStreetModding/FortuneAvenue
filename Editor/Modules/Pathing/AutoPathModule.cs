using System.Collections.Generic;
using FSEditor.FSData;

namespace FortuneAvenue.Modules.Pathing
{
    public static class AutoPathModule
    {
        public static void PathSquare(SquareData square, List<SquareData> startingTiles)
        {
            var start1Exits = new List<SquareData>();
            var start2Exits = new List<SquareData>();
            var start3Exits = new List<SquareData>();
            var start4Exits = new List<SquareData>();

            if (startingTiles.Count > 4 || startingTiles.Count < 0) { return; }

            WaypointModule.PopulateExitLists(startingTiles, start1Exits, start2Exits, start3Exits, start4Exits);

            var exitsList = new List<SquareData>[4]; //packaging the lists up for action in another method
            exitsList[0] = start1Exits;
            exitsList[1] = start2Exits;
            exitsList[2] = start3Exits;
            exitsList[3] = start4Exits;

            WaypointModule.ZeroOutAllWaypointValues(square);
            ApplyPathingRules(square, startingTiles, exitsList);
            WaypointModule.UpdateWaypointValues(square, startingTiles, exitsList);
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
