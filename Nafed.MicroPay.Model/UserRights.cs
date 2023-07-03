using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace Nafed.MicroPay.Model
{
   public class UserRights
    {
        public int UserID { get; set; }
        public int MenuID { get; set; }
        public int DepartmentID { get; set; }        
        public string UserName { get; set; }
        public string hdnCheckedVal { get; set; }
        public bool ShowMenu { get; set; }
        public bool isAll { get; set; }
    }
}
