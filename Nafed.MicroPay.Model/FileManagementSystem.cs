using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Model
{
   public class FileManagementSystem
    {
        public FileManagement fileManagement = new FileManagement();
        public List<FileManagement> fileManagemenList = new List<FileManagement>();
        public List<FileTrackingDocuments> fileDocumentsList = new List<FileTrackingDocuments>();

        public ProcessWorkFlow processWorkFlow { get; set; }
    }
}
