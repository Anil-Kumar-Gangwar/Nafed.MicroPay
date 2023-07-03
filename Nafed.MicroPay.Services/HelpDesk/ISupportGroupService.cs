using System.Collections.Generic;
using Nafed.MicroPay.Services.IServices;
namespace Nafed.MicroPay.Services.HelpDesk
{
    public interface ISupportGroupService
    {
        List<Model.SupportGroup> GetSupportGroupList();
        bool SupportGroupExists(string supportGroup, string description);
        int InsertSupportGroup(Model.SupportGroup supportGroup);
        Model.SupportGroup GetSupportGroupByID(int supportGroupID);
        bool UpdateSupportGroup(Model.SupportGroup supportGroup);
        bool Delete(int supportGroupID);

    }
}
