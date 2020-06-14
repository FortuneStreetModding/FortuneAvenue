using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Threading;

namespace CustomStreetManager
{
    public partial class CSMM : Form
    {
        //Disc Operations
        string ExtractDiscFileName = "_windowspatchscript_extractDisc.bat";
        string CompileDiscFileName = "_windowspatchscript_compileDisc.bat";
        string CompileDiscWithWiimmfiFileName = "_windowspatchscript_compileDiscwithWiimmfi.bat";

        //Other Options
        string DeflaktorASMBootDol = @"..\_asm\main.dol";
        string DeflaktorASMUIMessageEN = @"..\_asm\ui_message.en.csv";
        string DeflaktorASMUIMessageDE = @"..\_asm\ui_message.de.csv";
        string DeflaktorASMUIMessageUK = @"..\_asm\ui_message.uk.csv";
        string DeflaktorASMUIMessageENPlusWiimmfi = @"..\_asm\ui_message_wiimmfi.en.csv";
        string DeflaktorASMUIMessageDEPlusWiimmfi = @"..\_asm\ui_message_wiimmfi.de.csv";
        string DeflaktorASMUIMessageUKPlusWiimmfi = @"..\_asm\ui_message_wiimmfi.uk.csv";

        string USWiimmfiUIUpdateFile = @"..\_wiimmfi\ui_message_wiimmfi_without_asm.en.csv";
        string UKWiimmfiUIUpdateFile = @"..\_wiimmfi\ui_message_wiimmfi_without_asm.uk.csv";
        string DEWiimmfiUIUpdateFile = @"..\_wiimmfi\ui_message_wiimmfi_without_asm.de.csv";

        string originalBootDol = @"..\_working_directory\DATA\sys\main.dol";
        string originalUIMessageEN = @"..\_working_directory\DATA\files\localize\ui_message.en.csv";
        string originalUIMessageDE = @"..\_working_directory\DATA\files\localize\ui_message.de.csv";
        string originalUIMessageUK = @"..\_working_directory\DATA\files\localize\ui_message.uk.csv";


        string originalBootDol2 = @"..\_working_directory\sys\main.dol";
        string originalUIMessageEN2 = @"..\_working_directory\files\localize\ui_message.en.csv";
        string originalUIMessageDE2 = @"..\_working_directory\files\localize\ui_message.de.csv";
        string originalUIMessageUK2 = @"..\_working_directory\files\localize\ui_message.uk.csv";


        //Maps
        string angel0FilePath = @"..\_working_directory\data\files\param\angel0.frb";
        string angel1FilePath = @"..\_working_directory\data\files\param\angel1.frb";
        string angel2FilePath = @"..\_working_directory\data\files\param\angel2.frb";
        string angel3FilePath = @"..\_working_directory\data\files\param\angel3.frb";

        string alephgardFilePath = @"..\_working_directory\data\files\param\alephgard.frb";
        string circuitFilePath = @"..\_working_directory\data\files\param\circuit.frb";
        string colonyFilePath = @"..\_working_directory\data\files\param\colony.frb";
        string dahmaFilePath = @"..\_working_directory\data\files\param\dahma.frb";
        string dolpicFilePath = @"..\_working_directory\data\files\param\dolpic.frb";
        string galaxy1FilePath = @"..\_working_directory\data\files\param\galaxy1.frb";
        string hidariFilePath = @"..\_working_directory\data\files\param\2-07-hidari.frb";
        string kandataFilePath = @"..\_working_directory\data\files\param\kandata.frb";
        string koopaFilePath = @"..\_working_directory\data\files\param\koopa.frb";

        string majinzo0FilePath = @"..\_working_directory\data\files\param\majinzo1.frb";
        string majinzo1FilePath = @"..\_working_directory\data\files\param\majinzo2.frb";

        string marioFilePath = @"..\_working_directory\data\files\param\mario.frb";
        string mooncityFilePath = @"..\_working_directory\data\files\param\mooncity.frb";
        string slabbakatouFilePath = @"..\_working_directory\data\files\param\slabakkatou.frb";
        string stadiumFilePath = @"..\_working_directory\data\files\param\studium.frb";
        string torodeenFilePath = @"..\_working_directory\data\files\param\torodeen.frb";

        string volcano0FilePath = @"..\_working_directory\data\files\param\volcano0.frb";
        string volcano1FilePath = @"..\_working_directory\data\files\param\volcano1.frb";

        string yosshiFilePath = @"..\_working_directory\data\files\param\yosshi.frb";
        string yuureisenFilePath = @"..\_working_directory\data\files\param\yuureisen.frb";

        //Maps 2
        string angel0FilePath2 = @"..\_working_directory\files\param\angel0.frb";
        string angel1FilePath2 = @"..\_working_directory\files\param\angel1.frb";
        string angel2FilePath2 = @"..\_working_directory\files\param\angel2.frb";
        string angel3FilePath2 = @"..\_working_directory\files\param\angel3.frb";
        string alephgardFilePath2 = @"..\_working_directory\files\param\alephgard.frb";
        string circuitFilePath2 = @"..\_working_directory\files\param\circuit.frb";
        string colonyFilePath2 = @"..\_working_directory\files\param\colony.frb";
        string dahmaFilePath2 = @"..\_working_directory\files\param\dahma.frb";
        string dolpicFilePath2 = @"..\_working_directory\files\param\dolpic.frb";
        string galaxy1FilePath2 = @"..\_working_directory\files\param\galaxy1.frb";
        string hidariFilePath2 = @"..\_working_directory\files\param\2-07-hidari.frb";
        string kandataFilePath2 = @"..\_working_directory\files\param\kandata.frb";
        string koopaFilePath2 = @"..\_working_directory\files\param\koopa.frb";
        string majinzo0FilePath2 = @"..\_working_directory\files\param\majinzo1.frb";
        string majinzo1FilePath2 = @"..\_working_directory\files\param\majinzo2.frb";
        string marioFilePath2 = @"..\_working_directory\files\param\mario.frb";
        string mooncityFilePath2 = @"..\_working_directory\files\param\mooncity.frb";
        string slabbakatouFilePath2 = @"..\_working_directory\files\param\slabakkatou.frb";
        string stadiumFilePath2 = @"..\_working_directory\files\param\studium.frb";
        string torodeenFilePath2 = @"..\_working_directory\files\param\torodeen.frb";
        string volcano0FilePath2 = @"..\_working_directory\files\param\volcano0.frb";
        string volcano1FilePath2 = @"..\_working_directory\files\param\volcano1.frb";
        string yosshiFilePath2 = @"..\_working_directory\files\param\yosshi.frb";
        string yuureisenFilePath2 = @"..\_working_directory\files\param\yuureisen.frb";

        string newMapFolderPath = @"..\_place_one_map_file_here_at_a_time\";
        string outputFolderPath = @"..\_patched_iso_will_go_here\";
        string vanillaISOFolderPath = @"..\_place_vanilla_iso_here\";

        //Music files
        //directory 1
        string soundConfig1 = @"..\_working_directory\DATA\files\sound\Itast.brsar";
        string soundConfig2 = @"..\_working_directory\DATA\files\sound\Itast2.brsar";
        string soundConfig3 = @"..\_working_directory\DATA\files\sound\Itast3.brsar";

        //directory 2
        string soundConfig1_dir2 = @"..\_working_directory\files\sound\Itast.brsar";
        string soundConfig2_dir2 = @"..\_working_directory\files\sound\Itast2.brsar";
        string soundConfig3_dir2 = @"..\_working_directory\files\sound\Itast3.brsar";

        string musiclessConfigFile = @"..\_zmisc\Itast.brsar";
        string directoryTestFolderPath = @"..\_working_directory\";

        bool dir1 = false;
        bool dir2 = false;


        int observatoryDynamicCount = 0;
        int colossusDynamicCount = 0;
        int mtMagDynamicCount = 0;

        int timeLeft = 5;
        bool changesFinalized = false;

        string Arguments = "start /wait /min /c";

        public CSMM()
        {
            InitializeComponent();
        }

        public void Awake()
        {
            Directory.SetCurrentDirectory(Directory.GetCurrentDirectory());
        }

