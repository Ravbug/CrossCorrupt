using System;
using System.IO;
using System.Collections.Generic;
using Eto.Forms;
using Eto.Drawing;
using Eto.Serialization.Xaml;
using System.Diagnostics;

namespace CrossCorrupt
{
    public class MainForm : Form
    {
        //define UI elements up here, with identical name to one set in ID property (ETO doesn't auto generate these like WPF does)
        private TextBox InfileTxt;
        private TextBox OutfileTxt;
        private ProgressBar MainProg;
        private RadioButtonList SelectTypeList;
        private NumericStepper nBytesStepper;
        private NumericStepper startByteStepper;
        private NumericStepper endBytesStepper;
        private NumericStepper oldByteStepper;
        private NumericStepper newByteStepper;
        private CheckBox autoEndCheck;
        private DropDown CorruptTypeCombo;
        private GroupBox FolderCorruptBox;
        private GroupBox FolderScrambleBox;
        private CheckBox EnableFolderScrambleChck;
        private CheckBox EnableSubfolderScramble;
        private TextBox FileTypesTxt;
        private RadioButtonList folderCorruptSelect;
        private Button RunCorruptBtn;
        private CheckBox FolderScrambleInvertChk;
        private TextArea FolderScrambleTypesTxt;

        private CheckBox AutoChangeCorruptEveryChck;
        private CheckBox AutoChangeStartChck;
        private CheckBox AutoOldByte;
        private CheckBox AutoNewByte;
        private NumericStepper AutoEveryMinStepper; private NumericStepper AutoEveryMaxStepper;
        private NumericStepper AutoStartMinStepper; private NumericStepper AutoStartMaxStepper;
        private NumericStepper AutoOldMinStepper; private NumericStepper AutoOldMaxStepper;
        private NumericStepper AutoNewMinStepper; private NumericStepper AutoNewMaxStepper;

        private TextBox FolderScrambleRoot;
        private TextArea ConsoleLog;

        private string[] sourceFiles;
        private bool running = false;
        private CorruptManager cm;
        private Random random;
      
            public MainForm()
        {
            XamlReader.Load(this);
            //initialize the output area
            Console.outputArea = ConsoleLog;

            SelectTypeList.SelectedIndex = 0;
            folderCorruptSelect.SelectedIndex = 0;
            random = new Random();
        }

        /// <summary>
        /// Called when the RunCorrupt button is clicked
        /// </summary>
        /// <param name="sender">Object that raised the event</param>
        /// <param name="e">Event arguments</param>
        protected void RunCorrupt(object sender, EventArgs e)
        {
            //cancel if already running
            if (running)
            {
                cm.CancelWorker();

                running = false;
                RunCorruptBtn.Text = "Run Corrupt";
                Console.Log("UI: User aborted corruption",Console.LogTypes.Error);

            }
            //otherwise setup and run
            else if (InfileTxt.Text.Trim().Length > 0 && OutfileTxt.Text.Trim().Length > 0)
            {
                Console.Log("UI: Initializing corruption backend",Console.LogTypes.Info);
                //randomize inputs if applicable
                randomizeInputs();

                CorruptManager.CorruptionType type = (CorruptManager.CorruptionType)CorruptTypeCombo.SelectedIndex;
                if (SelectTypeList.SelectedIndex == 0)
                {
                    cm = new CorruptManager(sourceFiles, OutfileTxt.Text, type, (long)startByteStepper.Value, (long)endBytesStepper.Value, (int)nBytesStepper.Value, (byte)oldByteStepper.Value, (byte)newByteStepper.Value);
                }
                else
                {
                    //make filetypes 
                    HashSet<string> filetypes = new HashSet<string>(FileTypesTxt.Text.Split(','));
                    if (FileTypesTxt.Text.Trim().Length == 0)
                    {
                        filetypes = null;
                    }
                    Console.Log("UI: Setting filetypes = " + FileTypesTxt.Text.Trim(), Console.LogTypes.Info);
                    //determine the output folder
                    var asArray = InfileTxt.Text.Split(Path.DirectorySeparatorChar);
                    var outfile = OutfileTxt.Text + Path.DirectorySeparatorChar + asArray[asArray.Length-1];
                    //fix any duplicating slashes (windows only)
                    outfile = outfile.Replace("\\\\", "\\");
                    cm = new CorruptManager(InfileTxt.Text, outfile, type, (long)startByteStepper.Value, (long)endBytesStepper.Value, (int)nBytesStepper.Value, (byte)oldByteStepper.Value, (byte)newByteStepper.Value,filetypes, folderCorruptSelect.SelectedIndex!=0);
                }
                running = true;

                //run the corruptmanager
                RunCorruptBtn.Text = "Stop";
                Console.Log("UI: Running corruption on background thread", Console.LogTypes.Info);
                cm.Run((double prog, System.IO.FileInfo f) =>
                {
                    //run UI things on the UI thread
                    Application.Instance.Invoke(() =>
                   {
                       MainProg.Value = (int)prog;
                       if (prog >= 100)
                      {
                           //TODO: create and run the FolderScrambler, if applicable
                           if ((bool)EnableFolderScrambleChck.Checked)
                           {
                               //run the folder scrambler here:
                               Console.Log("UI: Corruption task completed, starting folder scramble", Console.LogTypes.Info);
                               runFolderScramble(InfileTxt.Text, FolderScrambleRoot.Text, OutfileTxt.Text);
                           }
                           else
                           {
                               running = false;
                               Console.Log("UI: Corruption task completed, skipped folder scramble", Console.LogTypes.Info);
                               RunCorruptBtn.Text = "Run Corrupt";
                               MessageBox.Show("Corruption complete!","CrossCorrupt");
                           }
                       }
                   });
                });
            }
            else
            {
                Console.Log("UI: The form is invalid", Console.LogTypes.Warning);
                MessageBox.Show("Please enter an input file.","CrossCorrupt");
            }      
        }

