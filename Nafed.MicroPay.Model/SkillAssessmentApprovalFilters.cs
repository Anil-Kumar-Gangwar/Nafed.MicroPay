using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Model
{

    public class SkillAssessmentApprovalFilters
    {
        public int formID { set; get; }
        public List<SelectListModel> appraisalForms { set; get; }
        public List<SelectListModel> reportingYrs { set; get; } = new List<SelectListModel>();
        public List<SelectListModel> employees { set; get; } = new List<SelectListModel>();

        public int selectedFormID { set; get; }
        public int selectedReportingYr { set; get; }
        public int selectedEmployeeID { set; get; }

        public int loggedInEmployeeID { get; set; }
        public string reportingYr { get; set; }
        public CompetencyFormState competencyFormState { get; set; }
    }
}
