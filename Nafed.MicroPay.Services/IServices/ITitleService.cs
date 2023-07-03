using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nafed.MicroPay.Model;
using Nafed.MicroPay.Services.IServices;

namespace Nafed.MicroPay.Services.IServices
{
    public interface ITitleService
    {
        List<Title> GetTitleList();
        bool TitleNameExists(int? titleID, string titleName);
        bool UpdateTitle(Model.Title editTitle);
        bool InsertTitle(Model.Title createTitle);
        Model.Title GetTitleByID(int titleID);
        bool Delete(int titleID);
    }
}
