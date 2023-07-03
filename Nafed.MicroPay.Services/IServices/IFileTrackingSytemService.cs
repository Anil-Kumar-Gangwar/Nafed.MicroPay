using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Services.IServices
{
  public interface IFileTrackingSytemService:IBaseService
    {
        List<Model.FileWorkflow> GetFileTrackingList();
        bool InsertFileTracking(Model.FileWorkflow fileWorkFlow);
        bool UpdateFileTracking(Model.FileWorkflow fileWorkFlow);       
        List<Model.FileWorkflow> GetFileTrackingListForPopup(int departmentID);

        #region File Tracking Type

        List<Model.FileTrackingType> GetFileTrackingTypeList();


        bool FileTypeExists(string fileType);


        int InsertFileTrackingType(Model.FileTrackingType fileType);

        Model.FileTrackingType GetFileTrackingTypeByID(int id);

        bool UpdateFileTrackingType(Model.FileTrackingType fileType);

        bool Delete(int id);
        #endregion

        #region  File No Generate
        bool IsEmpEligibleForFTMS(int employeeID);
        Model.FileManagement GetFileInitiatorDetail(int employeeID);
        bool GenerateFile(Model.FileManagementSystem fileManagement);
        List<Model.FileManagement> GetFileList(int userID);
        Model.FileManagementSystem GetFileDetails(int fileID);
        bool UpdateFileDetail(Model.FileManagementSystem fileManagement);
        bool DeleteDocument(int docID, out string DocName);
        #endregion

        bool ForwardFile(Model.ProcessWorkFlow pWorkFlow);
        List<Model.FileManagement> GetInboxFileList(int receiverID);
        List<Model.FileManagement> GetOutboxFileList(int senderID);
        List<Model.FileManagement> GetFileForwardDetails(int referenceID);
        bool TabForwardFile(Model.FileManagementSystem fileManagement);
        List<Model.FileManagement> GetFileListForGridView(Model.FileManagement fileManagement);
        bool FileClosed(Model.ProcessWorkFlow pWorkFlow);
        bool FileReceive(Model.ProcessWorkFlow pWorkFlow);
        bool FileRollback(Model.ProcessWorkFlow pWorkFlow);
        bool ValidateUser(Model.ValidateLogin userCredential, out int userId);
    }
}
