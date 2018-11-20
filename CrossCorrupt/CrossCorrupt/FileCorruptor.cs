using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CrossCorrupt
{
    class FileCorruptor
    {
        private string inFile;
        private string outFile;

        /// <summary>
        /// Constructor for a FileCorruptor object
        /// </summary>
        /// <param name="fIn">Fully-qualified path to the file</param>
        /// <param name="fOut">Fully-quallified path to the destination file</param>
        public FileCorruptor()
        {
            
        }

        /// <summary>
        /// Constructor for a FileCorruptor object
        /// </summary>
        /// <param name="fIn">Fully-qualified path to the file</param>
        /// <param name="fOut">Fully-quallified path to the destination file</param>
        public FileCorruptor(string fIn, string fOut)
        {
            inFile = fIn;
            outFile = fOut;
        }

        /// <summary>
        /// Corrupts a file by changing every nth byte to a specified byte, and writes the resulting file to the outFile directory
        /// </summary>
        /// <param name="replacement">replacement byte</param>
        /// <param name="n">the nth byte to change</param>
        public void ReplaceCorrupt(byte replacement,int n)
        {
            //get the length of the file -- this will throw if the target file does not exist
            FileInfo fi = new FileInfo(inFile);

            byte[] output = new byte[fi.Length];

            //read the file one byte at a time (reading it all at once is bad)
            FileStream fileStream = File.OpenRead(inFile);            
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
                    output[i] = (byte)fileStream.ReadByte();
                }
            }

            //write out the file
            File.WriteAllBytes(outFile,output);
        }

        /// <summary>
        /// Corrupts the file by inserting extra bytes every n bytes, and writes the resulting file to the outFile directory
        /// </summary>
        /// <param name="insertion">byet to insert</param>
        /// <param name="n">the nth byte to insert after</param>
        public void InsertCorrupt(byte insertion, int n)
        {
            //get the length of the file -- this will throw if the target file does not exist
            FileInfo fi = new FileInfo(inFile);

            byte[] output = new byte[fi.Length + fi.Length/n];

            //read the file one byte at a time (reading it all at once is bad)
            FileStream fileStream = File.OpenRead(inFile);
            for (int i = 0; i < output.Length; i++)
            {
                //if this is the nth byte, set the replacement byte
                if (i % n == 0)
                {   
                    //add the byte and the extra byte
                    output[i] = (byte)fileStream.ReadByte();
                    output[i + 1] = insertion;
                    i++;
                }
                else
                {
                    //add the byte from the file
                    output[i] = (byte)fileStream.ReadByte();
                }
            }

            //write out the file
            File.WriteAllBytes(outFile, output);
        }

        /// <summary>
        /// Corrupts the file by deleting every nth byte, and writes the resulting file to the outFile directory
        /// </summary>
        /// <param name="n">the nth byte to delete</param>
        public void DeleteCorrupt(int n)
        {
            //get the length of the file -- this will throw if the target file does not exist
            FileInfo fi = new FileInfo(inFile);

            List<byte> output = new List<byte>();

            //read the file one byte at a time (reading it all at once is bad)
            FileStream fileStream = File.OpenRead(inFile);
            for (int i = 0; i < fi.Length-fi.Length/n; i++)
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
            File.WriteAllBytes(outFile, output.ToArray());
        }

        /// <summary>
        /// Sets the new input file for corruption
        /// </summary>
        /// <param name="newIn">The path of new file to be used</param>
        public void SetInFile(string newIn)
        {
            inFile = newIn;
        }

        /// <summary>
        /// Sets the new output file for corruption
        /// </summary>
        /// <param name="newIn">The path of new file to be used</param>
        public void SetOutFile(string newOut)
        {
            outFile = newOut;
        }

        public string GetInfIle()
        {
            return inFile.ToString();
        }

        public string GetOutFile()
        {
            return outFile.ToString();
        }
    }
}
