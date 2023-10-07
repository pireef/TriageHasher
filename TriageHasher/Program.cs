using CsvHelper;
using System.Globalization;
using TriageHasher;
using System.CommandLine;
using System.Reflection;

internal class Program
{
    static async Task Main(string[] args)
    {
        var searchDir = new Option<string>
            (name: "--dir",
            description: "Drive or directory you wish to search.  The search will be recursive through all directories and files",
            getDefaultValue: () => "C:\\");
        searchDir.IsRequired = true;
        var earlyExit = new Option<bool>
            (name: "--earlyExit",
            description: "Flag if you wish to stop the search on the first hash match.  This is useful if 1 match is all you need to sieze that device for further processing.",
            getDefaultValue: () => false);
        earlyExit.IsRequired = false;
        var hashFileName = new Option<string>
            (name: "--hash",
            description: "File name containing the list of MD5 hashes you are looking for.",
            getDefaultValue: () => "knownHash.txt");
        hashFileName.IsRequired = false;
        var fileExtension = new Option<string>
            (name: "--ext",
            description: "File extension you want to scan.",
            getDefaultValue: () => ".*");
        fileExtension.IsRequired = false;
        var outputFile = new Option<string>
            (name: "--out",
            description: "File name of a CSV file you wish to have the output saved to.");
        outputFile.IsRequired = false;

        var rootCommand = new RootCommand(Info.GetLogo())
        {
            searchDir,
            earlyExit,
            hashFileName,
            fileExtension,
            outputFile
        };

        rootCommand.SetHandler((searchDirValue, earlyExitValue, hashFileNameValue, fileExtensionValue, outputFileValue) =>
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(Info.GetLogo());
            Console.WriteLine(Info.GetVersion());
            Console.ResetColor();

            Scan newScan = new Scan(searchDirValue, hashFileNameValue, fileExtensionValue, earlyExitValue);
            if(newScan.ValidSetup)
            {
                newScan.StartScan();
            }
            if(outputFileValue != null)
            {
                //we want to output the data to a report.
                Console.WriteLine("Sending output to: " + outputFileValue);
                using (var writer = new StreamWriter(args[4]))
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csv.WriteRecords(newScan.Results);
                }

                using (var inwriter = new StreamWriter("inaccessible-" + outputFileValue))
                using (var csv = new CsvWriter(inwriter, CultureInfo.InvariantCulture))
                {
                    csv.WriteRecords(newScan.InaccessibleResults);
                }

            }

        }, searchDir, earlyExit, hashFileName, fileExtension, outputFile);
        await rootCommand.InvokeAsync(args);
        
    }
}