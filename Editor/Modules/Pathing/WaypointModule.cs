using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Cryptography.X509Certificates;
using System.Windows;
using FSEditor.FSData;

namespace FortuneAvenue.Modules.Pathing
{
    public static class WaypointModule
    {
        public static void PopulateExitLists(List<SquareData> startingTiles, List<SquareData> start1Exits, List<SquareData> start2Exits, List<SquareData> start3Exits,
            List<SquareData> start4Exits)
        {
            for (var i = 0; i < startingTiles.Count; i++)
            {
                switch (i)
                {
                    case 0: { start1Exits.AddRange(startingTiles.Where(entry => entry.Id != startingTiles[i].Id)); break; }
                    case 1: { start2Exits.AddRange(startingTiles.Where(entry => entry.Id != startingTiles[i].Id)); break; }
                    case 2: { start3Exits.AddRange(startingTiles.Where(entry => entry.Id != startingTiles[i].Id)); break; }
                    case 3: { start4Exits.AddRange(startingTiles.Where(entry => entry.Id != startingTiles[i].Id)); break; }
                }
            }
        }

        public static void ZeroOutAllWaypointValues(SquareData square)
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

        public static void CleanPathsFromSouthEast(SquareData square, List<SquareData>[] exitsList)
        {
            if (square.lower != null) { CleanWaypointList(square.lower.Id, square.fromSouthEastAllowSouth, exitsList); }
            if (square.lowerLeft != null) { CleanWaypointList(square.lowerLeft.Id, square.fromSouthEastAllowSouthWest, exitsList); }
            if (square.left != null) { CleanWaypointList(square.left.Id, square.fromSouthEastAllowWest, exitsList); }
            if (square.upperLeft != null) { CleanWaypointList(square.upperLeft.Id, square.fromSouthEastAllowNorthWest, exitsList); }
            if (square.upper != null) { CleanWaypointList(square.upper.Id, square.fromSouthEastAllowNorth, exitsList); }
            if (square.upperRight != null) { CleanWaypointList(square.upperRight.Id, square.fromSouthEastAllowNorthEast, exitsList); }
            if (square.right != null) { CleanWaypointList(square.right.Id, square.fromSouthEastAllowEast, exitsList); }
        }

        public static void CleanPathsFromEast(SquareData square, List<SquareData>[] exitsList)
        {
            if (square.lower != null) { CleanWaypointList(square.lower.Id, square.fromEastAllowSouth, exitsList); }
            if (square.lowerLeft != null) { CleanWaypointList(square.lowerLeft.Id, square.fromEastAllowSouthWest, exitsList); }
            if (square.left != null) { CleanWaypointList(square.left.Id, square.fromEastAllowWest, exitsList); }
            if (square.upperLeft != null) { CleanWaypointList(square.upperLeft.Id, square.fromEastAllowNorthWest, exitsList); }
            if (square.upper != null) { CleanWaypointList(square.upper.Id, square.fromEastAllowNorth, exitsList); }
            if (square.upperRight != null) { CleanWaypointList(square.upperRight.Id, square.fromEastAllowNorthEast, exitsList); }
            if (square.lowerRight != null) { CleanWaypointList(square.lowerRight.Id, square.fromEastAllowSouthEast, exitsList); }
        }

        public static void CleanPathsFromNorthEast(SquareData square, List<SquareData>[] exitsList)
        {
            if (square.lower != null) { CleanWaypointList(square.lower.Id, square.fromNorthEastAllowSouth, exitsList); }
            if (square.lowerLeft != null) { CleanWaypointList(square.lowerLeft.Id, square.fromNorthEastAllowSouthWest, exitsList); }
            if (square.left != null) { CleanWaypointList(square.left.Id, square.fromNorthEastAllowWest, exitsList); }
            if (square.upperLeft != null) { CleanWaypointList(square.upperLeft.Id, square.fromNorthEastAllowNorthWest, exitsList); }
            if (square.upper != null) { CleanWaypointList(square.upper.Id, square.fromNorthEastAllowNorth, exitsList); }
            if (square.right != null) { CleanWaypointList(square.right.Id, square.fromNorthEastAllowEast, exitsList); }
            if (square.lowerRight != null) { CleanWaypointList(square.lowerRight.Id, square.fromNorthEastAllowSouthEast, exitsList); }
        }

