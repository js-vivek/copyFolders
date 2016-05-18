using System;
using System.Configuration;
using System.IO;

namespace CopyFiles
{
    class Program
    {
        /// <summary>
        /// Author: Vivek Gupta
        /// Date : 24 Feb 2016
        /// Description: To make Release Folder to deploy on production
        /// </summary>

        //numDays gets from config, how days older files has to be taken
        static int numDays = Convert.ToInt32(ConfigurationManager.AppSettings["DaysOld"].ToString());

        //files extensions which are to be ignored
        static string[] ignoreFiles = ConfigurationManager.AppSettings["IgnoreFiles"].ToString().Split(',');

        //folders which are to be ignored
        static string[] ignoreFolders = ConfigurationManager.AppSettings["IgnoreFolders"].ToString().Split(',');

        static DateTime fromDateTime = Convert.ToDateTime(ConfigurationManager.AppSettings["FromDateTime"]);

        static void Main(string[] args)
        {
            //source folder path from where files to take
            string copyPath = ConfigurationManager.AppSettings["FolderPath"].ToString();

            //target folder path where files need to copy
            string path = ConfigurationManager.AppSettings["FileSourcePath"].ToString();

            //Console.WriteLine(fromDateTime);
            //Console.ReadLine();

            //calling function to copy files
            CopyAllFiles(path, copyPath);


            //comapre with my code logic test
            //CopyDirectory(path, copyPath, true);
        }

        /// <summary>
        /// Craeted By : vivek gupta
        /// Date : 24 feb 2016
        /// Desc: this function works recursively to create folders and sub folders and copy files to respective folders
        /// </summary>
        /// <param name="path"></param>
        /// <param name="copyPath"></param>
        public static void CopyAllFiles(string path, string targetPath)
        {

            path.Replace(@"\\", @"\");
            targetPath.Replace(@"\\", @"\");
            //it will read all files inside the folder
            foreach (string fileName in Directory.GetFiles(path))
            {
                //Console.WriteLine(fileName);

                //files extension to check ignored file list
                string fileExtension = Path.GetExtension(fileName);

                //date when file was last written to check how many days older file need to copy
                DateTime lastModifiedDate = File.GetLastWriteTime(fileName);

                //daysDiff is number of days file is older
                //int daysDiff = (DateTime.Now - lastModifiedDate).Days;                
                //Console.WriteLine(fileExtension);
                //Console.WriteLine(lastModifiedDate);

                //if (fileExtension.Contains("xml"))
                //{
                //    Console.WriteLine(fileExtension);

                //    Console.ReadLine();
                //}

                //required checks
                if (!string.IsNullOrEmpty(fileExtension) && Convert.ToInt32(Array.IndexOf(ignoreFiles, fileExtension)) < 0 && lastModifiedDate >= fromDateTime)
                {
                    //if folder does not exist, it wil create new one
                    if (!Directory.Exists(targetPath))
                    {
                        Directory.CreateDirectory(targetPath);
                    }

                    //copy files to the respective folders, it will even overwrite files if already exists
                    File.Copy(fileName, targetPath + Path.GetFileName(fileName), true);

                    //if (fileExtension.Contains("xml"))
                    //{
                    //    Console.WriteLine(fileExtension + " inside copy");

                    //    Console.ReadLine();
                    //}

                    if (fileExtension.Equals(".js") || fileExtension.Equals(".css"))
                    {
                        string newTargetPath = targetPath.Replace(@"\website\", @"\css n js\");
                        if (!Directory.Exists(newTargetPath))
                        {
                            Directory.CreateDirectory(newTargetPath);
                        }

                        //copy files to the respective folders, it will even overwrite files if already exists
                        File.Copy(fileName, newTargetPath + Path.GetFileName(fileName), true);
                    }
                }
            }

            //it will read all folders(directories) inside the folder and then call itself to create sub folders
            foreach (string directories in Directory.GetDirectories(path))
            {
                string folderName = directories.Replace(path, "");
                string newCopyPath = targetPath + folderName;

                //if (newCopyPath.Contains("obj"))
                //{
                //    Console.WriteLine(folderName);
                //    Console.WriteLine(newCopyPath + " value: " + Array.IndexOf(ignoreFolders, newCopyPath) + " ignored files : " + ignoreFolders.ToString());
                //    foreach (string i in ignoreFolders)
                //    {
                //        Console.WriteLine(i);
                //        Console.WriteLine(ignoreFolders[0]);
                //    }
                //    Console.ReadLine();
                //}

                if (Convert.ToInt32(Array.IndexOf(ignoreFolders, folderName)) < 0)
                {
                    newCopyPath = newCopyPath + @"\";
                    //Console.WriteLine(directories.Replace(path, ""));
                    //Console.WriteLine(newCopyPath);                    
                    CopyAllFiles(directories, newCopyPath);//calling itself
                }
            }
        }



        private static bool CopyDirectory(string SourcePath, string DestinationPath, bool overwriteexisting)
        {
            bool ret = true;
            try
            {
                SourcePath = SourcePath.EndsWith(@"\") ? SourcePath : SourcePath + @"\";
                DestinationPath = DestinationPath.EndsWith(@"\") ? DestinationPath : DestinationPath + @"\";

                if (Directory.Exists(SourcePath))
                {
                    if (Directory.Exists(DestinationPath) == false)
                        Directory.CreateDirectory(DestinationPath);

                    foreach (string fls in Directory.GetFiles(SourcePath))
                    {
                        FileInfo flinfo = new FileInfo(fls);
                        flinfo.CopyTo(DestinationPath + flinfo.Name, overwriteexisting);
                    }
                    foreach (string drs in Directory.GetDirectories(SourcePath))
                    {
                        DirectoryInfo drinfo = new DirectoryInfo(drs);
                        if (CopyDirectory(drs, DestinationPath + drinfo.Name, overwriteexisting) == false)
                            ret = false;
                    }
                    //Directory.CreateDirectory(DI_Target + "//Database");
                }
                else
                {
                    ret = false;
                }
            }
            catch (Exception ex)
            {
                ret = false;
            }
            return ret;
        }
    }
}
