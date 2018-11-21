using System;
using System.Collections.Generic;
using Eto.Forms;
using Eto.Drawing;
using Eto.Serialization.Xaml;

namespace CrossCorrupt
{
    public class MainForm : Form
    {
        //define UI elements up here, with identical name to one set in ID property (ETO doesn't auto generate these like WPF does)
        private TextBox InfileTxt;
        private TextBox OutfileTxt;
        private ProgressBar MainProg;

        public MainForm()
        {
            XamlReader.Load(this);
        }

        //some sample methods to delete later
        protected void HandleClickMe(object sender, EventArgs e)
        {
            MessageBox.Show("I was clicked!");
        }

        protected void HandleAbout(object sender, EventArgs e)
        {
            new AboutDialog().ShowDialog(this);
        }

        protected void HandleQuit(object sender, EventArgs e)
        {
            Application.Instance.Quit();
        }

        /// <summary>
        /// Called when the RunCorrupt button is clicked
        /// </summary>
        /// <param name="sender">Object that raised the event</param>
        /// <param name="e">Event arguments</param>
        protected void RunCorrupt(object sender, EventArgs e)
        {
            
            if (InfileTxt.Text.Trim().Length > 0 && OutfileTxt.Text.Trim().Length > 0)
            {
                CorruptManager cm = new CorruptManager( new string[] { InfileTxt.Text }, OutfileTxt.Text, CorruptManager.CorruptionType.Insert, 10, (byte)'A');
                cm.Run((double prog, System.IO.FileInfo f) =>
                {
                    Application.Instance.Invoke(() =>
                   {
                       MainProg.Value = (int)prog;
                       Console.WriteLine(prog + " " + f.Name);
                   });             
                });
            }
            else
            {
                MessageBox.Show("Please enter an input file.");
            }
            
        }

        /// <summary>
        /// Called when the Choose Input button is pressed
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event args</param>
        protected void ChooseInputClicked(object sender, EventArgs e)
        {
            InfileTxt.Text = PromptForFile("Select the file you want to corrupt");
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
        private string PromptForFile(string prompt)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.MultiSelect = false;
            dialog.Title = prompt;
            if (dialog.ShowDialog(this) == DialogResult.Ok)
            {
                return dialog.FileName;
            }
            else
            {
                return null;
            }
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
    }
}
