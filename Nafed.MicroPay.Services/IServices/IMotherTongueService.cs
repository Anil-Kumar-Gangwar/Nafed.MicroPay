using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Services.IServices
{
    public interface IMotherTongueService
    {
        List<Model.MotherTongueModel> GetMotherTongueList();

        bool MotherTongueExists(int? iD, string value);

        int InsertMotherTongueDetails(Model.MotherTongueModel createMotherTongueDetails);

        Model.MotherTongueModel GetmotherTongueDetailsByID(int motherTongueId);

        bool UpdateMotherTongueDetails(Model.MotherTongueModel createMotherTongue);

        bool Delete(int motherTongueId);
    }
}