        private void UpdateMapButton_Click(object sender, EventArgs e)
        {
            string extractDiscBatFilePath = Path.Combine(Directory.GetCurrentDirectory(), ExtractDiscFileName);

            ProgressBar instance = new ProgressBar();
            instance.Show();

            UpdateProgressWindow(instance, "Extracting disc...", 25);

            if (File.Exists(setInputISOLocation.Text))
            {
                if (setOutputPathLabel.Text != "None")
                {
                    ProcessStartInfo psi = new ProcessStartInfo(extractDiscBatFilePath, Arguments);
                    psi.Arguments = "\"" + setInputISOLocation.Text + "\"";
                    psi.CreateNoWindow = true;
                    psi.UseShellExecute = false;
                    psi.WorkingDirectory = Directory.GetCurrentDirectory();

                    var p = Process.Start(psi);
                    p.WaitForExit();

                    DetermineISOFolderStructure(instance);

                    UpdateProgressWindow(instance, "Replacing maps...", 40);

                    foreach (ListViewItem item in listOfMapsToPatchIn.Items)
                    {
                        string newMapPath = item.SubItems[2].Text;
                        string mapToReplace = item.SubItems[1].Text;
                        //the below declarations are determining what this does when it's clicked, as it is map-dependent. (probably should be its own method)

                        //dynamic maps; maps that take up more than 1 map file

                        if (item.SubItems[1].Text == "*The Observatory" && item.SubItems[3].Text == "true")
                        {
                            ReplaceDynamicObservatory(item, item.SubItems[2].Text);
                        }

                        else if (item.SubItems[1].Text == "*Colossus" && item.SubItems[3].Text == "true")
                        {
                            ReplaceDynamicColossus(item, item.SubItems[2].Text);
                        }
                        else if (item.SubItems[1].Text == "*Mt. Magmageddon" && item.SubItems[3].Text == "true")
                        {
                            ReplaceDynamicMtMagmageddon(item, item.SubItems[2].Text);
                        }

                        //simple maps

                        if (mapToReplace == "Castle Trodain")
                        {
                            ReplaceCastleTrodain(newMapPath);
                        }
                        else if (mapToReplace == "The Observatory")
                        {
                            ReplaceStaticObservatory(newMapPath);
                        }
                        else if (mapToReplace == "Ghost Ship")
                        {
                            ReplaceGhostShip(newMapPath);
                        }
                        else if (mapToReplace == "Slimenia")
                        {
                            ReplaceSlimenia(newMapPath);
                        }
                        else if (mapToReplace == "Mt. Magmageddon")
                        {
                            ReplaceStaticMtMagmageddon(newMapPath);
                        }
                        else if (mapToReplace == "Robbin' Hood Ruins")
                        {
                            ReplaceRobbinHoodRuins(newMapPath);
                        }
                        else if (mapToReplace == "Peach's Castle")
                        {
                            ReplacePeachsCastle(newMapPath);
                        }
                        else if (mapToReplace == "Delfino Plaza")
                        {
                            ReplaceDelfinoPlaza(newMapPath);
                        }
                        else if (mapToReplace == "Yoshi's Island")
                        {
                            ReplaceYoshisIsland(newMapPath);
                        }
                        else if (mapToReplace == "Mario Circuit")
                        {
                            ReplaceMarioCircuit(newMapPath);
                        }
                        else if (mapToReplace == "Starship Mario")
                        {
                            ReplaceStarshipMario(newMapPath);
                        }
                        else if (mapToReplace == "Mario Stadium")
                        {
                            ReplaceMarioStadium(newMapPath);
                        }
                        else if (mapToReplace == "Alltrades Abbey")
                        {
                            ReplaceAlltradesAbbey(newMapPath);
                        }
                        else if (mapToReplace == "Colossus")
                        {
                            ReplaceColossus(newMapPath);
                        }
                        else if (mapToReplace == "Good Egg Galaxy")
                        {
                            ReplaceGoodEggGalaxy(newMapPath);
                        }
                        else if (mapToReplace == "Bowser's Castle")
                        {
                            ReplaceBowsersCastle(newMapPath);
                        }
                        else if (mapToReplace == "Super Mario Bros.")
                        {
                            ReplaceSMB(newMapPath);
                        }
                        else if (mapToReplace == "Alefgard")
                        {
                            ReplaceAlefgard(newMapPath);
                        }
                        else if (noneButton.Checked)
                        {
                            //not much to do here!
                        }
                    }
                    UpdateProgressWindow(instance, "Setting Options...", 70);

                    RemoveMusic();
                    ASMHacks();
                    UpdateUIForWiimmfi();

                    UpdateProgressWindow(instance, "Finalizing changes...", 85);

                    while (!changesFinalized)
                    {
                        Thread.Sleep(5000);
                        changesFinalized = true; //super crude pause to wait for the disk to write all the changes
                    }

                    UpdateProgressWindow(instance, "Re-compiling disc...", 90);

                    CompileTheDisc();

                    UpdateProgressWindow(instance, "Done!", 100);
                    instance.SetButtonToClose();
                }
                else
                {
                    instance.SetProgressBarLabel("Error: The output file path cannot be blank!");
                    instance.SetProgressBarValue(100);
                    instance.SetButtonToGoBack();
                }
            }
            else
            {
                instance.SetProgressBarLabel("Error: The source ISO could not be opened.");
                instance.SetProgressBarValue(100);
                instance.SetButtonToGoBack();
            }
        }

        void timer_Tick(object sender, EventArgs e)
        {
            if(timeLeft > 0)
            {
                timeLeft -= 1;
            }
            else
            {
                changesFinalized = true;
            }
        }


        private void DetermineISOFolderStructure(ProgressBar instance)
        {
            if (Directory.Exists(directoryTestFolderPath + "files"))
            {
                dir1 = false;
                dir2 = true;
                instance.SetProgressBarLabel("1/4 - Extracting to dir2");
            }
            else if (Directory.Exists(directoryTestFolderPath + "data"))
            {
                dir1 = true;
                dir2 = false;
                instance.SetProgressBarLabel("1/4 - Extracting to dir1");
            }
            else
            {
                dir1 = false;
                dir2 = false;
                instance.SetProgressBarLabel("Error: The working directory is missing or empty!");
                instance.SetButtonToClose();
            }
        }

        private void ReplaceDynamicMtMagmageddon(ListViewItem item, string newMapPath)
        {
            if (item.SubItems[4].Text == "1")
            {
                if (dir1)
                {
                    if (File.Exists(newMapPath) && File.Exists(volcano0FilePath))
                    {
                        MoveFile(newMapPath, volcano0FilePath);
                    }
                }
                else if (dir2)
                {
                    if (File.Exists(newMapPath) && File.Exists(volcano0FilePath2))
                    {
                        MoveFile(newMapPath, volcano0FilePath2);
                    }
                }
            }
            else if (item.SubItems[4].Text == "2")
            {
                if (dir1)
                {
                    if (File.Exists(newMapPath) && File.Exists(volcano1FilePath))
                    {
                        MoveFile(newMapPath, volcano1FilePath);
                    }
                }
                else if (dir2)
                {
                    if (File.Exists(newMapPath) && File.Exists(volcano1FilePath2))
                    {
                        MoveFile(newMapPath, volcano1FilePath2);
                    }
                }
            }
        }

        private void ReplaceDynamicColossus(ListViewItem item, string newMapPath)
        {
            if (item.SubItems[4].Text == "1")
            {
                if (dir1)
                {
                    if (File.Exists(newMapPath) && File.Exists(majinzo0FilePath))
                    {
                        MoveFile(newMapPath, majinzo0FilePath);
                    }
                }
                else if (dir2)
                {
                    if (File.Exists(newMapPath) && File.Exists(majinzo0FilePath2))
                    {
                        MoveFile(newMapPath, majinzo0FilePath2);
                    }
                }
            }
            else if (item.SubItems[4].Text == "2")
            {
                if (dir1)
                {
                    if (File.Exists(newMapPath) && File.Exists(majinzo1FilePath))
                    {
                        MoveFile(newMapPath, majinzo1FilePath);
                    }
                }
                else if (dir2)
                {
                    if (File.Exists(newMapPath) && File.Exists(majinzo1FilePath2))
                    {
                        MoveFile(newMapPath, majinzo1FilePath2);
                    }
                }
            }
        }

