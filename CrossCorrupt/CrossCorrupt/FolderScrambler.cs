using System;
using System.Collections.Generic;
using System.IO;

namespace CrossCorrupt
{
    class FolderScrambler
    {

        //a dictionary with lists of files keyed by their extensions
        private Dictionary<string, List<string>> fileNames;

        //a dictionary that holds each swap made during the randomization to enable backtracking
        private Dictionary<string, string> reverseFileNames;

        //random number generator
        private Random random;

        private string folderPath;

        //temporary extension used to write the scrambled files
        private const string tempExtension = ".new";

        private DirectoryInfo[] subFolders;

        private bool includeSubFolders;

        private bool allExcept;

        private HashSet<string> extensions;

        private LinkedList<FolderScrambler> scrambledSubFolders;

        /// <summary>
        /// Contructs a FolderScrambler with a path to a folder.
        /// </summary>
        /// <param name="folder">Folder to scramble names</param>
        /// <param name="allExcept">True if the extensions given are exclusions</param>
        /// <param name="extensions">A HashSet of all the extensions to look for</param>
        public FolderScrambler(string folder, bool allExcept, HashSet<string> extensions = null,
            bool includeSubFolders = false)
        {
            this.extensions = extensions;
            this.allExcept = allExcept;
            if (folder.EndsWith(Path.DirectorySeparatorChar.ToString()))
            {
                folderPath = folder;
            }
            else
            {
                folderPath = folder + Path.DirectorySeparatorChar;
            }
            this.includeSubFolders = includeSubFolders;
            random = new Random();
            fileNames = new Dictionary<string, List<string>>();
            GenerateNameList(folder, allExcept, extensions);
            if (includeSubFolders)
            {
                FindSubFolders(folder);
            }
            Console.Log("FolderScrambler: Initialized \"" + folderPath + "\" for scrambling, found subfolders = " + includeSubFolders, Console.LogTypes.Info);
        }

        /// <summary>
        /// Creates a Dictionary with the names of the files.
        /// </summary>
        /// <param name="folder">Folder to scramble names</param>
        /// <param name="allExcept">True if the extensions given are exclusions</param>
        /// <param name="extensions">A HashSet of all the extensions to look for</param>
        private void GenerateNameList(string folder, bool allExcept, HashSet<string> extensions)
        {
            DirectoryInfo di = new DirectoryInfo(folder);
            FileInfo[] inFiles = di.GetFiles();
            string ext;
            for (int i = 0; i < inFiles.Length; i++)
            {
                ext = inFiles[i].Extension;
                if (extensions == null
                    || allExcept && !extensions.Contains(ext)
                    || !allExcept && extensions.Contains(ext))
                {
                    if (!fileNames.ContainsKey(ext))
                    {
                        fileNames.Add(ext, new List<string>());
                    }
                    fileNames[ext].Add(inFiles[i].Name);
                }
            }

        }

        /// <summary>
        /// Finds all of the subfolders in the directory and adds them to a list.
        /// </summary>
        /// <param name="folder">The folder to look for subfolders within.</param>
        private void FindSubFolders(string folder)
        {
            DirectoryInfo[] allSubFolders = new DirectoryInfo(folder).GetDirectories();
            if (allSubFolders.Length > 0)
            {
                subFolders = allSubFolders;
            }
            if (subFolders == null)
            {
                includeSubFolders = false;
            }
        }

