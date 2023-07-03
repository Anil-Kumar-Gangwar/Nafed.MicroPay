using Nafed.MicroPay.Model;
using System;
using System.Collections.Generic;

namespace Nafed.MicroPay.Services.IServices
{
    public interface IDashBoardService
    {
        IEnumerable<Model.Dashboard> GetEmployeeDetailForDashBoard(int? empCode);

        IEnumerable<Model.EmployeeDobDoj> GetEmployeeDOBWorkAnniversary(int? branchID, DateTime ? todayDate);

        IEnumerable<Model.DashboardDocuments> GetDashboardDocumentList();
        List<SelectListModel> GetDocumentType();
        bool SaveDocument(DashboardDocuments document);
        bool DeleteDocument(int documentID);
        UserDetail GetUserInfo(string userName);

        Dashboard GetDashBoardForRetiredEmployee(int employeeID);


    }
}
