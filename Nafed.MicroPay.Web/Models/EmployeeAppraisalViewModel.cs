using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nafed.MicroPay.Model;


namespace MicroPay.Web.Models
{
   public class EmployeeAppraisalViewModel
    {
        public List<AppraisalFormHdr> formList { get; set; } = new List<AppraisalFormHdr>();
        public UserAccessRight userRights { get; set; }

        public int ? empAppraisalFormID { set; get; }
        public EmployeeProcessApproval approvalSetting { set; get; } = new EmployeeProcessApproval();
    }
}
