using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace CrossCorrupt
{
    class CorruptManager
    {
        private string[] queue;
        private string outFolder;
        private bool overwrite;

        private HashSet<string> fileTypes;
        private Thread worker;

        /// <summary>
        /// Constructs a CorruptManager with an arbitrary array of files
        /// will mirror the folder structure in the output destination 
        /// </summary>
        /// <param name="files">Array of file paths to corrupt</param>
        /// <param name="destination">Root folder to corrupt to</param>
        /// <param name="overwriteOriginal">True if the program should overwrite the existing files</param>
        public CorruptManager(string[] files, string destination, bool overwriteOriginal=false)
        {
            queue = files;
            outFolder = destination;
            overwrite = overwriteOriginal;
        }

        /// <summary>
        /// Constructs a CorruptManager which corrupts all the files in all subdirectories
        /// </summary>
        /// <param name="rootFolder">Root folder to corrupt</param>
        /// <param name="destination">Folder to place the corrupted files</param>
        /// <param name="filetypes">Array of file extensions to corrupt, or null to corrupt all file extensions</param>
        /// <param name="overwriteOriginal">True if the program should overwrite the existing files</param>
        public CorruptManager(string rootFolder, string destination, HashSet<string> filetypes=null, bool overwriteOriginal=false)
        {
            outFolder = destination;
            overwrite = overwriteOriginal;
            fileTypes = filetypes;
            //build the queue of files
            queue = queueFromRoot(rootFolder);           
        }

        public void Run()
        {
            //initialize the background worker
            worker = new Thread(() => {
                FileCorruptor fc = new FileCorruptor(null,null);
                //corrupt the files literally, or corrupt only certain types?
                if (fileTypes == null)
                {
                    foreach (string filename in queue)
                    {

                       // fc.updateFiles(filename,);
                    }
                }             

                //if overwrite is true, then instead of copying files, leave them where they are
            });
            worker.Start();
        }

        /// <summary>
        /// Gets the list of file names inside a folder, incuding subfolders
        /// </summary>
        /// <param name="root">Path to the root folder</param>
        /// <returns>A string array containing all the filenames</returns>
        private string[] queueFromRoot(string root)
        {
            List<string> paths = new List<string>();

            DirectoryInfo di = new DirectoryInfo(root);
            DirectoryInfo[] subfolders = di.GetDirectories();

            FileInfo[] contents = di.GetFiles();
            foreach (FileInfo f in contents)
            {
                paths.Add(f.FullName);
            }

            //loop through each subfolder
            foreach (DirectoryInfo d in subfolders)
            {
                //get the file names in the directory
                FileInfo[] fileNames = d.GetFiles();
                foreach (FileInfo f in fileNames)
                {
                    paths.Add(f.FullName);
                }

                paths.AddRange(queueFromRoot(d.FullName));
            }
            return paths.ToArray();
        }
    }
}
