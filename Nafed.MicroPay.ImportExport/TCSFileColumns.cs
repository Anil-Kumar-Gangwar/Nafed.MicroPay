using System.Collections.Generic;

namespace Nafed.MicroPay.ImportExport
{
    public class TCSFileColumns
    {
        private TCSFileColumns()
        {

        }

        public static List<string> GetTCSFileColumns()
        {
            return new List<string>() {
                "loctcd",
                 "mmyy" ,
                 "emplcd" ,
                 "emplname" ,
                 "comp_dep",
                 "em_loan",
                 "em_ln_int",
                 "gl_loan",
                 "gl_ln_int",
                 "recr_dep",
                 "misc_dedn1" ,
                 "misc_dedn2" ,
                 "tcsln",
                 "gl_bal","el-bal"
            };
        }
    }
}
