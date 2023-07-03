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
    public interface IEmailConfigurationService
    {

        List<Model.EmailConfiguration> GetEmailConfigList();


        bool CreateEmailConfiguration(Model.EmailConfiguration emailConfig);


        bool UpdateEmailConfiguration(Model.EmailConfiguration emailConfig);

    }
}
