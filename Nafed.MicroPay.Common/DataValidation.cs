using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;

namespace Nafed.MicroPay.Common
{
    public static class DataValidation
    {
        public static bool ValidateHeader(List<string> listHeader, List<string> RequiredHeader, out List<string> sMissingHeaderList,
     out List<string> invalidColumns, bool isExactMatch = false)
        {
            bool IsHeaderValidated = true;
            sMissingHeaderList = new List<string>();
            invalidColumns = new List<string>();

            foreach (string sStr in RequiredHeader)
            {
                bool IsHeaderMatched = false;
                foreach (string sCol in listHeader)
                {
                    if (isExactMatch)
                    {
                        if (sCol.ToLower().Equals(sStr.ToLower()))
                        {
                            IsHeaderMatched = true;
                            break;
                        }
                        else if (!RequiredHeader.Any(y => y == sCol))
                        {
                            if (!invalidColumns.Contains(sCol))
                                invalidColumns.Add(sCol);
                        }
                    }
                    else
                    {
                        if (sCol.ToLower().Trim().Replace(" ", "").Equals(sStr.ToLower().Trim().Replace(" ", "")))
                        {
                            IsHeaderMatched = true;
                            break;
                        }
                    }
                }
                if (!IsHeaderMatched)
                {
                    IsHeaderValidated = false;
                    if (!sMissingHeaderList.Contains(sStr))
                        sMissingHeaderList.Add(sStr);

                }
            }
            return IsHeaderValidated;
        }

