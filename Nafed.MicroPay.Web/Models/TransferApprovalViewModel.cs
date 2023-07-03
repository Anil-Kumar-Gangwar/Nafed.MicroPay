using Nafed.MicroPay.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MicroPay.Web.Models
{
    public class TransferApprovalViewModel
    {
        public int type { get; set; }
        public int? ProcessId { get; set; }
        public List<SelectListModel> ProcessList { set; get; }
        public int fromEmployeeID { get; set; }
        public string fromEmployeeName { get; set; }
        public string fromDesignationName { get; set; }
        public string fromEmployeeCode { get; set; }
        public string fromEmployeeDepartment { get; set; }
        public string fromEmployeeBranch { get; set; }
        public string fromDepartmentName { get; set; }

        public int toEmployeeID { get; set; }
        public string toEmployeeName { get; set; }
        public string toDesignationName { get; set; }
        public string toEmployeeCode { get; set; }
        public string toEmployeeDepartment { get; set; }
        public string toEmployeeBranch { get; set; }
        public string toDepartmentName { get; set; }
        public List<TrainingParticipantsDetail> employeeDetail { set; get; }
    }
}