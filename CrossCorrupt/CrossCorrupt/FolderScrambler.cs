using System;
using System.Collections.Generic;
using System.IO;

namespace CrossCorrupt
{
    class FolderScrambler
    {

        //a dictionary with lists of files keyed by their extensions
        private Dictionary<string, List<string>> fileNames;
        private Random random; //random number generator
        private string folderPath;
        private const string tempExtension = ".new"; //temporary extension used to write the scrambled files

        /// <summary>
        /// Contructs a FolderScrambler with a path to a folder.
        /// </summary>
        /// <param name="folder">Folder to scramble names</param>
        /// <param name="allExcept">True if the extensions given are exclusions</param>
        /// <param name="extensions">A HashSet of all the extensions to look for</param>
        public FolderScrambler(string folder, bool allExcept, HashSet<string> extensions = null)
        {
            folderPath = folder;
            random = new Random();
            GenerateNameList(folder, allExcept, extensions);
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
        /// Scrambles the names of the files in the directory
        /// </summary>
        public void ScrambleNames()
        {
            foreach (string extension in fileNames.Keys)
            {
                List<string> names = fileNames[extension];
                List<int> usedNums = new List<int>(names.Count);
                int randomNum = random.Next(0, names.Count);
                for (int i = 0; i < names.Count; i++)
                {
                    while (!usedNums.Contains(randomNum))
                    {
                        randomNum = random.Next(0, names.Count);
                    }
                    usedNums.Add(randomNum);
                    File.Move(CreateFullPath(names[i]), CreateFullPath(names[randomNum]) + tempExtension);
                }
            }

            CleanTempExtensions();
        }

        /// <summary>
        /// Removes the temporary extension given to all files generated while scrambling.
        /// </summary>
        private void CleanTempExtensions()
        {
            DirectoryInfo di = new DirectoryInfo(folderPath);
            FileInfo[] newFiles = di.GetFiles();
            foreach (FileInfo file in newFiles)
            {
                if (file.Extension == tempExtension)
                {
                    File.Move(file.FullName, CreateFullPath(file.Name));
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
    }
}
