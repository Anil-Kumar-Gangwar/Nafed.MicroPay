using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nafed.MicroPay.Model;
using System.ComponentModel.DataAnnotations;

namespace MicroPay.Web.Models
{
    public class SkillTypeViewModel
    {
        [Display(Name = "Deleted Records:")]
        public bool isDeleted { get; set; }
        public List<SkillType> listSkillType { get; set; }
        public UserAccessRight userRights { get; set; } = new UserAccessRight();
    }
}