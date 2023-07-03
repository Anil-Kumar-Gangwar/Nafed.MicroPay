using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Model = Nafed.MicroPay.Model;

namespace MicroPay.Web.Models
{
    public class NonRefunPFLoanDataViewModel
    {
        public List<Model.NonRefundablePFLoan> NRPFLoanList { set; get; }
        public Model.UserAccessRight userRights { get; set; } = new Model.UserAccessRight();

      
        [Display(Name = "Employee")]
        public int EmployeeId { get; set; }
        [Display(Name = "Branch")]


        public int BranchID { get; set; }
        public Model.EmployeeProcessApproval approvalSetting { set; get; } = new Model.EmployeeProcessApproval();
    }
}