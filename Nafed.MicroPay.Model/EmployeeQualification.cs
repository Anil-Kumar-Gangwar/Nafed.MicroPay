using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Model
{
    public class EmployeeQualification
    {
        public List<Model.Employee> Employee { set; get; }

        public Employee _Employee { get; set; }

        public List<EmployeeQualificationM> EmployeeQualificationList = new List<EmployeeQualificationM>();
     
        public int[] CheckBoxListAcademicvalue { get; set; }
        public int[] CheckBoxListProfessionalvalue { get; set; }
    }
}
