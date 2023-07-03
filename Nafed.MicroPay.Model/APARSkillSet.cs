using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace Nafed.MicroPay.Model
{
    public class APARSkillSet
    {
        public int SkillSetID { get; set; }
        public int EmployeeID { get; set; }
        [Display(Name = "Department")]
        [Range(1, Int32.MaxValue, ErrorMessage = "Please select Department")]
        public int DepartmentID { get; set; }

        public string Department { get; set; }

        [Display(Name = "Designation")]
        [Range(1, Int32.MaxValue, ErrorMessage = "Please select Designation")]
        public int DesignationID { get; set; }
        public string Designation { get; set; }
        public string Skill { get; set; }
        public string SkillType { get; set; }
        public short StatusID { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public bool IsDeleted { get; set; }
    }
}
