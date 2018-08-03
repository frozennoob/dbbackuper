using System;
using System.Configuration;
using System.IO;

namespace dbbackuper
{
    class Program
    {
        static void Main(string[] args)
        {
            // Get settings from app.config
            String source = Path.GetFullPath(ConfigurationManager.AppSettings["Source"]);
            String target = Path.GetFullPath(ConfigurationManager.AppSettings["Target"]);
            bool checkSubdir = Convert.ToBoolean(ConfigurationManager.AppSettings["CheckSubdir"]);
            
            
            
            // If last char of target path is not '\' , then add '\' to it
            if (target[target.Length - 1] != '\\')
                target += '\\';

            // Add DateTime info for target path name
            string targetWDate = target + DateTime.Now.ToString("ddMMyyyy_hhmmss");
   
            // Make copy
            Backuper.DirectoryCopy(source, targetWDate, checkSubdir);

            Backuper.ClearBackupDir(target);
        }
    }
}
