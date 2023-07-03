using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Nafed.MicroPay.Model
{
    public class Form11FormApprovalFilter
    {
        public List<SelectListModel> employees { set; get; } = new List<SelectListModel>();
        public int selectedEmployeeID { set; get; }
        public List<SelectListModel> confirmationForms { set; get; }
        public int selectedFormID { set; get; }
        public int loggedInEmployeeID { get; set; }
        public int selectedProcessID { set; get; }
        public List<SelectListModel> processList { set; get; } = new List<SelectListModel>();
    }
}
