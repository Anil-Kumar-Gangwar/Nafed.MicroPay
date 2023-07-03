using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Services.IServices
{
   public interface ISkillService
    {
        #region Skill Type
        List<Model.SkillType> GetSkillTypeList(bool isDeleted);
        bool SkillTypeExists(int skillTypeID, string skillType);
        bool InsertSkillType(Model.SkillType createSkillType);
        Model.SkillType GetSkillTypeID(int skillTypeID);
        bool UpdateSkillType(Model.SkillType editSkillType);
        bool Delete(int cadreID);
        #endregion
        #region Skill
        List<Model.Skill> GetSkillList(bool isDeleted);
        bool SkillExists(int skillID, string skill);
        bool InsertSkill(Model.Skill createSkill);
        Model.Skill GetSkillByID(int skillID);
        bool UpdateSkill(Model.Skill editSkill);
        bool DeleteSkill(int skillID);
        #endregion
    }
}
