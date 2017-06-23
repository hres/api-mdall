using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.IO;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using MdallWebApi.Models;

namespace MdallWebApi
{
    public class CsvFormatter : BufferedMediaTypeFormatter
    {

        static char[] _specialChars = new char[] { ',', '\n', '\r', '"' };

        public CsvFormatter()
        {
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/csv"));
        }

        public override bool CanReadType(Type type)
        {

            if(type == typeof(WebApiConfig))
            {
                return true;
            }
            else
            {
                return false;
            }
            /*
            else
            {
                Type enumerableType = typeof(IEnumerable<WebApiConfig>);
                return enumerableType.IsAssignableFrom(type);
            }
            */
            //throw new NotImplementedException();
        }

        public override bool CanWriteType(Type type)
        {
            if (type == typeof(Company))    //use Generic types here
            {
                return true;
            }
            else
            {
                Type enumerableType = typeof(IEnumerable<Company>); //use Generic types here
                return enumerableType.IsAssignableFrom(type);
            }
        }

        public override void WriteToStream(Type type, object value, Stream writeStream, HttpContent content)
        {
            using (var writer = new StreamWriter(writeStream))
            {
                var products = value as IEnumerable<Company>;   //use Generic types here
                if (products != null)
                {
                    foreach (var product in products)
                    {
                        WriteItem(product, writer);
                    }
                }
                else
                {
                    var singleProduct = value as Company;
                    if (singleProduct == null)
                    {
                        throw new InvalidOperationException("Cannot serialize type");
                    }
                    WriteItem(singleProduct, writer);
                }
            }
        }

        // Helper methods for serializing Products to CSV format. 
        private void WriteItem(Company product, StreamWriter writer)
        {
            writer.WriteLine("{0},{1},{2},{3}", Escape(product.company_id),
                Escape(product.company_name), Escape(product.company_name), Escape(product.country_cd));
        }
      

        private string Escape(object o)
        {
            if (o == null)
            {
                return "";
            }
            string field = o.ToString();
            if (field.IndexOfAny(_specialChars) != -1)
            {
                // Delimit the entire field with quotes and replace embedded quotes with "".
                return String.Format("\"{0}\"", field.Replace("\"", "\"\""));
            }
            else return field;
        }
    }
}