using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace Nafed.MicroPay.Model
{
    public class TrainingRating
    {
        public int trainingID { set; get; }
        public List<string> headerCols { set; get; }
        public DataTable empRatingRows { set; get; }

        public List<double> avgRating { set; get; }

        public string TrainingTitle { get; set; }

        public dynamic trainingVM { set; get; }

        public string TrainingVenue { set; get; }
    }
}
