using System.Collections.Generic;
using FSEditor.FSData;

namespace Editor
{
    public static class WaypointModule
    {
        public static void PopulateWaypoints(SquareData square, List<SquareData> touchingSquares)
        {
            if (touchingSquares.Count > 0)
            {
                square.Waypoint1.EntryId = touchingSquares[0].Id;

                if (touchingSquares.Count > 1)
                    square.Waypoint1.Destination1 = touchingSquares[1].Id;
                else square.Waypoint1.Destination1 = 255;

                if (touchingSquares.Count > 2)
                    square.Waypoint1.Destination2 = touchingSquares[2].Id;
                else square.Waypoint1.Destination2 = 255;

                if (touchingSquares.Count > 3)
                    square.Waypoint1.Destination3 = touchingSquares[3].Id;
                else square.Waypoint1.Destination3 = 255;
            }
            else
            {
                square.Waypoint1.EntryId = 255;
                square.Waypoint1.Destination1 = 255;
                square.Waypoint1.Destination2 = 255;
                square.Waypoint1.Destination3 = 255;
            }

            if (touchingSquares.Count > 1)
            {
                square.Waypoint2.EntryId = touchingSquares[1].Id;
                square.Waypoint2.Destination1 = touchingSquares[0].Id;

                if (touchingSquares.Count > 2)
                    square.Waypoint2.Destination2 = touchingSquares[2].Id;
                else square.Waypoint2.Destination2 = 255;

                if (touchingSquares.Count > 3)
                    square.Waypoint2.Destination3 = touchingSquares[3].Id;
                else square.Waypoint2.Destination3 = 255;
            }
            else
            {
                square.Waypoint2.EntryId = 255;
                square.Waypoint2.Destination1 = 255;
                square.Waypoint2.Destination2 = 255;
                square.Waypoint2.Destination3 = 255;
            }

            if (touchingSquares.Count > 2)
            {
                square.Waypoint3.EntryId = touchingSquares[2].Id;
                square.Waypoint3.Destination1 = touchingSquares[0].Id;
                square.Waypoint3.Destination2 = touchingSquares[1].Id;

                if (touchingSquares.Count > 3)
                    square.Waypoint3.Destination3 = touchingSquares[3].Id;
                else square.Waypoint3.Destination3 = 255;
            }
            else
            {
                square.Waypoint3.EntryId = 255;
                square.Waypoint3.Destination1 = 255;
                square.Waypoint3.Destination2 = 255;
                square.Waypoint3.Destination3 = 255;
            }

            if (touchingSquares.Count > 3)
            {
                square.Waypoint4.EntryId = touchingSquares[3].Id;
                square.Waypoint4.Destination1 = touchingSquares[0].Id;
                square.Waypoint4.Destination2 = touchingSquares[1].Id;
                square.Waypoint4.Destination3 = touchingSquares[2].Id;
            }
            else
            {
                square.Waypoint4.EntryId = 255;
                square.Waypoint4.Destination1 = 255;
                square.Waypoint4.Destination2 = 255;
                square.Waypoint4.Destination3 = 255;
            }
        }
        public static void AddSquareToTouchingSquaresList(SquareData square, List<SquareData> touchingSquares)
        {
            touchingSquares.Add(square);
        }
    }
}