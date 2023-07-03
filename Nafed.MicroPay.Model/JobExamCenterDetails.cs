using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Nafed.MicroPay.Model
{
    public class JobExamCenterDetails
    {
        public int sno { set; get; }
        public int ExamCenterID { get; set; }
        public int RequirementID { get; set; }
        public int LocTypeID { get; set; }
        [Range(1, Int32.MaxValue, ErrorMessage = "Please Select Location")]
        public int SelectedLocID { get; set; }
        [Required(ErrorMessage = "Please Enter Exam Center Address")]
        public string ExamCentreAddress { get; set; }
        [Required(ErrorMessage = "Please Enter Reporting Time")]
        public System.TimeSpan ReportingTime { get; set; }
        [Required(ErrorMessage = "Please Enter Entry Close Time")]
        public System.TimeSpan EntryCloseTime { get; set; }
        [Required(ErrorMessage = "Please Enter Exam Timing")]
        public System.TimeSpan ExamTiming { get; set; }
        //public List<Model.SelectListModel> LocationList { set; get; }
    }
}
