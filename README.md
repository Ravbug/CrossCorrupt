# CrossCorrupt
Cross platform file / folder corrupting tool, written in C# and Eto.forms.

## About
This is a general purpose file corruption program. It's inspired by the [Vinesauce ROM corruptor](https://github.com/Rikerz/VRC) but designed to be cross platform. The Vinesauce Corruptor is for Windows only. We designed CrossCorrupt in Eto.Forms because unlike Java, Eto.Forms uses the platform's native UI frameworks and compiles into mostly native code.
 The program corrupts files on the byte level, so any file, binary or not, should work. 

## Features
- Byte corrupt any file (insert, replace, delete byte)
- Corrupt entire folders (choose which file types to corrupt / not to corrupt)
- Folder scramble: randomize the names of the items in a folder (for messing up textures / sounds)
- Auto randomize settings, with configuration

See the [Wiki](https://github.com/Ravbug/CrossCorrupt/wiki) for a more detailed feature list.

## Screenshots
Screenshots coming soon.

## How to install
Installation instructions coming soon.

## System requirements
Here are what we officially support:
1. macOS 10.12 Sierra or later
2. Windows 10 version 1803 or later

The app should work on Linux, but you will have to compile it yourself.

## Compiling it yourself
You will need Visual Studio Community 2017 (Windows or Mac) or later to compile this. Open the .SLN and press Build Solution. Then you can run the app!
For more detailed instructions, see the [Wiki](https://github.com/Ravbug/CrossCorrupt/wiki).
  
 ## Issue reporting
 Please use the [Issues](https://github.com/Ravbug/CrossCorrupt/issues) section of this repository to report bugs. Ensure to provide what your settings are, details about your system, and about the files you are trying to corrupt. 
 Without adequate information, we cannot fix issues.
