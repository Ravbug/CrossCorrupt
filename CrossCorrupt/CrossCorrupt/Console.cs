using System.Timers;
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

        //the log buffer
        private static string buffer;

        //the timer for delayed posting
        private static Timer timer = new Timer(250);

        /// <summary>
        /// Log the specified text and type (schedules it to prevent spam hanging the main thread).
        /// </summary>
        /// <param name="text">Text to log</param>
        /// <param name="type">Type of the log</param>
        public static void Log(string text,LogTypes type = LogTypes.Debug)
        {
            buffer += "[" + logFormatMap[type] + "] " + text + "\n";
            timer.Elapsed -= PostLogAsync;
            timer.Elapsed += PostLogAsync;
            timer.Start();
            timer.AutoReset = true;
        }

        /// <summary>
        /// Logs an exception.
        /// </summary>
        /// <param name="e">The exception to log</param>
        /// <param name="notes">Programmer's notes to be displayed with the exception.</param>
        public static void LogException(System.Exception e, string notes = " none provided.")
        {
            Log(e.GetType() + " occured in " + e.Source + ": " + e.Message + "\n" + e.StackTrace + "\nProgrammer's notes: " + notes, LogTypes.Error);
        }

        /// <summary>
        /// Posts the log async. This prevents the UI from hanging if there's a high volume of logs.
        /// </summary>
        private static void PostLogAsync(object source, System.Timers.ElapsedEventArgs e)
        {
            //run always on UI thread
            Application.Instance.Invoke(() =>
            {
                if (buffer.Length > 0)
                {
                    outputArea.Append(buffer,true);
                    buffer = "";
                }
                timer.AutoReset = false;
            });
        }
    }
}
