using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Model
{
    public class DateCompareAttribute : ValidationAttribute
    {
        private string _startDatePropertyName;
        public DateCompareAttribute(string startDatePropertyName)
        {
            _startDatePropertyName = startDatePropertyName;
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            try
            {
                var propertyInfo = validationContext.ObjectType.GetProperty(_startDatePropertyName);
                if (propertyInfo == null)
                {
                    return new ValidationResult(string.Format("Unknown property {0}", _startDatePropertyName));
                }
                var propertyValue = propertyInfo.GetValue(validationContext.ObjectInstance, null);
                if ((DateTime)value >= (DateTime)propertyValue)
                {
                    return ValidationResult.Success;
                }
                else
                {

                    return new ValidationResult(validationContext.DisplayName + " must be later than " + _startDatePropertyName + ".");
                }
            }
            catch (Exception)
            {
            }
            return new ValidationResult("");
        }
    }
}
