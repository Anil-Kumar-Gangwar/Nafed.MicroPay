using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nafed.MicroPay.Services.IServices;
using Nafed.MicroPay.Data.Repositories.IRepositories;
using DTOModel = Nafed.MicroPay.Data.Models;
using AutoMapper;

namespace Nafed.MicroPay.Services
{
    public interface ISMSConfigurationService
    {

        bool CreateSMSConfiguration(Model.SMSConfiguration smsConfig);

        List<Model.SMSConfiguration> GetSMSConfiguration();

        bool UpdateSMSConfiguration(Model.SMSConfiguration smsConfig);


    }
}
