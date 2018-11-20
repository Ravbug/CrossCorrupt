using System;
using Eto.Forms;
using Eto.Drawing;

//main file for program
namespace CrossCorrupt.Desktop
{
    //very nice
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            new Application(Eto.Platform.Detect).Run(new MainForm());
        }
    }
}