using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Model
{
    public class EmployeeForm12BBDocument
    {

        public int Form12BBDocumentID { get; set; }
        public int EmpFormID { get; set; }
        public string NatureOfClaim { get; set; }
        public string FileName { get; set; }
        public string FileDescription { get; set; }
        public Nullable<int> SectionID { get; set; }
        public Nullable<int> SubSectionID { get; set; }
        public Nullable<int> SubSectionDescriptionID { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }

       
    }
}
