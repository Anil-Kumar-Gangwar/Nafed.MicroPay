using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nafed.MicroPay.Data.Models;
using Nafed.MicroPay.Data.Repositories.IRepositories;
namespace Nafed.MicroPay.Data.Repositories
{
    public class FileManagementSystemRepository : BaseRepository, IFileManagementSystemRepository
    {
        public List<GetInboxFiles_Result> GetInboxFilesList(int receiverID)
        {
            return db.GetInboxFiles(receiverID).ToList();
        }

        public List<GetOutboxFiles_Result> GetOutboxFilesList(int senderID)
        {
            return db.GetOutboxFiles(senderID).ToList();
        }

        public List<GetFileForwardDetails_Result> GetFileForwardList(int referenceID)
        {
            return db.GetFileForwardDetails(referenceID).ToList();
        }

        public List<GetFileTrackingListForSearch_Result> GetFileListForGridView(string fileNo,DateTime? fromdate,DateTime? todate)
        {
            return db.GetFileTrackingListForSearch(fileNo, fromdate, todate).ToList();
        }
    }
}
