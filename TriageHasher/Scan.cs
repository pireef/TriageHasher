using System.Security.Cryptography;


namespace TriageHasher
{
    internal class Scan
    {
        private string[] args;
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

        public Scan(string[] cmdargs)
        {
            this.args = cmdargs;
            results = new List<ScanResults>();
            inaccessibleResults = new List<ScanResults>();

            if (IsValidFolderPath(cmdargs[0]))
            {
                scanLocation = cmdargs[0];
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Invalid Drive or Directory: " + args[0]);
                bValidSetup = false;
            }

            try
            {
                bEarlyAbort = Convert.ToBoolean(args[1]);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Invalid argument: " + args[1] + " Early abort must be true or false");
                bValidSetup = false;
            }

            if ((args[2] != null) & (args[2].Length != 0))
                knownHashfile = args[2];
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("Invalid Known Hash File " + args[2]);
                bValidSetup = false;
            }

            if (!File.Exists(Directory.GetCurrentDirectory() + "\\" + knownHashfile))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Known hashfile does not exist..." + knownHashfile);
                bValidSetup = false;
            }

            if ((args[3] != null) & ((args[3].Length != 0)))
                searchType = "*" + args[3];
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Search type needs to contain file extension, ie .*, .jpg, .txt...." + knownHashfile);
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

            Console.WriteLine("Getting list of files.");
            var allFiles = Directory.EnumerateFiles(scanLocation, searchType, new EnumerationOptions { IgnoreInaccessible = false, RecurseSubdirectories = true }); ;
            Console.WriteLine("Done!\n\nStarting Hash...");

            foreach (string file in allFiles)
            {
                ScanResults result = new ScanResults();
                FileInfo fileInfo = new FileInfo(file);
                result.File = fileInfo;
                result.TimeScanned = DateTime.Now;

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
