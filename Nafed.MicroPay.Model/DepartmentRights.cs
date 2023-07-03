using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace Nafed.MicroPay.Model
{
   public class DepartmentRights
    {
        public int MenuID { get; set; }

        [Display(Name = "Department")]
        public string DepartmentName { get; set; }

        public int DepartmentID { get; set; }

        public string hdnCheckedVal { get; set; }

        public bool ShowMenu { get; set; }
        public bool isAll { get; set; }
    }
}
