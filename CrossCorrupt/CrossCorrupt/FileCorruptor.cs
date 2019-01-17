using System;
using System.Collections.Generic;
using System.IO;

namespace CrossCorrupt
{
    class FileCorruptor
    {
        private FileInfo inFile;
        private FileInfo outFile;

        private long startByte;
        private long endByte;

        /// <summary>
        /// Constructor for a FileCorruptor object
        /// </summary>
        /// <param name="fIn">source file FileInfo object</param>
        /// <param name="fOut">Destination path FileInfo object</param>
        public FileCorruptor(FileInfo fIn, FileInfo fOut, long sByte, long eByte)
        {
            inFile = fIn;
            outFile = fOut;
            startByte = sByte;
            endByte = eByte;
        }

        /// <summary>
        /// Updates the file and output path so the object is reusable
        /// </summary>
        /// <param name="newIn">New file path to corrupt</param>
        /// <param name="newOut">New output destination</param>
        public void updateFiles(FileInfo newIn, FileInfo newOut)
        {
            inFile = newIn;
            outFile = newOut;
        }

        /// <summary>
        /// Reads the file into an array of bytes
        /// </summary>
        /// <param name="file">FileInfo represnting file to read</param>
        /// <returns>List of integers representing the file</returns>
        private List<byte> readFile(FileInfo file)
        {
            var fileContents = new List<byte>();
            FileStream filestream;
            try
            {
                filestream = File.OpenRead(file.FullName);
            }
            catch (FileNotFoundException) { return null; }

            long stop = file.Length;

            if (endByte == -1)
            {
                endByte = stop;
            }

            for (int i = 0; i < stop; i++)
            {
                fileContents.Add((byte)filestream.ReadByte());
            }

            return fileContents;
        }

        /// <summary>
        /// Writes a file to disk
        /// </summary>
        /// <param name="file">List with the file data</param>
        private void writeFile(List<byte> file)
        {
            //write out the file
            try
            {
                File.WriteAllBytes(outFile.FullName, file.ToArray());
            }
            catch (Exception) { };
        }

        /// <summary>
        /// Corrupts a file by changing every nth byte to a specified byte, and writes the resulting file to the outFile directory
        /// </summary>
        /// <param name="replacement">replacement byte</param>
        /// <param name="n">the nth byte to change</param>
        public void ReplaceCorrupt(byte old, byte replacement,int n)
        {
            var file = readFile(inFile);

            for (int i = 0; i < endByte && i < file.Count; i++)
            {
                if (i % n == 0 && file[i] == old)
                {
                    file[i] = replacement;
                }
            }
            writeFile(file);
        }

        /// <summary>
        /// Corrupts the file by inserting extra bytes every n bytes, and writes the resulting file to the outFile directory
        /// </summary>
        /// <param name="insertion">byet to insert</param>
        /// <param name="n">the nth byte to insert after</param>
        public void InsertCorrupt(byte insertAfter, byte insertion, int n)
        {
            var file = readFile(inFile);

            for (int i = 0; i < endByte && i < file.Count; i++)
            {
                if (i % n == 0 && file[i] == insertAfter)
                {
                    file.Insert(i, insertion);
                    endByte++;
                    i++; //avoid infinite loops if insertion == insertAfter
                }
            }
            writeFile(file);
        }

        /// <summary>
        /// Corrupts the file by deleting every nth byte, and writes the resulting file to the outFile directory
        /// </summary>
        /// <param name="n">the nth byte to delete</param>
        public void DeleteCorrupt(byte toDelete, int n)
        {
            var file = readFile(inFile);

            for (long i = endByte; i >= startByte; i--)
            {
                if (i % n == 0 && file[(int)i] == toDelete)
                {
                    file.RemoveAt((int)i);
                }
            }
            writeFile(file);
        }
    }
}
