using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Services.IServices
{
   public interface IAcadmicProfessionalDetails
    {
        List<Model.AcadmicProfessionalDetailsModel> GetAcadmicProfessionalList(int? typeID);
        bool AcadmicProfessionalDetailsExists(int? id, string value, int typeID);

        int InsertAcadmicProfessionalDetails(Model.AcadmicProfessionalDetailsModel createAcadmicProfessionalDetails);

        Model.AcadmicProfessionalDetailsModel GetAcadmicProfessionalDetailsByID(int acadmicProfessionalDetailsID);

        bool UpdateAcadmicProfessionalDetails(Model.AcadmicProfessionalDetailsModel editAcadmicProfessionalDetails);

        bool Delete(int acadmicProfessionalDetailsId);
    }
}
