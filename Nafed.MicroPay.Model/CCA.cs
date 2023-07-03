using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Model
{
    public class CCA    {       
        
        [Required]
        [DisplayName("Upper Limit :")]
        [DisplayFormat(DataFormatString = "{0:0.##}", ApplyFormatInEditMode = true)]
        public decimal UpperLimit { get; set; }
        [Required]
        [DisplayName("A1 :")]
        [DisplayFormat(DataFormatString = "{0:0.##}", ApplyFormatInEditMode = true)]
        public Nullable<decimal> AmtCityGradeA1 { get; set; }
        [Required]
        [DisplayName("A :")]
        [DisplayFormat(DataFormatString = "{0:0.##}", ApplyFormatInEditMode = true)]
        public Nullable<decimal> AmtCityGradeA { get; set; }
        [Required]
        [DisplayName("B1 :")]
        [DisplayFormat(DataFormatString = "{0:0.##}", ApplyFormatInEditMode = true)]
        public Nullable<decimal> AmtCityGradeB1 { get; set; }
        [Required]
        [DisplayName("B2 :")]
        [DisplayFormat(DataFormatString = "{0:0.##}", ApplyFormatInEditMode = true)]
        public Nullable<decimal> AmtCityGradeB2 { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }
    }
}
