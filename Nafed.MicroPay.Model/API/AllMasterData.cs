using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Model.API
{
    public class AllMasterData
    {
        public IEnumerable<EmpAttendance> EmployeesAttendance { get; set; }
        public IEnumerable<LeaveRule> LeaveRule { get; set; }
        public IEnumerable<Employee> EmployeeDetails { get; set; }
        public IEnumerable<Title> Title { get; set; }
        public IEnumerable<EmployeeType> EmployeeType { get; set; }
        public IEnumerable<Branch> Branch { get; set; }
        public IEnumerable<Department> Department { get; set; }
        public IEnumerable<Designation> Designation { get; set; }
        public IEnumerable<Category> Category { get; set; }
        public IEnumerable<Religion> Religion { get; set; }
        public IEnumerable<MotherTongueModel> MotherTongue { get; set; }
        public IEnumerable<MaritalStatus> MaritalStatus { get; set; }
        public IEnumerable<Cadre> Cadre { get; set; }
        public IEnumerable<Division> Division { get; set; }
        public IEnumerable<Section> Section { get; set; }
        public IEnumerable<Relation> Relation { get; set; }
        public IEnumerable<BloodGroup> BloodGroup { get; set; }
        public IEnumerable<EmployeeCategory> EmployeeCategory { get; set; }
        public IEnumerable<Gender> Gender { get; set; }

    }
}