        public static void CleanPathsFromNorth(SquareData square, List<SquareData>[] exitsList)
        {
            if (square.lower != null) { CleanWaypointList(square.lower.Id, square.fromNorthAllowSouth, exitsList); }
            if (square.lowerLeft != null) { CleanWaypointList(square.lowerLeft.Id, square.fromNorthAllowSouthWest, exitsList); }
            if (square.left != null) { CleanWaypointList(square.left.Id, square.fromNorthAllowWest, exitsList); }
            if (square.upperLeft != null) { CleanWaypointList(square.upperLeft.Id, square.fromNorthAllowNorthWest, exitsList); }
            if (square.upperRight != null) { CleanWaypointList(square.upperRight.Id, square.fromNorthAllowNorthEast, exitsList); }
            if (square.right != null) { CleanWaypointList(square.right.Id, square.fromNorthAllowEast, exitsList); }
            if (square.lowerRight != null) { CleanWaypointList(square.lowerRight.Id, square.fromNorthAllowSouthEast, exitsList); }
        }

        public static void CleanPathsFromNorthWest(SquareData square, List<SquareData>[] exitsList)
        {
            if (square.lower != null) { CleanWaypointList(square.lower.Id, square.fromNorthWestAllowSouth, exitsList); }
            if (square.lowerLeft != null) { CleanWaypointList(square.lowerLeft.Id, square.fromNorthWestAllowSouthWest, exitsList); }
            if (square.left != null) { CleanWaypointList(square.left.Id, square.fromNorthWestAllowWest, exitsList); }
            if (square.upper != null) { CleanWaypointList(square.upper.Id, square.fromNorthWestAllowNorth, exitsList); }
            if (square.upperRight != null) { CleanWaypointList(square.upperRight.Id, square.fromNorthWestAllowNorthEast, exitsList); }
            if (square.right != null) { CleanWaypointList(square.right.Id, square.fromNorthWestAllowEast, exitsList); }
            if (square.lowerRight != null) { CleanWaypointList(square.lowerRight.Id, square.fromNorthWestAllowSouthEast, exitsList); }
        }

        public static void CleanPathsFromWest(SquareData square, List<SquareData>[] exitsList)
        {
            if (square.lower != null) { CleanWaypointList(square.lower.Id, square.fromWestAllowSouth, exitsList); }
            if (square.lowerLeft != null) { CleanWaypointList(square.lowerLeft.Id, square.fromWestAllowSouthWest, exitsList); }
            if (square.upperLeft != null) { CleanWaypointList(square.upperLeft.Id, square.fromWestAllowNorthWest, exitsList); }
            if (square.upper != null) { CleanWaypointList(square.upper.Id, square.fromWestAllowNorth, exitsList); }
            if (square.upperRight != null) { CleanWaypointList(square.upperRight.Id, square.fromWestAllowNorthEast, exitsList); }
            if (square.right != null) { CleanWaypointList(square.right.Id, square.fromWestAllowEast, exitsList); }
            if (square.lowerRight != null) { CleanWaypointList(square.lowerRight.Id, square.fromWestAllowSouthEast, exitsList); }
        }

        public static void CleanPathsFromSouthWest(SquareData square, List<SquareData>[] exitsList)
        {
            if (square.lower != null) { CleanWaypointList(square.lower.Id, square.fromSouthWestAllowSouth, exitsList); }
            if (square.left != null) { CleanWaypointList(square.left.Id, square.fromSouthWestAllowWest, exitsList); }
            if (square.upperLeft != null) { CleanWaypointList(square.upperLeft.Id, square.fromSouthWestAllowNorthWest, exitsList); }
            if (square.upper != null) { CleanWaypointList(square.upper.Id, square.fromSouthWestAllowNorth, exitsList); }
            if (square.upperRight != null) { CleanWaypointList(square.upperRight.Id, square.fromSouthWestAllowNorthEast, exitsList); }
            if (square.right != null) { CleanWaypointList(square.right.Id, square.fromSouthWestAllowEast, exitsList); }
            if (square.lowerRight != null) { CleanWaypointList(square.lowerRight.Id, square.fromSouthWestAllowSouthEast, exitsList); }
        }

