using System.Collections.Generic;
using Nafed.MicroPay.Services.IServices;
namespace Nafed.MicroPay.Services.HelpDesk
{
    public interface ISupportTeamService
    {
        List<Model.SupportTeam> GetSupportTeamList();
        bool SupportTeamExists(string SupportTeam, string description);
        int InsertSupportTeam(Model.SupportTeam SupportTeam);
        Model.SupportTeam GetSupportTeamByID(int SupportTeamID);
        bool UpdateSupportTeam(Model.SupportTeam SupportTeam);
        bool Delete(int SupportTeamID);

    }
}
