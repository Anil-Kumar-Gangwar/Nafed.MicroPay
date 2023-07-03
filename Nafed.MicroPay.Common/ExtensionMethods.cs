using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Common
{
    public static class ExtensionMethods
    {
        public static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static string BoolToYesNo(bool bValue)
        {
            if (bValue)
                return "Yes";
            else
                return "No";
        }

        public static void DeleteFile(string sFileFullPath)
        {
            if (System.IO.File.Exists(sFileFullPath))
            {
                System.IO.File.Delete(sFileFullPath);
            }
        }

        public static int CalculateAge(this DateTime dateOfBirth)
        {
            int age = 0;
            age = DateTime.Now.Year - dateOfBirth.Year;
            if (DateTime.Now.DayOfYear < dateOfBirth.DayOfYear)
                age = age - 1;

            return age;
        }
        public static string SetUniqueFileName(this string fileName, FileExtension fileExtension)
        {
            string renamedFileName = fileName + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Millisecond.ToString();
            return renamedFileName + '.' + fileExtension;
        }

        public static string SetUniqueFileName(this string fileName, string fileExtension)
        {
            string renamedFileName = fileName + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Millisecond.ToString();
            return renamedFileName + fileExtension;
        }

        /// <summary>
        /// Method to convert datatable to list====
        /// </summary>
        /// <typeparam name="T">T is annoynomous type class </typeparam>
        /// <param name="dt">Input datatable</param>
        /// <returns>List</returns>
        public static List<T> ConvertToList<T>(DataTable dt)
        {
            List<T> data = new List<T>();
            foreach (DataRow row in dt.Rows)
            {
                T item = GetItem<T>(row);
                data.Add(item);
            }
            return data;
        }
        private static T GetItem<T>(DataRow dr)
        {
            Type temp = typeof(T);
            T obj = Activator.CreateInstance<T>();

            foreach (DataColumn column in dr.Table.Columns)
            {
                foreach (PropertyInfo pro in temp.GetProperties())
                {
                    if (pro.Name == column.ColumnName)
                        pro.SetValue(obj, (dr[column.ColumnName].ToString().Trim().Equals(string.Empty)) ? null : dr[column.ColumnName], null);
                    else
                        continue;
                }
            }
            return obj;
        }

        public static DataTable ToDataTable<T>(this IList<T> data)
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            foreach (PropertyDescriptor prop in properties)
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            foreach (T item in data)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                    row[prop.Name] = (prop.GetValue(item) == null || prop.GetValue(item).ToString().Trim().Equals(string.Empty)) ? DBNull.Value : prop.GetValue(item);
                table.Rows.Add(row);
            }
            return table;
        }
        public static List<T> GetDistinctCellValues<T>(DataTable dtSource, string sourceColName)
        {
            try
            {
                return (from row in dtSource.AsEnumerable()
                        where (!CheckNullOrEmpty<T>(row.Field<T>(sourceColName)))
                        select row.Field<T>(sourceColName)).Distinct().ToList<T>();

            }
            catch (Exception ex)
            {
                log.Error("Message-" + ex.Message + " StackTrace-" + ex.StackTrace + " DatetimeStamp-" + DateTime.Now);
                throw ex;
            }
        }
        public static bool CheckNullOrEmpty<T>(T value)
        {
            if (typeof(T) == typeof(string))
                return (string.IsNullOrEmpty(value as string) || ((value as string).Trim().Length == 0));

            return value == null || value.Equals(default(T));
        }
        public static List<U> FindDuplicates<T, U>(this List<T> list, Func<T, U> keySelector)
        {
            return list.GroupBy(keySelector)
                .Where(group => group.Count() > 1)
                .Select(group => group.Key).ToList();
        }

        public static IEnumerable<TResult> LeftOuterJoin<TOuter, TInner, TKey, TResult>(
       this IEnumerable<TOuter> outer,
       IEnumerable<TInner> inner,
       Func<TOuter, TKey> outerKeySelector,
       Func<TInner, TKey> innerKeySelector,
       Func<TOuter, TInner, TResult> resultSelector)
        {
            return outer
                .GroupJoin(inner, outerKeySelector, innerKeySelector, (a, b) => new
                {
                    a,
                    b
                })
                .SelectMany(x => x.b.DefaultIfEmpty(), (x, b) => resultSelector(x.a, b));
        }

        public static string GetRomanValueByNumber(int key)
        {
            return NumberToRoman[key];
        }

        public static string GetFinancialYr(this DateTime date)
        {
            int CurrentYear = date.Year;
            int PreviousYear = date.Year - 1;
            int NextYear = date.Year + 1;
            string PreYear = PreviousYear.ToString();
            string NexYear = NextYear.ToString();
            string CurYear = CurrentYear.ToString();
            string FinYear = null;

            if (DateTime.Today.Month > 3)
                FinYear = CurYear + "-" + NexYear;
            else
                FinYear = PreYear + "-" + CurYear;
            return FinYear.Trim();

        }
        public static int GetMonthFromDate(this DateTime date)
        {
            return date.Month;

        }


        public static readonly Dictionary<int, string> NumberToRoman = new Dictionary<int, string>(100)
        {
            {1, "i"},
            {2, "ii"},
            {3, "iii"},
            {4, "iv"},
            {5, "v"},
            {6, "vi"},
            {7, "vii"},
            {8, "viii"},
            {9, "ix"},
            {10, "x"}
        };

        public static string GeneratePassword()
        {
            string newpassword = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
            return newpassword;
        }
        public static DateTime ChangeYear(this DateTime dt, int newYear)
        {
            return dt.AddYears(newYear - dt.Year);
        }

        public static List<string> GetFinancialYrList(int fromYear, int toYear)
        {
            List<string> yearList = new List<string>();
            while (fromYear <= toYear)
            {
                string FinYear = null;
                int startingYear = fromYear;
                int NextYear = startingYear + 1;
                FinYear = startingYear + "-" + NextYear.ToString();
                yearList.Add(FinYear);
                fromYear++;
            }
            return yearList;
        }

        public static Dictionary<string,string> GetBudgetFinancialYrList(int fromYear, int toYear)
        {
            Dictionary<string,string> yearList = new Dictionary<string,string>();
            while (fromYear <= toYear)
            {
                string FinYearText = null;
                string FinYearVal = null;
                int startingYear = fromYear;
                int NextYear = (startingYear + 1);
                FinYearText = startingYear + "-" + NextYear.ToString();
                FinYearVal = startingYear + "-" + NextYear.ToString().Substring(2, 2);
                yearList.Add(FinYearText, FinYearVal);
                fromYear++;
            }
            return yearList;
        }
        public static string ToOrdinal(this int value)
        {
            if (value <= 0) return "";
            // Start with the most common extension.
            string extension = "th";

            // Examine the last 2 digits.
            int last_digits = value % 100;

            // If the last digits are 11, 12, or 13, use th. Otherwise:
            if (last_digits < 11 || last_digits > 13)
            {
                // Check the last digit.
                switch (last_digits % 10)
                {
                    case 1:
                        extension = "st";
                        break;
                    case 2:
                        extension = "nd";
                        break;
                    case 3:
                        extension = "rd";
                        break;
                }
            }

            return extension;
        }

        public static List<string> GetYearBetweenYearsList(int fromYear, int toYear)
        {
            List<string> yearList = new List<string>();
            while (fromYear <= toYear)
            {
                string FinYear = null;
                int startingYear = fromYear;
                int NextYear = startingYear + 1;
                FinYear = startingYear + " - " + NextYear;
                yearList.Add(FinYear);
                fromYear++;
            }
            return yearList;
        }

        public static string ThousandSeprator(this decimal? value)
        {
            NumberFormatInfo nfi = new CultureInfo("en-US", false).NumberFormat;
            if (value.HasValue)
                return value.Value.ToString("N", nfi);
            else return "0.00";

        }
        public static string ThousandSeprator(this decimal value)
        {
            NumberFormatInfo nfi = new CultureInfo("en-US", false).NumberFormat;            
                return value.ToString("N", nfi);       
        }

        public static string ThousandSeprator(this double ? value)
        {
            NumberFormatInfo nfi = new CultureInfo("en-US", false).NumberFormat;
            if (value.HasValue)
                return value.Value.ToString("N", nfi);
            else return "0.00";
        }

        public static string ThousandSeprator(this double value)
        {
            NumberFormatInfo nfi = new CultureInfo("en-US", false).NumberFormat;
            return value.ToString("N", nfi);
        }
    }
}
