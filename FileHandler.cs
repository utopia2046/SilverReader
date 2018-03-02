using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace SilverReaderApp
{
    class FileHandler
    {
        public const int maxFileSizeInBytes = 10 * 1024 * 1024;

        public static bool checkFileSize(string fileName)
        {
            var fi = new FileInfo(fileName);
            return fi.Length <= maxFileSizeInBytes;
        }

        public static string[] ReadFile(string fileName, Encoding encoding)
        {
            List<string> arr = new List<string>();
            using (var sr = new StreamReader(fileName, encoding, true))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    if (!String.IsNullOrWhiteSpace(line))
                    {
                        arr.Add(line);
                    }
                }
            }
            return arr.ToArray();
        }

        public static void SaveFile(string[] content, string fileName, Encoding encoding)
        {
            using (var sw = new StreamWriter(fileName, false, encoding))
            {
                foreach (string line in content)
                {
                    sw.WriteLine(line);
                }
            }
        }
    }
}
