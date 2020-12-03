// James

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Security.Cryptography;

namespace DuplicateFinder
{
    public static class Hashing
    {
        // SHA conversion from class example. uses file to generate a SHA1 hash that is unique to the contents 
        //   of that file. If two files contain same data they will generate the same hash. 
        public static string SHA1(this string msg)
        {
            using (SHA1 sha1Hash = System.Security.Cryptography.SHA1.Create())
            {
                byte[] sourceBytes = Encoding.UTF8.GetBytes(msg);
                byte[] hashBytes = sha1Hash.ComputeHash(sourceBytes);
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }
    }
    class Program
    {
        public static void Main()
        {
            // Prompt user input + error checking
            Console.WriteLine("Please enter the name of the directory to search: ");
            string filePath = @"" + Console.ReadLine();

            // Clean up whitespace and check to see if user included the empty \ at the end. needed for filestream call
            filePath = filePath.TrimEnd();
            if (!filePath.EndsWith("\\"))
            {
                filePath += "\\";
            }
            // my hardcoded path during debugging was:  String filePath = @"c:\Users\James\Desktop\TEST\";

            // Create an info directory based on the contents of the provided file path. Generate a FileInfo list from that 
            DirectoryInfo di = new DirectoryInfo(filePath);
            Dictionary<string, int> myTable = new Dictionary<string, int>();
            FileInfo[] fileCollection = di.GetFiles();
            Console.WriteLine("The directory {0} originally contains: ", di.Name);

            // Counters for byte sizes 
            long totalSize = 0;
            long spaceSaved = 0;
            var listOfDupes = new List<string>();
            foreach (FileInfo file in fileCollection)
            {
                Console.WriteLine("    {0}  _size: {1} bytes", file.Name, file.Length);
                totalSize += file.Length;
                String fileHash;
                // appends file name to the current directory path to open file stream to be able to convert contents of the file currently
                //   being references into a hash to use as the dictionary key 
                using (FileStream dataInFile = File.OpenRead((filePath+file.Name)))
                {
                    using (SHA1 sha1Hash = SHA1.Create())
                    {
                        fileHash = BitConverter.ToString(sha1Hash.ComputeHash(dataInFile)).Replace("-", "").ToLower();
                        dataInFile.Close();
                    }
                }

                // ContainsKey will search for duplicate files based on hash value set in the keys
                // duplicate files will increment the int value to mark how often they occured
                if (myTable.ContainsKey(fileHash))
                {
                    myTable[fileHash] = myTable[fileHash] + 1;
                    listOfDupes.Add(file.Name);
                    spaceSaved += file.Length;
                }
                else { myTable.Add(fileHash, 0); }

                Console.WriteLine("      Hash: " + fileHash);
            }

            Console.WriteLine("\nThe full size of directory is {0} bytes.\n", totalSize);
            Console.WriteLine("The amount of space saved by removing duplicates is {0} bytes.", spaceSaved);

            Console.WriteLine("The duplicates are: ");
            foreach (string fName in listOfDupes)
                Console.WriteLine("   " + fName);
            Console.ReadLine();
        }
    }
}
