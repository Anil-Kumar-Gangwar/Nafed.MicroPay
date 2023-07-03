using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nafed.MicroPay.Model;
using Nafed.MicroPay.Services.IServices;

namespace Nafed.MicroPay.Services.IServices
{
   public interface IStaffLeaveDetailsService
    {
        
        List<EmployeeLeave> GetStaffLeaveDetailsList(Model.EmployeeLeave empLeave, int? reportingTo, int? revwingTo, int empId);
        List<EmployeeLeave> GetEmployeeLeaveDetailsList(Model.EmployeeLeave empLeave, int? reportingTo, int? revwingTo, int empId);

    }
}
