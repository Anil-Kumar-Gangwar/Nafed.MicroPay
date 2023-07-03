using Nafed.MicroPay.Services.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nafed.MicroPay.Model;
using Nafed.MicroPay.Data.Repositories.IRepositories;
using AutoMapper;

namespace Nafed.MicroPay.Services
{
    public class TitleService : BaseService, ITitleService
    {
        private readonly IGenericRepository genericRepo;
        public TitleService(IGenericRepository genericRepo)
        {
            this.genericRepo = genericRepo;
        }
        public bool Delete(int titleID)
        {
            log.Info($"TitleService/Delete");
            bool flag = false;
            try
            {
                Data.Models.Title dtoTitle = new Data.Models.Title();
                dtoTitle = genericRepo.GetByID<Data.Models.Title>(titleID);
                dtoTitle.IsDeleted = true;
                genericRepo.Update<Data.Models.Title>(dtoTitle);
                flag = true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return flag;
        }

        public Title GetTitleByID(int titleID)
        {
            log.Info($"TitleService/GetTitleByID/{titleID}");

            try
            {
                var titleObj = genericRepo.GetByID<Data.Models.Title>(titleID);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Data.Models.Title, Model.Title>()

                    .ForMember(c => c.TitleName, c => c.MapFrom(m => m.TitleName))
                    .ForAllOtherMembers(c => c.Ignore());
                });
                var obj = Mapper.Map<Data.Models.Title, Model.Title>(titleObj);
                return obj;
            }
            catch (Exception ex)
            {

                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public List<Title> GetTitleList()
        {
            log.Info($"TitleService/GetTitleList");

            try
            {
                var result = genericRepo.Get<Data.Models.Title>(x => x.IsDeleted == false);
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Data.Models.Title, Model.Title>();
                });
                var listTitle = Mapper.Map<List<Model.Title>>(result);
                return listTitle.OrderBy(x => x.TitleName).ToList();
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }

        public bool InsertTitle(Title createTitle)
        {
            log.Info($"TitleService/InsertTitle");

           
            bool flag = false;
            try
            {
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Model.Title, Data.Models.Title>()
                    .ForMember(c => c.TitleName, c => c.MapFrom(m => m.TitleName))
                    .ForMember(c => c.CreatedBy, c => c.MapFrom(m => m.CreatedBy))
                    .ForMember(c => c.CreatedOn, c => c.MapFrom(m => m.CreatedOn))
                    .ForAllOtherMembers(c => c.Ignore());
                });
                var dtoTitle = Mapper.Map<Data.Models.Title>(createTitle);
                genericRepo.Insert<Data.Models.Title>(dtoTitle);
                flag = true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
            return flag;
        }

        public bool TitleNameExists(int? titleID, string titleName)
        {
            log.Info($"TitleService/TitleNameExists/{titleID}/{titleName}");

            return genericRepo.Exists<Data.Models.Title>(x => x.TitleID != titleID && x.TitleName == titleName && x.IsDeleted == false);
        }

        public bool UpdateTitle(Title editTitle)
        {
            log.Info($"TitleService/UpdateTitle");

            bool flag = false;
            try
            {
                var dtoObj = genericRepo.GetByID<Data.Models.Title>(editTitle.TitleID);
                dtoObj.TitleName = editTitle.TitleName;
                genericRepo.Update<Data.Models.Title>(dtoObj);
                flag = true;
            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }

            return flag;
        }
    }
}
