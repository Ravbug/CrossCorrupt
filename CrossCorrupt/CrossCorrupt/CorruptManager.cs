using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace CrossCorrupt
{
    class CorruptManager
    {
        private FileInfo[] queue;
        private string outFolder;
        private string rootfolder;
        private bool overwrite;

        private HashSet<string> fileTypes;
        private bool invertFiletypes; //if the above set should be use to corrupt all file types EXCEPT those listed inside
        private Thread worker;

        //args for corruption
        private int n;
        private byte oldByte;
        private byte newByte;
        private long startByte;
        private long endByte;

        CorruptionType corruptType;

        public enum CorruptionType
        {
            Replace,Insert,Delete
        }

        /// <summary>
        /// Constructs a CorruptManager with an arbitrary array of files
        /// will mirror the folder structure in the output destination 
        /// </summary>
        /// <param name="files">Array of file paths to corrupt</param>
        /// <param name="destination">Root folder to corrupt to, without any trailing / or \ in the path</param>
        /// <param name="mode">Whether to Insert, Replace, or Delete corrupt the file</param>
        /// <param name="sByte">Byte to start corrupting from.</param>
        /// <param name="eByte">Byte to stop corrupting from. Pass -1 to automatically set the end byte.</param>
        /// <param name="nVal">N value for the corrupting methods</param>
        /// <param name="obyte">bye to replace / insert after / delete</param>
        /// <param name="nByte">Replacement / insertion byte</param>
        /// <param name="overwriteOriginal">True if the program should overwrite the existing files</param>
        public CorruptManager(string[] files, string destination, CorruptionType mode, long sByte, long eByte, int nVal, byte obyte, byte nByte=0, bool overwriteOriginal=false)
        {
            queue = new FileInfo[files.Length];
            for (int i = 0; i < files.Length; i++)
            {
                queue[i] = new FileInfo(files[i]);
            }
            outFolder = destination;
            overwrite = overwriteOriginal;
            corruptType = mode;
            n = nVal; newByte = nByte;
            startByte = sByte;
            endByte = eByte;
            newByte = nByte;
            oldByte = obyte;
        }

        /// <summary>
        /// Constructs a CorruptManager which corrupts all the files in all subdirectories
        /// </summary>
        /// <param name="rootFolder">Root folder to corrupt</param>
        /// <param name="destination">Folder to place the corrupted files</param>
        /// <param name="mode">Whether to Insert, Replace, or Delete corrupt the file</param>\
        /// <param name="sByte">Byte to start corrupting from.</param>
        /// <param name="eByte">Byte to stop corrupting from. Pass -1 to automatically set the end byte.</param>
        /// <param name="nVal">N value for the corrupting methods</param>
        /// <param name="obyte">bye to replace / insert after / delete</param>
        /// <param name="nByte">Replacement / insertion byte</param>
        /// <param name="filetypes">Array of file extensions to corrupt, or null to corrupt all file extensions</param>
        /// <param name="overwriteOriginal">True if the program should overwrite the existing files</param>
        public CorruptManager(string rootFolder, string destination, CorruptionType mode, long sByte, long eByte, int nVal, byte oByte, byte nByte=0, HashSet<string> filetypes=null, bool overwriteOriginal=false) : this(new string[0], destination, mode, sByte, eByte, nVal, oByte, nByte, overwriteOriginal)
        {
            //calls the above constructor before executing this
            fileTypes = filetypes;
            //build the queue of files
            rootfolder = rootFolder;
            queue = queueFromRoot(rootFolder);
        }

        /// <summary>
        /// Runs the corruptor on a background thread
        /// </summary
        /// <param name="callback">Void-returning method to call on progress updates. 1st param is double, 2nd param is FileInfo</param>
        public void Run(Action<double, FileInfo> callback = null)
        {
            //initialize the background worker
            worker = new Thread(() => {
                FileCorruptor fc = new FileCorruptor(null,null,startByte,endByte);
                //corrupt all the files, or corrupt only certain types?
                if (fileTypes == null)
                {
                    //corrupt each file in the list
                    for (int i = 0; i < queue.Length; i++)
                    {
                        FileInfo f = queue[i];
                        //if the corruptor is corrupting a batch of singletons, newname is different
                        string newName;
                        if (rootfolder == null)
                        {
                            newName = outFolder + Path.DirectorySeparatorChar + f.Name;
                        }
                        else
                        {
                            newName = f.FullName.Replace(rootfolder, outFolder);
                        }
                       fc.updateFiles(f,new FileInfo(newName));
                        //create necessary folders 
                        Directory.CreateDirectory(newName.Replace(f.Name,""));

                        //run the corrupt
                        if (corruptType == CorruptionType.Insert)
                        {
                            fc.InsertCorrupt(oldByte,newByte,n);
                        }
                        else if (corruptType == CorruptionType.Delete)
                        {
                            fc.DeleteCorrupt(oldByte,n);
                        }
                        else
                        {
                            fc.ReplaceCorrupt(oldByte,newByte, n);
                        }

                        //progress update
                        if (i == queue.Length - 1 || i % 10 == 0)
                        {
                            callback?.Invoke(i / queue.Length * 100, f);
                        }
                    }
                }
                else
                {
                    for(int i = 0; i <queue.Length; i++)
                    {
                        FileInfo f = queue[i];
                        //corrupt if file type is compatible
                        //TODO: invertFileTypes support
                        if (fileTypes.Contains(f.Extension))
                        {
                            string newName = f.FullName.Replace(rootfolder, outFolder);
                            fc.updateFiles(f, new FileInfo(newName));
                            //create necessary directories
                            Directory.CreateDirectory(newName.Replace(f.Name, ""));

                            //run the corrupt
                            if (corruptType == CorruptionType.Insert)
                            {
                                fc.InsertCorrupt(oldByte,newByte, n);
                            }
                            else if (corruptType == CorruptionType.Delete)
                            {
                                fc.DeleteCorrupt(oldByte,n);
                            }
                            else
                            {
                                fc.ReplaceCorrupt(oldByte,newByte, n);
                            }

                            //progress update
                            if (i == queue.Length - 1 || i % 10 == 0)
                            {
                                callback?.Invoke(i / queue.Length * 100, f);
                            }

                        }
                        //otherwise copy the file
                        else
                        {
                            //minimize unnecessary disk IO
                            if (!overwrite)
                            {
                                string newName = f.FullName.Replace(rootfolder, outFolder);
                                //create necessary directories
                                Directory.CreateDirectory(newName.Replace(f.Name, ""));
                                //copy the file
                                File.Copy(f.FullName, newName);
                            }
                        }
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
        private FileInfo[] queueFromRoot(string root)
        {
            List<FileInfo> paths = new List<FileInfo>();

            DirectoryInfo di = new DirectoryInfo(root);
            DirectoryInfo[] subfolders = di.GetDirectories();

            FileInfo[] contents = di.GetFiles();
            foreach (FileInfo f in contents)
            {
                paths.Add(f);
            }

            //loop through each subfolder
            foreach (DirectoryInfo d in subfolders)
            {
                //get the file names in the directory
                FileInfo[] fileNames = d.GetFiles();
                paths.AddRange(fileNames);

                paths.AddRange(queueFromRoot(d.FullName));
            }
            return paths.ToArray();
        }
    }
}