        public static void CleanPathsFromSouth(SquareData square, List<SquareData>[] exitsList)
        {
            if (square.lowerLeft != null) { CleanWaypointList(square.lowerLeft.Id, square.fromSouthAllowSouthWest, exitsList); }
            if (square.left != null) { CleanWaypointList(square.left.Id, square.fromSouthAllowWest, exitsList); }
            if (square.upperLeft != null) { CleanWaypointList(square.upperLeft.Id, square.fromSouthAllowNorthWest, exitsList); }
            if (square.upper != null) { CleanWaypointList(square.upper.Id, square.fromSouthAllowNorth, exitsList); }
            if (square.upperRight != null) { CleanWaypointList(square.upperRight.Id, square.fromSouthAllowNorthEast, exitsList); }
            if (square.right != null) { CleanWaypointList(square.right.Id, square.fromSouthAllowEast, exitsList); }
            if (square.lowerRight != null) { CleanWaypointList(square.lowerRight.Id, square.fromSouthAllowSouthEast, exitsList); }
        }

        public static void CleanWaypointList(byte directionalReferenceIdToCheck, bool directionalRuleToCheck, List<SquareData>[] exitsList)
        {
            foreach (var waypointDestinationList in exitsList.ToList()) //for each list of Waypoint Destinations
            {
                foreach (var destinationSquare in waypointDestinationList.ToList()) //for each square in that list
                {
                    //if the destination is one that we disallow, remove it.
                    if (destinationSquare.Id == directionalReferenceIdToCheck && !directionalRuleToCheck) { waypointDestinationList.Remove(destinationSquare); }
                }
            }
        }

