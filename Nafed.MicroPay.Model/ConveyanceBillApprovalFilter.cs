using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Model
{
    public class ConveyanceBillApprovalFilter
    {
        public List<SelectListModel> employees { set; get; } = new List<SelectListModel>();
        public int selectedEmployeeID { set; get; }
        public Nullable<DateTime> FromDate { get; set; }
        public Nullable<DateTime> ToDate { get; set; }
        public int loggedInEmployeeID { get; set; }
        public ConveyanceFormStatus conveyanceFormState { get; set; }
        public int? StatusId { get; set; }
    }
}
