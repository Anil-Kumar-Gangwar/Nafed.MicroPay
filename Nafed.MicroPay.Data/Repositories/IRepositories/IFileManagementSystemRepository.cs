using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTOModel = Nafed.MicroPay.Data.Models;
namespace Nafed.MicroPay.Data.Repositories.IRepositories
{
    public interface IFileManagementSystemRepository : IDisposable
    {
        List<DTOModel.GetInboxFiles_Result> GetInboxFilesList(int receiverID);
        List<DTOModel.GetOutboxFiles_Result> GetOutboxFilesList(int senderID);
        List<DTOModel.GetFileForwardDetails_Result> GetFileForwardList(int referenceID);
        List<DTOModel.GetFileTrackingListForSearch_Result> GetFileListForGridView(string fileNo, DateTime? fromdate, DateTime? todate);
    }
}
