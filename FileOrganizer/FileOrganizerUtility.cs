using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tuple = System.Tuple;

namespace FileOrganizer
{
    public class FileOrganizerUtility
    {
        public static string AddGuidToFileName(string filePath)
        {
            string dir = Path.GetDirectoryName(filePath).TrimEnd(new char[] { '\\' });
            string fName = Path.GetFileNameWithoutExtension(filePath);
            string ext = Path.GetExtension(filePath);
            string guid = Guid.NewGuid().ToString();
            return $"{dir}\\{fName}_duplicate_{guid}{ext}";
        }

        public static string AddGuidToFileNameConflict(string filePath)
        {
            string dir = Path.GetDirectoryName(filePath).TrimEnd(new char[] { '\\' });
            string fName = Path.GetFileNameWithoutExtension(filePath);
            string ext = Path.GetExtension(filePath);
            string guid = Guid.NewGuid().ToString();
            return $"{dir}\\{fName}_name_conflict_{guid}{ext}";
        }

        public static string PrepareTargetDirFromFile(
            string dir,
            FileInfo file)
        {
            string year = file.LastWriteTime.Year.ToString();
            string month = CultureInfo.InvariantCulture.DateTimeFormat.GetMonthName(file.LastWriteTime.Month);
            return $"{dir}\\{year}\\{month}";
        }

        public static bool AreSameFile(FileInfo a, FileInfo b)
        {
            if (a.Length != b.Length ||
                Path.GetExtension(a.FullName).ToLower() != Path.GetExtension(b.FullName).ToLower())
            {
                return false;
            }

            int x;
            int y;
            using (StreamReader r1 = new StreamReader(a.FullName))
            using (StreamReader r2 = new StreamReader(b.FullName))
            {
                {
                    do
                    {
                        x = r1.Read();
                        y = r2.Read();
                    } while (x == y && x != -1 && y != -1);
                }
            }

            return x == y;
        }

        public static string GetTargetFileName(string srcFile, string destFolder)
        {
            string newFilePath = $"{destFolder}\\{Path.GetFileName(srcFile)}";

            DirectoryInfo info = new DirectoryInfo(destFolder);
            foreach (var f in info.GetFiles("*.*", SearchOption.AllDirectories))
            {
                if (AreSameFile(f, new FileInfo(srcFile)))
                {
                    newFilePath = AddGuidToFileName(f.FullName);
                    break;
                }
            }

            return newFilePath;
        }
    }
}
