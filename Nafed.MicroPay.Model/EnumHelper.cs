using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace Nafed.MicroPay.Model
{
    public static class EnumHelper
    {
        public static string GetDisplayName(this Enum value)
        {

            try
            {

                int i = (int)Enum.ToObject(value.GetType(), value);

                var type = value.GetType();

                if (i >0)

                    return value.GetType()?
                   .GetMember(value.ToString())?.First()?
                   .GetCustomAttribute<DisplayAttribute>()?
                   .Name;

                else
                    return string.Empty;
            }
            catch (Exception ex)
            {

                throw ex;
            }

           
        }

    }
}