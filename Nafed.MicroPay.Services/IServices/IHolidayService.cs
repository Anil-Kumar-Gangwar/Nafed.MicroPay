using System;
using System.Collections.Generic;
using Model = Nafed.MicroPay.Model;
namespace Nafed.MicroPay.Services.IServices
{
    public interface IHolidayService
    {
        List<Model.Holiday> GetHolidayList(int? branchID, int? CYear);
        bool HolidayNameExists(DateTime? HolidayDate, string HolidayName, int? BranchId, int? CYear);


        bool UpdateHoliday(Model.Holiday editHolidayItem);
        bool InsertHoliday(Model.Holiday createHoliday);
        Model.Holiday GetHolidayByID(int HolidayID);

        bool Delete(int holidayID);
    }
}