        /// <summary>
        /// randomize the settings if required
        /// </summary>
        private void randomizeInputs()
        {
            if ((bool)AutoChangeCorruptEveryChck.Checked)
            {
                nBytesStepper.Value += random.Next((int)AutoEveryMinStepper.Value,(int)AutoEveryMaxStepper.Value);
            }
            if ((bool)AutoChangeStartChck.Checked)
            {
                startByteStepper.Value += random.Next((int)AutoStartMinStepper.Value, (int)AutoStartMaxStepper.Value);
            }
            if ((bool)AutoOldByte.Checked)
            {
                oldByteStepper.Value += random.Next((int)AutoOldMinStepper.Value, (int)AutoOldMaxStepper.Value);
            }
            if ((bool)AutoNewByte.Checked)
            {
                newByteStepper.Value += random.Next((int)AutoNewMinStepper.Value, (int)AutoNewMaxStepper.Value);
            }
        }


        /// <summary>
        /// Called when the Choose Output button is pressed
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event args</param>
        protected void ChooseOutputClicked(object sender, EventArgs e)
        {
            OutfileTxt.Text = PromptForFolder("Select where you want to write the corrupted file");
        }

        /// <summary>
        /// Prompts the user to select a file
        /// </summary>
        /// <param name="prompt">Text to use to prompt the user</param>
        /// <returns>File path selected, or null if nothing was selected</returns>
        private string[] PromptForFile(string prompt,bool multiSelect=false)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.MultiSelect = multiSelect;
            dialog.Title = prompt;
            if (dialog.ShowDialog(this) == DialogResult.Ok)
            {
                List<string> files = new List<string>();
                foreach (string file in dialog.Filenames)
                {
                    files.Add(file);
                }
                return files.ToArray();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Runs the folderscrambler on a background thread
        /// </summary>
        /// <param name="inputRoot">Path to the whole corruption task to corupt</param>
        /// <param name="scrambleRoot">Path to just the rootfolder of the folder scramble task</param>
        /// <param name="destinationRoot">Root path where the corrupt job will be written</param>
        private void runFolderScramble(string inputRoot, string scrambleRoot,string destinationRoot)
        {
            int index = scrambleRoot.IndexOf(inputRoot);
            if (index < 0)
            {
                MessageBox.Show("Folder Scramble directory is not a subfolder of the parent directory. Aborted.","CrossCorrupt");
                RunCorruptBtn.Text = "Run Corrupt";

                Console.Log("UI: Unable to scramble folder, subdirectory " + scrambleRoot + " is not inside " + inputRoot, Console.LogTypes.Error);
                return;
            }

            Console.Log("UI: Setting up folder scramble output directory ", Console.LogTypes.Info);

            //build the new destination folder by taking the common parts of the scrambleRoot (user picked) and the inputRoot
            var split = inputRoot.Split(Path.DirectorySeparatorChar);
            string newPath = destinationRoot + Path.DirectorySeparatorChar + split[split.Length-1] + Path.DirectorySeparatorChar + scrambleRoot.Substring(inputRoot.Length);
            //fix directoryseparatorchar duplicating characters
            newPath = newPath.Replace("//","/");
            newPath = newPath.Replace("\\\\", "\\");
            Console.Log("UI: Folder scramble output directory = " + newPath, Console.LogTypes.Info);

            //build filetypes hashset
            HashSet<string> fileTypes = new HashSet<string>(FolderScrambleTypesTxt.Text.Split(','));
            Console.Log("UI: FileTypes = " + FolderScrambleTypesTxt.Text + "; scrambling " + ((bool)EnableSubfolderScramble.Checked? "these types only":"all except these types"), Console.LogTypes.Info);

            FolderScrambler fs = new FolderScrambler(newPath,(bool)FolderScrambleInvertChk.Checked,fileTypes,(bool)EnableSubfolderScramble.Checked);
            fs.ScrambleNames((double prog) =>
            {
                //progress updates go here
                //run UI things on the UI thread
                Application.Instance.Invoke(() =>
                {
                    MainProg.Value = (int)prog;
                    if (prog >= 100)
                    {   
                         
                        RunCorruptBtn.Text = "Run Corrupt";
                        running = false;
                        Console.Log("UI: Folder Scramble completed!",Console.LogTypes.Info);
                        MessageBox.Show("Corruption complete!","CrossCorrupt");               
                    }
                });
            });
        }

        /// <summary>
        /// Prompts the user to select a folder
        /// </summary>
        /// <param name="prompt">Text to use to prompt the user</param>
        /// <returns>Folder path selected, or null if nothing was selected</returns>
        private string PromptForFolder(string prompt)
        {
            SelectFolderDialog dialog = new SelectFolderDialog();
            dialog.Title = prompt;
            if (dialog.ShowDialog(this) == DialogResult.Ok)
            {
                return dialog.Directory;
            }
            else
            {
                return null;
            }
        }

        //------------------------------------------------------ UI click handlers go below here ------------------------------------------------------

        //when the Auto End byte is clicked
        protected void handleAutoEndClick(object sender, EventArgs e)
        {
            if ((bool)autoEndCheck.Checked)
            {
                endBytesStepper.Value = -1;
            }
            else
            {
                endBytesStepper.Value = 100;
            }
            endBytesStepper.Enabled = !(bool)autoEndCheck.Checked;
        }

        /// <summary>
        /// Called when Choose Folder Scramble Root button is clicked
        /// </summary>
        /// <param name="sender">CPointer to caller</param>
        /// <param name="e">Event arguments</param>
        protected void chooseFolderScrambleFolder(object sender, EventArgs e)
        {
            string path = PromptForFolder("Select the folder to scramble items");
            if (path != null)
            {
                FolderScrambleRoot.Text = path;
            }
        }

        /// <summary>
        /// Called when the Choose Input button is pressed
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event args</param>
        protected void ChooseInputClicked(object sender, EventArgs e)
        {
            if (SelectTypeList.SelectedIndex == 0)
            {
                sourceFiles = PromptForFile("Select the file you want to corrupt", true);
                if (sourceFiles != null)
                {
                    string text = "";
                    text += sourceFiles[0];
                    if (sourceFiles.Length > 1)
                    {
                        text += " and " + (sourceFiles.Length - 1) + " more";
                    }
                    InfileTxt.Text = text;
                }
            }
            else
            {
                InfileTxt.Text = PromptForFolder("Select the folder to corrupt");
            }
        }

        /// <summary>
        /// Called when one of the auto changers is clicked
        /// </summary>
        /// <param name="sender">Object that fired the event</param>
        /// <param name="e">EventArgs</param>
        protected void AutoChangers(object sender, EventArgs e)
        {
            CheckBox csender = (CheckBox)sender;
            if (csender == AutoChangeCorruptEveryChck)
            {
                AutoEveryMinStepper.Enabled=!(bool)csender.Checked;
                AutoEveryMaxStepper.Enabled = !(bool)csender.Checked;
            }
            else if (csender == AutoChangeStartChck)
            {
                AutoStartMinStepper.Enabled = !(bool)csender.Checked;
                AutoStartMaxStepper.Enabled = !(bool)csender.Checked;
            }
            else if (csender == AutoOldByte)
            {
                AutoOldMinStepper.Enabled = !(bool)csender.Checked;
                AutoOldMaxStepper.Enabled = !(bool)csender.Checked;
            }
            else if (csender == AutoNewByte)
            {
                AutoNewMinStepper.Enabled = !(bool)csender.Checked;
                AutoNewMaxStepper.Enabled = !(bool)csender.Checked;
            }
        }

        //when the selection mode changed
        protected void corruptTypeClicked(object sender, EventArgs e)
        {
            if (SelectTypeList.SelectedIndex ==1)
            {
                FolderCorruptBox.Enabled = true;
                if (EnableFolderScrambleChck.Checked == true)
                {
                    FolderScrambleBox.Enabled = true;
                }
            }
            else
            {
                FolderCorruptBox.Enabled = false;
                FolderScrambleBox.Enabled = false;
            }
            //prevent crashing due to user changing mode without changing files
            InfileTxt.Text = "";
        }

        //when folder scramble is enabled or disabled
        protected void enableFolderScrambleStateChanged(object sender,EventArgs e)
        {
            if (EnableFolderScrambleChck.Checked == true && SelectTypeList.SelectedIndex == 1)
            {
                FolderScrambleBox.Enabled = true;
            }
            else
            {
                FolderScrambleBox.Enabled = false;
            }
        }

        //but keep these
        protected void HandleAbout(object sender, EventArgs e)
        {
            new AboutDialog().ShowDialog(this);
        }

        protected void HandleQuit(object sender, EventArgs e)
        {
            Application.Instance.Quit();
        }

        protected void HandleCheckUpdate(object sender, EventArgs e)
        {
            Process.Start("https://github.com/Ravbug/CrossCorrupt/releases");
        }
    }
}
