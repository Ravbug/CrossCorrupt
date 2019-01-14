using System;
using Eto.Forms;

namespace CrossCorrupt
{
    public class Console
    {
        public static TextArea outputArea;

        //used for logging headers
        public enum LogTypes{Info,Warning,Error,Debug}

        //in case we want to use a RichTextField with fancy formatting
        private static System.Collections.Generic.Dictionary<LogTypes, string> logFormatMap = new System.Collections.Generic.Dictionary<LogTypes, string>()
            {
                {LogTypes.Info,"Info"},
                {LogTypes.Warning,"Warning"},
                {LogTypes.Error,"ERROR"},
                {LogTypes.Debug,"Debug"}
            };

        public Console()
        {
        }

        public static void Log(string text,LogTypes type = LogTypes.Debug)
        {
            outputArea.Text += "[" + logFormatMap[type] + "] " + text + "\n"; 
        }
    }
}
