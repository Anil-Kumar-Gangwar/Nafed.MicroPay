using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nafed.MicroPay.Model; 

namespace MicroPay.Web.Models
{
    public class DesignationAssignmentVM
    {
        public DesignationAssignment designationAssignment { set; get; }
        public UserAccessRight userRights { get; set; } = new UserAccessRight();
    }
}