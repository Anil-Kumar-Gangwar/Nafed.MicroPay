using System.Collections.Generic;

namespace Nafed.MicroPay.Common
{
    public class SharedControllers
    {
        private SharedControllers()
        {
        }

        public static readonly List<string> controllers = new List<string>() {
          
            "base",
            "employeeprofile",
            "salaryheadfieldsdescription",
            "salaryheadrules","approvalrequest","designationappraisalform","changepassword",
            "designationpayscale","designationassignment","branchtransfer","stafftransfer",
           "confirmationform","candidateregistration","joblist","candidateregistration","genrateregistrationno","requirement",
         "trainingfeedbackform","importmonthlyinput","sanctionloan","quickmonthlyinput","salaryreport","adjustoldloan",
           "documentmanagement","filetrackingsystem","competencyapar","filetrackingtype","generatefileno","publishsalary","appraisal",
           "aparskills","ltc","crreport","prheader","propertyreturn","loanapplication","otaslip","epfnomination","ticket","acarapar",
            "salaryreport","empwisesalaryreport","childreneducation","viewholidays","conveyancebill","employeepforganisationheader",
            "subordinatetraining","taxdeduction","taxdeductionsection","nonrefundablepfloan","insurance","helpdesk","ftsuser","separation","importexgratiaincometax","daarrearreports"
        };
    }
}
