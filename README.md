# TriageHasher

Recently I was having a conversation with a collegue and he was telling me about a class he went to several years ago and they gave him a USB with a program on it.  You ran this program on a target computer it would supposedly find all the CSAM items on said computer.  This collegue has a tendancy (meaning all day, with everything they talk about) to overstate things to tell the biggest story he can.  Being the technical minded individual I am, I knew that theoretically what he was talking about could be done, maybe just not exactly what he was thinking.  

Fast forward a few months and I was taking a class on digital forensic triage.  In many digital forensic investigations, investigators start from a place they know what file they are looking for by it's hash.  I started thinking that it would be nice to have a lightweight, fast, efficient way to search a file system as a step during triage.  The investigator can then prioritize the devices they know contain the files they are looking for. 

I had an unexpected 11 days off from work due to covid in the family and decided let's give this a try!

<H2>About this Tool</H2>

This is a command line program written in C#.  The program takes the following arguments when executing:

<ul>-help - Displays the help menu</ul>
<ul>1. [Required] Drive letter, or directory to search.  The program will search all files and directories within this destination recursively.</ul>
<ul>2. [Required] True or False, tells the program whether to stop searching once a match is found</ul>
<ul>3. [Required] File name containing the list of MD5 hashes you are searching for.</ul>
<ul>4. [Required] File extension of the files you are looking for.  This simply will help speed up the search, the program will only hash the files matching the extension.  Passing '.*' will search all files. </ul>
<ul>5. [Optional] Filename for a report.  This report contains a lot of useful information to a forensic investigator.  During the hashing process if the program finds a file that it can't hash (locked, inaccessible, etc) it will output that information to a file labeled "inaccessible-[yourfilename]"</ul>

Example Usage:
<ul>./TriageHasher.exe E:\ false known_hashes.txt .*</ul>

<ul>./TriageHAsher.exe E:\ false known_hashes.txt .* output.csv</ul>

<ul>./TriageHasher.exe E:\ true known_hashes.txt .jpg output.csv</ul>

<H2>Knowing the tool</H2>

Any DFIR class will always, without fail, will have a "know your tool" speach.  Here's mine in regards to this tool with the few days I've been playing with it.

1. If you do not use a write blocker of some type, it WILL modify the last accessed time.  I am exploring some options to avoid this, but I don't think it can be done.  Using diskpart to set the drive to readonly is a good option.
2. There is no file carving occurring.  The program is taking the directories and files the operating system knows about.
3. There is no file extension mistmatch detection.  If you are searching .txt and the extension has been changed to .ccd, that file will not be hashed and not checked against your list.

<h2>What the future holds..</h2>

In no particular order:

- Clean up the output report.  Right now it's dumping out the entire FileInfo class, which has several sets of dates and times.  It's hard to tell exactly what it's referring to.
- Add in file name searching.  Just one more way to find those files.
- Clean up the input arguments, right now it's strictly in the order shown above.

Please report any issues and contribute! 
