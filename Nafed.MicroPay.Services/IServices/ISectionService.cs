using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Services.IServices
{
  public interface ISectionService
    {
        List<Model.Section> GetSectionList();
        bool SectionNameExists(int? SectionID, string SectionName);
        bool SectionCodeExists(int? SectionID, string SectionCode);
        bool UpdateSection(Model.Section editSectionItem);
        bool InsertSection(Model.Section createSection);
        Model.Section GetSectionByID(int SectionID);
        bool Delete(int SectionID);
    }
}
