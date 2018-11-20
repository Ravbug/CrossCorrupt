using System;
using System.Collections.Generic;
using Eto.Forms;
using Eto.Drawing;
using Eto.Serialization.Xaml;

namespace CrossCorrupt
{
    public class MainForm : Form
    {

        private FileCorruptor fc;

        public MainForm()
        {
            XamlReader.Load(this);
            fc = new FileCorruptor();
        }

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
        /// Update the FileCorrputor's input file when the textbox is changed
        /// </summary>
        protected void SetInput(object sender, EventArgs e)
        {
            fc.SetInFile(((TextBox)sender).Text);
        }

        /// <summary>
        /// Update the FileCorrputor's output file when the textbox is changed
        /// </summary>
        protected void SetOutput(object sender, EventArgs e)
        {
            fc.SetOutFile(((TextBox)sender).Text);
        }

        protected void RunCorrupt(object sender, EventArgs e)
        {
            if (fc.GetInFile().Trim().Length > 0)
            {
                fc.DeleteCorrupt(5);
            }
            else
            {
                MessageBox.Show("Please enter an input file.");
            }
            
        }
    }
}
