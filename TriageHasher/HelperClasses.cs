using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TriageHasher
{
    internal class ScanResults
    {
        private string hash = string.Empty;
        private bool hashmatch = false;
        private DateTime timeScanned;
        private string message = String.Empty;
        private DateTime lastWriteTimeUtc;
        private DateTime creationTimeUtc;
        private DateTime lastAccessTimeUtc;
        private string fileName;
        private string fullPath;
        FileAttributes attributes;
        string extension;
        long length;

        public string Hash { get => hash; set => hash = value; }
        public bool Hashmatch { get => hashmatch; set => hashmatch = value; }
        public DateTime TimeScanned { get => timeScanned; set => timeScanned = value; }
        public string Message { get => message; set => message = value; }
        public DateTime LastWriteTimeUtc { get => lastWriteTimeUtc; set => lastWriteTimeUtc = value; }
        public DateTime CreationTimeUtc { get => creationTimeUtc; set => creationTimeUtc = value; }
        public DateTime LastAccessTimeUtc { get => lastAccessTimeUtc; set => lastAccessTimeUtc = value; }
        public string FileName { get => fileName; set => fileName = value; }
        public string FullPath { get => fullPath; set => fullPath = value; }
        public FileAttributes Attributes { get => attributes; set => attributes = value; }
        public string Extension { get => extension; set => extension = value; }
        public long Length { get => length; set => length = value; }

        public ScanResults(FileInfo _input)
        {
            lastWriteTimeUtc = _input.LastWriteTimeUtc;
            creationTimeUtc = _input.CreationTimeUtc;
            lastAccessTimeUtc = _input.LastAccessTimeUtc;
            fileName = _input.Name;
            fullPath = _input.FullName;
            attributes = _input.Attributes;
            extension = _input.Extension;
            length = _input.Length;
        }
    }
}
