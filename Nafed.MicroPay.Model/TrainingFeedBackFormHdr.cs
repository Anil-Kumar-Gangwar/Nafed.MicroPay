using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Model
{
   public class TrainingFeedBackFormHdr
    {
        public int FeedBackFormHdrID { get; set; }
        public int TrainingID { get; set; }
        public string RatingType { get; set; }
        public Nullable<byte> LowerRatingValue { get; set; }
        public Nullable<byte> UpperRatingValue { get; set; }
        public string ActionPlan { get; set; }
    }
}