        public static void CheckPathsFromNorth(SquareData square, List<SquareData>[] exitsList)
        {
            if (square.lower != null) { square.fromNorthAllowSouth = EnumerateDirectionsFromWaypoints(square.lower.Id, exitsList); }
            if (square.lowerLeft != null) { square.fromNorthAllowSouthWest = EnumerateDirectionsFromWaypoints(square.lowerLeft.Id, exitsList); }
            if (square.left != null) { square.fromNorthAllowWest = EnumerateDirectionsFromWaypoints(square.left.Id, exitsList); }
            if (square.upperLeft != null) { square.fromNorthAllowNorthWest = EnumerateDirectionsFromWaypoints(square.upperLeft.Id, exitsList); }
            if (square.upperRight != null) { square.fromNorthAllowNorthEast = EnumerateDirectionsFromWaypoints(square.upperRight.Id, exitsList); }
            if (square.right != null) { square.fromNorthAllowEast = EnumerateDirectionsFromWaypoints(square.right.Id, exitsList); }
            if (square.lowerRight != null) { square.fromNorthAllowSouthEast = EnumerateDirectionsFromWaypoints(square.lowerRight.Id, exitsList); }
        }
        public static void CheckPathsFromSouth(SquareData square, List<SquareData>[] exitsList)
        {
            if (square.lowerLeft != null) { square.fromSouthAllowSouthWest = EnumerateDirectionsFromWaypoints(square.lowerLeft.Id, exitsList); }
            if (square.left != null) { square.fromSouthAllowWest = EnumerateDirectionsFromWaypoints(square.left.Id, exitsList); }
            if (square.upperLeft != null) { square.fromSouthAllowNorthWest = EnumerateDirectionsFromWaypoints(square.upperLeft.Id, exitsList); }
            if (square.upper != null) { square.fromSouthAllowNorth = EnumerateDirectionsFromWaypoints(square.upper.Id, exitsList); }
            if (square.upperRight != null) { square.fromSouthAllowNorthEast = EnumerateDirectionsFromWaypoints(square.upperRight.Id, exitsList); }
            if (square.right != null) { square.fromSouthAllowEast = EnumerateDirectionsFromWaypoints(square.right.Id, exitsList); }
            if (square.lowerRight != null) { square.fromSouthAllowSouthEast = EnumerateDirectionsFromWaypoints(square.lowerRight.Id, exitsList); }
        }
        public static void CheckPathsFromWest(SquareData square, List<SquareData>[] exitsList)
        {
            if (square.lower != null) { square.fromWestAllowSouth = EnumerateDirectionsFromWaypoints(square.lower.Id, exitsList); }
            if (square.lowerLeft != null) { square.fromWestAllowSouthWest = EnumerateDirectionsFromWaypoints(square.lowerLeft.Id, exitsList); }
            if (square.upperLeft != null) { square.fromWestAllowNorthWest = EnumerateDirectionsFromWaypoints(square.upperLeft.Id, exitsList); }
            if (square.upper != null) { square.fromWestAllowNorth = EnumerateDirectionsFromWaypoints(square.upper.Id, exitsList); }
            if (square.upperRight != null) { square.fromWestAllowNorthEast = EnumerateDirectionsFromWaypoints(square.upperRight.Id, exitsList); }
            if (square.right != null) { square.fromWestAllowEast = EnumerateDirectionsFromWaypoints(square.right.Id, exitsList); }
            if (square.lowerRight != null) { square.fromWestAllowSouthEast = EnumerateDirectionsFromWaypoints(square.lowerRight.Id, exitsList); }
        }
        public static void CheckPathsFromEast(SquareData square, List<SquareData>[] exitsList)
        {
            if (square.lower != null) { square.fromEastAllowSouth = EnumerateDirectionsFromWaypoints(square.lower.Id, exitsList); }
            if (square.lowerLeft != null) { square.fromEastAllowSouthWest = EnumerateDirectionsFromWaypoints(square.lowerLeft.Id, exitsList); }
            if (square.left != null) { square.fromEastAllowWest = EnumerateDirectionsFromWaypoints(square.left.Id, exitsList); }
            if (square.upperLeft != null) { square.fromEastAllowNorthWest = EnumerateDirectionsFromWaypoints(square.upperLeft.Id, exitsList); }
            if (square.upper != null) { square.fromEastAllowNorth = EnumerateDirectionsFromWaypoints(square.upper.Id, exitsList); }
            if (square.upperRight != null) { square.fromEastAllowNorthEast = EnumerateDirectionsFromWaypoints(square.upperRight.Id, exitsList); }
            if (square.lowerRight != null) { square.fromEastAllowSouthEast = EnumerateDirectionsFromWaypoints(square.lowerRight.Id, exitsList); }
        }
        public static void CheckPathsFromNorthWest(SquareData square, List<SquareData>[] exitsList)
        {
            if (square.lower != null) { square.fromNorthWestAllowSouth = EnumerateDirectionsFromWaypoints(square.lower.Id, exitsList); }
            if (square.lowerLeft != null) { square.fromNorthWestAllowSouthWest = EnumerateDirectionsFromWaypoints(square.lowerLeft.Id, exitsList); }
            if (square.left != null) { square.fromNorthWestAllowWest = EnumerateDirectionsFromWaypoints(square.left.Id, exitsList); }
            if (square.upper != null) { square.fromNorthWestAllowNorth = EnumerateDirectionsFromWaypoints(square.upper.Id, exitsList); }
            if (square.upperRight != null) { square.fromNorthWestAllowNorthEast = EnumerateDirectionsFromWaypoints(square.upperRight.Id, exitsList); }
            if (square.right != null) { square.fromNorthWestAllowEast = EnumerateDirectionsFromWaypoints(square.right.Id, exitsList); }
            if (square.lowerRight != null) { square.fromNorthWestAllowSouthEast = EnumerateDirectionsFromWaypoints(square.lowerRight.Id, exitsList); }
        }
        public static void CheckPathsFromNorthEast(SquareData square, List<SquareData>[] exitsList)
        {
            if (square.lower != null) { square.fromNorthEastAllowSouth = EnumerateDirectionsFromWaypoints(square.lower.Id, exitsList); }
            if (square.lowerLeft != null) { square.fromNorthEastAllowSouthWest = EnumerateDirectionsFromWaypoints(square.lowerLeft.Id, exitsList); }
            if (square.upperLeft != null) { square.fromNorthEastAllowNorthWest = EnumerateDirectionsFromWaypoints(square.upperLeft.Id, exitsList); }
            if (square.left != null) { square.fromNorthEastAllowWest = EnumerateDirectionsFromWaypoints(square.left.Id, exitsList); }
            if (square.upper != null) { square.fromNorthEastAllowNorth = EnumerateDirectionsFromWaypoints(square.upper.Id, exitsList); }
            if (square.right != null) { square.fromNorthEastAllowEast = EnumerateDirectionsFromWaypoints(square.right.Id, exitsList); }
            if (square.lowerRight != null) { square.fromNorthEastAllowSouthEast = EnumerateDirectionsFromWaypoints(square.lowerRight.Id, exitsList); }
        }
        public static void CheckPathsFromSouthWest(SquareData square, List<SquareData>[] exitsList)
        {
            if (square.lower != null) { square.fromSouthWestAllowSouth = EnumerateDirectionsFromWaypoints(square.lower.Id, exitsList); }
            if (square.left != null) { square.fromSouthWestAllowWest = EnumerateDirectionsFromWaypoints(square.left.Id, exitsList); }
            if (square.upperLeft != null) { square.fromSouthWestAllowNorthWest = EnumerateDirectionsFromWaypoints(square.upperLeft.Id, exitsList); }
            if (square.upper != null) { square.fromSouthWestAllowNorth = EnumerateDirectionsFromWaypoints(square.upper.Id, exitsList); }
            if (square.upperRight != null) { square.fromSouthWestAllowNorthEast = EnumerateDirectionsFromWaypoints(square.upperRight.Id, exitsList); }
            if (square.right != null) { square.fromSouthWestAllowEast = EnumerateDirectionsFromWaypoints(square.right.Id, exitsList); }
            if (square.lowerRight != null) { square.fromSouthWestAllowSouthEast = EnumerateDirectionsFromWaypoints(square.lowerRight.Id, exitsList); }
        }
        public static void CheckPathsFromSouthEast(SquareData square, List<SquareData>[] exitsList)
        {
            if (square.lower != null) { square.fromSouthEastAllowSouth = EnumerateDirectionsFromWaypoints(square.lower.Id, exitsList); }
            if (square.lowerLeft != null) { square.fromSouthEastAllowSouthWest = EnumerateDirectionsFromWaypoints(square.lowerLeft.Id, exitsList); }
            if (square.upperLeft != null) { square.fromSouthEastAllowNorthWest = EnumerateDirectionsFromWaypoints(square.upperLeft.Id, exitsList); }
            if (square.left != null) { square.fromSouthEastAllowWest = EnumerateDirectionsFromWaypoints(square.left.Id, exitsList); }
            if (square.upper != null) { square.fromSouthEastAllowNorth = EnumerateDirectionsFromWaypoints(square.upper.Id, exitsList); }
            if (square.upperRight != null) { square.fromSouthEastAllowNorthEast = EnumerateDirectionsFromWaypoints(square.lowerRight.Id, exitsList); }
            if (square.right != null) { square.fromSouthEastAllowEast = EnumerateDirectionsFromWaypoints(square.right.Id, exitsList); }
        }

        public static bool EnumerateDirectionsFromWaypoints(byte directionalReferenceIdToCheck, List<SquareData>[] exitsList)
        {
            foreach (var waypointDestinationList in exitsList.ToList()) //for each list of Waypoint Destinations
            {
                foreach (var destinationSquare in waypointDestinationList.ToList()) //for each square in that list
                {
                    //if the destination exists, we'll assume we allow pathing to it.
                    if (destinationSquare.Id == directionalReferenceIdToCheck)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}