        private void ReplaceDynamicObservatory(ListViewItem item, string newMapPath)
        {
            if (item.SubItems[4].Text == "1")
            {
                if (dir1)
                {
                    if (File.Exists(newMapPath) && File.Exists(angel0FilePath))
                    {
                        MoveFile(newMapPath, angel0FilePath);
                    }
                }
                else if (dir2)
                {
                    if (File.Exists(newMapPath) && File.Exists(angel0FilePath2))
                    {
                        MoveFile(newMapPath, angel0FilePath2);
                    }
                }
            }
            else if (item.SubItems[4].Text == "2")
            {
                if (dir1)
                {
                    if (File.Exists(newMapPath) && File.Exists(angel1FilePath))
                    {
                        MoveFile(newMapPath, angel1FilePath);
                    }
                }
                else if (dir2)
                {
                    if (File.Exists(newMapPath) && File.Exists(angel1FilePath2))
                    {
                        MoveFile(newMapPath, angel1FilePath2);
                    }
                }
            }
            else if (item.SubItems[4].Text == "3")
            {
                if (dir1)
                {
                    if (File.Exists(newMapPath) && File.Exists(angel2FilePath))
                    {
                        MoveFile(newMapPath, angel2FilePath);
                    }
                }
                else if (dir2)
                {
                    if (File.Exists(newMapPath) && File.Exists(angel2FilePath2))
                    {
                        MoveFile(newMapPath, angel2FilePath2);
                    }
                }
            }
            else if (item.SubItems[4].Text == "4")
            {
                if (dir1)
                {
                    if (File.Exists(newMapPath) && File.Exists(angel3FilePath))
                    {
                        MoveFile(newMapPath, angel3FilePath);
                    }
                }
                else if (dir2)
                {
                    if (File.Exists(newMapPath) && File.Exists(angel3FilePath2))
                    {
                        MoveFile(newMapPath, angel3FilePath2);
                    }
                }
            }
        }

        private void ReplaceCastleTrodain(string newMapPath)
        {
            if (dir1)
            {
                if (File.Exists(newMapPath) && File.Exists(torodeenFilePath))
                {
                    MoveFile(newMapPath, torodeenFilePath);
                }
            }
            else if (dir2)
            {
                if (File.Exists(newMapPath) && File.Exists(torodeenFilePath2))
                {
                    MoveFile(newMapPath, torodeenFilePath2);
                }
            }
        }

        private void ReplaceStaticObservatory(string newMapPath)
        {
            if (dir1)
            {
                if (File.Exists(newMapPath) && File.Exists(angel0FilePath))
                {
                    MoveFile(newMapPath, angel0FilePath);
                }
            }
            else if (dir2)
            {
                if (File.Exists(newMapPath) && File.Exists(angel0FilePath2))
                {
                    MoveFile(newMapPath, angel0FilePath2);
                }
            }
        }

        private void ReplaceGhostShip(string newMapPath)
        {
            if (dir1)
            {
                if (File.Exists(newMapPath) && File.Exists(yuureisenFilePath))
                {
                    MoveFile(newMapPath, yuureisenFilePath);
                }
            }
            else if (dir2)
            {
                if (File.Exists(newMapPath) && File.Exists(yuureisenFilePath2))
                {
                    MoveFile(newMapPath, yuureisenFilePath2);
                }
            }
        }

        private void ReplaceSlimenia(string newMapPath)
        {
            if (dir1)
            {
                if (File.Exists(newMapPath) && File.Exists(slabbakatouFilePath))
                {
                    MoveFile(newMapPath, slabbakatouFilePath);
                }
            }
            else if (dir2)
            {
                if (File.Exists(newMapPath) && File.Exists(slabbakatouFilePath2))
                {
                    MoveFile(newMapPath, slabbakatouFilePath2);
                }
            }
        }

        private void ReplaceStaticMtMagmageddon(string newMapPath)
        {
            if (dir1)
            {
                if (File.Exists(newMapPath) && File.Exists(volcano0FilePath))
                {
                    MoveFile(newMapPath, volcano0FilePath);
                }
            }
            else if (dir2)
            {
                if (File.Exists(newMapPath) && File.Exists(volcano0FilePath2))
                {
                    MoveFile(newMapPath, volcano0FilePath2);
                }
            }
        }

        private void ReplaceRobbinHoodRuins(string newMapPath)
        {
            if (dir1)
            {
                if (File.Exists(newMapPath) && File.Exists(kandataFilePath))
                {
                    MoveFile(newMapPath, kandataFilePath);
                }
            }
            else if (dir2)
            {
                if (File.Exists(newMapPath) && File.Exists(kandataFilePath2))
                {
                    MoveFile(newMapPath, kandataFilePath2);
                }
            }
        }

        private void ReplacePeachsCastle(string newMapPath)
        {
            if (dir1)
            {
                if (File.Exists(newMapPath) && File.Exists(hidariFilePath))
                {
                    MoveFile(newMapPath, hidariFilePath);
                }
            }
            else if (dir2)
            {
                if (File.Exists(newMapPath) && File.Exists(hidariFilePath2))
                {
                    MoveFile(newMapPath, hidariFilePath2);
                }
            }
        }

        private void ReplaceDelfinoPlaza(string newMapPath)
        {
            if (dir1)
            {
                if (File.Exists(newMapPath) && File.Exists(dolpicFilePath))
                {
                    MoveFile(newMapPath, dolpicFilePath);
                }
            }
            else if (dir2)
            {
                if (File.Exists(newMapPath) && File.Exists(dolpicFilePath2))
                {
                    MoveFile(newMapPath, dolpicFilePath2);
                }
            }
        }

        private void ReplaceYoshisIsland(string newMapPath)
        {
            if (dir1)
            {
                if (File.Exists(newMapPath) && File.Exists(yosshiFilePath))
                {
                    MoveFile(newMapPath, yosshiFilePath);
                }
            }
            else if (dir2)
            {
                if (File.Exists(newMapPath) && File.Exists(yosshiFilePath2))
                {
                    MoveFile(newMapPath, yosshiFilePath2);
                }
            }
        }

        private void ReplaceMarioCircuit(string newMapPath)
        {
            if (dir1)
            {
                if (File.Exists(newMapPath) && File.Exists(circuitFilePath))
                {
                    MoveFile(newMapPath, circuitFilePath);
                }
            }
            else if (dir2)
            {
                if (File.Exists(newMapPath) && File.Exists(circuitFilePath2))
                {
                    MoveFile(newMapPath, circuitFilePath2);
                }
            }
        }

        private void ReplaceStarshipMario(string newMapPath)
        {
            if (dir1)
            {
                if (File.Exists(newMapPath) && File.Exists(mooncityFilePath))
                {
                    MoveFile(newMapPath, mooncityFilePath);
                }
            }
            else if (dir2)
            {
                if (File.Exists(newMapPath) && File.Exists(mooncityFilePath2))
                {
                    MoveFile(newMapPath, mooncityFilePath2);
                }
            }
        }

        private void ReplaceMarioStadium(string newMapPath)
        {
            if (dir1)
            {
                if (File.Exists(newMapPath) && File.Exists(stadiumFilePath))
                {
                    MoveFile(newMapPath, stadiumFilePath);
                }
            }
            else if (dir2)
            {
                if (File.Exists(newMapPath) && File.Exists(stadiumFilePath2))
                {
                    MoveFile(newMapPath, stadiumFilePath2);
                }
            }
        }

        private void ReplaceAlltradesAbbey(string newMapPath)
        {
            if (dir1)
            {
                if (File.Exists(newMapPath) && File.Exists(dahmaFilePath))
                {
                    MoveFile(newMapPath, dahmaFilePath);
                }
            }
            else if (dir2)
            {
                if (File.Exists(newMapPath) && File.Exists(dahmaFilePath2))
                {
                    MoveFile(newMapPath, dahmaFilePath2);
                }
            }
        }

        private void ReplaceColossus(string newMapPath)
        {
            if (dir1)
            {
                if (File.Exists(newMapPath) && File.Exists(majinzo0FilePath))
                {
                    MoveFile(newMapPath, majinzo0FilePath);
                }
            }
            else if (dir2)
            {
                if (File.Exists(newMapPath) && File.Exists(majinzo0FilePath2))
                {
                    MoveFile(newMapPath, majinzo0FilePath2);
                }
            }
        }

        private void ReplaceGoodEggGalaxy(string newMapPath)
        {
            if (dir1)
            {
                if (File.Exists(newMapPath) && File.Exists(colonyFilePath))
                {
                    MoveFile(newMapPath, colonyFilePath);
                }
            }
            else if (dir2)
            {
                if (File.Exists(newMapPath) && File.Exists(colonyFilePath2))
                {
                    MoveFile(newMapPath, colonyFilePath2);
                }
            }
        }

        private void ReplaceBowsersCastle(string newMapPath)
        {
            if (dir1)
            {
                if (File.Exists(newMapPath) && File.Exists(koopaFilePath))
                {
                    MoveFile(newMapPath, koopaFilePath);
                }
            }
            else if (dir2)
            {
                if (File.Exists(newMapPath) && File.Exists(koopaFilePath2))
                {
                    MoveFile(newMapPath, koopaFilePath2);
                }
            }
        }

        private void ReplaceSMB(string newMapPath)
        {
            if (dir1)
            {
                if (File.Exists(newMapPath) && File.Exists(marioFilePath))
                {
                    MoveFile(newMapPath, marioFilePath);
                }
            }
            else if (dir2)
            {
                if (File.Exists(newMapPath) && File.Exists(marioFilePath2))
                {
                    MoveFile(newMapPath, marioFilePath2);
                }
            }
        }

