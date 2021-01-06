using FSEditor.FSData;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomStreetMapManager
{
    public class MapDescriptorTemplate
    {
        private MapDescriptor md;
        public MapDescriptorTemplate(MapDescriptor mapDescriptor) { this.md = mapDescriptor; }

        public override string ToString()
        {
            StringBuilder s = new StringBuilder();
            s.AppendLine("# " + md.Name[Locale.EN]);
            s.AppendLine();
            s.AppendLine(md.Desc[Locale.EN]);
            s.AppendLine();
            s.AppendLine("## Screenshots");
            s.AppendLine();
            s.AppendLine("<Placeholder for screenshots>");
            s.AppendLine();
            s.AppendLine("## Features");
            s.AppendLine();
            s.AppendLine("### Map Properties");
            s.AppendLine();
            s.AppendLine(GetMapPropertiesTable());
            s.AppendLine();
            s.AppendLine("### Map Background");
            s.AppendLine();
            s.AppendLine(EncloseExpansionTag(GetMapBackgroundTable()));
            s.AppendLine();
            s.AppendLine("### Map Icon");
            s.AppendLine();
            s.AppendLine(EncloseExpansionTag(GetMapIconTable()));
            s.AppendLine();
            s.AppendLine("### Map Background Music");
            s.AppendLine();
            s.AppendLine(EncloseExpansionTag(GetBackgroundMusicTable()));
            s.AppendLine();
            if (!string.IsNullOrEmpty(GetSwitchRotationOriginPointsTable()))
            {
                s.AppendLine("### Switch Rotation Animation Configuration");
                s.AppendLine();
                s.AppendLine(EncloseExpansionTag(GetSwitchRotationOriginPointsTable()));
                s.AppendLine();
            }
            if (!string.IsNullOrEmpty(GetLoopingModeTable()))
            {
                s.AppendLine("### Looping Mode Configuration");
                s.AppendLine();
                s.AppendLine(EncloseExpansionTag(GetLoopingModeTable()));
                s.AppendLine();
            }
            s.AppendLine("### Tour Configuration");
            s.AppendLine();
            s.AppendLine(EncloseExpansionTag(GetTourConfigurationTable()));
            s.AppendLine();
            s.AppendLine("### Localization");
            s.AppendLine();
            s.AppendLine(EncloseExpansionTag(GetLocalizationTable()));
            s.AppendLine();
            s.AppendLine("### Venture Cards");
            s.AppendLine();
            s.AppendLine(EncloseExpansionTag(GetVentureCardTable()));
            s.AppendLine();
            s.AppendLine("## Changelog");
            s.AppendLine();
            s.AppendLine("### v1");
            s.AppendLine();
            s.AppendLine("- <Placeholder for version information>");
            s.AppendLine();
            s.AppendLine("## Authors");
            s.AppendLine();
            s.AppendLine("- <Placeholder for author information>");
            return s.ToString();
        }

        public StringBuilder RemoveTrailingLineEnding(StringBuilder s)
        {
            var lineEnding = new StringBuilder().AppendLine().ToString();
            s.Replace(lineEnding, null, s.Length - lineEnding.Length - 1, lineEnding.Length + 1);
            return s;
        }

        public string EncloseExpansionTag(string enclosingText)
        {
            StringBuilder s = new StringBuilder();
            s.AppendLine("<details>");
            s.AppendLine("  <summary>Click to expand!</summary>");
            s.AppendLine();
            s.AppendLine(enclosingText);
            s.AppendLine();
            s.AppendLine("</details>");
            RemoveTrailingLineEnding(s);
            return s.ToString();
        }

        public string GetMapPropertiesTable()
        {
            StringBuilder s = new StringBuilder();
            var table = "| {0,-16} | {1,20} |";
            s.AppendLine(String.Format(table, "Map Properties", "Value"));
            var tableDashLine = String.Format(table, GetDashLine(16), GetDashLine(20));
            s.AppendLine(tableDashLine);

            s.AppendLine(String.Format(table, "Initial Cash", md.InitialCash));
            s.AppendLine(String.Format(table, "Target Amount", md.TargetAmount));
            s.AppendLine(String.Format(table, "Base Salary", md.BaseSalary));
            s.AppendLine(String.Format(table, "Salary Increment", md.SalaryIncrement));
            s.AppendLine(String.Format(table, "Max Dice Roll", md.MaxDiceRoll));
            s.AppendLine(String.Format(table, "Looping Mode", md.LoopingMode));
            s.AppendLine(String.Format(table, "Rules", md.RuleSet));
            s.AppendLine(String.Format(table, "Theme", md.Theme));
            s.AppendLine(String.Format(table, "FRB File Name 1", md.FrbFile1));
            s.AppendLine(String.Format(table, "FRB File Name 2", md.FrbFile2));
            s.AppendLine(String.Format(table, "FRB File Name 3", md.FrbFile3));
            s.AppendLine(String.Format(table, "FRB File Name 4", md.FrbFile4));
            RemoveTrailingLineEnding(s);
            return s.ToString();
        }

        public string GetMapBackgroundTable()
        {
            StringBuilder s = new StringBuilder();
            var table = "| {0,-3} | {1,-10} | {2,-20} |";
            s.AppendLine(String.Format(table, "On", "Background", "Description"));
            var tableDashLine = String.Format(table, GetDashLine(3), GetDashLine(10), GetDashLine(20));
            s.AppendLine(tableDashLine);

            var vanilla = false;
            foreach (var row in VanillaDatabase.getMapTable().Rows)
            {
                var dataRow = (DataRow)row;
                var bg = dataRow.Field<string>("Background");
                var desc = dataRow.Field<string>("Description");
                var on = "   ";
                if (md.Background == bg)
                {
                    vanilla = true;
                    on = ":o:";
                }
                s.AppendLine(String.Format(table, on, bg, desc));
            }
            if (!vanilla)
            {
                s.AppendLine(String.Format(table, ":o:", md.Background, ""));
            }
            RemoveTrailingLineEnding(s);
            return s.ToString();
        }

        public string GetMapIconTable()
        {
            StringBuilder s = new StringBuilder();
            var table = "| {0,-3} | {1,-10} | {2,-20} |";
            s.AppendLine(String.Format(table, "On", "Icon", "Description"));
            var tableDashLine = String.Format(table, GetDashLine(3), GetDashLine(10), GetDashLine(20));
            s.AppendLine(tableDashLine);

            var vanilla = false;
            foreach (var row in VanillaDatabase.getMapIconTable().Rows)
            {
                var dataRow = (DataRow)row;
                var mapIcon = dataRow.Field<string>("Map Icon");
                var desc = dataRow.Field<string>("Description");
                var on = "   ";
                if (md.MapIcon == mapIcon)
                {
                    vanilla = true;
                    on = ":o:";
                }
                s.AppendLine(String.Format(table, on, mapIcon, desc));
            }
            if (!vanilla)
            {
                s.AppendLine(String.Format(table, ":o:", md.MapIcon, ""));
            }
            RemoveTrailingLineEnding(s);
            return s.ToString();
        }

        public string GetBackgroundMusicTable()
        {
            StringBuilder s = new StringBuilder();
            var table = "| {0,3} | {1,3} | {2,5} | {3,-24} | {4,-20} |";
            s.AppendLine(String.Format(table, "On", "BGM", "Brsar", "Filename", "Description"));
            var tableDashLine = String.Format(table, GetDashLine(3), GetDashLine(3), GetDashLine(5), GetDashLine(24), GetDashLine(20));
            s.AppendLine(tableDashLine);

            var vanilla = false;
            foreach (var row in VanillaDatabase.getBgmTable().Rows)
            {
                var dataRow = (DataRow)row;
                var bgmId = dataRow.Field<uint>("Bgm Id");
                var brsarMario = dataRow.Field<uint>("Brsar Mario");
                var brsarDQ = dataRow.Field<uint>("Brsar DragonQuest");
                string brsar = brsarMario + "";
                if (brsarDQ != brsarMario)
                {
                    brsar += "/" + brsarDQ;
                }
                var filename = dataRow.Field<string>("Filename");
                var desc = dataRow.Field<string>("Description");
                var on = "   ";
                if (md.BGMID == bgmId)
                {
                    vanilla = true;
                    on = ":o:";
                }
                s.AppendLine(String.Format(table, on, bgmId, brsar, filename, desc));
            }
            if (!vanilla)
            {
                // TODO read out brsar table and filename
                s.AppendLine(String.Format(table, ":o:", md.BGMID, "", "", "", ""));
            }
            RemoveTrailingLineEnding(s);
            return s.ToString();
        }

        public string GetSwitchRotationOriginPointsTable()
        {
            if (md.SwitchRotationOriginPoints.Count == 0)
                return "";
            StringBuilder s = new StringBuilder();
            var table = "| {0,-29} | {1,7} |";
            s.AppendLine(String.Format(table, "Switch Rotation Origin Points", "Value"));
            var tableDashLine = String.Format(table, GetDashLine(29), GetDashLine(7));
            s.AppendLine(tableDashLine);

            for (int i = 0; i < md.SwitchRotationOriginPoints.Count; i++)
            {
                s.AppendLine(String.Format(table, "Rotation Origin Point " + (i + 1) + " X", md.SwitchRotationOriginPoints[i].X.ToString(CultureInfo.InvariantCulture)));
                s.AppendLine(String.Format(table, "Rotation Origin Point " + (i + 1) + " Y", md.SwitchRotationOriginPoints[i].Y.ToString(CultureInfo.InvariantCulture)));
            }
            RemoveTrailingLineEnding(s);
            return s.ToString();
        }

        public string GetLoopingModeTable()
        {
            if (md.LoopingMode == LoopingMode.None)
                return "";
            StringBuilder s = new StringBuilder();
            var table = "| {0,-26} | {1,10} |";
            s.AppendLine(String.Format(table, "Looping Mode Configuration", "Value"));
            var tableDashLine = String.Format(table, GetDashLine(26), GetDashLine(10));
            s.AppendLine(tableDashLine);

            s.AppendLine(String.Format(table, "Radius", md.LoopingModeRadius.ToString(CultureInfo.InvariantCulture)));
            s.AppendLine(String.Format(table, "Horizontal Padding", md.LoopingModeHorizontalPadding.ToString(CultureInfo.InvariantCulture)));
            s.AppendLine(String.Format(table, "Vertical Square Count", md.LoopingModeVerticalSquareCount.ToString(CultureInfo.InvariantCulture)));
            RemoveTrailingLineEnding(s);
            return s.ToString();
        }

        public string GetTourConfigurationTable()
        {
            StringBuilder s = new StringBuilder();
            var table = "| {0,-21} | {1,15} |";
            s.AppendLine(String.Format(table, "Tour Configuration", "Value"));
            var tableDashLine = String.Format(table, GetDashLine(21), GetDashLine(15));
            s.AppendLine(tableDashLine);

            s.AppendLine(String.Format(table, "Tour Bankruptcy Limit", md.TourBankruptcyLimit));
            s.AppendLine(String.Format(table, "Tour Initial Cash", md.TourInitialCash));
            s.AppendLine(String.Format(table, "Tour Opponent 1", md.TourOpponent1));
            s.AppendLine(String.Format(table, "Tour Opponent 2", md.TourOpponent2));
            s.AppendLine(String.Format(table, "Tour Opponent 3", md.TourOpponent3));
            s.AppendLine(String.Format(table, "Tour Clear Rank", md.TourClearRank));
            RemoveTrailingLineEnding(s);
            return s.ToString();
        }

        public string GetLocalizationTable()
        {
            StringBuilder s = new StringBuilder();
            var table = "| {0,-9} | {1,-9} |";
            s.AppendLine(String.Format(table, "Message", "String"));
            var tableDashLine = String.Format(table, GetDashLine(9), GetDashLine(9));
            s.AppendLine(tableDashLine);

            s.AppendLine(String.Format(table, "Name (DE)", md.Name[Locale.DE]));
            s.AppendLine(String.Format(table, "Name (ES)", md.Name[Locale.ES]));
            s.AppendLine(String.Format(table, "Name (FR)", md.Name[Locale.FR]));
            s.AppendLine(String.Format(table, "Name (IT)", md.Name[Locale.IT]));
            s.AppendLine(String.Format(table, "Name (JP)", md.Name[Locale.JP]));
            s.AppendLine(String.Format(table, "Desc (DE)", md.Desc[Locale.DE]));
            s.AppendLine(String.Format(table, "Desc (ES)", md.Desc[Locale.ES]));
            s.AppendLine(String.Format(table, "Desc (FR)", md.Desc[Locale.FR]));
            s.AppendLine(String.Format(table, "Desc (IT)", md.Desc[Locale.IT]));
            s.AppendLine(String.Format(table, "Desc (JP)", md.Desc[Locale.JP]));
            RemoveTrailingLineEnding(s);
            return s.ToString();
        }

        public string GetVentureCardTable()
        {
            StringBuilder s = new StringBuilder();
            var table = "| {0,3} | {1,3} | {2,-112} |";
            s.AppendLine(String.Format(table, "ID", "On", "Description"));
            var tableDashLine = String.Format(table, GetDashLine(3), GetDashLine(3), GetDashLine(112));
            s.AppendLine(tableDashLine);

            foreach (var row in VanillaDatabase.getVentureCardTable().Rows)
            {
                var dataRow = (DataRow)row;
                var ventureCardId = dataRow.Field<byte>("Id");
                var desc = dataRow.Field<string>("Description");
                var on = "   ";
                if (md.VentureCard[ventureCardId - 1] != 0)
                {
                    on = ":o:";
                }
                s.AppendLine(String.Format(table, ventureCardId, on, desc));
            }
            RemoveTrailingLineEnding(s);
            return s.ToString();
        }

        private string GetDashLine(int numberChars)
        {
            return "".PadLeft(numberChars, '-');
        }
    }
}