        /// <summary>
        /// Scrambles the names of the files in the directory
        /// </summary>
        /// <param name="progress">method(double) to call on progress updates</param>
        public void ScrambleNames(Action<double> progress=null)
        {
           
            Console.Log("FolderScrambler: Scrambling \"" + folderPath + "\"", Console.LogTypes.Info);
            
            int prog = 0;
            int max = fileNames.Keys.Count;
            if (includeSubFolders)
            {
                max += subFolders.Length;
            }
            foreach (string extension in fileNames.Keys)
            {
                List<string> names = fileNames[extension];

                List<int> randomNumbers = GetRandomNumbers(names.Count);

                int randomNum;

                for (int i = 0; i < names.Count && i < randomNumbers.Count; i++)
                {
                    randomNum = randomNumbers[i];
                    //reverseFileNames.Add(names[randomNum], names[i]);
                    //Turns off the reverse scrambling ability
                    File.Move(CreateFullPath(names[i]), CreateFullPath(names[randomNum]) + tempExtension);
                }
                prog++;
                //update progress
                progress?.Invoke(prog / max);
            }

            CleanTempExtensions();

            if (includeSubFolders)
            {
                foreach (DirectoryInfo directory in subFolders)
                {
                    FolderScrambler sc = new FolderScrambler(directory.FullName, allExcept, extensions, includeSubFolders);
                    //scrambledSubFolders.AddLast(sc); For reversability
                    sc.ScrambleNames(null);//TODO make sure that is how it works
                    prog++;
                    progress?.Invoke(prog/max);
                }
            }
            //ensure progress is complete
            progress?.Invoke(100);
            Console.Log("FolderScramlber: Completed scramble for \"" + folderPath + "\"", Console.LogTypes.Info);           
        }

        /// <summary>
        /// Un-scrambles the items in a folder. The items must have been scrambled first
        /// </summary>
        /// <param name="progress">method(double) to call on progress updates</param>
        public void RevertScramble(Action<double> progress)
        {
            //TODO: insert code which undos the folder scramble
            Console.Log("FolderScrambler: Removing Scramble for \"" + folderPath + "\"", Console.LogTypes.Info);
            foreach (string changedFile in reverseFileNames.Keys)
            {
                File.Move(CreateFullPath(changedFile), CreateFullPath(reverseFileNames[changedFile]) + tempExtension);
            }

            CleanTempExtensions();

            if (scrambledSubFolders != null)
            {
                foreach (FolderScrambler sc in scrambledSubFolders)
                {
                    sc.RevertScramble(progress);
                }
            }


        }

        /// <summary>
        /// Removes the temporary extension given to all files generated while scrambling.
        /// </summary>
        private void CleanTempExtensions()
        {
            Console.Log("FolderScrambler: Cleaning extensions for \"" + folderPath + "\"", Console.LogTypes.Info);
            DirectoryInfo di = new DirectoryInfo(folderPath);
            FileInfo[] newFiles = di.GetFiles();
            foreach (FileInfo file in newFiles)
            {
                if (file.Extension == tempExtension)
                {
                    File.Move(file.FullName, CreateFullPath(file.Name.Substring(0, file.Name.Length - tempExtension.Length)));
                }
            }
        }

        /// <summary>
        /// Returns the full path of the name given.
        /// </summary>
        /// <param name="name">The file name to get the path of.</param>
        /// <returns>The full path of the file given</returns>
        private string CreateFullPath(string name)
        {
            return folderPath + name;
        }

        /// <summary>
        /// Returns an array of all possible ints numbers under the given maximum randomized.
        /// </summary>
        /// <param name="range">The maximum value and number of ints</param>
        /// <returns>An array of randomized ints</returns>
        private List<int> GetRandomNumbers(int range)
        {
            //create an array of all ints in the range
            List<int> numbersInOrder = new List<int>(range);
            for (int i = 0; i < range; i++)
            {
                numbersInOrder.Add (i);
            }

            //randomize the array
            Random rand = new Random();
            List<int> randomizedNumbers = new List<int>(range);
            int randomIndex;
            while (numbersInOrder.Count > 0)
            {
                randomIndex = rand.Next(0, numbersInOrder.Count);
                randomizedNumbers.Add(numbersInOrder[randomIndex]);
                numbersInOrder.RemoveAt(randomIndex);
            }

            return randomizedNumbers;
        }
    }
}
