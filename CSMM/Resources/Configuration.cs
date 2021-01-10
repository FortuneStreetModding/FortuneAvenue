using MiscUtil.Conversion;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace CustomStreetMapManager
{
    public class Configuration
    {
        public static void Save(string fileName, List<MapDescriptor> mapDescriptors, IProgress<ProgressInfo> progress, CancellationToken ct)
        {
            var dir = Directory.GetParent(fileName).FullName;
            var p = 0;
            using (StreamWriter stream = new StreamWriter(fileName))
            {
                var result = from m in mapDescriptors orderby m.MapSet descending, m.Zone, m.Order select m;
                foreach (var md in result)
                {
                    var i = mapDescriptors.IndexOf(md);
                    string filePath = md.MapDescriptorFilePath;
                    if (!string.IsNullOrEmpty(filePath))
                        filePath = Path.GetRelativePath(dir, filePath);
                    else
                        filePath = "";
                    stream.WriteLine("{0,2},{1,2},{2,2},{3,2},{4,5},{5}", i, md.MapSet, md.Zone, md.Order, md.IsPracticeBoard, filePath);
                    progress?.Report(100 * p / result.Count());
                    p++;
                }
            }
            progress?.Report(100);
            progress?.Report("Saved configuration at " + fileName);
        }

        public static string Import(string fileName, string mapDescriptorFile, Optional<int> mapId, Optional<sbyte> mapSet, Optional<sbyte> zone, Optional<sbyte> order, Optional<bool> tutorial, IProgress<ProgressInfo> progress, CancellationToken ct)
        {
            var dir = Directory.GetParent(fileName).FullName;
            var lines = File.ReadAllLines(fileName).ToList();
            var mapDescriptorRelativePath = Path.GetRelativePath(dir, mapDescriptorFile);

            if (!mapId.Any())
            {
                var highestMapId = 0;
                sbyte highestMapSet = 0;
                sbyte highestZone = 0;
                sbyte highestOrder = 0;
                foreach (var line in lines)
                {
                    string[] columns = line.Split(new[] { ',' }, 6);
                    var mapIdConfig = int.Parse(columns[0].Trim());
                    if (mapIdConfig > highestMapId)
                        highestMapId = mapIdConfig;
                }
                foreach (var line in lines)
                {
                    string[] columns = line.Split(new[] { ',' }, 6);
                    var mapSetConfig = sbyte.Parse(columns[1].Trim());
                    if (mapSetConfig > highestMapSet)
                        highestMapSet = mapSetConfig;
                }
                foreach (var line in lines)
                {
                    string[] columns = line.Split(new[] { ',' }, 6);
                    var mapSetConfig = sbyte.Parse(columns[1].Trim());
                    if (mapSetConfig == highestMapSet)
                    {
                        var zoneConfig = sbyte.Parse(columns[2].Trim());
                        if (zoneConfig > highestZone)
                            highestZone = zoneConfig;
                    }
                }
                foreach (var line in lines)
                {
                    string[] columns = line.Split(new[] { ',' }, 6);
                    var mapSetConfig = sbyte.Parse(columns[1].Trim());
                    if (mapSetConfig == highestMapSet)
                    {
                        var zoneConfig = sbyte.Parse(columns[2].Trim());
                        if (zoneConfig == highestZone)
                        {
                            var orderConfig = sbyte.Parse(columns[3].Trim());
                            if (orderConfig > highestOrder)
                                highestOrder = orderConfig;
                        }
                    }
                }
                var newLine = String.Format("{0,2},{1,2},{2,2},{3,2},{4,5},{5}", highestMapId + 1, mapSet.OrElse(highestMapSet), zone.OrElse(highestZone), order.OrElse((sbyte)(highestOrder + 1)), tutorial.OrElse(false), mapDescriptorRelativePath);
                lines.Add(newLine);
                File.WriteAllLines(fileName, lines);
                progress?.Report("Added new map: " + newLine);
                return newLine;
            }
            else
            {
                for (int i = 0; i < lines.Count; i++)
                {
                    string[] columns = lines[i].Split(new[] { ',' }, 6);
                    var mapIdConfig = int.Parse(columns[0].Trim());
                    var mapSetConfig = mapSet.OrElse(sbyte.Parse(columns[1].Trim()));
                    var zoneConfig = zone.OrElse(sbyte.Parse(columns[2].Trim()));
                    var orderConfig = order.OrElse(sbyte.Parse(columns[3].Trim()));
                    var isPracticeBoardConfig = tutorial.OrElse(bool.Parse(columns[4].Trim()));
                    if (mapIdConfig == mapId.Single())
                    {
                        var newLine = String.Format("{0,2},{1,2},{2,2},{3,2},{4,5},{5}", mapId.Single(), mapSetConfig, zoneConfig, orderConfig, isPracticeBoardConfig, mapDescriptorRelativePath);
                        lines[i] = newLine;
                        progress?.Report("Replaced map: " + newLine);
                        return newLine;
                    }
                }
                throw new ArgumentException("Could not find map with id " + mapId.Single());
            }
        }

        public static void Load(string fileName, List<MapDescriptor> mapDescriptors, IProgress<ProgressInfo> progress, CancellationToken ct)
        {
            var dir = Directory.GetParent(fileName).FullName;

            string[] lines = File.ReadAllLines(fileName);
            var p = 0;
            // how many ids are there?
            var maxId = 0;
            foreach (var line in lines)
            {
                string[] columns = line.Split(new[] { ',' }, 6);
                var i = int.Parse(columns[0].Trim());
                if (i > maxId)
                    maxId = i;
            }
            var tempMapDescriptors = new List<MapDescriptor>();
            // add as many new map descriptors
            for (int i = 0; i < maxId + 1; i++)
            {
                var md = new MapDescriptor();
                tempMapDescriptors.Add(md);
                if (i < mapDescriptors.Count)
                    md.set(mapDescriptors[i]);
            }
            foreach (var line in lines)
            {
                string[] columns = line.Split(new[] { ',' }, 6);
                var i = int.Parse(columns[0].Trim());
                var mapSet = sbyte.Parse(columns[1].Trim());
                var zone = sbyte.Parse(columns[2].Trim());
                var order = sbyte.Parse(columns[3].Trim());
                var isPracticeBoard = bool.Parse(columns[4].Trim());
                var mapDescriptorFilePath = columns[5].Trim();

                tempMapDescriptors[i].MapSet = mapSet;
                tempMapDescriptors[i].Zone = zone;
                tempMapDescriptors[i].Order = order;
                tempMapDescriptors[i].IsPracticeBoard = isPracticeBoard;

                if (!string.IsNullOrEmpty(mapDescriptorFilePath))
                {
                    mapDescriptorFilePath = Path.Combine(dir, mapDescriptorFilePath);
                    var importMd = PatchProcess.ImportMd(mapDescriptorFilePath, ProgressInfo.makeNoProgress(progress), ct);
                    tempMapDescriptors[i].setFromImport(importMd);
                }
                progress?.Report(100 * p / lines.Count());
                p++;
            }

            while (mapDescriptors.Count > tempMapDescriptors.Count)
            {
                mapDescriptors.RemoveAt(mapDescriptors.Count - 1);
            }
            for (int i = mapDescriptors.Count; i < tempMapDescriptors.Count; i++)
            {
                var md = new MapDescriptor();
                // only add new maps, if there is a map descriptor file path available
                if (!string.IsNullOrEmpty(tempMapDescriptors[i].MapDescriptorFilePath))
                {
                    mapDescriptors.Add(md);
                }
                else
                {
                    progress?.Report("Warning: Could not load the configuration after map " + i + " because the md files are not set.");
                    break;
                }
            }

            for (int i = 0; i < mapDescriptors.Count; i++)
            {
                if (!tempMapDescriptors[i].Equals(mapDescriptors[i]))
                {
                    tempMapDescriptors[i].Dirty = true;
                }
                mapDescriptors[i].setFromImport(tempMapDescriptors[i]);
                mapDescriptors[i].MapSet = tempMapDescriptors[i].MapSet;
                mapDescriptors[i].Zone = tempMapDescriptors[i].Zone;
                mapDescriptors[i].Order = tempMapDescriptors[i].Order;
                mapDescriptors[i].IsPracticeBoard = tempMapDescriptors[i].IsPracticeBoard;
            }
            progress?.Report(100);
            progress?.Report("Loaded configuration from " + fileName);
        }
    }
}
