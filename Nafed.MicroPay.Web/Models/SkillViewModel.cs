using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nafed.MicroPay.Model;
using System.ComponentModel.DataAnnotations;

namespace MicroPay.Web.Models
{
    public class SkillViewModel
    {
        public List<Skill> listSkill { get; set; }
        public UserAccessRight userRights { get; set; } = new UserAccessRight();
        [Display(Name = "Deleted Records:")]
        public bool isDeleted { get; set; }
    }
}