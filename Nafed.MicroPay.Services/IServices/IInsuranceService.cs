using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Services.IServices
{
    public interface IInsuranceService
    {
        List<Model.Insurance> GetInsuranceList(int? employeeID, int? branchID=null,bool? Status=null);
        bool InsertInsuranceDetails(Model.Insurance Insurance);
        Model.Insurance GetInsuranceByID(int employeeid, int insuranceId);

        bool UpdateInsuranceDetails(Model.Insurance Insurance);

        bool DeleteInsuranceDetails(int empInsuranceID);
        List<Model.InsuranceDependent> GetDependentList(int employeeId);
        decimal? GetFmilyAssuredByDesignationId(int employeeId, out string designation);

    }
}