        private void ReplaceAlefgard(string newMapPath)
        {
            if (dir1)
            {
                if (File.Exists(newMapPath) && File.Exists(alephgardFilePath))
                {
                    MoveFile(newMapPath, alephgardFilePath);
                }
            }
            else if (dir2)
            {
                if (File.Exists(newMapPath) && File.Exists(alephgardFilePath2))
                {
                    MoveFile(newMapPath, alephgardFilePath2);
                }
            }
        }

        private void RemoveMusic()
        {
            if (removeIntroMenuAndMapBgmToolStripMenuItem.Checked)
            {
                
                if (File.Exists(musiclessConfigFile))
                {
                    if (dir1)
                    {
                        MoveFile(musiclessConfigFile, soundConfig1);
                        MoveFile(musiclessConfigFile, soundConfig2);
                        MoveFile(musiclessConfigFile, soundConfig3);
                    }
                    else if (dir2)
                    {
                        MoveFile(musiclessConfigFile, soundConfig1_dir2);
                        MoveFile(musiclessConfigFile, soundConfig2_dir2);
                        MoveFile(musiclessConfigFile, soundConfig3_dir2);
                    }
                }
            }
        }

        private void ASMHacks()
        {
            if (deflaktorsASMHacksToolStripMenuItem.Checked)
            {
                if (dir1)
                {
                    if (File.Exists(originalBootDol) && File.Exists(DeflaktorASMBootDol))
                    {
                        MoveFile(DeflaktorASMBootDol, originalBootDol);
                    }

                    if (patchToWiimmfiToolStripMenuItem.Checked)
                    {
                        if (File.Exists(originalUIMessageDE) && File.Exists(DeflaktorASMUIMessageDEPlusWiimmfi))
                        {
                            MoveFile(DeflaktorASMUIMessageDEPlusWiimmfi, originalUIMessageDE);
                        }

                        if (File.Exists(originalUIMessageEN) && File.Exists(DeflaktorASMUIMessageENPlusWiimmfi))
                        {
                            MoveFile(DeflaktorASMUIMessageENPlusWiimmfi, originalUIMessageEN);
                        }
                        if (File.Exists(originalUIMessageUK) && File.Exists(DeflaktorASMUIMessageUKPlusWiimmfi))
                        {
                            MoveFile(DeflaktorASMUIMessageUKPlusWiimmfi, originalUIMessageUK);
                        }
                    }
                    else
                    {
                        if (File.Exists(originalUIMessageDE) && File.Exists(DeflaktorASMUIMessageDE))
                        {
                            MoveFile(DeflaktorASMUIMessageDE, originalUIMessageDE);
                        }

                        if (File.Exists(originalUIMessageEN) && File.Exists(DeflaktorASMUIMessageEN))
                        {
                            MoveFile(DeflaktorASMUIMessageEN, originalUIMessageEN);
                        }
                        if (File.Exists(originalUIMessageUK) && File.Exists(DeflaktorASMUIMessageUK))
                        {
                            MoveFile(DeflaktorASMUIMessageUK, originalUIMessageUK);
                        }
                    }
                        
                }
                else if (dir2)
                {
                    if (File.Exists(originalBootDol2) && File.Exists(DeflaktorASMBootDol))
                    {
                        MoveFile(DeflaktorASMBootDol, originalBootDol2);
                    }

                    if (patchToWiimmfiToolStripMenuItem.Checked)
                    {
                        if (File.Exists(originalUIMessageDE2) && File.Exists(DeflaktorASMUIMessageDEPlusWiimmfi))
                        {
                            MoveFile(DeflaktorASMUIMessageDEPlusWiimmfi, originalUIMessageDE2);
                        }

                        if (File.Exists(originalUIMessageEN2) && File.Exists(DeflaktorASMUIMessageENPlusWiimmfi))
                        {
                            MoveFile(DeflaktorASMUIMessageENPlusWiimmfi, originalUIMessageEN2);
                        }
                        if (File.Exists(originalUIMessageUK) && File.Exists(DeflaktorASMUIMessageUKPlusWiimmfi))
                        {
                            MoveFile(DeflaktorASMUIMessageUKPlusWiimmfi, originalUIMessageUK2);
                        }
                    }
                    else
                    {
                        if (File.Exists(originalUIMessageDE2) && File.Exists(DeflaktorASMUIMessageDE))
                        {
                            MoveFile(DeflaktorASMUIMessageDE, originalUIMessageDE2);
                        }

                        if (File.Exists(originalUIMessageEN2) && File.Exists(DeflaktorASMUIMessageEN))
                        {
                            MoveFile(DeflaktorASMUIMessageEN, originalUIMessageEN2);
                        }
                        if (File.Exists(originalUIMessageUK2) && File.Exists(DeflaktorASMUIMessageUK))
                        {
                            MoveFile(DeflaktorASMUIMessageUK, originalUIMessageUK2);
                        }
                    }
                }
            }
        }

        private void UpdateUIForWiimmfi() //updates the UI when Wiimmfi is being patched in, and the ASM hacks aren't. (They do their own UI stuff).
        {
            if(patchToWiimmfiToolStripMenuItem.Checked && !deflaktorsASMHacksToolStripMenuItem.Checked)
            {
                if (dir1)
                {
                    if (File.Exists(USWiimmfiUIUpdateFile) && File.Exists(originalUIMessageEN))
                    {
                        MoveFile(USWiimmfiUIUpdateFile, originalUIMessageEN);
                    }
                    if (File.Exists(UKWiimmfiUIUpdateFile) && File.Exists(originalUIMessageUK))
                    {
                        MoveFile(UKWiimmfiUIUpdateFile, originalUIMessageUK);
                    }
                    if (File.Exists(DEWiimmfiUIUpdateFile) && File.Exists(originalUIMessageDE))
                    {
                        MoveFile(DEWiimmfiUIUpdateFile, originalUIMessageDE);
                    }
                }
                else if (dir2)
                {
                    if (File.Exists(USWiimmfiUIUpdateFile) && File.Exists(originalUIMessageEN2))
                    {
                        MoveFile(USWiimmfiUIUpdateFile, originalUIMessageEN2);
                    }
                    if (File.Exists(UKWiimmfiUIUpdateFile) && File.Exists(originalUIMessageUK2))
                    {
                        MoveFile(UKWiimmfiUIUpdateFile, originalUIMessageUK2);
                    }
                    if (File.Exists(DEWiimmfiUIUpdateFile) && File.Exists(originalUIMessageDE2))
                    {
                        MoveFile(DEWiimmfiUIUpdateFile, originalUIMessageDE2);
                    }
                }
            }
        }

        private static void UpdateProgressWindow(ProgressBar instance, string status, int amount)
        {
            instance.SetProgressBarLabel(status);
            instance.SetProgressBarValue(amount);
        }

        private void CompileTheDisc()
        {
            string compileDiscBatFilePath = Path.Combine(Directory.GetCurrentDirectory(), CompileDiscFileName);
            string compileDiscWithWiimmfiBatFilePath = Path.Combine(Directory.GetCurrentDirectory(), CompileDiscWithWiimmfiFileName);
            
            if (File.Exists(compileDiscBatFilePath) || File.Exists(compileDiscWithWiimmfiBatFilePath))
            {
                if (patchToWiimmfiToolStripMenuItem.Checked)
                {
                    ProcessStartInfo psi3 = new ProcessStartInfo(compileDiscWithWiimmfiBatFilePath, Arguments);
                    psi3.Arguments = "\"" + setOutputPathLabel.Text + "\"";
                    psi3.CreateNoWindow = true;
                    psi3.UseShellExecute = false;
                    psi3.WorkingDirectory = Directory.GetCurrentDirectory();
                    var p3 = Process.Start(psi3);
                    p3.WaitForExit();
                }
                else if (!patchToWiimmfiToolStripMenuItem.Checked)
                {
                    ProcessStartInfo psi3 = new ProcessStartInfo(compileDiscBatFilePath, Arguments);
                    psi3.Arguments = "\"" + setOutputPathLabel.Text + "\"";
                    psi3.CreateNoWindow = true;
                    psi3.UseShellExecute = false;
                    psi3.WorkingDirectory = Directory.GetCurrentDirectory();
                    var p3 = Process.Start(psi3);
                    p3.WaitForExit();
                }
            }
        }

        private void MoveFile(string newFilePath, string fileToReplacePath)
        {
            File.Delete(fileToReplacePath);
            File.Copy(newFilePath, fileToReplacePath, true);
        }

