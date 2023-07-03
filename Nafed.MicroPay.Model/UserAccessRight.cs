using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Model
{
    public class UserAccessRight:BaseEntity
    {
        public int DepartmentID { get; set; }
        public int UserID { get; set; }
        public int MenuID { get; set; }
        public bool ShowMenu { get; set; }
        public bool Create { get; set; }
        public bool Edit { get; set; }
        public bool View { get; set; }
        public bool Delete { get; set; }
        public Menu menu { set; get; }
        public string HelpText { set; get; }
    }
}
