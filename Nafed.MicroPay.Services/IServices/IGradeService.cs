using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nafed.MicroPay.Model;
using Nafed.MicroPay.Services.IServices; 

namespace Nafed.MicroPay.Services.IServices
{
    public interface IGradeService 
    {
        List<Grade> GetGradeList();
        bool GradeNameExists(int? gradeID, string gradeName);
        bool UpdateGade(Model.Grade editGradeEntity);
        bool InsertGrade(Model.Grade createGrade);
        Model.Grade GetGradeByID(int gradeID);
        bool Delete(int gradeID);
    }
}
