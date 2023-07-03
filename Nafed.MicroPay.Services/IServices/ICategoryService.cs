using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Services.IServices
{
    public  interface ICategoryService 
    {
        List<Model.Category> GetCategoryList();
        bool CategoryNameExists(int? categoryID, string categoryName);
        bool CategoryCodeExists(int? categoryID, string categoryCode);
        bool UpdateCategory(Model.Category editCategoryItem);
        bool InsertCategory(Model.Category createCategory);
        Model.Category GetCategoryByID(int categoryID);
        bool Delete(int categoryID);
    }
}
