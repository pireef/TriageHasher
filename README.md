
# Triage Hasher

Recently I was having a conversation with a colleague and he was telling me about a class he went to several years ago and they gave him a USB with a program on it.  You ran this program on a target computer it would supposedly find all the CSAM items on said computer.  This colleague has a tendency (meaning all day, with everything they talk about) to overstate things to tell the biggest story he can.  Being the technical minded individual I am, I knew that theoretically what he was talking about could be done, maybe just not exactly what he was thinking.  

Fast forward a few months and I was taking a class on digital forensic triage.  In many digital forensic investigations, investigators start from a place they know what file they are looking for by it's hash.  I started thinking that it would be nice to have a lightweight, fast, efficient way to search a file system as a step during triage.  The investigator can then prioritize the devices they know contain the files they are looking for. 

I had an unexpected 11 days off from work due to covid in the family and decided let's give this a try!




## About this tool

This is a command line tool written in C# and takes the following command line arguments.  

Usage:
  TriageHasher [options]

Options:

  --dir <dir> (REQUIRED)  Drive or directory you wish to search.  The search will be recursive through all directories and files [default: C:\]dir</dir

  --earlyExit             Flag if you wish to stop the search on the first hash match.  This is useful if 1 match is all you need to sieze that device for further processing. [default: False]

  --hash <hash>           File name containing the list of MD5 hashes you are looking for. [default: knownHash.txt]

  --ext <ext>             File extension you want to scan. [default: .*]

  --out <out>             File name of a CSV file you wish to have the output saved to.

  --version               Show version information

  -?, -h, --help          Show help and usage information


## Usage/Examples

```shell
./TriageHasher.exe --help
#Displays the help page

./TriageHasher.exe --version
#Displays the Version Information

./TriageHasher.exe --dir E:
#scans all files on the E: drive using the knownHash.txt file in the application directory

./TriageHasher.exe --dir E: --hash example.txt 
#scans all files on the E drive using the list of hashes in example.txt 

./TriageHasher.exe --dir E: --hash example.txt --earlyExit true 
#will abort the search once a hash match is found

./TriageHasher.exe --dir E: --hash example.txt --out report.csv
#creates a CSV report of files it scanned.  A separate csv file is created for inaccessible files.  
```


## Knowing the Tool

Any DFIR class will always, without fail, will have a "know your tool" speech.  Here's mine in regards to this tool with the few days I've been playing with it.

1. If you do not use a write blocker of some type, it WILL modify the last accessed time.  I am exploring some options to avoid this, but I don't think it can be done.  Using diskpart to set the drive to readonly is a good option.
2. There is no file carving occurring.  The program is taking the directories and files the operating system knows about.
3. There is no file extension mistmatch detection.  If you are searching .txt and the extension has been changed to .ccd, that file will not be hashed and not checked against your list.


## Installation

Simply unzip the file into a directory! 

There is an included E01 image file and an example.txt known hash file for your use in testing.  Mount the image as a drive with your favorite image mounter (I prefer Aresent Image Mounter) and then scan the drive.  
    
## Contributing

It would be awesome to have some contrubitors to the project! I am not a professional programmer, I am definitely more of a hobbyist who does ocassional DFIR work at my main job.  

Here are some brainstorming ideas of things I would like to implement in the future. 

- File name searching in addition to hashes.  
- Get the program to prioritize scanning user folders first followed by system folders
- Scan multiple file types.  Right now it's all or one file extension.  It would be nice to give a list of file extensions to scan.  
- Integration with VirusTotal API (or something similar) 