        private void openMapButton_Click(object sender, EventArgs e)
        {
            if (addMapButton.Text == "Add map") //if one item is selected
            {
                dynMapLabel1.Text = "";
                dynMap2Label.Text = "";
                dynMap3Label.Text = "";
                dynMap4Label.Text = "";

                addMapsDialog.Multiselect = true;
                addMapsDialog.Filter = "Map files (*.frb)|*.frb|All files (*.*)|*.*";

                if (addMapsDialog.ShowDialog() == DialogResult.OK)
                {
                    string[] arr = new string[5];
                    System.IO.StreamReader sr = new System.IO.StreamReader(addMapsDialog.FileName);

                    for (int i = 0; i < addMapsDialog.SafeFileNames.Count(); i++)
                    {
                        ListViewItem newItem;

                        arr[0] = addMapsDialog.SafeFileNames[i];
                        arr[1] = "None";
                        arr[2] = addMapsDialog.FileNames[i];
                        arr[3] = "false";
                        arr[4] = "0";

                        newItem = new ListViewItem(arr);

                        listOfMapsToPatchIn.Items.Add(newItem);
                    }

                    //MessageBox.Show(sr.ReadToEnd());
                    sr.Close();
                }
            }
            else if (addMapButton.Text == "Set Dynamic") //if two or more maps are selected, the button changes!
            {

                // create temp font from the item, using BOLD
                if(listOfMapsToPatchIn.SelectedItems.Count == 2)
                {
                    dynamicMapPanel.Visible = true;
                    dynMapLabel1.Visible = true;
                    dynMap2Label.Visible = true;
                    dynMap3Label.Visible = false;
                    dynMap4Label.Visible = false;

                    map1OrderDropdown.Visible = true;
                    map2OrderDropdown.Visible = true;
                    map3OrderDropdown.Visible = false;
                    map4OrderDropdown.Visible = false;

                    map1OrderDropdown.SelectedItem = null;
                    map2OrderDropdown.SelectedItem = null;

                    map1OrderDropdown.Items.Clear();
                    map2OrderDropdown.Items.Clear();

                    dynMapLabel1.Text = "";
                    dynMap2Label.Text = "";

                    map1OrderDropdown.Items.Add("1");
                    map1OrderDropdown.Items.Add("2");

                    map2OrderDropdown.Items.Add("1");
                    map2OrderDropdown.Items.Add("2");


                    for (int i = 0; i < listOfMapsToPatchIn.SelectedItems.Count; i++)
                    {
                        if(i == 0)
                        {
                            dynMapLabel1.Text = listOfMapsToPatchIn.SelectedItems[i].SubItems[0].Text;
                        }
                        else if(i == 1)
                        {
                            dynMap2Label.Text = listOfMapsToPatchIn.SelectedItems[i].SubItems[0].Text;
                        }
                        
                    }
                    
                }
                else if(listOfMapsToPatchIn.SelectedItems.Count == 3)
                {
                    dynMapLabel1.Visible = true;
                    dynMap2Label.Visible = true;
                    dynMap3Label.Visible = true;
                    dynMap4Label.Visible = false;

                    map1OrderDropdown.Visible = true;
                    map2OrderDropdown.Visible = true;
                    map3OrderDropdown.Visible = true;
                    map4OrderDropdown.Visible = false;

                    dynamicMapPanel.Visible = true;

                    dynMapLabel1.Text = "";
                    dynMap2Label.Text = "";
                    dynMap3Label.Text = "";

                    map1OrderDropdown.SelectedItem = null;
                    map2OrderDropdown.SelectedItem = null;
                    map3OrderDropdown.SelectedItem = null;

                    map1OrderDropdown.Items.Clear();
                    map2OrderDropdown.Items.Clear();
                    map3OrderDropdown.Items.Clear();

                    map1OrderDropdown.Items.Add("1");
                    map1OrderDropdown.Items.Add("2");
                    map1OrderDropdown.Items.Add("3");

                    map2OrderDropdown.Items.Add("1");
                    map2OrderDropdown.Items.Add("2");
                    map2OrderDropdown.Items.Add("3");

                    map3OrderDropdown.Items.Add("1");
                    map3OrderDropdown.Items.Add("2");
                    map3OrderDropdown.Items.Add("3");

                    for (int i = 0; i < listOfMapsToPatchIn.SelectedItems.Count; i++)
                    {
                        if (i == 0)
                        {
                            dynMapLabel1.Text = listOfMapsToPatchIn.SelectedItems[i].SubItems[0].Text;
                        }
                        else if (i == 1)
                        {
                            dynMap2Label.Text = listOfMapsToPatchIn.SelectedItems[i].SubItems[0].Text;
                        }
                        else if (i == 2)
                        {
                            dynMap3Label.Text = listOfMapsToPatchIn.SelectedItems[i].SubItems[0].Text;
                        }

                    }
                }
                else if (listOfMapsToPatchIn.SelectedItems.Count == 4)
                {
                    dynamicMapPanel.Visible = true;
                    dynMapLabel1.Visible = true;
                    dynMap2Label.Visible = true;
                    dynMap3Label.Visible = true;
                    dynMap4Label.Visible = true;
                    map1OrderDropdown.Visible = true;
                    map2OrderDropdown.Visible = true;
                    map3OrderDropdown.Visible = true;
                    map4OrderDropdown.Visible = true;

                    mtmagButton.Enabled = false;
                    colossusButton.Enabled = false;

                    map1OrderDropdown.SelectedItem = null;
                    map2OrderDropdown.SelectedItem = null;
                    map3OrderDropdown.SelectedItem = null;
                    map4OrderDropdown.SelectedItem = null;

                    map1OrderDropdown.Items.Clear();
                    map2OrderDropdown.Items.Clear();
                    map3OrderDropdown.Items.Clear();
                    map4OrderDropdown.Items.Clear();

                    dynMapLabel1.Text = "";
                    dynMap2Label.Text = "";
                    dynMap3Label.Text = "";
                    dynMap4Label.Text = "";

                    map1OrderDropdown.Items.Add("1");
                    map1OrderDropdown.Items.Add("2");
                    map1OrderDropdown.Items.Add("3");
                    map1OrderDropdown.Items.Add("4");

                    map2OrderDropdown.Items.Add("1");
                    map2OrderDropdown.Items.Add("2");
                    map2OrderDropdown.Items.Add("3");
                    map2OrderDropdown.Items.Add("4");

                    map3OrderDropdown.Items.Add("1");
                    map3OrderDropdown.Items.Add("2");
                    map3OrderDropdown.Items.Add("3");
                    map3OrderDropdown.Items.Add("4");

                    map4OrderDropdown.Items.Add("1");
                    map4OrderDropdown.Items.Add("2");
                    map4OrderDropdown.Items.Add("3");
                    map4OrderDropdown.Items.Add("4");

                    for (int i = 0; i < listOfMapsToPatchIn.SelectedItems.Count; i++)
                    {
                        if (i == 0)
                        {
                            dynMapLabel1.Text = listOfMapsToPatchIn.SelectedItems[i].SubItems[0].Text;
                        }
                        else if (i == 1)
                        {
                            dynMap2Label.Text = listOfMapsToPatchIn.SelectedItems[i].SubItems[0].Text;
                        }
                        else if (i == 2)
                        {
                            dynMap3Label.Text = listOfMapsToPatchIn.SelectedItems[i].SubItems[0].Text;
                        }
                        else if (i == 3)
                        {
                            dynMap4Label.Text = listOfMapsToPatchIn.SelectedItems[i].SubItems[0].Text;
                        }

                    }
                }
                else if (listOfMapsToPatchIn.SelectedItems.Count > 4)
                {
                    System.Windows.Forms.MessageBox.Show("No map supports greater than 4 slots.");
                }
                foreach (ListViewItem item in listOfMapsToPatchIn.SelectedItems)
                {
                    item.SubItems[3].Text = "true";
                }         
                
            }
            
        }

        private void addMapDialog_FileOk(object sender, CancelEventArgs e)
        {

        }

        private void removeMapButton_Click(object sender, EventArgs e)
        {
            ListView.SelectedIndexCollection indexes = listOfMapsToPatchIn.SelectedIndices;
            for(int i = (listOfMapsToPatchIn.Items.Count - 1); i >= 0; i--)
            {
                if (indexes.Contains(i))
                {
                    listOfMapsToPatchIn.Items.RemoveAt(i);
                }
            }
            whichMapShouldWeReplaceLabel.Text = "Which map should we replace?";
            noneButton.Checked = true;
            addMapButton.Text = "Add map";
            SetAllMapButtonsActive();
            foreach (ListViewItem item in listOfMapsToPatchIn.Items)
            {
                string m = item.SubItems[1].Text;
                CheckAndDisableMapButton(m);
            }
        }

