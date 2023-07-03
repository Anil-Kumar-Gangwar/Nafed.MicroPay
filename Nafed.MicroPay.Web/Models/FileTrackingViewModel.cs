using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nafed.MicroPay.Model;
using System.ComponentModel.DataAnnotations;

namespace MicroPay.Web.Models
{
    public class FileTrackingViewModel
    {
        public FileWorkflow fileWorkFlow { get; set; }
        public FileManagement fileManagement { get; set; }

        public List<FileWorkflow> fileWorkFlowList { get; set; }
        public List<FileManagement> fileManagementList { get; set; }
        public bool IsEligibleForFTMS { get; set; }

        public List<FileTrackingDocuments> documentFiles { get; set; }

        public ProcessWorkFlow processWorkFlow { get; set; }
        public List<SelectListModel> lstEmployeeList = new List<SelectListModel>();

    }

}