using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
//using Nafed.MicroPay.Model;
using System.ComponentModel.DataAnnotations;
using System.Collections;
using Model = Nafed.MicroPay.Model;
namespace MicroPay.Web.Models
{
    public class EmpPFOrgViewModel
    {
        public List<Model.EmployeePFORG> EmpPFOrgList { get; set; }
        public int EmpPFID { get; set; }

        public Model.UserAccessRight userRights { get; set; } = new Model.UserAccessRight();
     
       
        public IEnumerable<Model.SelectListModel> EmployeeList { set; get; }
        [Display(Name = "Employee")]
        public Nullable<int> EmployeeId { get; set; }

        [Display(Name = "Code")]
        public string EmployeeCode { set; get; }



        [Display(Name = "Name")]
        public string Employeename { get; set; }
        //public Model.EmployeeProcessApproval EmpProceeApproval { set; get; } = new Model.EmployeeProcessApproval();
       public Model.EmployeeProcessApproval approvalSetting { set; get; } = new Model.EmployeeProcessApproval();

    }
}