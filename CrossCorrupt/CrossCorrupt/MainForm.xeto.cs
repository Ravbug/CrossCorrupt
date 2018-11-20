using System;
using System.Collections.Generic;
using Eto.Forms;
using Eto.Drawing;
using Eto.Serialization.Xaml;

namespace CrossCorrupt
{
    public class MainForm : Form
    {
        public MainForm()
        {
            XamlReader.Load(this);

            CorruptManager c = new CorruptManager("C:\\Users\\Main\\Downloads", "C:\\Users\\Main\\Documents");

            FileCorruptor fc = new FileCorruptor("C:\\Users\\Main\\Desktop\\test.txt", "C:\\Users\\Main\\Desktop\\test1.txt");
            fc.DeleteCorrupt(5);
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
    }
}
