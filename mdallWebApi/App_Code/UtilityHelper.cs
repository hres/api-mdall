using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using MdallWebApi.Models;
using MdallWebApi.Controllers;
using System.Text;
using System.Data;
using System.IO;

namespace MdallWebApi
{
    public static class UtilityHelper
    {
        public static void SetDefaultCulture(string lang)
        {
            if (lang == "en")
            {
                Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en-CA");
                Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture("en-CA");
            }
            else
            {
                Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("fr-FR");
                Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture("fr-FR");
            }
        }


        public static List<T> ToNonNullList<T>(this IEnumerable<T> obj)
        {
            return obj == null ? new List<T>() : obj.ToList();
        }

        public static string BuildAddress(Company company)
        {
            var address = new StringBuilder();
            address.Append(company.addr_line_1).Append(" ");
            address.Append(company.addr_line_2).Append(" ");
            address.Append(company.addr_line_3).AppendLine();
            address.Append(company.city).Append(",");
            address.Append(company.region_cd).Append(",");
            address.Append(company.country_cd).Append(",");
            address.Append(company.postal_code);
            return address.ToString();
        }

        public static int GetNumberTerm(string term)
        {
            int result;
            bool res = int.TryParse(term, out result);
            if (res == false)
            {
                return 0;
            }
            else
            {
                return result;
            }
        }

        public static void WriteDataTable(DataTable sourceTable, TextWriter writer, bool includeHeaders)
        {
            if (includeHeaders)
            {
                IEnumerable<String> headerValues = sourceTable.Columns
                    .OfType<DataColumn>()
                    .Select(column => QuoteValue(column.ColumnName));

                writer.WriteLine(String.Join(",", headerValues));
            }

            IEnumerable<String> items = null;

            foreach (DataRow row in sourceTable.Rows)
            {
                items = row.ItemArray.Select(o => QuoteValue(o.ToString()));
                writer.WriteLine(String.Join(",", items));
            }

            writer.Flush();
        }

        private static string QuoteValue(string value)
        {
            return String.Concat("\"",
            value.Replace("\"", "\"\""), "\"");
        }

    }
}