using System.Collections.Generic;
using FSEditor.FSData;

namespace Editor
{
    public static class WaypointModule
    {
        public static void PopulateWaypoints(SquareData square, List<SquareData> touchingSquares)
        {
            switch (touchingSquares.Count)
            {
                case 0:
                    ZeroOutAllWaypointValues(square);
                    break;
                case 1:
                    UpdateWaypoint1Values(square, touchingSquares);
                    break;
                case 2:
                    UpdateWaypoint1Values(square, touchingSquares);
                    UpdateWaypoint2Values(square, touchingSquares);
                    break;
                case 3:
                    UpdateWaypoint1Values(square, touchingSquares);
                    UpdateWaypoint2Values(square, touchingSquares);
                    UpdateWaypoint3Values(square, touchingSquares);
                    break;
                case 4:
                    UpdateWaypoint1Values(square, touchingSquares);
                    UpdateWaypoint2Values(square, touchingSquares);
                    UpdateWaypoint3Values(square, touchingSquares);
                    UpdateWaypoint4Values(square, touchingSquares);
                    break;
            }
        }

        private static void ZeroOutAllWaypointValues(SquareData square)
        {
            square.Waypoint1.EntryId = 255;
            square.Waypoint1.Destination1 = 255;
            square.Waypoint1.Destination2 = 255;
            square.Waypoint1.Destination3 = 255;
            square.Waypoint2.EntryId = 255;
            square.Waypoint2.Destination1 = 255;
            square.Waypoint2.Destination2 = 255;
            square.Waypoint2.Destination3 = 255;
            square.Waypoint3.EntryId = 255;
            square.Waypoint3.Destination1 = 255;
            square.Waypoint3.Destination2 = 255;
            square.Waypoint3.Destination3 = 255;
            square.Waypoint4.EntryId = 255;
            square.Waypoint4.Destination1 = 255;
            square.Waypoint4.Destination2 = 255;
            square.Waypoint4.Destination3 = 255;
        }

        private static void UpdateWaypoint1Values(SquareData square, List<SquareData> touchingSquares)
        {
            square.Waypoint1.EntryId = touchingSquares.Count > 1 ? touchingSquares[0].Id : (byte)255;
            square.Waypoint1.Destination1 = touchingSquares.Count > 1 ? touchingSquares[1].Id : (byte) 255;
            square.Waypoint1.Destination2 = touchingSquares.Count > 2 ? touchingSquares[2].Id : (byte) 255;
            square.Waypoint1.Destination3 = touchingSquares.Count > 3 ? touchingSquares[3].Id : (byte) 255;
        }
        private static void UpdateWaypoint2Values(SquareData square, List<SquareData> touchingSquares)
        {
            square.Waypoint2.EntryId = touchingSquares.Count > 1 ? touchingSquares[1].Id : (byte)255;
            square.Waypoint2.Destination1 = touchingSquares.Count > 1 ? touchingSquares[0].Id : (byte)255;
            square.Waypoint2.Destination2 = touchingSquares.Count > 2 ? touchingSquares[2].Id : (byte)255;
            square.Waypoint2.Destination3 = touchingSquares.Count > 3 ? touchingSquares[3].Id : (byte)255;
        }
        private static void UpdateWaypoint3Values(SquareData square, List<SquareData> touchingSquares)
        {
            square.Waypoint3.EntryId = touchingSquares.Count > 2 ? touchingSquares[2].Id : (byte)255;
            square.Waypoint3.Destination1 = touchingSquares.Count > 2 ? touchingSquares[0].Id : (byte)255;
            square.Waypoint3.Destination2 = touchingSquares.Count > 2 ? touchingSquares[1].Id : (byte)255;
            square.Waypoint3.Destination3 = touchingSquares.Count > 3 ? touchingSquares[3].Id : (byte)255;
        }
        private static void UpdateWaypoint4Values(SquareData square, List<SquareData> touchingSquares)
        {
            square.Waypoint4.EntryId = touchingSquares.Count > 3 ? touchingSquares[3].Id : (byte)255;
            square.Waypoint4.Destination1 = touchingSquares.Count > 3 ? touchingSquares[0].Id : (byte)255;
            square.Waypoint4.Destination2 = touchingSquares.Count > 3 ? touchingSquares[1].Id : (byte)255;
            square.Waypoint4.Destination3 = touchingSquares.Count > 3 ? touchingSquares[2].Id : (byte)255;
        }

        public static void AddSquareToTouchingSquaresList(SquareData square, List<SquareData> touchingSquares)
        {
            touchingSquares.Add(square);
        }
    }
}