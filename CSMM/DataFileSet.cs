using FSEditor.FSData;
using FSEditor.MapDescriptor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace CustomStreetManager
{
    public class DataFileSet
    {
        public string rootDir;
        public string main_dol;
        public readonly Dictionary<string, string> ui_message_csv;
        public string param_folder;
        public readonly Dictionary<string, string> game_sequence_arc;

        private static readonly DataFileSet baseFileSet = createBaseFileSet();

        private static DataFileSet createBaseFileSet()
        {
            DataFileSet fileSet = new DataFileSet();
            fileSet.rootDir = "";
            fileSet.main_dol = Path.Combine("sys", "main.dol");
            fileSet.param_folder = Path.Combine("files", "param");
            fileSet.ui_message_csv[Locale.DE] = Path.Combine("files", "localize", "ui_message.de.csv");
            fileSet.ui_message_csv[Locale.EN] = Path.Combine("files", "localize", "ui_message.en.csv");
            fileSet.ui_message_csv[Locale.FR] = Path.Combine("files", "localize", "ui_message.fr.csv");
            fileSet.ui_message_csv[Locale.IT] = Path.Combine("files", "localize", "ui_message.it.csv");
            fileSet.ui_message_csv[Locale.JP] = Path.Combine("files", "localize", "ui_message.jp.csv");
            fileSet.ui_message_csv[Locale.ES] = Path.Combine("files", "localize", "ui_message.su.csv");
            fileSet.ui_message_csv[Locale.UK] = Path.Combine("files", "localize", "ui_message.uk.csv");
            // game_sequence
            fileSet.game_sequence_arc[Locale.EN] = Path.Combine("files", "game", "langEN", "game_sequence_EN.arc");
            fileSet.game_sequence_arc[Locale.DE] = Path.Combine("files", "game", "langDE", "game_sequence_DE.arc");
            fileSet.game_sequence_arc[Locale.ES] = Path.Combine("files", "game", "langES", "game_sequence_ES.arc");
            fileSet.game_sequence_arc[Locale.FR] = Path.Combine("files", "game", "langFR", "game_sequence_FR.arc");
            fileSet.game_sequence_arc[Locale.IT] = Path.Combine("files", "game", "langIT", "game_sequence_IT.arc");
            fileSet.game_sequence_arc[Locale.UK] = Path.Combine("files", "game", "langUK", "game_sequence_UK.arc");
            fileSet.game_sequence_arc[Locale.JP] = Path.Combine("files", "game", "game_sequence.arc");
            // game_sequence_wifi
            fileSet.game_sequence_arc[Locale.EN] = Path.Combine("files", "game", "langEN", "game_sequence_wifi_EN.arc");
            fileSet.game_sequence_arc[Locale.DE] = Path.Combine("files", "game", "langDE", "game_sequence_wifi_DE.arc");
            fileSet.game_sequence_arc[Locale.ES] = Path.Combine("files", "game", "langES", "game_sequence_wifi_ES.arc");
            fileSet.game_sequence_arc[Locale.FR] = Path.Combine("files", "game", "langFR", "game_sequence_wifi_FR.arc");
            fileSet.game_sequence_arc[Locale.IT] = Path.Combine("files", "game", "langIT", "game_sequence_wifi_IT.arc");
            fileSet.game_sequence_arc[Locale.UK] = Path.Combine("files", "game", "langUK", "game_sequence_wifi_UK.arc");
            fileSet.game_sequence_arc[Locale.JP] = Path.Combine("files", "game", "game_sequence_wifi.arc");
            return fileSet;
        }

        private DataFileSet()
        {
            ui_message_csv = new Dictionary<string, string>();
            game_sequence_arc = new Dictionary<string, string>();
            main_dol = null;
            param_folder = null;
            rootDir = null;
        }

        public DataFileSet(string dataDir)
        {
            rootDir = dataDir;
            main_dol = Path.Combine(dataDir, baseFileSet.main_dol);
            param_folder = Path.Combine(dataDir, baseFileSet.param_folder);
            ui_message_csv = new Dictionary<string, string>();
            game_sequence_arc = new Dictionary<string, string>();
            foreach (string locale in baseFileSet.ui_message_csv.Keys)
            {
                ui_message_csv[locale] = Path.Combine(dataDir, baseFileSet.ui_message_csv[locale]);
                game_sequence_arc[locale] = Path.Combine(dataDir, baseFileSet.game_sequence_arc[locale]);
            }
        }

        public void createDirectoryStructure()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(main_dol));
            Directory.CreateDirectory(param_folder);
            foreach (string locale in baseFileSet.ui_message_csv.Keys)
            {
                Directory.CreateDirectory(Path.GetDirectoryName(ui_message_csv[locale]));
                Directory.CreateDirectory(Path.GetDirectoryName(game_sequence_arc[locale]));
            }
        }

        public void copy(ICollection<string> filesToCopyInParamDir, DataFileSet destDataFileSet, bool overwrite)
        {
            File.Copy(main_dol, destDataFileSet.main_dol, overwrite);
            foreach (string locale in baseFileSet.ui_message_csv.Keys)
            {
                File.Copy(ui_message_csv[locale], destDataFileSet.ui_message_csv[locale], overwrite);
                File.Copy(game_sequence_arc[locale], destDataFileSet.game_sequence_arc[locale], overwrite);
            }
            foreach (string file in filesToCopyInParamDir)
            {
                if (File.Exists(Path.Combine(param_folder, file)))
                    File.Copy(Path.Combine(param_folder, file), Path.Combine(destDataFileSet.param_folder, file), overwrite);
            }
        }
    }
}
