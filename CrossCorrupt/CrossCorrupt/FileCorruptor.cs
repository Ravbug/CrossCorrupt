﻿using System;
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
        public void replaceCorrupt(byte replacement,int n)
        {
            //get the length of the file -- this will throw if the target file does not exist
            FileInfo fi = new FileInfo(inFile);
            long stop = fi.Length;

            byte[] output = new byte[fi.Length];

            //read the file one byte at a time (reading it all at once is bad)
            FileStream fileStream = File.OpenRead(inFile);            
            for (int i = 0; i < stop; i++)
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
    }
}
