using MdallWebApi.Models;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Web;
using System.Web.Mvc;

namespace MdallWebApi
{
    public class DBConnection
    {
        private string _lang;
        public string Lang
        {
            get { return this._lang; }
            set { this._lang = value; }
        }

        public DBConnection(string lang)
        {
            this._lang = lang;
        }

        private string MdallDBConnection
        {
            get { return ConfigurationManager.ConnectionStrings["mdall"].ToString(); }
        }


        public void CSV()
        {

            var commandText = "SELECT  * FROM asdaB_ACS.PAS_LICENCE L";

            string csv = string.Empty;

            using (OracleConnection con = new OracleConnection(MdallDBConnection))
            {

                OracleCommand cmd = new OracleCommand(commandText, con);
                try
                {
                    con.Open();
                    using (OracleDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.HasRows)
                        {
                            for (int i = 0; i < dr.FieldCount; i++)
                            {
                                csv += dr.GetName(i) + ", ";
                            }

                            csv += "\r\n";

                            while (dr.Read())
                            {
                                for (int x = 0; x < dr.FieldCount; x++)          //Data may contain commas - find a way to escape these
                                {
                                    csv += dr.GetValue(x) + ", ";
                                }
                                csv += "\r\n";
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    string errorMessages = string.Format("DbConnection.cs - GetAllEstablishment()");
                    ExceptionHelper.LogException(ex, errorMessages);
                }
                finally
                {
                    if (con.State == ConnectionState.Open)
                        con.Close();
                }
            }

            //System.Diagnostics.Debug.WriteLine(csv);


            var filePath = @"./logs";
            //var filePath = @"C:\";
            System.Diagnostics.Debug.WriteLine(filePath);
            var logFile = string.Format("DB Call", filePath, DateTime.Now);


            FileStream fs = File.Create("DB Call.txt");
            fs.Close();

            using (StreamWriter sw = new StreamWriter("DB Call.txt", true))
            {
                sw.Write(csv);
                sw.Close();
            }

            /*
            System.Web.HttpResponse response = System.Web.HttpContext.Current.Response;

            //Download the CSV file.
            response.Clear();
            response.Buffer = false;
            response.AddHeader("content-disposition", "attachment;filename=SqlExport.csv");
            response.Charset = "";
            response.ContentType = "application/text";
            response.Write(csv);
            response.Output.Write(csv);
            response.TransmitFile(System.Web.HttpContext.Current.Server.MapPath("SqlExport.csv"));
            response.Flush();
            response.End();

            */
            System.Diagnostics.Debug.WriteLine("Done");
        }

        public List<Licence> GetAllLicence(string state="", string licenceName="", string lang="")
        {
            /*
            DateTime invokeTime = DateTime.Now;
            System.Diagnostics.Debug.WriteLine("Invoke Time: " + invokeTime);

            DateTime startTime = DateTime.Now;
            */

            var items = new List<Licence>();
            string commandText = "SELECT L.* ";
            if ( lang != null && lang.Equals("en") )
            {
                commandText += ", T.LICENCE_TYPE_DESC_E LICENCE_TYPE_DESC "; 
            }
            else
            {
                commandText += ", T.LICENCE_TYPE_DESC_F LICENCE_TYPE_DESC ";
            }
            commandText += " FROM PUB_ACS.PAS_LICENCE L ";
            commandText += " LEFT JOIN PUB_ACS.PAS_LICENCE_TYPE T ";
            commandText += " ON L.LICENCE_TYPE_CD =T.LICENCE_TYPE_CD ";
            if ((!string.IsNullOrEmpty(state)) || (!string.IsNullOrEmpty(licenceName)))
            {
                commandText += " WHERE";
                if (!string.IsNullOrEmpty(state))
                {
                    if (state.Equals("active"))
                    {
                        commandText += " L.END_DATE IS NULL ";
                        if (!string.IsNullOrEmpty(licenceName)) commandText += " AND";
                    }
                    if (state.Equals("archived"))
                    {
                        commandText += " L.END_DATE IS NOT NULL ";
                        if (!string.IsNullOrEmpty(licenceName)) commandText += " AND";
                    }
                }

                if (!string.IsNullOrEmpty(licenceName))
                {
                    commandText += " UPPER(L.LICENCE_NAME) LIKE :licenceName ";
                }
                

            }
            
            //System.Diagnostics.Debug.WriteLine(commandText);

            using (OracleConnection con = new OracleConnection(MdallDBConnection))
            {
                OracleCommand cmd = new OracleCommand(commandText, con);
                if (!string.IsNullOrEmpty(licenceName))
                {
                    cmd.Parameters.Add(":licenceName", '%' + licenceName.ToUpper().Trim() + '%');
                }
                    
                try
                {
                    con.Open();
                    using (OracleDataReader dr = cmd.ExecuteReader())
                    {

                        //read time before entering query
                        //startTime = DateTime.Now;
                        //System.Diagnostics.Debug.WriteLine("Start Time: " + startTime);

                        if (dr.HasRows)
                        {
                            while (dr.Read())
                            {

                                var item = new Licence();
                                item.original_licence_no = dr["ORIGINAL_LICENCE_NO"] == DBNull.Value ? 0 : Convert.ToInt32(dr["ORIGINAL_LICENCE_NO"]);
                                item.licence_status = dr["LICENCE_STATUS"] == DBNull.Value ? string.Empty : dr["LICENCE_STATUS"].ToString().Trim();
                                //item.application_id = dr["APPLICATION_ID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["APPLICATION_ID"]);
                                item.appl_risk_class = dr["APPL_RISK_CLASS"] == DBNull.Value ? 0 : Convert.ToInt32(dr["APPL_RISK_CLASS"]);
                                item.licence_name = dr["LICENCE_NAME"] == DBNull.Value ? string.Empty : dr["LICENCE_NAME"].ToString().Trim();
                                item.first_licence_status_dt = dr["FIRST_LICENCE_STATUS_DT"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(dr["FIRST_LICENCE_STATUS_DT"]);
                                item.last_refresh_dt = dr["LAST_REFRESH_DT"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(dr["LAST_REFRESH_DT"]);
                                item.end_date = dr["END_DATE"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(dr["END_DATE"]);
                                item.licence_type_cd = dr["LICENCE_TYPE_CD"] == DBNull.Value ? string.Empty : dr["LICENCE_TYPE_CD"].ToString().Trim();
                                item.company_id = dr["COMPANY_ID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["COMPANY_ID"]);
                                item.licence_type_desc = dr["LICENCE_TYPE_DESC"] == DBNull.Value ? string.Empty : dr["LICENCE_TYPE_DESC"].ToString().Trim();
                                //item.has_location = false;

                                //if (!string.IsNullOrWhiteSpace(item.licence_type_cd))
                                //{
                                //    LicenceType licenceType = GetLicenceTypeByCode(item.licence_type_cd);
                                //    item.licence_type_desc = licenceType.licence_type_desc;
                                //}
                                //SbdLocation location = GetSbdLocationById(item.original_licence_no);
                                //if(location.original_licence_no != 0)
                                //{
                                //    item.has_location = true;
                                //    item.noc_location = location.sbd_notice_web_loc;
                                //    item.sbd_location = location.sbd_web_loc;
                                //}
                                items.Add(item);
                                //System.Diagnostics.Debug.WriteLine("Record #" + items.Count + " at " + DateTime.Now);
                            }
                        }                       

                    }
                }
                catch (Exception ex)
                {
                    string errorMessages = string.Format("DbConnection.cs - GetAllLicence()");
                    ExceptionHelper.LogException(ex, errorMessages);
                }
                finally
                {
                    if (con.State == ConnectionState.Open)
                        con.Close();
                }
            }


            /* read time at the end of the query
            DateTime stopTime = DateTime.Now;
            System.Diagnostics.Debug.WriteLine("End Time: " + stopTime);
            
            // calculate the duration of the query
            TimeSpan duration = stopTime - startTime;
            System.Diagnostics.Debug.WriteLine(items.Count + " records found in " + duration);
            */
            return items;
        }

        public Licence GetLicenceById(int id, string state,string lang)
        {
            var item = new Licence();
            string commandText = "SELECT L.* ";
            if (lang != null && lang.Equals("en"))
            {
                commandText += ", T.LICENCE_TYPE_DESC_E LICENCE_TYPE_DESC ";
            }
            else
            {
                commandText += ", T.LICENCE_TYPE_DESC_F LICENCE_TYPE_DESC ";
            }
            commandText += " FROM PUB_ACS.PAS_LICENCE L ";
            commandText += " LEFT JOIN PUB_ACS.PAS_LICENCE_TYPE T ";
            commandText += " ON L.LICENCE_TYPE_CD =T.LICENCE_TYPE_CD ";

            commandText += " WHERE";
            if (!string.IsNullOrEmpty(state))
            {
                if (state.Equals("active"))
                {
                    commandText += " L.END_DATE IS NULL AND";
                }
                if (state.Equals("archived"))
                {
                    commandText += " L.END_DATE IS NOT NULL AND";
                }
            }
            commandText += " L.ORIGINAL_LICENCE_NO = :id ";

            using (OracleConnection con = new OracleConnection(MdallDBConnection))
            {
                using (OracleCommand cmd = new OracleCommand(commandText, con))
                {
                    try
                    {
                        con.Open();
                        cmd.Parameters.Add(new OracleParameter(":id", id));
                        using (OracleDataReader dr = cmd.ExecuteReader())
                        {
                            if (dr.HasRows)
                            {
                                while (dr.Read())
                                {
                                    item.original_licence_no = dr["ORIGINAL_LICENCE_NO"] == DBNull.Value ? 0 : Convert.ToInt32(dr["ORIGINAL_LICENCE_NO"]);
                                    item.licence_status = dr["LICENCE_STATUS"] == DBNull.Value ? string.Empty : dr["LICENCE_STATUS"].ToString().Trim();
                                    //item.application_id = dr["APPLICATION_ID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["APPLICATION_ID"]);
                                    item.appl_risk_class = dr["APPL_RISK_CLASS"] == DBNull.Value ? 0 : Convert.ToInt32(dr["APPL_RISK_CLASS"]);
                                    item.licence_name = dr["LICENCE_NAME"] == DBNull.Value ? string.Empty : dr["LICENCE_NAME"].ToString().Trim();
                                    item.first_licence_status_dt = dr["FIRST_LICENCE_STATUS_DT"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(dr["FIRST_LICENCE_STATUS_DT"]);
                                    item.last_refresh_dt = dr["LAST_REFRESH_DT"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(dr["LAST_REFRESH_DT"]);
                                    item.end_date = dr["END_DATE"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(dr["END_DATE"]);
                                    item.licence_type_cd = dr["LICENCE_TYPE_CD"] == DBNull.Value ? string.Empty : dr["LICENCE_TYPE_CD"].ToString().Trim();
                                    item.company_id = dr["COMPANY_ID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["COMPANY_ID"]);
                                    item.licence_type_desc = dr["LICENCE_TYPE_DESC"] == DBNull.Value ? string.Empty : dr["LICENCE_TYPE_DESC"].ToString().Trim();
                                    //item.has_location = false;
                                    //if (!string.IsNullOrWhiteSpace(item.licence_type_cd))
                                    //{
                                    //    LicenceType licenceType = GetLicenceTypeByCode(item.licence_type_cd);
                                    //    item.licence_type_desc = licenceType.licence_type_desc;
                                    //}


                                    //SbdLocation location = GetSbdLocationById(item.original_licence_no);
                                    //if(location.original_licence_no !=0)
                                    //{
                                    //    item.has_location = true;
                                    //    item.noc_location = location.sbd_notice_web_loc;
                                    //    item.sbd_location = location.sbd_web_loc;
                                    //}

                                }
                            }


                        }
                    }
                    catch (Exception ex)
                    {
                        string errorMessages = string.Format("DbConnection.cs - GetLicenceById()");
                        ExceptionHelper.LogException(ex, errorMessages);
                    }
                    finally
                    {
                        if (con.State == ConnectionState.Open)
                            con.Close();
                    }
                }
            }
            return item;
        }

        //public Licence GetArchivedLicenceById(int id)
        //{
        //    var item = new Licence();
        //    string commandText = "SELECT * FROM PUB_ACS.PAS_LICENCE L";
        //    commandText += " WHERE";
        //    //commandText += " L.END_DATE IS NOT NULL AND";                
        //    commandText += " L.ORIGINAL_LICENCE_NO = :id";

        //    using (OracleConnection con = new OracleConnection(MdallDBConnection))
        //    {
        //        using (OracleCommand cmd = new OracleCommand(commandText, con))
        //        {
        //            try
        //            {
        //                con.Open();
        //                cmd.Parameters.Add(new OracleParameter("id", id));
        //                using (OracleDataReader dr = cmd.ExecuteReader())
        //                {
        //                    if (dr.HasRows)
        //                    {
        //                        while (dr.Read())
        //                        {
        //                            item.original_licence_no = dr["ORIGINAL_LICENCE_NO"] == DBNull.Value ? 0 : Convert.ToInt32(dr["ORIGINAL_LICENCE_NO"]);
        //                            item.licence_status = dr["LICENCE_STATUS"] == DBNull.Value ? string.Empty : dr["LICENCE_STATUS"].ToString().Trim();
        //                            item.application_id = dr["APPLICATION_ID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["APPLICATION_ID"]);
        //                            item.appl_risk_class = dr["APPL_RISK_CLASS"] == DBNull.Value ? 0 : Convert.ToInt32(dr["APPL_RISK_CLASS"]);
        //                            item.licence_name = dr["LICENCE_NAME"] == DBNull.Value ? string.Empty : dr["LICENCE_NAME"].ToString().Trim();
        //                            item.first_licence_status_dt = dr["FIRST_LICENCE_STATUS_DT"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(dr["FIRST_LICENCE_STATUS_DT"]);
        //                            item.last_refresh_dt = dr["LAST_REFRESH_DT"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(dr["LAST_REFRESH_DT"]);
        //                            item.end_date = dr["END_DATE"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(dr["END_DATE"]);
        //                            item.licence_type_cd = dr["LICENCE_TYPE_CD"] == DBNull.Value ? string.Empty : dr["LICENCE_TYPE_CD"].ToString().Trim();
        //                            item.company_id = dr["COMPANY_ID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["COMPANY_ID"]);
        //                            item.has_location = false;
        //                            if (!string.IsNullOrWhiteSpace(item.licence_type_cd))
        //                            {
        //                                LicenceType licenceType = GetLicenceTypeByCode(item.licence_type_cd);
        //                                item.licence_type_desc = licenceType.licence_type_desc;
        //                            }


        //                            SbdLocation location = GetSbdLocationById(item.original_licence_no);
        //                            if (location.original_licence_no != 0)
        //                            {
        //                                item.has_location = true;
        //                                item.noc_location = location.sbd_notice_web_loc;
        //                                item.sbd_location = location.sbd_web_loc;
        //                            }

        //                        }
        //                    }
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                string errorMessages = string.Format("DbConnection.cs - GetArchivedLicenceById()");
        //                ExceptionHelper.LogException(ex, errorMessages);
        //            }
        //            finally
        //            {
        //                if (con.State == ConnectionState.Open)
        //                    con.Close();
        //            }
        //        }
        //    }
        //    return item;
        //}
        public List<Licence> GetAllLicenceByCompanyId(int company_id, string state, string lang = "")
        {
            var items = new List<Licence>();
            string commandText = "SELECT UNIQUE L.* ";
            if (lang != null && lang.Equals("en"))
            {
                commandText += ", T.LICENCE_TYPE_DESC_E LICENCE_TYPE_DESC ";
            }
            else
            {
                commandText += ", T.LICENCE_TYPE_DESC_F LICENCE_TYPE_DESC ";
            }
            commandText += " FROM PUB_ACS.PAS_LICENCE L ";
            commandText += " LEFT JOIN PUB_ACS.PAS_LICENCE_TYPE T ON L.LICENCE_TYPE_CD = T.LICENCE_TYPE_CD ";
            commandText += " , PUB_ACS.PAS_LICENCE_DEVICE D ";            

            //string commandText = "SELECT UNIQUE L.* FROM PUB_ACS.PAS_LICENCE L, PUB_ACS.PAS_LICENCE_DEVICE D ";
            commandText += " WHERE ";
            if (!string.IsNullOrEmpty(state))
            {
                if (state.Equals("active"))
                {
                    commandText += " L.END_DATE IS NULL AND";
                }

                else
                {
                    // commandText += " L.END_DATE IS NOT NULL OR ";
                    commandText += " D.END_DATE IS NOT NULL AND L.ORIGINAL_LICENCE_NO = D.ORIGINAL_LICENCE_NO AND ";

                }
            }
            commandText += " L.COMPANY_ID = :company_id ";
            commandText += " ORDER BY UPPER(L.ORIGINAL_LICENCE_NO)";
            using (

            OracleConnection con = new OracleConnection(MdallDBConnection))
            {
                OracleCommand cmd = new OracleCommand(commandText, con);
                cmd.Parameters.Add(":company_id", company_id);
                try
                {
                    con.Open();
                    using (OracleDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.HasRows)
                        {
                            while (dr.Read())
                            {
                                var item = new Licence();
                                item.original_licence_no = dr["ORIGINAL_LICENCE_NO"] == DBNull.Value ? 0 : Convert.ToInt32(dr["ORIGINAL_LICENCE_NO"]);
                                item.licence_status = dr["LICENCE_STATUS"] == DBNull.Value ? string.Empty : dr["LICENCE_STATUS"].ToString().Trim();
                                //item.application_id = dr["APPLICATION_ID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["APPLICATION_ID"]);
                                item.appl_risk_class = dr["APPL_RISK_CLASS"] == DBNull.Value ? 0 : Convert.ToInt32(dr["APPL_RISK_CLASS"]);
                                item.licence_name = dr["LICENCE_NAME"] == DBNull.Value ? string.Empty : dr["LICENCE_NAME"].ToString().Trim();
                                item.first_licence_status_dt = dr["FIRST_LICENCE_STATUS_DT"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(dr["FIRST_LICENCE_STATUS_DT"]);
                                item.last_refresh_dt = dr["LAST_REFRESH_DT"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(dr["LAST_REFRESH_DT"]);
                                item.end_date = dr["END_DATE"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(dr["END_DATE"]);
                                item.licence_type_cd = dr["LICENCE_TYPE_CD"] == DBNull.Value ? string.Empty : dr["LICENCE_TYPE_CD"].ToString().Trim();
                                item.company_id = dr["COMPANY_ID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["COMPANY_ID"]);
                                item.licence_type_desc = dr["LICENCE_TYPE_DESC"] == DBNull.Value ? string.Empty : dr["LICENCE_TYPE_DESC"].ToString().Trim();
                                //if (!string.IsNullOrWhiteSpace(item.licence_type_cd))
                                //{
                                //    LicenceType licenceType = GetLicenceTypeByCode(item.licence_type_cd);
                                //    item.licence_type_desc = licenceType.licence_type_desc;
                                //}
                                //SbdLocation location = GetSbdLocationById(item.original_licence_no);
                                //if (location.original_licence_no != 0)
                                //{
                                //    item.has_location = true;
                                //    item.noc_location = location.sbd_notice_web_loc;
                                //    item.sbd_location = location.sbd_web_loc;
                                //}
                                items.Add(item);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    string errorMessages = string.Format("DbConnection.cs - GetAllLicenceByCompanyId()");
                    ExceptionHelper.LogException(ex, errorMessages);
                }
                finally
                {
                    if (con.State == ConnectionState.Open)
                        con.Close();
                }
            }
            return items;
        }



        public List<Company> GetAllCompany(string status, string companyName)
        {
            var items = new List<Company>();
            string commandText = "SELECT DISTINCT * FROM PUB_ACS.PAS_LICENCE_COMPANY ";                   

            if (!string.IsNullOrEmpty(status))
            {
                commandText += "WHERE ";
                commandText += " COMPANY_STATUS = :status "; 
                
            }
            if (!string.IsNullOrEmpty(companyName))
            {
                if (!string.IsNullOrEmpty(status))
                {
                    commandText += " AND";
                    commandText += " UPPER(COMPANY_NAME) LIKE :companyName ";
                }
                else {
                    commandText += " WHERE";
                    commandText += " UPPER(COMPANY_NAME) LIKE :companyName ";

                }
            }

            using (OracleConnection con = new OracleConnection(MdallDBConnection))
            {
                OracleCommand cmd = new OracleCommand(commandText, con);
                if (!string.IsNullOrEmpty(status))
                {
                    cmd.Parameters.Add(":status", status.ToUpper().Trim());
                }
                    
                if (!string.IsNullOrEmpty(companyName))
                {
                    cmd.Parameters.Add(":companyName", '%' + companyName.ToUpper().Trim() + '%');
                }
                


                try
                {
                    con.Open();
                    using (OracleDataReader dr = cmd.ExecuteReader())
                    {


                        //read time before entering query
                        // DateTime startTime = DateTime.Now;
                        //System.Diagnostics.Debug.WriteLine(startTime);


                        if (dr.HasRows)
                        {
                            while (dr.Read())
                            {
                                var item = new Company();
                                item.company_id = dr["COMPANY_ID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["COMPANY_ID"]);
                                item.company_name = dr["COMPANY_NAME"] == DBNull.Value ? string.Empty : dr["COMPANY_NAME"].ToString().Trim();
                                item.addr_line_1 = dr["ADDR_LINE_1"] == DBNull.Value ? string.Empty : dr["ADDR_LINE_1"].ToString().Trim();
                                item.addr_line_2 = dr["ADDR_LINE_2"] == DBNull.Value ? string.Empty : dr["ADDR_LINE_2"].ToString().Trim();
                                item.addr_line_3 = dr["ADDR_LINE_3"] == DBNull.Value ? string.Empty : dr["ADDR_LINE_3"].ToString().Trim();
                                item.postal_code = dr["POSTAL_CODE"] == DBNull.Value ? string.Empty : dr["POSTAL_CODE"].ToString().Trim();
                                item.city = dr["CITY"] == DBNull.Value ? string.Empty : dr["CITY"].ToString().Trim();
                                item.country_cd = dr["COUNTRY_CD"] == DBNull.Value ? string.Empty : dr["COUNTRY_CD"].ToString().Trim();
                                item.region_cd = dr["REGION_CD"] == DBNull.Value ? string.Empty : dr["REGION_CD"].ToString().Trim();
                                item.company_status = dr["COMPANY_STATUS"] == DBNull.Value ? string.Empty : dr["COMPANY_STATUS"].ToString().Trim();
                                items.Add(item);
                            }
                        }
                        // read time at the end of the query
                        /*DateTime stopTime = DateTime.Now;
                        System.Diagnostics.Debug.WriteLine(stopTime);

                        // calculate the duration of the query
                        TimeSpan duration = stopTime - startTime;
                        System.Diagnostics.Debug.WriteLine(items.Count + " records found in " + duration);
                        */
                    }
                }
                catch (Exception ex)
                {
                    string errorMessages = string.Format("DbConnection.cs - GetAllLicenceCompany()");
                    ExceptionHelper.LogException(ex, errorMessages);
                }
                finally
                {
                    if (con.State == ConnectionState.Open)
                        con.Close();
                }
            }

            return items;
        }

        public Company GetCompanyById(string status, int id)
        {
            var company = new Company();
            string commandText = "SELECT * FROM PUB_ACS.PAS_LICENCE_COMPANY WHERE COMPANY_ID = :id ";            
            
            if (!string.IsNullOrEmpty(status))
            {
                commandText += " AND COMPANY_STATUS = :status ";
            }

            using (

                OracleConnection con = new OracleConnection(MdallDBConnection))
            {
                OracleCommand cmd = new OracleCommand(commandText, con);
                cmd.Parameters.Add(":id", id);
                if (!string.IsNullOrEmpty(status))
                {
                    cmd.Parameters.Add(":status", status.ToUpper().Trim());
                }
               
                try
                {
                    con.Open();
                    using (OracleDataReader dr = cmd.ExecuteReader())
                    {

                        if (dr.HasRows)
                        {
                            while (dr.Read())
                            {
                                var item = new Company();
                                item.company_id = dr["COMPANY_ID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["COMPANY_ID"]);
                                item.company_name = dr["COMPANY_NAME"] == DBNull.Value ? string.Empty : dr["COMPANY_NAME"].ToString().Trim();
                                item.addr_line_1 = dr["ADDR_LINE_1"] == DBNull.Value ? string.Empty : dr["ADDR_LINE_1"].ToString().Trim();
                                item.addr_line_2 = dr["ADDR_LINE_2"] == DBNull.Value ? string.Empty : dr["ADDR_LINE_2"].ToString().Trim();
                                item.addr_line_3 = dr["ADDR_LINE_3"] == DBNull.Value ? string.Empty : dr["ADDR_LINE_3"].ToString().Trim();
                                item.postal_code = dr["POSTAL_CODE"] == DBNull.Value ? string.Empty : dr["POSTAL_CODE"].ToString().Trim();
                                item.city = dr["CITY"] == DBNull.Value ? string.Empty : dr["CITY"].ToString().Trim();
                                item.country_cd = dr["COUNTRY_CD"] == DBNull.Value ? string.Empty : dr["COUNTRY_CD"].ToString().Trim();
                                item.region_cd = dr["REGION_CD"] == DBNull.Value ? string.Empty : dr["REGION_CD"].ToString().Trim();
                                item.company_status = dr["COMPANY_STATUS"] == DBNull.Value ? string.Empty : dr["COMPANY_STATUS"].ToString().Trim();

                                company = item;
                            }
                        }




                    }
                }
                catch (Exception ex)
                {
                    string errorMessages = string.Format("DbConnection.cs - GetLicenceCompanyById()");
                    ExceptionHelper.LogException(ex, errorMessages);
                }
                finally
                {
                    if (con.State == ConnectionState.Open)
                        con.Close();
                }
            }
            return company;
        }
        public List<Device> GetAllDevice(string state, string deviceName, int licenceId)
        {
            var items = new List<Device>();
            string commandText = "SELECT DISTINCT D.* FROM PUB_ACS.PAS_LICENCE_DEVICE D";

            if ((!string.IsNullOrEmpty(state)) || (licenceId > 0))
            {
                commandText += " , PUB_ACS.PAS_LICENCE L WHERE D.ORIGINAL_LICENCE_NO = L.ORIGINAL_LICENCE_NO";
            }
            else if (!string.IsNullOrEmpty(deviceName))
            {
                commandText += " WHERE";
            }
            if (!string.IsNullOrEmpty(state))
            {
                if (state.Equals("active"))
                {
                    commandText += " AND L.END_DATE IS NULL AND D.END_DATE IS NULL";
                }
                else
                {
                    //commandText += " AND L.END_DATE IS NOT NULL AND D.END_DATE IS NOT NULL ";
                    commandText += " AND D.END_DATE IS NOT NULL ";
                }
            }

            if (!string.IsNullOrEmpty(deviceName))
            {
                if (!string.IsNullOrEmpty(state)) commandText += " AND";
                commandText += " D.TRADE_NAME LIKE :deviceName ";
            }
            else if (licenceId > 0)
            {

                commandText += " AND L.ORIGINAL_LICENCE_NO = :licenceId ";
            }
            commandText += " ORDER BY UPPER(D.TRADE_NAME)";
            using (OracleConnection con = new OracleConnection(MdallDBConnection))
            {
                OracleCommand cmd = new OracleCommand(commandText, con);
                if (licenceId > 0)
                {
                    cmd.Parameters.Add(":licenceId", licenceId);
                }
                if (!string.IsNullOrEmpty(deviceName))
                {
                    cmd.Parameters.Add(":deviceName", '%' + deviceName.ToUpper().Trim() + '%');
                }


                    try
                {
                    con.Open();
                    using (OracleDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.HasRows)
                        {
                            while (dr.Read())
                            {
                                var item = new Device();
                                //item.deviceIdentifierList = new List<DeviceIdentifier>();
                                item.original_licence_no = dr["ORIGINAL_LICENCE_NO"] == DBNull.Value ? 0 : Convert.ToInt32(dr["ORIGINAL_LICENCE_NO"]);
                                item.device_id = dr["DEVICE_ID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["DEVICE_ID"]);
                                item.first_licence_dt = dr["FIRST_LICENCE_DT"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(dr["FIRST_LICENCE_DT"]);
                                item.end_date = dr["END_DATE"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(dr["END_DATE"]);
                                item.trade_name = dr["TRADE_NAME"] == DBNull.Value ? string.Empty : dr["TRADE_NAME"].ToString().Trim();

                                //var deviceIdentifierList = GetAllDeviceIdentifier("", 0, item.device_id);
                                //if(deviceIdentifierList != null && deviceIdentifierList.Count > 0)
                                //{
                                //    item.deviceIdentifierList = deviceIdentifierList;
                                //}
                                items.Add(item);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    string errorMessages = string.Format("DbConnection.cs - GetAllLicenceDevice()");
                    ExceptionHelper.LogException(ex, errorMessages);
                }
                finally
                {
                    if (con.State == ConnectionState.Open)
                        con.Close();
                }
            }
            return items;
        }



        public Device GetDeviceById(int id)
        {
            var device = new Device();
            string commandText = "SELECT * FROM PUB_ACS.PAS_LICENCE_DEVICE WHERE DEVICE_ID = :id ";

            using (

                OracleConnection con = new OracleConnection(MdallDBConnection))
            {
                OracleCommand cmd = new OracleCommand(commandText, con);
                cmd.Parameters.Add(":id", id);
                try
                {
                    con.Open();
                    using (OracleDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.HasRows)
                        {
                            while (dr.Read())
                            {
                                var item = new Device();
                                item.original_licence_no = dr["ORIGINAL_LICENCE_NO"] == DBNull.Value ? 0 : Convert.ToInt32(dr["ORIGINAL_LICENCE_NO"]);
                                item.device_id = dr["DEVICE_ID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["DEVICE_ID"]);
                                item.first_licence_dt = dr["FIRST_LICENCE_DT"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(dr["FIRST_LICENCE_DT"]);
                                item.end_date = dr["END_DATE"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(dr["END_DATE"]);
                                item.trade_name = dr["TRADE_NAME"] == DBNull.Value ? string.Empty : dr["TRADE_NAME"].ToString().Trim();
                                device = item;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    string errorMessages = string.Format("DbConnection.cs - GetLicenceDevice()");
                    ExceptionHelper.LogException(ex, errorMessages);
                }
                finally
                {
                    if (con.State == ConnectionState.Open)
                        con.Close();
                }
            }
            return device;
        }


        public List<DeviceIdentifier> GetAllDeviceIdentifier(string state="", string deviceIdentifierName="", int licenceId=0, int deviceId=0)
        {
            var items = new List<DeviceIdentifier>();
            string commandText = "SELECT DISTINCT * FROM PUB_ACS.PAS_LICENCE_DEV_IDENT";

            if (!string.IsNullOrEmpty(state)||!string.IsNullOrEmpty(deviceIdentifierName)|| (licenceId > 0) || (deviceId > 0)) commandText += " WHERE";
            if (!string.IsNullOrEmpty(state))
            {

                if (state.Equals("active"))
                {
                    commandText += " END_DATE IS NULL ";

                }
                else
                {
                    commandText += " END_DATE IS NOT NULL ";
                }

                if (!string.IsNullOrEmpty(deviceIdentifierName) || (licenceId > 0) || (deviceId > 0))
                {
                    commandText += " AND";

                }
            }

            if (!string.IsNullOrEmpty(deviceIdentifierName))
            {
                commandText += " DEVICE_IDENTIFIER LIKE :deviceIdentifierName ";
            }


            if (licenceId > 0)
            {
                if (!string.IsNullOrEmpty(deviceIdentifierName)) commandText += " AND";
                commandText += " ORIGINAL_LICENCE_NO = :licenceId ";
            }


            if (deviceId > 0)
            {
                if ((!string.IsNullOrEmpty(deviceIdentifierName)) || (licenceId > 0)) commandText += " AND";
                commandText += " DEVICE_ID = :deviceId ";
            }

           

            commandText += " ORDER BY UPPER(DEVICE_IDENTIFIER)";

            using (OracleConnection con = new OracleConnection(MdallDBConnection))
            {
                OracleCommand cmd = new OracleCommand(commandText, con);
                if (licenceId > 0)
                {
                    cmd.Parameters.Add(":licenceId", licenceId);
                }
                if (deviceId > 0)
                {
                    cmd.Parameters.Add(":deviceId", deviceId);
                }
                    
                if (!string.IsNullOrEmpty(deviceIdentifierName))
                {
                    cmd.Parameters.Add(":deviceIdentifierName", '%' + deviceIdentifierName.ToUpper().Trim() + '%');
                }
                    
                try
                {
                    con.Open();
                    using (OracleDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.HasRows)
                        {
                            while (dr.Read())
                            {
                                var item = new DeviceIdentifier();
                                item.original_licence_no = dr["ORIGINAL_LICENCE_NO"] == DBNull.Value ? 0 : Convert.ToInt32(dr["ORIGINAL_LICENCE_NO"]);
                                item.device_id = dr["DEVICE_ID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["DEVICE_ID"]);
                                item.first_licence_dt = dr["FIRST_LICENCE_DT"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(dr["FIRST_LICENCE_DT"]);
                                item.end_date = dr["END_DATE"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(dr["END_DATE"]);
                                item.device_identifier = dr["DEVICE_IDENTIFIER"] == DBNull.Value ? string.Empty : dr["DEVICE_IDENTIFIER"].ToString().Trim();

                                items.Add(item);
                            }
                            
                        }
                    }
                }
                catch (Exception ex)
                {
                    string errorMessages = string.Format("DbConnection.cs - GetAllLicenceDeviceIdentifier()");
                    ExceptionHelper.LogException(ex, errorMessages);
                }
                finally
                {
                    if (con.State == ConnectionState.Open)
                        con.Close();
                }
            }
            return items;
        }

        public List<DeviceIdentifier> GetDeviceIdentifierById(int id)
        {
            //var deviceIdentifier = new DeviceIdentifier();
            var items = new List<DeviceIdentifier>();
            string commandText = "SELECT * FROM PUB_ACS.PAS_LICENCE_DEV_IDENT WHERE DEVICE_ID = :id ";

            using (OracleConnection con = new OracleConnection(MdallDBConnection))
            {
                OracleCommand cmd = new OracleCommand(commandText, con);
                cmd.Parameters.Add(":id", id);
                try
                {
                    con.Open();
                    using (OracleDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.HasRows)
                        {
                            while (dr.Read())
                            {
                                var item = new DeviceIdentifier();
                                item.original_licence_no = dr["ORIGINAL_LICENCE_NO"] == DBNull.Value ? 0 : Convert.ToInt32(dr["ORIGINAL_LICENCE_NO"]);
                                item.device_id = dr["DEVICE_ID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["DEVICE_ID"]);
                                item.first_licence_dt = dr["FIRST_LICENCE_DT"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(dr["FIRST_LICENCE_DT"]);
                                item.end_date = dr["END_DATE"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(dr["END_DATE"]);
                                item.device_identifier = dr["DEVICE_IDENTIFIER"] == DBNull.Value ? string.Empty : dr["DEVICE_IDENTIFIER"].ToString().Trim();

                                items.Add(item);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    string errorMessages = string.Format("DbConnection.cs - GetLicenceDeviceIdentifierById()");
                    ExceptionHelper.LogException(ex, errorMessages);
                }
                finally
                {
                    if (con.State == ConnectionState.Open)
                        con.Close();
                }
            }
            return items;
        }

        public List<LicenceCompany> GetLicenceCompanyByCriteria(string lang, string state, string licenceName, Int64 licenceNumber, string companyName, int companyId)
        {

            var items = new List<LicenceCompany>();
            string commandText = "SELECT DISTINCT L.ORIGINAL_LICENCE_NO, L.LICENCE_STATUS, L.APPLICATION_ID, L.APPL_RISK_CLASS, L.LICENCE_NAME, L.FIRST_LICENCE_STATUS_DT, L.LAST_REFRESH_DT, L.END_DATE, L.LICENCE_TYPE_CD, ";
            commandText += " C.COMPANY_NAME, C.ADDR_LINE_1, C.ADDR_LINE_2, C.ADDR_LINE_3, C.POSTAL_CODE, C.CITY, C.COUNTRY_CD, C.REGION_CD, C.COMPANY_STATUS, C.COMPANY_ID";
            //commandText += " T.LICENCE_TYPE_CD,";
            //if (lang != null && lang.Equals("fr"))
            //{
            //    commandText += " T.LICENCE_TYPE_DESC_F AS LICENCE_TYPE_DESC";
            //}
            //else {
            //    commandText += " T.LICENCE_TYPE_DESC_E AS LICENCE_TYPE_DESC";
            //}

            commandText += " FROM PUB_ACS.PAS_LICENCE L";
            commandText += " LEFT OUTER JOIN PUB_ACS.PAS_LICENCE_COMPANY C ON L.COMPANY_ID = C.COMPANY_ID";
            //commandText += " LEFT OUTER JOIN PUB_ACS.PAS_LICENCE_TYPE T ON L.LICENCE_TYPE_CD = T.LICENCE_TYPE_CD";
            commandText += " WHERE";
            if (!string.IsNullOrEmpty(state))
            {
                if (state.Equals("active"))
                {
                    commandText += " L.END_DATE IS NULL";
                }
                else
                {
                    commandText += " L.END_DATE IS NOT NULL";
                }
            }
            if ((!string.IsNullOrEmpty(state)) && (!string.IsNullOrEmpty(licenceName)) || (licenceNumber > 0) || (!string.IsNullOrEmpty(companyName)) || (companyId > 0))
            {
                commandText += " AND";
            }
            if (!string.IsNullOrEmpty(licenceName))
            {
                commandText += " UPPER(L.LICENCE_NAME) LIKE '%" + licenceName.ToUpper().Trim() + "%'";
                commandText += " ORDER BY UPPER(L.LICENCE_NAME)";
            }
            if (licenceNumber > 0)
            {
                commandText += " UPPER(L.ORIGINAL_LICENCE_NO) LIKE '%" + licenceNumber + "%'";

            }
            if (!string.IsNullOrEmpty(companyName))
            {
                commandText += " UPPER(C.COMPANY_NAME) LIKE '%" + companyName.ToUpper().Trim() + "%'";
                commandText += " ORDER BY UPPER(L.LICENCE_NAME)";
            }
            if (companyId > 0)
            {
                commandText += " UPPER(L.COMPANY_ID) LIKE '%" + companyId + "%'";
                commandText += " ORDER BY UPPER(L.LICENCE_NAME)";
            }
            using (OracleConnection con = new OracleConnection(MdallDBConnection))
            {
                OracleCommand cmd = new OracleCommand(commandText, con);
                try
                {
                    con.Open();
                    using (OracleDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.HasRows)
                        {
                            while (dr.Read())
                            {
                                var item = new LicenceCompany();
                                item.original_licence_no = dr["ORIGINAL_LICENCE_NO"] == DBNull.Value ? 0 : Convert.ToInt32(dr["ORIGINAL_LICENCE_NO"]);
                                item.licence_status = dr["LICENCE_STATUS"] == DBNull.Value ? string.Empty : dr["LICENCE_STATUS"].ToString().Trim();
                                item.application_id = dr["APPLICATION_ID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["APPLICATION_ID"]);
                                item.appl_risk_class = dr["APPL_RISK_CLASS"] == DBNull.Value ? 0 : Convert.ToInt32(dr["APPL_RISK_CLASS"]);
                                item.licence_name = dr["LICENCE_NAME"] == DBNull.Value ? string.Empty : dr["LICENCE_NAME"].ToString().Trim();
                                item.first_licence_status_dt = dr["FIRST_LICENCE_STATUS_DT"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(dr["FIRST_LICENCE_STATUS_DT"]);
                                item.last_refresh_dt = dr["LAST_REFRESH_DT"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(dr["LAST_REFRESH_DT"]);
                                item.end_date = dr["END_DATE"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(dr["END_DATE"]);
                                //item.licence_type_cd = dr["LICENCE_TYPE_CD"] == DBNull.Value ? string.Empty : dr["LICENCE_TYPE_CD"].ToString().Trim();
                                //item.licence_type_desc = dr["LICENCE_TYPE_DESC"] == DBNull.Value ? string.Empty : dr["LICENCE_TYPE_DESC"].ToString().Trim();
                                item.company_id = dr["COMPANY_ID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["COMPANY_ID"]);
                                item.company_name = dr["COMPANY_NAME"] == DBNull.Value ? string.Empty : dr["COMPANY_NAME"].ToString().Trim();
                                item.addr_line_1 = dr["ADDR_LINE_1"] == DBNull.Value ? string.Empty : dr["ADDR_LINE_1"].ToString().Trim();
                                item.addr_line_2 = dr["ADDR_LINE_2"] == DBNull.Value ? string.Empty : dr["ADDR_LINE_2"].ToString().Trim();
                                item.addr_line_3 = dr["ADDR_LINE_3"] == DBNull.Value ? string.Empty : dr["ADDR_LINE_3"].ToString().Trim();
                                item.postal_code = dr["POSTAL_CODE"] == DBNull.Value ? string.Empty : dr["POSTAL_CODE"].ToString().Trim();
                                item.city = dr["CITY"] == DBNull.Value ? string.Empty : dr["CITY"].ToString().Trim();
                                item.country_cd = dr["COUNTRY_CD"] == DBNull.Value ? string.Empty : dr["COUNTRY_CD"].ToString().Trim();
                                item.region_cd = dr["REGION_CD"] == DBNull.Value ? string.Empty : dr["REGION_CD"].ToString().Trim();
                                item.company_status = dr["COMPANY_STATUS"] == DBNull.Value ? string.Empty : dr["COMPANY_STATUS"].ToString().Trim();

                                items.Add(item);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    string errorMessages = string.Format("DbConnection.cs - GetAllLicence()");
                    ExceptionHelper.LogException(ex, errorMessages);
                }
                finally
                {
                    if (con.State == ConnectionState.Open)
                        con.Close();
                }
            }
            return items;
        }

        public List<LicenceCompany> GetLicenceCompanyByCriteria(string lang, string state, int companyId)
        {

            var items = new List<LicenceCompany>();
            string commandText = "SELECT DISTINCT L.ORIGINAL_LICENCE_NO, L.LICENCE_STATUS, L.APPLICATION_ID, L.APPL_RISK_CLASS, L.LICENCE_NAME, L.FIRST_LICENCE_STATUS_DT, L.LAST_REFRESH_DT, L.END_DATE, L.LICENCE_TYPE_CD, ";
            commandText += " C.COMPANY_NAME, C.ADDR_LINE_1, C.ADDR_LINE_2, C.ADDR_LINE_3, C.POSTAL_CODE, C.CITY, C.COUNTRY_CD, C.REGION_CD, C.COMPANY_STATUS, C.COMPANY_ID";
            commandText += " T.LICENCE_TYPE_CD,";
            if (lang != null && lang.Equals("fr"))
            {
                commandText += " T.LICENCE_TYPE_DESC_F AS LICENCE_TYPE_DESC";
            }
            else {
                commandText += " T.LICENCE_TYPE_DESC_E AS LICENCE_TYPE_DESC";
            }

            commandText += " FROM PUB_ACS.PAS_LICENCE L";
            commandText += " LEFT OUTER JOIN PUB_ACS.PAS_LICENCE_COMPANY C ON L.COMPANY_ID = C.COMPANY_ID";
            commandText += " LEFT OUTER JOIN PUB_ACS.PAS_LICENCE_TYPE T ON L.LICENCE_TYPE_CD = T.LICENCE_TYPE_CD";
            commandText += " WHERE";
            if (!string.IsNullOrEmpty(state))
            {
                if (state.Equals("active"))
                {
                    commandText += " L.END_DATE IS NULL";
                }
                else
                {
                    commandText += " L.END_DATE IS NOT NULL";
                }
            }
            if ((!string.IsNullOrEmpty(state)) && (companyId > 0))
            {
                commandText += " AND";
            }

            if (companyId > 0)
            {
                commandText += " UPPER(L.COMPANY_ID) LIKE '%" + companyId + "%'";
                commandText += " ORDER BY UPPER(L.LICENCE_NAME)";
            }
            using (OracleConnection con = new OracleConnection(MdallDBConnection))
            {
                OracleCommand cmd = new OracleCommand(commandText, con);
                try
                {
                    con.Open();
                    using (OracleDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.HasRows)
                        {
                            while (dr.Read())
                            {
                                var item = new LicenceCompany();
                                item.original_licence_no = dr["ORIGINAL_LICENCE_NO"] == DBNull.Value ? 0 : Convert.ToInt32(dr["ORIGINAL_LICENCE_NO"]);
                                item.licence_status = dr["LICENCE_STATUS"] == DBNull.Value ? string.Empty : dr["LICENCE_STATUS"].ToString().Trim();
                                item.application_id = dr["APPLICATION_ID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["APPLICATION_ID"]);
                                item.appl_risk_class = dr["APPL_RISK_CLASS"] == DBNull.Value ? 0 : Convert.ToInt32(dr["APPL_RISK_CLASS"]);
                                item.licence_name = dr["LICENCE_NAME"] == DBNull.Value ? string.Empty : dr["LICENCE_NAME"].ToString().Trim();
                                item.first_licence_status_dt = dr["FIRST_LICENCE_STATUS_DT"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(dr["FIRST_LICENCE_STATUS_DT"]);
                                item.last_refresh_dt = dr["LAST_REFRESH_DT"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(dr["LAST_REFRESH_DT"]);
                                item.end_date = dr["END_DATE"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(dr["END_DATE"]);
                                //item.licence_type_cd = dr["LICENCE_TYPE_CD"] == DBNull.Value ? string.Empty : dr["LICENCE_TYPE_CD"].ToString().Trim();
                                //item.licence_type_desc = dr["LICENCE_TYPE_DESC"] == DBNull.Value ? string.Empty : dr["LICENCE_TYPE_DESC"].ToString().Trim();
                                item.company_id = dr["COMPANY_ID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["COMPANY_ID"]);
                                item.company_name = dr["COMPANY_NAME"] == DBNull.Value ? string.Empty : dr["COMPANY_NAME"].ToString().Trim();
                                item.addr_line_1 = dr["ADDR_LINE_1"] == DBNull.Value ? string.Empty : dr["ADDR_LINE_1"].ToString().Trim();
                                item.addr_line_2 = dr["ADDR_LINE_2"] == DBNull.Value ? string.Empty : dr["ADDR_LINE_2"].ToString().Trim();
                                item.addr_line_3 = dr["ADDR_LINE_3"] == DBNull.Value ? string.Empty : dr["ADDR_LINE_3"].ToString().Trim();
                                item.postal_code = dr["POSTAL_CODE"] == DBNull.Value ? string.Empty : dr["POSTAL_CODE"].ToString().Trim();
                                item.city = dr["CITY"] == DBNull.Value ? string.Empty : dr["CITY"].ToString().Trim();
                                item.country_cd = dr["COUNTRY_CD"] == DBNull.Value ? string.Empty : dr["COUNTRY_CD"].ToString().Trim();
                                item.region_cd = dr["REGION_CD"] == DBNull.Value ? string.Empty : dr["REGION_CD"].ToString().Trim();
                                item.company_status = dr["COMPANY_STATUS"] == DBNull.Value ? string.Empty : dr["COMPANY_STATUS"].ToString().Trim();

                                items.Add(item);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    string errorMessages = string.Format("DbConnection.cs - GetAllLicence()");
                    ExceptionHelper.LogException(ex, errorMessages);
                }
                finally
                {
                    if (con.State == ConnectionState.Open)
                        con.Close();
                }
            }
            return items;
        }


        public LicenceCompany GetLicenceCompanyById(string lang, Int64 licence_id, string status)
        {
            var licence = new LicenceCompany();
            string commandText = "SELECT L.ORIGINAL_LICENCE_NO, L.LICENCE_STATUS, L.APPLICATION_ID, L.APPL_RISK_CLASS, L.LICENCE_NAME, L.FIRST_LICENCE_STATUS_DT, L.LAST_REFRESH_DT, L.END_DATE, L.LICENCE_TYPE_CD, ";
            commandText += " C.COMPANY_NAME, C.ADDR_LINE_1, C.ADDR_LINE_2, C.ADDR_LINE_3, C.POSTAL_CODE, C.CITY, C.COUNTRY_CD, C.REGION_CD, C.COMPANY_STATUS, C.COMPANY_ID";
            commandText += " T.LICENCE_TYPE_CD";
            if (lang != null && lang.Equals("fr"))
            {
                commandText += " T.LICENCE_TYPE_DESC_F AS LICENCE_TYPE_DESC";
            }
            else {
                commandText += " T.LICENCE_TYPE_DESC_E AS LICENCE_TYPE_DESC";
            }
            commandText += " FROM PUB_ACS.PAS_LICENCE L, PUB_ACS.PAS_LICENCE_COMPANY C, PUB_ACS.PAS_LICENCE_TYPE T";
            commandText += " WHERE L.COMPANY_ID = C.COMPANY_ID AND L.LICENCE_TYPE_CD = T.LICENCE_TYPE_CD";
            if (!string.IsNullOrEmpty(status))
            {
                if (status.Equals("active"))
                {
                    commandText += " AND L.END_DATE IS NULL";
                }
                else
                {
                    commandText += " AND L.END_DATE IS NOT NULL";
                }
            }
            //if(companyId > 0)
            //{
            //    commandText += " AND L.COMPANY_ID = " + companyId;
            //}
            if (licence_id > 0)
            {
                commandText += " AND L.ORIGINAL_LICENCE_NO = :licence_id";
            }

            using (

                OracleConnection con = new OracleConnection(MdallDBConnection))
            {
                OracleCommand cmd = new OracleCommand(commandText, con);
                if (licence_id > 0)
                {
                    cmd.Parameters.Add(":licence_id", licence_id);
                }
                   
                try
                {
                    con.Open();
                    using (OracleDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.HasRows)
                        {
                            while (dr.Read())
                            {
                                var item = new LicenceCompany();
                                item.original_licence_no = dr["ORIGINAL_LICENCE_NO"] == DBNull.Value ? 0 : Convert.ToInt32(dr["ORIGINAL_LICENCE_NO"]);
                                item.licence_status = dr["LICENCE_STATUS"] == DBNull.Value ? string.Empty : dr["LICENCE_STATUS"].ToString().Trim();
                                item.application_id = dr["APPLICATION_ID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["APPLICATION_ID"]);
                                item.appl_risk_class = dr["APPL_RISK_CLASS"] == DBNull.Value ? 0 : Convert.ToInt32(dr["APPL_RISK_CLASS"]);
                                item.licence_name = dr["LICENCE_NAME"] == DBNull.Value ? string.Empty : dr["LICENCE_NAME"].ToString().Trim();
                                item.first_licence_status_dt = dr["FIRST_LICENCE_STATUS_DT"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(dr["FIRST_LICENCE_STATUS_DT"]);
                                item.last_refresh_dt = dr["LAST_REFRESH_DT"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(dr["LAST_REFRESH_DT"]);
                                item.end_date = dr["END_DATE"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(dr["END_DATE"]);
                                item.licence_type_cd = dr["LICENCE_TYPE_CD"] == DBNull.Value ? string.Empty : dr["LICENCE_TYPE_CD"].ToString().Trim();
                                item.licence_type_desc = dr["LICENCE_TYPE_DESC"] == DBNull.Value ? string.Empty : dr["LICENCE_TYPE_DESC"].ToString().Trim();
                                item.company_id = dr["COMPANY_ID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["COMPANY_ID"]);
                                item.company_name = dr["COMPANY_NAME"] == DBNull.Value ? string.Empty : dr["COMPANY_NAME"].ToString().Trim();
                                item.addr_line_1 = dr["ADDR_LINE_1"] == DBNull.Value ? string.Empty : dr["ADDR_LINE_1"].ToString().Trim();
                                item.addr_line_2 = dr["ADDR_LINE_2"] == DBNull.Value ? string.Empty : dr["ADDR_LINE_2"].ToString().Trim();
                                item.addr_line_3 = dr["ADDR_LINE_3"] == DBNull.Value ? string.Empty : dr["ADDR_LINE_3"].ToString().Trim();
                                item.postal_code = dr["POSTAL_CODE"] == DBNull.Value ? string.Empty : dr["POSTAL_CODE"].ToString().Trim();
                                item.city = dr["CITY"] == DBNull.Value ? string.Empty : dr["CITY"].ToString().Trim();
                                item.country_cd = dr["COUNTRY_CD"] == DBNull.Value ? string.Empty : dr["COUNTRY_CD"].ToString().Trim();
                                item.region_cd = dr["REGION_CD"] == DBNull.Value ? string.Empty : dr["REGION_CD"].ToString().Trim();
                                item.company_status = dr["COMPANY_STATUS"] == DBNull.Value ? string.Empty : dr["COMPANY_STATUS"].ToString().Trim();

                                licence = item;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    string errorMessages = string.Format("DbConnection.cs - GetLicenceById()");
                    ExceptionHelper.LogException(ex, errorMessages);
                }
                finally
                {
                    if (con.State == ConnectionState.Open)
                        con.Close();
                }
            }
            return licence;
        }


        public List<ViewLicenceDevice> GetAllLicenceDeviceByLicenceId(string licenceId, string lang)
        {
            var items = new List<ViewLicenceDevice>();
            string commandText = "SELECT PLD.ORIGINAL_LICENCE_NO, PLD.DEVICE_ID, PLD.FIRST_LICENCE_DT, PLD.END_DATE, PLD.TRADE_NAME, PLDI.DEVICE_IDENTIFIER, PLDI.FIRST_LICENCE_DT AS IDENTIFIER_FIRST_LICENCE_DT ";
            commandText += " FROM PUB_ACS.PAS_LICENCE_DEVICE PLD, PUB_ACS.PAS_LICENCE_DEV_IDENT PLDI ";
            commandText += " WHERE PLD.ORIGINAL_LICENCE_NO = PLDI.ORIGINAL_LICENCE_NO ";
            commandText += " AND PLD.DEVICE_ID = PLDI.DEVICE_ID ";
            commandText += " AND PLD.ORIGINAL_LICENCE_NO = :licenceId ";
            commandText += " ORDER BY UPPER(PLD.TRADE_NAME) ";

            using (

                OracleConnection con = new OracleConnection(MdallDBConnection))
            {
                OracleCommand cmd = new OracleCommand(commandText, con);
                cmd.Parameters.Add(":licenceId",licenceId);
                try
                {
                    con.Open();
                    using (OracleDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.HasRows)
                        {
                            while (dr.Read())
                            {
                                var item = new ViewLicenceDevice();
                                item.original_licence_no = dr["ORIGINAL_LICENCE_NO"] == DBNull.Value ? 0 : Convert.ToInt32(dr["ORIGINAL_LICENCE_NO"]);
                                item.device_id = dr["DEVICE_ID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["DEVICE_ID"]);
                                item.first_licence_dt = dr["FIRST_LICENCE_DT"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(dr["FIRST_LICENCE_DT"]);
                                item.end_date = dr["END_DATE"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(dr["END_DATE"]);
                                item.trade_name = dr["TRADE_NAME"] == DBNull.Value ? string.Empty : dr["TRADE_NAME"].ToString().Trim();

                                item.device_identifier = dr["DEVICE_IDENTIFIER"] == DBNull.Value ? string.Empty : dr["DEVICE_IDENTIFIER"].ToString().Trim();
                                item.device_identifier_first_date = dr["IDENTIFIER_FIRST_LICENCE_DT"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(dr["IDENTIFIER_FIRST_LICENCE_DT"]);
                                //device = item;
                                items.Add(item);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    string errorMessages = string.Format("DbConnection.cs - GetLicenceDevice()");
                    ExceptionHelper.LogException(ex, errorMessages);
                }
                finally
                {
                    if (con.State == ConnectionState.Open)
                        con.Close();
                }
            }
            return items;
        }

        public List<LicenceType> GetAllLicenceType(string lang)
        {
            var items = new List<LicenceType>();
            string commandText = "SELECT LICENCE_TYPE_CD,";
            if (string.IsNullOrEmpty(lang))
            {
                commandText += " LICENCE_TYPE_DESC_E AS LICENCE_TYPE_DESC";

            }
            else if (lang != null && lang.Equals("fr"))
            {
                commandText += " LICENCE_TYPE_DESC_F AS LICENCE_TYPE_DESC";
            }
            else if (lang.Equals("en"))
            {
                commandText += " LICENCE_TYPE_DESC_E AS LICENCE_TYPE_DESC";
            }
            commandText += " FROM PUB_ACS.PAS_LICENCE_TYPE";

            using (OracleConnection con = new OracleConnection(MdallDBConnection))
            {
                OracleCommand cmd = new OracleCommand(commandText, con);
                try
                {
                    con.Open();
                    using (OracleDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.HasRows)
                        {
                            while (dr.Read())
                            {
                                var item = new LicenceType();
                                item.licence_type_cd = dr["LICENCE_TYPE_CD"] == DBNull.Value ? string.Empty : dr["LICENCE_TYPE_CD"].ToString().Trim();
                                item.licence_type_desc = dr["LICENCE_TYPE_DESC"] == DBNull.Value ? string.Empty : dr["LICENCE_TYPE_DESC"].ToString().Trim();

                                items.Add(item);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    string errorMessages = string.Format("DbConnection.cs - GetAllLicenceType()");
                    ExceptionHelper.LogException(ex, errorMessages);
                }
                finally
                {
                    if (con.State == ConnectionState.Open)
                        con.Close();
                }
            }
            return items;
        }

        public LicenceType GetLicenceTypeByCode(string code)
        {
            var type = new LicenceType();
            string commandText = "SELECT LICENCE_TYPE_CD, ";
            if (Lang != null && Lang.Equals("fr"))
            {
                commandText += " LICENCE_TYPE_DESC_F AS LICENCE_TYPE_DESC";
            }
            else {
                commandText += " LICENCE_TYPE_DESC_E AS LICENCE_TYPE_DESC";
            }
            commandText += " FROM PUB_ACS.PAS_LICENCE_TYPE";
            commandText += " WHERE LICENCE_TYPE_CD = :code ";

            using (OracleConnection con = new OracleConnection(MdallDBConnection))
            {
                OracleCommand cmd = new OracleCommand(commandText, con);
                cmd.Parameters.Add(":code", code.Trim());
                try
                {
                    con.Open();
                    using (OracleDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.HasRows)
                        {
                            while (dr.Read())
                            {
                                var item = new LicenceType();
                                item.licence_type_cd = dr["LICENCE_TYPE_CD"] == DBNull.Value ? string.Empty : dr["LICENCE_TYPE_CD"].ToString().Trim();
                                item.licence_type_desc = dr["LICENCE_TYPE_DESC"] == DBNull.Value ? string.Empty : dr["LICENCE_TYPE_DESC"].ToString().Trim();

                                type = item;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    string errorMessages = string.Format("DbConnection.cs - GetLicenceTypeByCode()");
                    ExceptionHelper.LogException(ex, errorMessages);
                }
                finally
                {
                    if (con.State == ConnectionState.Open)
                        con.Close();
                }
            }
            return type;
        }

        public List<SbdLocation> GetAllSbdLocation()
        {
            var items = new List<SbdLocation>();
            string commandText = "SELECT * FROM PUB_ACS.PAS_SBD_LOCATION_INFO";

            using (OracleConnection con = new OracleConnection(MdallDBConnection))
            {
                OracleCommand cmd = new OracleCommand(commandText, con);
                try
                {
                    con.Open();
                    using (OracleDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.HasRows)
                        {
                            while (dr.Read())
                            {
                                var item = new SbdLocation();
                                item.original_licence_no = dr["ORIGINAL_LICENCE_NO"] == DBNull.Value ? 0 : Convert.ToInt32(dr["ORIGINAL_LICENCE_NO"]);
                                item.sbd_notice_web_loc = dr["SBD_NOTICE_WEB_LOC"] == DBNull.Value ? string.Empty : dr["SBD_NOTICE_WEB_LOC"].ToString().Trim();
                                item.sbd_web_loc = dr["SBD_WEB_LOC"] == DBNull.Value ? string.Empty : dr["SBD_WEB_LOC"].ToString().Trim();
                                /*
                                item.pkg_insert1_keyword = dr["PKG_INSERT1_KEYWORD"] == DBNull.Value ? string.Empty : dr["PKG_INSERT1_KEYWORD"].ToString().Trim();
                                item.pkg_insert1_loc = dr["PKG_INSERT1_LOC"] == DBNull.Value ? string.Empty : dr["PKG_INSERT1_LOC"].ToString().Trim();
                                item.pkg_insert2_keyword = dr["PKG_INSERT2_KEYWORD"] == DBNull.Value ? string.Empty : dr["PKG_INSERT2_KEYWORD"].ToString().Trim();
                                item.pkg_insert2_loc = dr["PKG_INSERT2_LOC"] == DBNull.Value ? string.Empty : dr["PKG_INSERT2_LOC"].ToString().Trim();
                                item.pkg_insert3_keyword = dr["PKG_INSERT3_KEYWORD"] == DBNull.Value ? string.Empty : dr["PKG_INSERT3_KEYWORD"].ToString().Trim();
                                item.pkg_insert3_loc = dr["PKG_INSERT3_LOC"] == DBNull.Value ? string.Empty : dr["PKG_INSERT3_LOC"].ToString().Trim();
                                item.op_manual1_keyword = dr["OP_MANUAL1_KEYWORD"] == DBNull.Value ? string.Empty : dr["OP_MANUAL1_KEYWORD"].ToString().Trim();
                                item.op_manual1_loc = dr["OP_MANUAL1_LOC"] == DBNull.Value ? string.Empty : dr["OP_MANUAL1_LOC"].ToString().Trim();
                                item.op_manual2_keyword = dr["OP_MANUAL2_KEYWORD"] == DBNull.Value ? string.Empty : dr["OP_MANUAL2_KEYWORD"].ToString().Trim();
                                item.op_manual2_loc = dr["OP_MANUAL2_LOC"] == DBNull.Value ? string.Empty : dr["OP_MANUAL2_LOC"].ToString().Trim();
                                item.op_manual3_keyword = dr["OP_MANUAL3_KEYWORD"] == DBNull.Value ? string.Empty : dr["OP_MANUAL3_KEYWORD"].ToString().Trim();
                                item.op_manual3_loc = dr["OP_MANUAL3_LOC"] == DBNull.Value ? string.Empty : dr["OP_MANUAL3_LOC"].ToString().Trim();
                                item.additional_op_pkg = dr["ADDITIONAL_OP_PKG"] == DBNull.Value ? string.Empty : dr["ADDITIONAL_OP_PKG"].ToString().Trim();
                                */

                                items.Add(item);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    string errorMessages = string.Format("DbConnection.cs - GetAllSbdLocationInfo()");
                    ExceptionHelper.LogException(ex, errorMessages);
                }
                finally
                {
                    if (con.State == ConnectionState.Open)
                        con.Close();
                }
            }
            return items;
        }

        public SbdLocation GetSbdLocationById(int id)
        {
            var sbdLocationInfo = new SbdLocation();
            string commandText = "SELECT * FROM PUB_ACS.PAS_SBD_LOCATION_INFO WHERE ORIGINAL_LICENCE_NO = :id ";

            using (OracleConnection con = new OracleConnection(MdallDBConnection))
            {
                OracleCommand cmd = new OracleCommand(commandText, con);
                cmd.Parameters.Add(":id", id);
                try
                {
                    con.Open();
                    using (OracleDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.HasRows)
                        {
                            while (dr.Read())
                            {
                                var item = new SbdLocation();
                                item.original_licence_no = dr["ORIGINAL_LICENCE_NO"] == DBNull.Value ? 0 : Convert.ToInt32(dr["ORIGINAL_LICENCE_NO"]);
                                item.sbd_notice_web_loc = dr["SBD_NOTICE_WEB_LOC"] == DBNull.Value ? string.Empty : dr["SBD_NOTICE_WEB_LOC"].ToString().Trim();
                                item.sbd_web_loc = dr["SBD_WEB_LOC"] == DBNull.Value ? string.Empty : dr["SBD_WEB_LOC"].ToString().Trim();
                                /*
                                item.pkg_insert1_keyword = dr["PKG_INSERT1_KEYWORD"] == DBNull.Value ? string.Empty : dr["PKG_INSERT1_KEYWORD"].ToString().Trim();
                                item.pkg_insert1_loc = dr["PKG_INSERT1_LOC"] == DBNull.Value ? string.Empty : dr["PKG_INSERT1_LOC"].ToString().Trim();
                                item.pkg_insert2_keyword = dr["PKG_INSERT2_KEYWORD"] == DBNull.Value ? string.Empty : dr["PKG_INSERT2_KEYWORD"].ToString().Trim();
                                item.pkg_insert2_loc = dr["PKG_INSERT2_LOC"] == DBNull.Value ? string.Empty : dr["PKG_INSERT2_LOC"].ToString().Trim();
                                item.pkg_insert3_keyword = dr["PKG_INSERT3_KEYWORD"] == DBNull.Value ? string.Empty : dr["PKG_INSERT3_KEYWORD"].ToString().Trim();
                                item.pkg_insert3_loc = dr["PKG_INSERT3_LOC"] == DBNull.Value ? string.Empty : dr["PKG_INSERT3_LOC"].ToString().Trim();
                                item.op_manual1_keyword = dr["OP_MANUAL1_KEYWORD"] == DBNull.Value ? string.Empty : dr["OP_MANUAL1_KEYWORD"].ToString().Trim();
                                item.op_manual1_loc = dr["OP_MANUAL1_LOC"] == DBNull.Value ? string.Empty : dr["OP_MANUAL1_LOC"].ToString().Trim();
                                item.op_manual2_keyword = dr["OP_MANUAL2_KEYWORD"] == DBNull.Value ? string.Empty : dr["OP_MANUAL2_KEYWORD"].ToString().Trim();
                                item.op_manual2_loc = dr["OP_MANUAL2_LOC"] == DBNull.Value ? string.Empty : dr["OP_MANUAL2_LOC"].ToString().Trim();
                                item.op_manual3_keyword = dr["OP_MANUAL3_KEYWORD"] == DBNull.Value ? string.Empty : dr["OP_MANUAL3_KEYWORD"].ToString().Trim();
                                item.op_manual3_loc = dr["OP_MANUAL3_LOC"] == DBNull.Value ? string.Empty : dr["OP_MANUAL3_LOC"].ToString().Trim();
                                item.additional_op_pkg = dr["ADDITIONAL_OP_PKG"] == DBNull.Value ? string.Empty : dr["ADDITIONAL_OP_PKG"].ToString().Trim();
                                */
                                sbdLocationInfo = item;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    string errorMessages = string.Format("DbConnection.cs - GetSbdLocationInfoById()");
                    ExceptionHelper.LogException(ex, errorMessages);
                }
                finally
                {
                    if (con.State == ConnectionState.Open)
                        con.Close();
                }
            }
            return sbdLocationInfo;
        }

    }

}