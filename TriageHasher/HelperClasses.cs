using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TriageHasher
{
    internal class ScanResults
    {
        private FileInfo _file;
        private string hash = string.Empty;
        private bool hashmatch = false;
        private DateTime timeScanned;
        private string message = String.Empty;

        public string Hash { get => hash; set => hash = value; }
        public bool Hashmatch { get => hashmatch; set => hashmatch = value; }
        public DateTime TimeScanned { get => timeScanned; set => timeScanned = value; }
        public FileInfo File { get => _file; set => _file = value; }
        public string Message { get => message; set => message = value; }
    }
}
