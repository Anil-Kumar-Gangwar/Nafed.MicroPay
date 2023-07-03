using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nafed.MicroPay.Model;
using System.ComponentModel.DataAnnotations;

namespace MicroPay.Web.Models
{
    public class UploadForm12BBDocument
    {
        public int ClaimDocumentID { set; get; }

        [Display(Name ="Nature of Claim")]
        [Required(ErrorMessage = "Please select nature of claim.")]
        public string selectedClaim { set; get; }

        [Display(Name = "Section")]
        [Required(ErrorMessage = "Please select section.")]
        public int ? sectionID { set; get; }

        [Display(Name = "Sub Section")]
        [Required(ErrorMessage = "Please select sub section.")]
        public int ? subSectionID { set; get; }

        [Display(Name = "Sub Section Description")]
        [Required(ErrorMessage = "Please select sub section description.")]
        public int ? subSectionDescriptionID { set; get; }

       
        public List<SelectListItem> claimList { set; get; } = new List<SelectListItem>();

        public List<SelectListModel> sections { set; get; } = new List<SelectListModel>();

        public List<SelectListModel> subSections { set; get; } = new List<SelectListModel>();

        public List<SelectListModel> subSectionDescriptions { set; get; } = new List<SelectListModel>();

        [Display(Name = "Upload Document")]
        public string FileName { set; get; }

        [Display(Name = "File Description")]
        [Required(ErrorMessage = "Please Enter File Description.")]

        public string FileDescription { set; get; }

        public int EmpFormID { set; get; }
        public int EmployeeID { set; get; }
        public int CreatedBy { set; get; }
        public DateTime CreatedOn { set; get; }
        public bool IsDeleted { set; get; }
    }
}