using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CrossCorrupt
{
    class FileCorruptor
    {
        private FileInfo inFile;
        private FileInfo outFile;

        /// <summary>
        /// Constructor for a FileCorruptor object
        /// </summary>
        /// <param name="fIn">Fully-qualified path to the file</param>
        /// <param name="fOut">Fully-quallified path to the destination file</param>
        public FileCorruptor()
        {
            //nothing to see here
        }

        /// <summary>
        /// Constructor for a FileCorruptor object
        /// </summary>
        /// <param name="fIn">source file FileInfo object</param>
        /// <param name="fOut">Destination path FileInfo object</param>
        public FileCorruptor(FileInfo fIn, FileInfo fOut)
        {
            inFile = fIn;
            outFile = fOut;
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
        /// Corrupts a file by changing every nth byte to a specified byte, and writes the resulting file to the outFile directory
        /// </summary>
        /// <param name="replacement">replacement byte</param>
        /// <param name="n">the nth byte to change</param>
        public void ReplaceCorrupt(byte replacement,int n)
        {

            //get the length of the file -- this will throw if the target file does not exist
           

            byte[] output = new byte[inFile.Length];

            FileStream fileStream;
            try
            {
                //read the file one byte at a time (reading it all at once is bad)
                fileStream = File.OpenRead(inFile.FullName);
            }
            catch (FileNotFoundException ex)
            {
                return;
            }

            for (int i = 0; i < output.Length; i++)
            {
                //if this is the nth byte, set the replacement byte
                if (i % n == 0)
                {
                    fileStream.ReadByte();
                    output[i] = replacement;
                }
                else
                {
                    //add the byte from the file
                    //for some reason ReadByte returns an int, so casts to byte
                    output[i] = (byte)fileStream.ReadByte();
                }
            }

            //write out the file
            try
            {
                File.WriteAllBytes(outFile.FullName, output);
            }
            catch (Exception) { };
        }

        /// <summary>
        /// Corrupts the file by inserting extra bytes every n bytes, and writes the resulting file to the outFile directory
        /// </summary>
        /// <param name="insertion">byet to insert</param>
        /// <param name="n">the nth byte to insert after</param>
        public void InsertCorrupt(byte insertion, int n)
        {
            //get the length of the file -- this will throw if the target file does not exist

            byte[] output = new byte[inFile.Length + inFile.Length/n];

            FileStream fileStream;
            try
            {
                //read the file one byte at a time (reading it all at once is bad)
                fileStream = File.OpenRead(inFile.FullName);
            }
            catch (FileNotFoundException ex)
            {
                return;
            }

            for (int i = 0; i < output.Length; i++)
            {
                //if this is the nth byte, set the replacement byte
                if (i % n == 0)
                {   
                    //add the byte and the extra byte
                    output[i] = (byte)fileStream.ReadByte();
                    if (i + 1 < output.Length)
                    {
                        output[i + 1] = insertion;
                    }
                    i++;
                }
                else
                {
                    //add the byte from the file
                    output[i] = (byte)fileStream.ReadByte();
                }
            }

            //write out the file
            try
            {
                File.WriteAllBytes(outFile.FullName, output);
            }
            catch (Exception) { };
        }

        /// <summary>
        /// Corrupts the file by deleting every nth byte, and writes the resulting file to the outFile directory
        /// </summary>
        /// <param name="n">the nth byte to delete</param>
        public void DeleteCorrupt(int n)
        {
            //get the length of the file -- this will throw if the target file does not exist

            List<byte> output = new List<byte>();

            FileStream fileStream;
            try
            {
                //read the file one byte at a time (reading it all at once is bad)
                fileStream = File.OpenRead(inFile.FullName);
            }
            catch (FileNotFoundException ex)
            {
                return;
            }

            for (int i = 0; i < inFile.Length-inFile.Length/n; i++)
            {
                //if this is the nth byte, set the replacement byte
                if (i % n == 0)
                {
                    //skip the byte
                    fileStream.ReadByte();
                }
                else
                {
                    //add the byte from the file
                    output.Add((byte)fileStream.ReadByte());
                }
            }

            //write out the file
            try
            {
                File.WriteAllBytes(outFile.FullName, output.ToArray());
            }
            catch (Exception e) { };
        }
    }
}
