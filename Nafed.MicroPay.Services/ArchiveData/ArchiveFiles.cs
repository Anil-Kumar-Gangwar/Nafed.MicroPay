using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Services.ArchiveData
{
    public class ArchiveFiles
    {
        public static bool MoveFilesSourceToDestination(DateTime startDate, DateTime endDate, string destParentDirectory)
        {
            try
            {
                if (!Directory.Exists(destParentDirectory)) // check if parent directory is exist on destination or not, if not then create.
                    Directory.CreateDirectory(destParentDirectory);

                var sourceParentDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Document"); // Souce Parent Directory.
                string[] subdirectoryEntries = Directory.GetDirectories(sourceParentDirectory);// get sub directories from Source path.
                foreach (var item in subdirectoryEntries)
                {
                    DirectoryInfo subdirectory = new DirectoryInfo(item);
                    var destSubDirectory = Path.Combine(destParentDirectory, subdirectory.Name);// create path for destination sub directory.
                    if (!Directory.Exists(destSubDirectory))// check if sub directory is exist on destination or not, if not then create.
                        Directory.CreateDirectory(destSubDirectory);

                    FileInfo[] files = subdirectory.GetFiles(); // get all files from sub directory.
                    List<string> filestoMove = files.Where(fi => fi.CreationTime >= startDate && fi.CreationTime <= endDate)
                                               .Select(fi => fi.FullName)
                                               .ToList(); // get files from sub directory based on date filter
                    if (filestoMove.Count() > 0)
                    {
                        foreach (string file in filestoMove)
                        {
                            DirectoryInfo dr = new DirectoryInfo(file);
                            File.Copy(file, Path.Combine(destSubDirectory, dr.Name)); // copy file from source to destination
                            File.Delete(file); // delete file from source.
                        }
                    }
                }

                // Move all files from source to destination, but do not delete these files from source.
                foreach (var item in subdirectoryEntries)
                {
                    DirectoryInfo subdirectory = new DirectoryInfo(item);
                    var destSubDirectory = Path.Combine(destParentDirectory, subdirectory.Name);// create path for destination sub directory.
                    FileInfo[] files = subdirectory.GetFiles(); // get all files from sub directory.
                    List<string> filestoMove = files.Select(fi => fi.FullName)
                                               .ToList(); // get files from sub directory based on date filter
                    if (filestoMove.Count() > 0)
                    {
                        foreach (string file in filestoMove)
                        {
                            DirectoryInfo dr = new DirectoryInfo(file);
                            if (!File.Exists(Path.Combine(destSubDirectory, dr.Name)))
                                File.Copy(file, Path.Combine(destSubDirectory, dr.Name)); // copy file from source to destination                           
                        }
                    }
                }

                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
