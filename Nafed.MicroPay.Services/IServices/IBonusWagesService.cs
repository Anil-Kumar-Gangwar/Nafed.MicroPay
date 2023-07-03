using System;
using System.Collections.Generic;
using Model = Nafed.MicroPay.Model;
using System.Data;

namespace Nafed.MicroPay.Services.IServices
{
    public interface IBonusWagesService
    {
        List<Model.BonusWages> GetBonusWagesList();
        bool BonusWagesExists(DateTime? fromdate, DateTime? todate);
        bool InsertBonusWages(Model.BonusWages createBonusWages);
        bool UpdateBonusWages(Model.BonusWages editBonusWages);
        Model.BonusWages GetBonusWagesByID(int ID);
        bool Delete(int ID);

        List<Model.BonusWages> GetBonusMiimumWagesList();
        Model.BonusWages GetBonusminimumWagesByID(int ID);
        bool UpdateBonusMinimumWages(Model.BonusWages editBonusWages);

        List<Model.BonusWages> GetBonusList(int fyear, int? branchID, int EmpTypeID);
        int calculateBonus(decimal bonusrate, string fromYear, string toYear, int branchID, int fYear, int empType);
        bool DeleteBonusAmount(int ID,int UserID);
        DataTable GetBonusExport(string fromYear, string toYear, int fyear, int branchID, int emptype);
    }
}
