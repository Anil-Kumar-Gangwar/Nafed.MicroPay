using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Model
{
    public class FTSUserDepartment
    {
        public int id { get; set; }
        public int UserId { get; set; }
        [Display(Name = "Department")]
        public int DepartmentId { get; set; }        
        public int[] intDepartmentId { get; set; }
        public List<SelectListModel> lstDepartmentList = new List<SelectListModel>();
        public string UserName { get; set; }
    }
}
