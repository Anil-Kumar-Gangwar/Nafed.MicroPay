using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nafed.MicroPay.Data.Repositories.IRepositories;
using Nafed.MicroPay.Model;
using Nafed.MicroPay.Common;
using static Nafed.MicroPay.Services.ArchiveData.ArchiveFiles;
using System.IO;

namespace Nafed.MicroPay.Services.ArchiveData
{
    public class ArchiveDataService : BaseService, IArchiveDataService
    {
        private readonly IArchiveDataRepository archiveDataRepo;
        public ArchiveDataService(IArchiveDataRepository archiveDataRepo)
        {

            this.archiveDataRepo = archiveDataRepo;

        }

        public bool ArchiveData(int userID ,DateTime fromdate, DateTime todate)
        {
            log.Info($"ArchiveDataService/ArchiveData/fromdata={fromdate},todate={todate}");
            try
            {
                bool res = false;
                var indexs = new Dictionary<int, int>();
                indexs.Add(1, 36);
                indexs.Add(37, 104);
                res = MoveFilesSourceToDestination(fromdate, todate, DocumentUploadFilePath.ArchiveDataPath);
                if (res)
                {
                    res = archiveDataRepo.ArchiveData(userID,fromdate, todate, indexs);

                }
                return res;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }

        }

        public IEnumerable<ArchivedDataTransaction> GetArchivedDataTransList()
        {
            log.Info($"ArchiveDataService/GetArchivedDataTransList");
            try
            {
                IEnumerable<ArchivedDataTransaction> transList = Enumerable.Empty<ArchivedDataTransaction>();

                var transactions = archiveDataRepo.GetArchivedDataTransList();
                transList = ExtensionMethods.ConvertToList<ArchivedDataTransaction>(transactions);
                return transList.ToList();
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public long DirectorySize(DirectoryInfo sourceDir, string destDirName, out long destDirAvailSize)
        {
            long destinationAvailSize = 0;

            destinationAvailSize = DestinationDirectorySize(destDirName);
            destDirAvailSize = destinationAvailSize;
            return SourceDirectorySize(sourceDir);

        }

        long DestinationDirectorySize(string destDirName)
        {
            DriveInfo dInfo = new DriveInfo(destDirName);

            long availableSpace = dInfo.AvailableFreeSpace;
            return availableSpace;
        }

        long SourceDirectorySize(DirectoryInfo sourceDir)
        {
            long totalSize = sourceDir.EnumerateFiles()
                         .Sum(file => file.Length);

            totalSize += sourceDir.EnumerateDirectories()
                     .Sum(dir => SourceDirectorySize(dir));
            return totalSize;
        }

        public bool CheckArchiveExistForGivenYr(int year)
        {
            log.Info($"ArchiveDataService/CheckArchiveExistForGivenYr/year={year}");
            try
            {               
               return archiveDataRepo.CheckArchiveExistForGivenYr(year);              
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }

        }
    }
}