        private void clearListButton_Click(object sender, EventArgs e)
        {
            listOfMapsToPatchIn.Items.Clear();
            noneButton.Checked = true;
            whichMapShouldWeReplaceLabel.Text = "Which map should we replace?";
            SetAllMapButtonsActive();
            addMapButton.Text = "Add map";
        }

        private void removeIntroMenuAndMapBmgToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void listOfMapsToPatchIn_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listOfMapsToPatchIn.SelectedItems.Count == 1)
            {
                addMapButton.Text = "Add map";
                dynamicMapPanel.Visible = false;

                SetAllMapButtonsActive();

                ListView.SelectedListViewItemCollection allSelectedItems =
                this.listOfMapsToPatchIn.SelectedItems;

                string mapToPatch = "";
                foreach (ListViewItem item in allSelectedItems)
                {
                    mapToPatch = item.SubItems[1].Text;
                    whichMapShouldWeReplaceLabel.Text = "Which map should " + item.SubItems[0].Text + " replace?";
                }

                CheckMap(mapToPatch); //see which map we're on and check the correct radio button

                foreach (ListViewItem item in listOfMapsToPatchIn.Items)
                {
                    string m = item.SubItems[1].Text;
                    CheckAndDisableMapButton(m);
                }

            }
            else if (listOfMapsToPatchIn.SelectedItems.Count == 2)
            {
                addMapButton.Text = "Set Dynamic";
            }
            else if (listOfMapsToPatchIn.SelectedItems.Count == 3)
            {
                addMapButton.Text = "Set Dynamic";
            }
            else if (listOfMapsToPatchIn.SelectedItems.Count == 4)
            {
                addMapButton.Text = "Set Dynamic";
            }
            else if (listOfMapsToPatchIn.SelectedItems.Count > 4)
            {
                addMapButton.Text = "Set Dynamic";
            }
            else //0 or fewer
            {

            }
        }

        private void CheckMap(string s)
        {
            switch (s)
            {
                case "Castle Trodain":
                    trodainButton.Checked = true;
                    return;
                case "The Observatory":
                    observatoryButton.Checked = true;
                    return;
                case "Ghost Ship":
                    ghostShipButton.Checked = true;
                    return;
                case "Slimenia":
                    slimeniaButton.Checked = true;
                    return;
                case "Mt. Magmageddon":
                    mtmagButton.Checked = true;
                    return;
                case "Robbin' Hood Ruins":
                    rhrButton.Checked = true;
                    return;
                case "Mario Stadium":
                    stadiumButton.Checked = true;
                    return;
                case "Starship Mario":
                    starshipButton.Checked = true;
                    return;
                case "Mario Circuit":
                    circuitButton.Checked = true;
                    return;
                case "Yoshi's Island":
                    yoshiButton.Checked = true;
                    return;
                case "Delfino Plaza":
                    delfinoButton.Checked = true;
                    return;
                case "Peach's Castle":
                    peachButton.Checked = true;
                    return;
                case "Alefgard":
                    alefgardButton.Checked = true;
                    return;
                case "Super Mario Bros.":
                    smbButton.Checked = true;
                    return;
                case "Bowser's Castle":
                    bowserButton.Checked = true;
                    return;
                case "Good Egg Galaxy":
                    gegButton.Checked = true;
                    return;
                case "Colossus":
                    colossusButton.Checked = true;
                    return;
                case "Alltrades Abbey":
                    alltradesButton.Checked = true;
                    return;
                case "*The Observatory":
                    dynTheObservatoryButton.Checked = true;
                    return;
                case "*Colossus":
                    dynTheColossusButton.Checked = true;
                    return;
                case "*Mt. Magmageddon":
                    dynMtMagButton.Checked = true;
                    return;
                case null:
                    noneButton.Checked = true;
                    return;
            }
        }

        private void CheckAndDisableMapButton(string s)
        {
            switch (s)
            {
                case "Castle Trodain":
                    trodainButton.Enabled = false;
                    return;
                case "The Observatory":
                    observatoryButton.Enabled = false;
                    dynTheObservatoryButton.Enabled = false;
                    return;
                case "*The Observatory":
                    observatoryButton.Enabled = false;
                    dynTheObservatoryButton.Enabled = false;
                    return;
                case "Ghost Ship":
                    ghostShipButton.Enabled = false;
                    return;
                case "Slimenia":
                    slimeniaButton.Enabled = false;
                    return;
                case "Mt. Magmageddon":
                    mtmagButton.Enabled = false;
                    dynMtMagButton.Enabled = false;
                    return;
                case "*Mt. Magmageddon":
                    mtmagButton.Enabled = false;
                    dynMtMagButton.Enabled = false;
                    return;
                case "Robbin' Hood Ruins":
                    rhrButton.Enabled = false;
                    return;
                case "Mario Stadium":
                    stadiumButton.Enabled = false;
                    return;
                case "Starship Mario":
                    starshipButton.Enabled = false;
                    return;
                case "Mario Circuit":
                    circuitButton.Enabled = false;
                    return;
                case "Yoshi's Island":
                    yoshiButton.Enabled = false;
                    return;
                case "Delfino Plaza":
                    delfinoButton.Enabled = false;
                    return;
                case "Peach's Castle":
                    peachButton.Enabled = false;
                    return;
                case "Alefgard":
                    alefgardButton.Enabled = false;
                    return;
                case "Super Mario Bros.":
                    smbButton.Enabled = false;
                    return;
                case "Bowser's Castle":
                    bowserButton.Enabled = false;
                    return;
                case "Good Egg Galaxy":
                    gegButton.Enabled = false;
                    return;
                case "Colossus":
                    colossusButton.Enabled = false;
                    dynTheColossusButton.Enabled = false;
                    return;
                case "*Colossus":
                    colossusButton.Enabled = false;
                    dynTheColossusButton.Enabled = false;
                    return;
                case "Alltrades Abbey":
                    alltradesButton.Enabled = false;
                    return;
                case "None":
                    return;
                case null:
                    return;
            }
        }

        private void SetAllMapButtonsActive()
        {
            trodainButton.Enabled = true;
            observatoryButton.Enabled = true;
            ghostShipButton.Enabled = true;
            slimeniaButton.Enabled = true;
            mtmagButton.Enabled = true;
            rhrButton.Enabled = true;
            stadiumButton.Enabled = true;
            starshipButton.Enabled = true;
            circuitButton.Enabled = true;
            yoshiButton.Enabled = true;
            delfinoButton.Enabled = true;
            peachButton.Enabled = true;
            alefgardButton.Enabled = true;
            smbButton.Enabled = true;
            bowserButton.Enabled = true;
            gegButton.Enabled = true;
            colossusButton.Enabled = true;
            alltradesButton.Enabled = true;
            dynTheColossusButton.Enabled = true;
            dynTheObservatoryButton.Enabled = true;
            dynMtMagButton.Enabled = true;
        }

        private void trodainButton_CheckedChanged(object sender, EventArgs e)
        {
            if (trodainButton.Checked)
            {
                ListView.SelectedListViewItemCollection allSelectedItems = this.listOfMapsToPatchIn.SelectedItems;
                if(allSelectedItems.Count != 0)
                {
                    foreach (ListViewItem item in allSelectedItems)
                    {
                        item.SubItems[1].Text = "Castle Trodain";
                    }
                }
            }
        }

        private void observatoryButton_CheckedChanged(object sender, EventArgs e)
        {
            if (observatoryButton.Checked)
            {
                ListView.SelectedListViewItemCollection allSelectedItems = this.listOfMapsToPatchIn.SelectedItems;
                if(allSelectedItems.Count != 0)
                {
                    foreach (ListViewItem item in allSelectedItems)
                    {
                        item.SubItems[1].Text = "The Observatory";
                    }
                }
            }
        }

        private void ghostShipButton_CheckedChanged(object sender, EventArgs e)
        {
            if (ghostShipButton.Checked)
            {
                ListView.SelectedListViewItemCollection allSelectedItems = this.listOfMapsToPatchIn.SelectedItems;
                if(allSelectedItems.Count != 0)
                {
                    foreach (ListViewItem item in allSelectedItems)
                    {
                        item.SubItems[1].Text = "Ghost Ship";
                    }
                }
            }
        }

        private void slimeniaButton_CheckedChanged(object sender, EventArgs e)
        {
            if (slimeniaButton.Checked)
            {
                ListView.SelectedListViewItemCollection allSelectedItems = this.listOfMapsToPatchIn.SelectedItems;
                if(allSelectedItems.Count != 0)
                {
                    foreach (ListViewItem item in allSelectedItems)
                    {
                        item.SubItems[1].Text = "Slimenia";
                    }
                }
            }
        }

        private void mtmagButton_CheckedChanged(object sender, EventArgs e)
        {
            if (mtmagButton.Checked)
            {
                ListView.SelectedListViewItemCollection allSelectedItems = this.listOfMapsToPatchIn.SelectedItems;
                if(allSelectedItems.Count != 0)
                {
                    foreach (ListViewItem item in allSelectedItems)
                    {
                        item.SubItems[1].Text = "Mt. Magmageddon";
                    }
                }
            }
        }
        private void robbinHoodRuinsButton_CheckedChanged(object sender, EventArgs e)
        {
            if (rhrButton.Checked)
            {
                ListView.SelectedListViewItemCollection allSelectedItems = this.listOfMapsToPatchIn.SelectedItems;
                if(allSelectedItems.Count != 0)
                {
                    foreach (ListViewItem item in allSelectedItems)
                    {
                        item.SubItems[1].Text = "Robbin' Hood Ruins";
                    }
                }
            }
        }

        private void stadiumButton_CheckedChanged(object sender, EventArgs e)
        {
            if (stadiumButton.Checked)
            {
                ListView.SelectedListViewItemCollection allSelectedItems = this.listOfMapsToPatchIn.SelectedItems;
                if (allSelectedItems.Count != 0)
                {
                    foreach (ListViewItem item in allSelectedItems)
                    {
                        item.SubItems[1].Text = "Mario Stadium";
                    }
                }
            }
        }

        private void starshipButton_CheckedChanged(object sender, EventArgs e)
        {
            if (starshipButton.Checked)
            {
                ListView.SelectedListViewItemCollection allSelectedItems = this.listOfMapsToPatchIn.SelectedItems;
                if (allSelectedItems.Count != 0)
                {
                    foreach (ListViewItem item in allSelectedItems)
                    {
                        item.SubItems[1].Text = "Starship Mario";
                    }
                }
            }
        }

        private void circuitButton_CheckedChanged(object sender, EventArgs e)
        {
            if (circuitButton.Checked)
            {
                ListView.SelectedListViewItemCollection allSelectedItems = this.listOfMapsToPatchIn.SelectedItems;
                if (allSelectedItems.Count != 0)
                {
                    foreach (ListViewItem item in allSelectedItems)
                    {
                        item.SubItems[1].Text = "Mario Circuit";
                    }
                }
            }
        }

        private void yoshiButton_CheckedChanged(object sender, EventArgs e)
        {
            if (yoshiButton.Checked)
            {
                ListView.SelectedListViewItemCollection allSelectedItems = this.listOfMapsToPatchIn.SelectedItems;
                if (allSelectedItems.Count != 0)
                {
                    foreach (ListViewItem item in allSelectedItems)
                    {
                        item.SubItems[1].Text = "Yoshi's Island";
                    }
                }
            }
        }

        private void delfinoButton_CheckedChanged(object sender, EventArgs e)
        {
            if (delfinoButton.Checked)
            {
                ListView.SelectedListViewItemCollection allSelectedItems = this.listOfMapsToPatchIn.SelectedItems;
                if (allSelectedItems.Count != 0)
                {
                    foreach (ListViewItem item in allSelectedItems)
                    {
                        item.SubItems[1].Text = "Delfino Plaza";
                    }
                }
            }
        }

        private void peachButton_CheckedChanged(object sender, EventArgs e)
        {
            if (peachButton.Checked)
            {
                ListView.SelectedListViewItemCollection allSelectedItems = this.listOfMapsToPatchIn.SelectedItems;
                if (allSelectedItems.Count != 0)
                {
                    foreach (ListViewItem item in allSelectedItems)
                    {
                        item.SubItems[1].Text = "Peach's Castle";
                    }
                }
            }
        }

        private void alefgardButton_CheckedChanged(object sender, EventArgs e)
        {
            if (alefgardButton.Checked)
            {
                ListView.SelectedListViewItemCollection allSelectedItems = this.listOfMapsToPatchIn.SelectedItems;
                if (allSelectedItems.Count != 0)
                {
                    foreach (ListViewItem item in allSelectedItems)
                    {
                        item.SubItems[1].Text = "Alefgard";
                    }
                }
            }
        }

        private void smbButton_CheckedChanged(object sender, EventArgs e)
        {
            if (smbButton.Checked)
            {
                ListView.SelectedListViewItemCollection allSelectedItems = this.listOfMapsToPatchIn.SelectedItems;
                if (allSelectedItems.Count != 0)
                {
                    foreach (ListViewItem item in allSelectedItems)
                    {
                        item.SubItems[1].Text = "Super Mario Bros.";
                    }
                }
            }
        }

        private void bowserButton_CheckedChanged(object sender, EventArgs e)
        {
            if (bowserButton.Checked)
            {
                ListView.SelectedListViewItemCollection allSelectedItems = this.listOfMapsToPatchIn.SelectedItems;
                if (allSelectedItems.Count != 0)
                {
                    foreach (ListViewItem item in allSelectedItems)
                    {
                        item.SubItems[1].Text = "Bowser's Castle";
                    }
                }
            }
        }

        private void gegButton_CheckedChanged(object sender, EventArgs e)
        {
            if (gegButton.Checked)
            {
                ListView.SelectedListViewItemCollection allSelectedItems = this.listOfMapsToPatchIn.SelectedItems;
                if (allSelectedItems.Count != 0)
                {
                    foreach (ListViewItem item in allSelectedItems)
                    {
                        item.SubItems[1].Text = "Good Egg Galaxy";
                    }
                }
            }
        }

        private void colossusButton_CheckedChanged(object sender, EventArgs e)
        {
            if (colossusButton.Checked)
            {
                ListView.SelectedListViewItemCollection allSelectedItems = this.listOfMapsToPatchIn.SelectedItems;
                if (allSelectedItems.Count != 0)
                {
                    foreach (ListViewItem item in allSelectedItems)
                    {
                        item.SubItems[1].Text = "Colossus";
                    }
                }
            }
        }

        private void alltradesButton_CheckedChanged(object sender, EventArgs e)
        {
            if (alltradesButton.Checked)
            {
                ListView.SelectedListViewItemCollection allSelectedItems = this.listOfMapsToPatchIn.SelectedItems;
                if (allSelectedItems.Count != 0)
                {
                    foreach (ListViewItem item in allSelectedItems)
                    {
                        item.SubItems[1].Text = "Alltrades Abbey";
                    }
                }
            }
        }

        private void noneButton_CheckedChanged(object sender, EventArgs e)
        {
            if (noneButton.Checked)
            {
                ListView.SelectedListViewItemCollection allSelectedItems = this.listOfMapsToPatchIn.SelectedItems;
                if (allSelectedItems.Count != 0)
                {
                    foreach (ListViewItem item in allSelectedItems)
                    {
                        item.SubItems[1].Text = "None";
                    }
                }
            }
        }

        private void aboutCustomStreetMapManagerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutPanel instance = new AboutPanel();
            instance.Show();
        }

        private void SaveFileDialog(object sender, EventArgs e) //set output location button
        {
            // Displays a SaveFileDialog so the user can save the Image
            // assigned to Button2.
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "ISO Image|*.iso";
            saveFileDialog1.Title = "Where should I save the patched ISO?";
            saveFileDialog1.FileName = "UpdatedISOFile";   

            if (saveFileDialog1.ShowDialog() == DialogResult.OK) 
            {
                setOutputPathLabel.Text = saveFileDialog1.FileName;
            }
            else
            {
                setOutputPathLabel.Text = "None";
            }     
        }

        private void OpenFileDialog(object sender, EventArgs e)
        {
            // Displays a SaveFileDialog so the user can save the Image
            // assigned to Button2.
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "ISO Image|*.iso|WBFS file|*.wbfs"; //"JPeg Image|*.jpg|Bitmap Image|*.bmp|Gif Image|*.gif"
            openFileDialog1.Title = "Which ISO image or WBFS file should we patch?";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                setInputISOLocation.Text = openFileDialog1.FileName;
            }
            else
            {
                setInputISOLocation.Text = "None";
            }
        }

        private void deflaktorsASMHacksToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(deflaktorsASMHacksToolStripMenuItem.Checked == true)
            {
                MessageBox.Show("Please be aware that I'm not checking for this -- " +
                    "but you need a PAL ISO to enable Deflaktor's ASM Hacks, otherwise " +
                    "your game will crash on startup.");
            }         
        }

        private void dynTheObservatoryButton_CheckedChanged(object sender, EventArgs e)
        {
            if (dynTheObservatoryButton.Checked)
            {
                ListView.SelectedListViewItemCollection allSelectedItems = this.listOfMapsToPatchIn.SelectedItems;
                if (allSelectedItems.Count == 4)
                {
                    if (map1OrderDropdown.SelectedItem != null && map2OrderDropdown.SelectedItem != null && map3OrderDropdown.SelectedItem != null && map4OrderDropdown.SelectedItem != null
                        && map1OrderDropdown.SelectedItem.ToString() != map2OrderDropdown.SelectedItem.ToString() && map1OrderDropdown.SelectedItem.ToString() != map3OrderDropdown.SelectedItem.ToString() && map1OrderDropdown.SelectedItem.ToString() != map4OrderDropdown.SelectedItem.ToString()
                        && map2OrderDropdown.SelectedItem.ToString() != map1OrderDropdown.SelectedItem.ToString() && map2OrderDropdown.SelectedItem.ToString() != map3OrderDropdown.SelectedItem.ToString() && map2OrderDropdown.SelectedItem.ToString() != map4OrderDropdown.SelectedItem.ToString()
                        && map3OrderDropdown.SelectedItem.ToString() != map1OrderDropdown.SelectedItem.ToString() && map3OrderDropdown.SelectedItem.ToString() != map2OrderDropdown.SelectedItem.ToString() && map3OrderDropdown.SelectedItem.ToString() != map4OrderDropdown.SelectedItem.ToString()
                        && map4OrderDropdown.SelectedItem.ToString() != map1OrderDropdown.SelectedItem.ToString() & map4OrderDropdown.SelectedItem.ToString() != map2OrderDropdown.SelectedItem.ToString() && map4OrderDropdown.SelectedItem.ToString() != map3OrderDropdown.SelectedItem.ToString())
                    {
                        foreach (ListViewItem item in allSelectedItems)
                        {
                            item.SubItems[1].Text = "*The Observatory";
                            if (dynMapLabel1.Text == item.SubItems[0].Text)
                            {
                                item.SubItems[4].Text = map1OrderDropdown.SelectedItem.ToString();
                            }
                            else if (dynMap2Label.Text == item.SubItems[0].Text)
                            {
                                item.SubItems[4].Text = map2OrderDropdown.SelectedItem.ToString();
                            }
                            else if (dynMap3Label.Text == item.SubItems[0].Text)
                            {
                                item.SubItems[4].Text = map3OrderDropdown.SelectedItem.ToString();
                            }
                            else if (dynMap4Label.Text == item.SubItems[0].Text)
                            {
                                item.SubItems[4].Text = map4OrderDropdown.SelectedItem.ToString();
                            }
                        }
                        observatoryDynamicCount = 4;


                    }
                }
                else if (allSelectedItems.Count == 3)
                {
                    if (map1OrderDropdown.SelectedItem != null && map2OrderDropdown.SelectedItem != null && map3OrderDropdown.SelectedItem != null
                        && map1OrderDropdown.SelectedItem.ToString() != map2OrderDropdown.SelectedItem.ToString()
                        && map1OrderDropdown.SelectedItem.ToString() != map3OrderDropdown.SelectedItem.ToString()
                        && map2OrderDropdown.SelectedItem.ToString() != map3OrderDropdown.SelectedItem.ToString())
                    {
                        foreach (ListViewItem item in allSelectedItems)
                        {
                            item.SubItems[1].Text = "*The Observatory";
                            if (dynMapLabel1.Text == item.SubItems[0].Text)
                            {
                                item.SubItems[4].Text = map1OrderDropdown.SelectedItem.ToString();
                            }
                            else if (dynMap2Label.Text == item.SubItems[0].Text)
                            {
                                item.SubItems[4].Text = map2OrderDropdown.SelectedItem.ToString();
                            }
                            else if (dynMap3Label.Text == item.SubItems[0].Text)
                            {
                                item.SubItems[4].Text = map3OrderDropdown.SelectedItem.ToString();
                            }
                        }
                        observatoryDynamicCount = 3;
                    }
                    MessageBox.Show("Dynamic map order must be set for each map, and must be unique for each map.");
                    dynTheObservatoryButton.Checked = false;
                }
                else if (allSelectedItems.Count == 2)
                {
                    if (map1OrderDropdown.SelectedItem != null && map2OrderDropdown.SelectedItem != null
                    && map1OrderDropdown.SelectedItem.ToString() != map2OrderDropdown.SelectedItem.ToString())
                    {
                        //MessageBox.Show(map1OrderDropdown.SelectedItem.ToString() + "  -->  " + map2OrderDropdown.SelectedItem.ToString());
                        foreach (ListViewItem item in allSelectedItems)
                        {
                            item.SubItems[1].Text = "*The Observatory";
                            if (dynMapLabel1.Text == item.SubItems[0].Text)
                            {
                                item.SubItems[4].Text = map1OrderDropdown.SelectedItem.ToString();
                            }
                            else if (dynMap2Label.Text == item.SubItems[0].Text)
                            {
                                item.SubItems[4].Text = map2OrderDropdown.SelectedItem.ToString();
                            }
                        }
                        observatoryDynamicCount = 2;
                    }
                    else
                    {
                        MessageBox.Show("Dynamic map order must be set for each map, and must be unique for each map.");
                        dynTheObservatoryButton.Checked = false;
                    }
                }
                else
                {
                    MessageBox.Show("Dynamic map order must be set for each map, and must be unique for each map.");
                    dynTheObservatoryButton.Checked = false;
                }
            }
        }

        private void dynMtMagButton_CheckedChanged_1(object sender, EventArgs e)
        {
            if (dynMtMagButton.Checked)
            {
                ListView.SelectedListViewItemCollection allSelectedItems = this.listOfMapsToPatchIn.SelectedItems;

                if (allSelectedItems.Count == 2)
                {
                    if (map1OrderDropdown.SelectedItem != null && map2OrderDropdown.SelectedItem != null
                   && map1OrderDropdown.SelectedItem.ToString() != map2OrderDropdown.SelectedItem.ToString())
                    {
                        //MessageBox.Show(map1OrderDropdown.SelectedItem.ToString() + "  -->  " + map2OrderDropdown.SelectedItem.ToString());
                        foreach (ListViewItem item in allSelectedItems)
                        {
                            item.SubItems[1].Text = "*Mt. Magmageddon";
                            if (dynMapLabel1.Text == item.SubItems[0].Text)
                            {
                                item.SubItems[4].Text = map1OrderDropdown.SelectedItem.ToString();
                            }
                            else if (dynMap2Label.Text == item.SubItems[0].Text)
                            {
                                item.SubItems[4].Text = map2OrderDropdown.SelectedItem.ToString();
                            }
                        }
                        mtMagDynamicCount = 2;
                    }
                    else
                    {
                        MessageBox.Show("Dynamic map order must be set for each map, and must be unique for each map.");
                        dynMtMagButton.Checked = false;
                    }
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show("This map only supports a maximum of 2 map slots.");
                    dynMtMagButton.Checked = false;
                }
            }
        }

        private void dynTheColossusButton_CheckedChanged_1(object sender, EventArgs e)
        {
            if (dynTheColossusButton.Checked)
            {
                ListView.SelectedListViewItemCollection allSelectedItems = this.listOfMapsToPatchIn.SelectedItems;
                if (allSelectedItems.Count == 2)
                {
                    if (map1OrderDropdown.SelectedItem != null && map2OrderDropdown.SelectedItem != null
                       && map1OrderDropdown.SelectedItem.ToString() != map2OrderDropdown.SelectedItem.ToString())
                    {
                        //MessageBox.Show(map1OrderDropdown.SelectedItem.ToString() + "  -->  " + map2OrderDropdown.SelectedItem.ToString());
                        foreach (ListViewItem item in allSelectedItems)
                        {
                            item.SubItems[1].Text = "*Colossus";
                            if (dynMapLabel1.Text == item.SubItems[0].Text)
                            {
                                item.SubItems[4].Text = map1OrderDropdown.SelectedItem.ToString();
                            }
                            else if (dynMap2Label.Text == item.SubItems[0].Text)
                            {
                                item.SubItems[4].Text = map2OrderDropdown.SelectedItem.ToString();
                            }
                        }
                        colossusDynamicCount = 2;
                    }
                    else
                    {
                        MessageBox.Show("Dynamic map order must be set for each map, and must be unique for each map.");
                        dynTheColossusButton.Checked = false;
                    }
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show("This map only supports a maximum of 2 map slots.");
                    dynTheColossusButton.Checked = false;
                }
            }
        }
    }
}
