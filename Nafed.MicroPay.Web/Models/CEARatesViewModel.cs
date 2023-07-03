using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Model = Nafed.MicroPay.Model;
namespace MicroPay.Web.Models
{
    public class CEARatesViewModel
    {
        public List<Model.CEARates> CEARatesList { set; get; }
        public Model.UserAccessRight userRights { get; set; } = new Model.UserAccessRight();
    }
}