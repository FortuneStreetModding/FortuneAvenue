using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Cryptography.X509Certificates;
using System.Windows;
using FSEditor.FSData;

namespace FortuneAvenue.Modules.Pathing
{
    public static class WaypointModule
    {

        public static void PopulateWaypoints(SquareData square, List<SquareData> touchingSquares)
        {

            /*
             *
             *
             * A List of Entries
             * A List of Exits For Each Entry
             * Pathing Rules On Top Of That.
             * Then a Foreach, plugging each member of those lists into the waypoint values.
             *
             *
             *
             */

            var startingTiles = new List<SquareData>();
            var start1Exits = new List<SquareData>();
            var start2Exits = new List<SquareData>();
            var start3Exits = new List<SquareData>();
            var start4Exits = new List<SquareData>();

            if (touchingSquares.Count > 4 || touchingSquares.Count < 0) { return; }
            foreach (var s in touchingSquares) { startingTiles.Add(s); }

            for (var i = 0; i < startingTiles.Count; i++) //for each entry
            {
                switch (i)
                {
                    case 0:
                    {
                        foreach(var entry in startingTiles) //run through again
                        {
                            if (entry.Id != startingTiles[i].Id) //and add each one that isn't the entry we're checking for
                            {
                                start1Exits.Add(entry); //to its exits list
                            }
                        }
                        break;
                    }
                    case 1:
                    {
                        foreach (var entry in startingTiles) { if (entry.Id != startingTiles[i].Id) { start2Exits.Add(entry); } } break;
                    }
                    case 2:
                    {
                        foreach (var entry in startingTiles) { if (entry.Id != startingTiles[i].Id) { start3Exits.Add(entry); } } break;
                    }
                    case 3:
                    { 
                        foreach (var entry in startingTiles) { if (entry.Id != startingTiles[i].Id) { start4Exits.Add(entry); } } break;
                    }
                }
            }

            var exitsList = new List<SquareData>[4]; //packaging the lists up for action in another method
            exitsList[0] = start1Exits;
            exitsList[1] = start2Exits;
            exitsList[2] = start3Exits;
            exitsList[3] = start4Exits;

            ZeroOutAllWaypointValues(square);
            UpdateWaypointValues(square, startingTiles, exitsList);
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

        public static void UpdateWaypointValues(SquareData square, List<SquareData> startSquaresList, List<SquareData>[] exitsList)
        {
            var exit1 = exitsList[0];
            var exit2 = exitsList[1];
            var exit3 = exitsList[2];
            var exit4 = exitsList[3];

            for (var i = 0; i < startSquaresList.Count; i++)
            {
                switch (i)
                {
                    case 0:
                        square.Waypoints[0].EntryId = startSquaresList[0].Id;
                        square.Waypoints[0].Destination1 = exit1.Count > 0 ? exit1[0].Id : (byte)255;
                        square.Waypoints[0].Destination2 = exit1.Count > 1 ? exit1[1].Id : (byte)255;
                        square.Waypoints[0].Destination3 = exit1.Count > 2 ? exit1[2].Id : (byte)255;
                        break;
                    case 1:
                        square.Waypoints[1].EntryId = startSquaresList[1].Id;
                        square.Waypoints[1].Destination1 = exit2.Count > 0 ? exit2[0].Id : (byte)255;
                        square.Waypoints[1].Destination2 = exit2.Count > 1 ? exit2[1].Id : (byte)255;
                        square.Waypoints[1].Destination3 = exit2.Count > 2 ? exit2[2].Id : (byte)255;
                        break;
                    case 2:
                        square.Waypoints[2].EntryId = startSquaresList[2].Id;
                        square.Waypoints[2].Destination1 = exit3.Count > 0 ? exit3[0].Id : (byte)255;
                        square.Waypoints[2].Destination2 = exit3.Count > 1 ? exit3[1].Id : (byte)255;
                        square.Waypoints[2].Destination3 = exit3.Count > 2 ? exit3[2].Id : (byte)255;
                        break;
                    case 3:
                        square.Waypoints[3].EntryId = startSquaresList[3].Id;
                        square.Waypoints[3].Destination1 = exit4.Count > 0 ? exit4[0].Id : (byte)255;
                        square.Waypoints[3].Destination2 = exit4.Count > 1 ? exit4[1].Id : (byte)255;
                        square.Waypoints[3].Destination3 = exit4.Count > 2 ? exit4[2].Id : (byte)255;
                        break;
                }
            }
        }
    }
}