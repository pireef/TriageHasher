using CsvHelper;
using System.Globalization;
using TriageHasher;

internal class Program
{

    private static void Main(string[] args)
    {
        string location = String.Empty;
        //bool bAbortIfFound = false;
        string knownhashfile = String.Empty;
        //List<string> knownHash;
        string splash = @"_________ _______ _________ _______  _______  _______             _______  _______           _______  _______ 
\__   __/(  ____ )\__   __/(  ___  )(  ____ \(  ____ \  |\     /|(  ___  )(  ____ \|\     /|(  ____ \(  ____ )
   ) (   | (    )|   ) (   | (   ) || (    \/| (    \/  | )   ( || (   ) || (    \/| )   ( || (    \/| (    )|
   | |   | (____)|   | |   | (___) || |      | (__      | (___) || (___) || (_____ | (___) || (__    | (____)|
   | |   |     __)   | |   |  ___  || | ____ |  __)     |  ___  ||  ___  |(_____  )|  ___  ||  __)   |     __)
   | |   | (\ (      | |   | (   ) || | \_  )| (        | (   ) || (   ) |      ) || (   ) || (      | (\ (   
   | |   | ) \ \_____) (___| )   ( || (___) || (____/\  | )   ( || )   ( |/\____) || )   ( || (____/\| ) \ \__
   )_(   |/   \__/\_______/|/     \|(_______)(_______/  |/     \||/     \|\_______)|/     \|(_______/|/   \__/
                                                                                                              ";

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(splash);
        Console.WriteLine("v 0.1");
        Console.ResetColor();

        if (args[0] == "-help")
        {
            DisplayHelp();
            Environment.Exit(0);
        }

        Scan newScan = new Scan(args);
        if (newScan.ValidSetup)
        {
            newScan.StartScan();
        }
        else
            DisplayHelp();

        if ((args[4] != null) &(args[4].Length != 0))
        {
            //we want to output the data to a report.
            Console.WriteLine("Sending output to: " + args[4]);
            using (var writer = new StreamWriter(args[4])) 
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture)) 
            {
                csv.WriteRecords(newScan.Results);
            }

            using (var inwriter = new StreamWriter("inaccessible-" + args[4]))
                using (var csv = new CsvWriter(inwriter, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(newScan.InaccessibleResults);
            }    
        }
    }

    private static void DisplayHelp()
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Triage Hasher v 0.1\n");
        Console.WriteLine("Help Page!!\n");
        Console.ResetColor();
        Console.WriteLine("Arguments:\n\n");
        Console.WriteLine("1. [Required] Drive or directory you wish to search.  The search will be recursive through all directories and files\n");
        Console.WriteLine("2. [Required] Flag if you wish to stop the search on the first hash match.  This is useful if 1 match is all you need to sieze that device for further processing.\n ");
        Console.WriteLine("3. [Required] File name containing the list of MD5 hashes you are looking for.\n");
        Console.WriteLine("4. [Required] File extension you want to scan.  .* searches all files.\n");
        Console.WriteLine("4. [Optional] File name of a CSV file you wish to have the output saved to.\n");
        Console.WriteLine("Usage:\n\n");
        Console.WriteLine("TriageHasher.exe [Drive Or Directory] [true or false] [Filename of Hashes] [File Extension to search] [Optional CSV file for output]\n");
        Console.WriteLine("Example: \n TriageHasher.exe C:\\ true knownhash.txt *.jpg output.csv");

        Console.ResetColor();
    }

}