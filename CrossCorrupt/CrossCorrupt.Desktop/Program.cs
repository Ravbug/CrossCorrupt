using System;
using Eto.Forms;
using Eto.Drawing;

//main file for program
namespace CrossCorrupt.Desktop
{
    //very nice 1
    public partial class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            new Application(Eto.Platform.Detect).Run(new MainForm());
        }
    }
}