        public static bool IsBlank(string sVal)
        {
            sVal = sVal.Trim();
            if (string.IsNullOrWhiteSpace(sVal) || string.IsNullOrEmpty(sVal))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool IsNumeric(string sVal)
        {
            double result;
            return Double.TryParse(sVal, out result);
        }

        public static bool IsGreatderThenZero(string sVal)
        {
            double value; bool IsValid = false;
            if (Double.TryParse(sVal, out value))
            {
                if (value > 0)
                {
                    IsValid = true;
                }
            }
            return IsValid;
        }

        public static bool IsInteger(string sVal)
        {
            int result;
            return int.TryParse(sVal, out result);
        }

        public static bool IsPostiveInteger(string sVal)
        {
            int result; bool IsValid = false;
            if (int.TryParse(sVal, out result))
            {
                if (result > 0)
                    IsValid = true;
            }
            return IsValid;
        }

        public static bool IsString(string sVal)
        {
            try
            {
                Convert.ToString(sVal);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool IsValidDate(string sVal)
        {
            DateTime result;
            return DateTime.TryParse(sVal, out result);
        }

        public static string Strip(string sVal)
        {
            return sVal.Replace("&nbsp;", string.Empty).Trim();
        }

        public static int NoOfDecimalPlaces(this string sVal)
        {
            int i = 0;
            string[] arry = sVal.ToString().Split('.');
            if (arry.Length > 1)
            {
                i = arry[1].Length;
            }
            return i;
        }

        public static bool HasFirstUpperCase(string sVal)
        {
            if (string.IsNullOrEmpty(sVal))
                return false;
            if (char.IsLetter(sVal[0]))
            {
                if (char.IsUpper(sVal[0]))
                    return true;
            }
            else
            {
                return true;
            }
            return false;
        }

        public static bool IsValidBoolean(string sVal)
        {
            sVal = sVal.ToLower();
            if (string.IsNullOrEmpty(sVal))
                return false;
            else if (string.IsNullOrWhiteSpace(sVal))
                return false;
            else if (sVal == "y" || sVal == "yes" || sVal == "true" || sVal == "n" || sVal == "no" || sVal == "false")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool ConvertToBoolean(string sVal)
        {
            sVal = sVal.ToLower();
            if (string.IsNullOrEmpty(sVal))
                return false;
            else if (string.IsNullOrWhiteSpace(sVal))
                return false;
            if (sVal == "y" || sVal == "yes" || sVal == "true")
            {
                return true;
            }
            else if (sVal == "n" || sVal == "no" || sVal == "false")
            {
                return false;
            }
            else
                return false;
        }

        public static bool IsPositiveNumber(string sVal)
        {
            double value; bool IsValid = false;
            if (Double.TryParse(sVal, out value))
            {
                if (value > 0)
                    IsValid = true;

            }
            else
            {
                IsValid = false;
            }
            return IsValid;
        }

        public static void DeleteTempFile(string sFileFullPath)
        {
            if (System.IO.File.Exists(sFileFullPath))
            {
                System.IO.File.Delete(sFileFullPath);
            }
        }

        public static void RemoveBlankRowAndColumns(ref DataTable dtExcel, bool bRemoveBlankCols, bool skipFirstRow = false, int rowsColStartIndex = 0)
        {
            //Trimming cells data
            dtExcel.AsEnumerable().ToList().ForEach(row =>
        {
            var cellList = row.ItemArray.ToList();
            row.ItemArray = cellList.Select(x => x.ToString().Trim()).ToArray();
        });

            DataRow[] drExcel = dtExcel.Rows.Cast<DataRow>().Where(row => !row.ItemArray.Skip(rowsColStartIndex).All(field => field is System.DBNull || string.Compare((field as string).Trim(), string.Empty) == 0)).ToArray<DataRow>();
            if (drExcel.Length > 0)
            {
                dtExcel = dtExcel.Rows.Cast<DataRow>().Where(row => !row.ItemArray.Skip(rowsColStartIndex).All(field => field is System.DBNull || string.Compare((field as string).Trim(), string.Empty) == 0)).CopyToDataTable();
            }
            if (bRemoveBlankCols)
            {
                // Removing blank columns
                foreach (var column in dtExcel.Columns.Cast<DataColumn>().ToArray())
                {
                    if (skipFirstRow)
                    {
                        if (dtExcel.AsEnumerable().Skip(1).All(dr => dr[column].ToString().Equals("")))
                            dtExcel.Columns.Remove(column);
                    }
                    else
                    if (dtExcel.AsEnumerable().All(dr => dr[column].ToString().Equals("")))
                        dtExcel.Columns.Remove(column);
                }
            }
            dtExcel.AcceptChanges();
        }
        public static int? TryParseNullable(string val)
        {
            int value;
            return int.TryParse(val, out value) ? (int?)value : null;
        }

        public static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
        public static bool isContainsWhiteSpace(string sVal)
        {
            try
            {
                if (sVal.Contains(" "))
                    return true;
                else
                    return false;
            }
            catch
            {
                return false;
            }
        }
        public static bool IsTrueOrFalse(string sVal, bool check)
        {
            sVal = sVal.ToLower();
            if (string.IsNullOrEmpty(sVal))
                return false;
            else if (string.IsNullOrWhiteSpace(sVal))
                return false;
            else if ((sVal == "y" || sVal == "yes" || sVal == "true") && check)
            {
                return true;
            }
            else if ((sVal == "n" || sVal == "no" || sVal == "false") && !check)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static bool IsWholeNumber(string inputString)
        {
            if ((Convert.ToDecimal(inputString) % 1) == 0)
                return true;
            else
                return false;
        }

        public static string PriceFormat(decimal? price)
        {
            decimal val = price ?? 0;
            string priceFormat = string.Empty;
            if (val != 0)
            {
                if (val < 1)
                    priceFormat = ((int)(val * 100)) + "p";
                else if (IsWholeNumber(val.ToString()))
                    priceFormat = "£" + val.ToString("N0");
                else
                    priceFormat = "£" + val.ToString("N2");
            }



            return priceFormat;
        }

        public static bool IsValidTimeIn24Format(this string input)
        {
            TimeSpan dummyOutput;
            return TimeSpan.TryParse(input, out dummyOutput);
        }

        public static bool IsValidDateFormat(this string input)
        {
            string[] formats = { "dd/MM/yyyy" };
            DateTime parsedDate;
            return DateTime.TryParseExact(input, formats, new CultureInfo("en-US"), DateTimeStyles.None, out parsedDate);
        }
    }
}
