using System.Collections.Generic;
using Model = Nafed.MicroPay.Model;

namespace Nafed.MicroPay.Services.IServices
{
    public interface IUpdateBasicService
    {
        List<Model.UpdateBasic> GetUpdateBasicList();
        bool UpdateBasic(Model.UpdateBasic editCadreItem);
        Model.UpdateBasic GetBasic(int EmployeeID);
        int ValidateNewBasicAmount(int EmployeeID, double newBasic);
        
        List<Model.UpdateBasic> GetBranchDesignationList();
        List<Model.UpdateBasic> GetCurrentDesignationBranch(string EmployeeCode);
        Model.UpdateBasic GetBranchDesignation(int EmployeeID);
        bool UpdateBranchDesignation(Model.UpdateBasic editUpdateBranchDesignation);
    }
}
