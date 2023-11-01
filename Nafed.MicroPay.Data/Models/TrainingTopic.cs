//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Nafed.MicroPay.Data.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class TrainingTopic
    {
        public int TrainingTopicID { get; set; }
        public int TrainingID { get; set; }
        public int SkillTypeID { get; set; }
        public int SkillID { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public Nullable<int> UpdateBy { get; set; }
        public Nullable<System.DateTime> UpdateOn { get; set; }
    
        public virtual Skill Skill { get; set; }
        public virtual SkillType SkillType { get; set; }
        public virtual Training Training { get; set; }
    }
}
