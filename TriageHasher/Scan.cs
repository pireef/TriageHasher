using System.Security.Cryptography;


namespace TriageHasher
{
    internal class Scan
    {
        private string scanLocation;
        private string knownHashfile;
        private List<string> knownHashes;
        private bool bValidSetup = true;
        private bool bEarlyAbort;
        private DateTime startScan;
        private DateTime endScan;
        int nNumberofMatches = 0;
        private List<ScanResults> results;
        private List<ScanResults> inaccessibleResults;
        private string searchType;

        public bool ValidSetup { get => bValidSetup; set => bValidSetup = value; }
        internal List<ScanResults> Results { get => results;}
        internal List<ScanResults> InaccessibleResults { get => inaccessibleResults; set => inaccessibleResults = value; }

        public Scan(string scanLocation, string knownHash, string searchType, bool earlyExit)
        {
            this.scanLocation = scanLocation;
            this.knownHashfile = knownHash;
            this.searchType = "*" + searchType;
            this.bEarlyAbort = earlyExit;

            results = new List<ScanResults>();
            inaccessibleResults = new List<ScanResults>(); 
            knownHashes = new List<string>();

            if (!IsValidFolderPath(scanLocation))
            {
                Console.ForegroundColor= ConsoleColor.Red;
                Console.WriteLine("Invalid Drive or Directory to scan.");
                Console.ResetColor();
                bValidSetup = false;
            }

            if (!File.Exists(Directory.GetCurrentDirectory() + "\\" + knownHashfile)) 
            {
                Console.ForegroundColor= ConsoleColor.Red;
                Console.WriteLine("Known Hash File does not exist " + knownHashfile);
                Console.ResetColor();
                bValidSetup = false;
            }
        }

        public void StartScan()
        {
            startScan = DateTime.Now;
            Console.WriteLine("======================================================================");
            Console.WriteLine("Starting Scan:\t\t\t" +  startScan);
            //load the knowns into memory
            Console.WriteLine("Loading known hashes into memory");
            knownHashes = ReadKnownFile(knownHashfile);

            Console.WriteLine("Getting list of files with the extension of: " + searchType);
            var allFiles = Directory.EnumerateFiles(scanLocation, searchType, new EnumerationOptions { IgnoreInaccessible = false, RecurseSubdirectories = true });
            Console.WriteLine("Found " + allFiles.Count() + " files");
            Console.WriteLine("Done!\n\nStarting Hash...");

            foreach (string file in allFiles)
            {
                FileInfo fileInfo = new FileInfo(file);
                ScanResults result = new ScanResults(fileInfo);
                result.TimeScanned = DateTime.UtcNow;

                using (HashAlgorithm hashAlgorithm = MD5.Create())
                {
                    try
                    {
                        using (var fileStream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                        {
                            var hash = hashAlgorithm.ComputeHash(fileStream);
                            var hashString = BitConverter.ToString(hash).Replace("-", String.Empty).ToLower();
                            result.Hash = hashString;
                            if (knownHashes.Any(s => s.Contains(hashString)))
                            {
                                Console.WriteLine("Found Hash match!! " + file + "\t" + hashString);
                                nNumberofMatches++;
                                result.Hashmatch = true;
                                if (bEarlyAbort)
                                {
                                    Console.WriteLine("Found 1 match, aborting further search.");
                                    Environment.Exit(1);
                                }
                            }
                        }
                    }
                    catch (UnauthorizedAccessException ex)
                    {
                        Console.WriteLine(fileInfo.Name + "Unable to access file, adding to inaccessible list.");
                        result.Message = ex.Message;
                        inaccessibleResults.Add(result);                        
                    }
                    catch (IOException ex)
                    {
                        Console.WriteLine(fileInfo.Name + "IO Exception, file likely in use, adding to inaccessible list.");
                        result.Message = ex.Message;
                        inaccessibleResults.Add(result);
                    }
                    catch (Exception ex)
                    {
                        Console.Write(fileInfo.Name + "Some other error occurred.");
                        result.Message = ex.Message;
                        inaccessibleResults.Add(result);
                    }
                }
                results.Add(result);
            }
            Console.WriteLine("Scanned " + allFiles.Count() + " files\tFound " +  nNumberofMatches + " matches.");
            Console.WriteLine("======================================================================");
            endScan = DateTime.Now;
            Console.WriteLine("Scan finished at:\t\t" + endScan);
        }

        private List<string> ReadKnownFile(string filename)
        {
            List<string> result = new List<string>();
            string path = Environment.CurrentDirectory + "\\" + filename;
            using (var filestream = File.OpenRead(filename))
            {
                using (var streadReader = new StreamReader(filestream))
                {
                    string line;
                    while ((line = streadReader.ReadLine()) != null)
                    {
                        result.Add(line.ToLower());
                    }
                }
            }
            return result;
        }
        private bool IsValidFolderPath(string path)
        {
            if ((path == null) | (path.Length == 0))
                return false;

            return Directory.Exists(path);
        }
    }
}
