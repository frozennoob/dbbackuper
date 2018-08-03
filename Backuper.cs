using System;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;

namespace dbbackuper
{
    class Backuper
    {
        public static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();
            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, false);
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }

        public static void ClearBackupDir(string destDirName)
        {
            
            DirectoryInfo dir = new DirectoryInfo(destDirName);

            // If directory not exist return
            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Directory does not exist or could not be found: "
                    + destDirName);
            }

            // Get all subdirs (Backups) from the path
            DirectoryInfo[] dirsArr = dir.GetDirectories();

            // Directories names validation
            ArrayList dirs = new ArrayList();
            foreach (DirectoryInfo d in dirsArr)
                if (ValidateDir(d.ToString()))
                    dirs.Add(d);

            
            //If count of backups < 10 return
            if (dirs.Count < 10)
            return;

            // Forming backups for delete list which was maked 10 or more days ago
            ArrayList dirsToDelete = new ArrayList();
            foreach (DirectoryInfo d in dirs)
                if (ConvertDirToDate(d.Name).Add(new TimeSpan(10, 0, 0, 0)) < DateTime.Now)
                    dirsToDelete.Add(d);
            // if after deleting we will have 9 backups or less then return
            if (dirs.Count - dirsToDelete.Count < 10)
                return;

            foreach (DirectoryInfo d in dirsToDelete)
                d.Delete(true);
        }

        // Conversion dirname to DateTime
        private static DateTime ConvertDirToDate(string date)
        {
            return Convert.ToDateTime(
                date.Substring(0,2) + '/' + date.Substring(2, 2) + '/' + date.Substring(4, 4));
        }

        //Validation dirnames function
        private static bool ValidateDir(string dir)
        {
            string pattern = @"^\d{8}_\d{6}$";
            return Regex.IsMatch(dir, pattern);
        }
    }
}

