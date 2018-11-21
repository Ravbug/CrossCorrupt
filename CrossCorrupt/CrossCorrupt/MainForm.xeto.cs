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
                CorruptManager cm = new CorruptManager( InfileTxt.Text, OutfileTxt.Text, CorruptManager.CorruptionType.Insert, 10, (byte)'A');
                cm.Run();
            }
            else
            {
                MessageBox.Show("Please enter an input file.");
            }
            
        }
    }
}
