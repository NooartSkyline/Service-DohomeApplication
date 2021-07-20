using Newtonsoft.Json;
using SourceCode.SmartObjects.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace Webservice_DohomeApplication
{
    /// <summary>
    /// Summary description for Check_location_area
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class Check_location_area : System.Web.Services.WebService
    {
        public string Connect = System.Configuration.ConfigurationManager.ConnectionStrings["DBMASTERConnectionString"].ConnectionString;
        Class_Configserver configserver = new Class_Configserver();
        private SmartObjectClientServer K2_CONNECT_SERVER()
        {
            DataSet ds = new DataSet();
            SmartObjectClientServer serverName = new SmartObjectClientServer();
            SourceCode.Hosting.Client.BaseAPI.SCConnectionStringBuilder connectionString = new SourceCode.Hosting.Client.BaseAPI.SCConnectionStringBuilder();
            connectionString.Authenticate = true;
            connectionString.Host = System.Configuration.ConfigurationManager.AppSettings["K2Server"];
            connectionString.Integrated = true;
            connectionString.IsPrimaryLogin = true;
            connectionString.Port = Convert.ToUInt32(System.Configuration.ConfigurationManager.AppSettings["Port"].ToString());
            connectionString.UserID = System.Configuration.ConfigurationManager.AppSettings["K2User"];
            connectionString.WindowsDomain = System.Configuration.ConfigurationManager.AppSettings["Domain"];
            connectionString.Password = System.Configuration.ConfigurationManager.AppSettings["K2Password"];
            connectionString.SecurityLabelName = System.Configuration.ConfigurationManager.AppSettings["SecurityLabel"];
            serverName.CreateConnection();
            serverName.Connection.Open(connectionString.ToString());
            return serverName;
        }

        #region 11/10/2018

        // บันทึกข้อมูลตรวจสอบตำแหน่งสินค้า (11/10/2018)
        [WebMethod]
        public string InsertToTBTrans_Check_Product_Location(string Jsonstring,string Jsonstring_reason)
        {
            DataTable objdt = new DataTable();            
            try
            {
                DataTable dt = new DataTable();
                DataTable dt2 = new DataTable();
                dt.TableName = "TBTrans_Gen_Docno_Check_Product_Location";
                dt = this.JsonToTable(Jsonstring);
               string docno = this.PreRunningNewDocNoCheckProductlocation(dt.Rows[0]["SITE"].ToString());
                string stringcon = configserver.StringConn(dt.Rows[0]["SITE"].ToString());
                string Leftdocno = LeftString(docno, 2);

                dt2.TableName = "TBTrans_Check_Product_Location_LossReason";
                dt2 = this.JsonToTable(Jsonstring_reason);

                SqlConnection conn = new SqlConnection();
                //conn.ConnectionString = Connect;
                conn.ConnectionString = stringcon;
                SqlCommand command = new SqlCommand();
                command.Connection = conn;
                command.CommandText = "SELECT TOP 1 DOCNO  FROM DBTRANS..TBTrans_Gen_Docno_Check_Product_Location where DOCNO like '%" + Leftdocno + "%' order by DOCNO desc";
                SqlDataAdapter da = new SqlDataAdapter(command);
                da.Fill(objdt);
                objdt.TableName = "TBTrans_Gen_Docno";
                if (dt.Rows.Count > 0 && objdt.Rows.Count > 0)
                {
                    conn = new SqlConnection();
                    //conn.ConnectionString = Connect;
                    conn.ConnectionString = stringcon;
                    conn.Open();
                    using (SqlTransaction tran = conn.BeginTransaction())
                    {
                        try
                        {
                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                String query = "INSERT INTO DBTRANS..TBTrans_Check_Product_Location"
                                             + "(DOCNO,"
                                             + "DOCDATE,"
                                             + "BRANCH,"
                                             + "SITE,"
                                             + "SLOC,"
                                             + "BINCODE,"
                                             //+ "BARCODE,"
                                             //+ "PRODUCTCODE,"
                                             //+ "PRODUCTNAME,"
                                             //+ "UNITCODE,"
                                             //+ "UNITNAME,"
                                             //+ "STATUS,"
                                             + "DATECREATE,"
                                             + "REMARKS,"
                                             + "EMPLOYEECODE,"
                                             + "EMPLOYEENAME,"
                                             + "CARETAKERSCODE,"
                                             + "CARETAKERSNAME)"

                                             // Value
                                             + " VALUES (@DOCNO,"
                                             + "@DOCDATE,"
                                             + "@BRANCH,"
                                             + "@SITE,"
                                             + "@SLOC,"
                                             + "@BINCODE,"
                                             //+ "@BARCODE,"
                                             //+ "@PRODUCTCODE,"
                                             //+ "@PRODUCTNAME,"
                                             //+ "@UNITCODE,"
                                             //+ "@UNITNAME,"
                                             //+ "@STATUS,"
                                             + "@DATECREATE,"
                                             + "@REMARKS,"
                                             + "@EMPLOYEECODE,"
                                             + "@EMPLOYEENAME,"
                                             + "@CARETAKERSCODE,"
                                             + "@CARETAKERSNAME)";
                                command = new SqlCommand(query, conn, tran);
                                // Parameters.AddWithValue
                                command.Parameters.AddWithValue("@DOCNO", objdt.Rows[0]["DOCNO"].ToString());
                                command.Parameters.AddWithValue("@DOCDATE", DateTime.Now.Date);
                                command.Parameters.AddWithValue("@BRANCH", dt.Rows[i]["BRANCH"].ToString());
                                command.Parameters.AddWithValue("@SITE", dt.Rows[i]["SITE"].ToString());
                                command.Parameters.AddWithValue("@SLOC", dt.Rows[i]["SLOC"].ToString());
                                command.Parameters.AddWithValue("@BINCODE", dt.Rows[i]["BINCODE"].ToString());
                                //command.Parameters.AddWithValue("@BARCODE", dt.Rows[i]["BARCODE"].ToString());
                                //command.Parameters.AddWithValue("@PRODUCTCODE", dt.Rows[i]["PRODUCTCODE"].ToString());
                                //command.Parameters.AddWithValue("@PRODUCTNAME", dt.Rows[i]["PRODUCTNAME"].ToString());
                                //command.Parameters.AddWithValue("@UNITCODE", dt.Rows[i]["UNITCODE"].ToString());
                                //command.Parameters.AddWithValue("@UNITNAME", dt.Rows[i]["UNITNAME"].ToString());
                                //command.Parameters.AddWithValue("@STATUS", dt.Rows[i]["STATUS"].ToString());
                                command.Parameters.AddWithValue("@DATECREATE", DateTime.Now);
                                command.Parameters.AddWithValue("@REMARKS", dt.Rows[i]["REMARKS"].ToString());
                                command.Parameters.AddWithValue("@EMPLOYEECODE", dt.Rows[i]["EMPLOYEECODE"].ToString());
                                command.Parameters.AddWithValue("@EMPLOYEENAME", dt.Rows[i]["EMPLOYEENAME"].ToString());
                                command.Parameters.AddWithValue("@CARETAKERSCODE", dt.Rows[i]["CARETAKERSCODE"].ToString());
                                command.Parameters.AddWithValue("@CARETAKERSNAME", dt.Rows[i]["CARETAKERSNAME"].ToString());
                                command.ExecuteNonQuery();
                            }
                            //tran.Commit();
                            for (int i = 0; i < dt2.Rows.Count; i++)
                            {
                                if (dt2.Rows[i]["ID"].ToString() != "") // ให้เปลี่ยนชื่อ Field REASONCODE เป็นชื่อ Field ที่ถูกส่งเข้ามา
                                {
                                    string strsql = "INSERT INTO DBTRANS..TBTrans_Check_Product_Location_LossReason"
                                                                             + "(DOCNO,"
                                                                             + "REASONCODE,"
                                                                             + "REASONNAME,"
                                                                             + "STATUS)"

                                                                             // Value
                                                                             + " VALUES (@DOCNO,"
                                                                             + "@REASONCODE,"
                                                                             + "@REASONNAME,"
                                                                             + "@STATUS)";
                                    command = new SqlCommand(strsql, conn, tran);
                                    // Parameters.AddWithValue
                                    command.Parameters.AddWithValue("@DOCNO", objdt.Rows[0]["DOCNO"].ToString());
                                    command.Parameters.AddWithValue("@REASONCODE", dt2.Rows[i]["ID"].ToString());
                                    command.Parameters.AddWithValue("@REASONNAME", dt2.Rows[i]["REASONDESCRIPTION"].ToString());
                                    command.Parameters.AddWithValue("@STATUS", dt2.Rows[i]["STATUS"].ToString());
                                    command.ExecuteNonQuery();
                                }
                            }
                            tran.Commit();
                            conn.Close();
                        }
                        catch (Exception ex)
                        {
                            tran.Rollback();
                            conn.Close();
                            return ex.Message;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
            return objdt.Rows[0]["DOCNO"].ToString();
        }

        // Get local Bin Code (11/10/2018)
        [WebMethod]
        public string GetBinCode_From_TBTrans_Check_Product_Location(string BranchCode, string Site, string Sloc, string BinCode, string EmpCode)
        {
            string stringcon = configserver.StringConn(Site);
            dev.lib.Utilities util = new dev.lib.Utilities();
            SqlConnection conn = new SqlConnection();
            //conn.ConnectionString = Connect;
            conn.ConnectionString = stringcon;
            SqlCommand command = new SqlCommand();
            command.Connection = conn;
            //string sDate = util.ConvertDateToString(DateTime.Now.Date);
            string sDate = DateTime.Now.ToString("yyyy/MM/dd", new CultureInfo("en-US"));
            command.CommandText = "select DOCNO, DATECREATE, BRANCH, SITE, SLOC, BINCODE, EMPLOYEECODE from DBTRANS..TBTrans_Check_Product_Location"
                                + " where DOCDATE = '" + sDate + "'"
                                + " and BRANCH = '" + BranchCode + "'"
                                + " and site = '" + Site + "'"
                                + " and sloc = '" + Sloc + "'"
                                + " and bincode like '%" + BinCode + "%' "
                                + " and EMPLOYEECODE = '" + EmpCode + "'";
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(command);
            da.Fill(dt);
            dt.TableName = "TB_Product_Location";
            ds.Tables.Add(dt);
            return ConvertDataSetToJSON(ds);
        }

        // Get local Bin Code And Reason Detail (10/10/2018)
        [WebMethod]
        public string GetBinCode_Detail_From_TBTrans_Check_Product_Location(string BinCode,string Docno,string site)
        {
            string stringcon = configserver.StringConn(site);
            dev.lib.Utilities util = new dev.lib.Utilities();
            SqlConnection conn = new SqlConnection();
            //conn.ConnectionString = Connect;
            conn.ConnectionString = stringcon;
            SqlCommand command = new SqlCommand();
            command.Connection = conn;
            //string sDate = util.ConvertDateToString(DateTime.Now.Date);
            string sDate = DateTime.Now.ToString("yyyy/MM/dd", new CultureInfo("en-US"));
            command.CommandText = "select *"
                                + " from DBTRANS..TBTrans_Check_Product_Location "
                                + " where DOCDATE = '" + sDate + "'"
                                + " and Docno = '" + Docno + "'"
                                + " and bincode like '%" + BinCode + "%' ";

            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(command);
            da.Fill(dt);
            dt.TableName = "TB_Product_Location";
            ds.Tables.Add(dt);

            SqlCommand command2 = new SqlCommand();
            command2.Connection = conn;

            command2.CommandText = "select *"                             
                               + " from DBTRANS..TBTrans_Check_Product_Location_LossReason"
                               + " where Docno = '" + Docno + "'";
 
            DataTable dt2 = new DataTable();
            SqlDataAdapter da2 = new SqlDataAdapter(command2);
            da2.Fill(dt2);
            dt2.TableName = "TB_Reason";
            ds.Tables.Add(dt2);

            return ConvertDataSetToJSON(ds);
        }

        [WebMethod]
        public string GetLocation_Reason()
        {
            DataTable dt = new DataTable();
            dev.lib.SQLConnection devdb = new dev.lib.SQLConnection(Connect);
            SqlConnection cnn = devdb.getSqlConncetion;
            DataSet ds = new DataSet();
            string strsql = "select ID, REASONDESCRIPTION from DBMASTER..TBMaster_Check_Product_Location_Reason";
            bool ret = devdb.ExecuteCommand(ref ds, strsql, "tbl", ref cnn);
            if (ret)
            {
                if (ds.Tables["tbl"].Rows.Count > 0)
                {
                    ds.Tables.Add(dt);
                }
            }
            return ConvertDataSetToJSON(ds);
        }

        [WebMethod]
        public string Check_TBTrans_Check_Product_Location(string Site,string Sloc,string Bincode)
        {
            dev.lib.Utilities util = new dev.lib.Utilities();
            string a = "0";
            DataTable dt = new DataTable();
            dev.lib.SQLConnection devdb = new dev.lib.SQLConnection(Connect);
            SqlConnection cnn = devdb.getSqlConncetion;
            DataSet ds = new DataSet();
            //string sDate = util.ConvertDateToString(DateTime.Now.Date);
            string sDate = DateTime.Now.ToString("yyyy/MM/dd", new CultureInfo("en-US"));
            string strsql = "select * from DBTRANS..TBTrans_Check_Product_Location where SITE = '" + Site 
                             + "' and SLOC = '"+ Sloc 
                             + "' and BINCODE = '" + Bincode 
                             + "' and DOCDATE = '" + sDate + "'";

            bool ret = devdb.ExecuteCommand(ref ds, strsql, "tbl", ref cnn);
            if (ret)
            {
                if (ds.Tables["tbl"].Rows.Count > 0)
                {
                  a = "1";
                }
                else
                {
                  a = "0";
                }
            }
            return a;
        }
        [WebMethod]
        public string PreRunningNewDocNoCheckProductlocation(string SITE)
        {
            string sResult = "";
            string sShortName;
            string stringcon = configserver.StringConn(SITE);
            //dev.lib.SQLConnection devdb = new dev.lib.SQLConnection(Connect);
            dev.lib.SQLConnection devdb = new dev.lib.SQLConnection(stringcon);
            dev.lib.UtilityDataBase dbutil = new dev.lib.UtilityDataBase();
            SqlConnection Conn1 = devdb.getSqlConncetion;
            //dbutil.ConnectionString = Connect;
            dbutil.ConnectionString = stringcon;
            dbutil.RunningFieldName = "DOCNO";
            //string sformat = "yyyyMMdd-####";
            string sformat = "-yyMMdd####";
            //string Leftsite = LeftString(SITE, 2);
            //string sShortName = Leftsite + "LS";

            string substr = SITE.ToUpper().Substring(0, 2);
            if (substr == "M0")
            {
                sShortName = SITE + "LA";
            }
            else
            {
                sShortName = substr + "LA";
            }

            dbutil.RunningGroup = sShortName;
            dbutil.RunningFormat = sformat;
            dbutil.RunningTableName = "dbtrans.dbo.TBTrans_Gen_Docno_Check_Product_Location";
            sResult = dbutil.RunningNewDocNo(devdb);
            string sql_update = "INSERT INTO dbtrans.dbo.TBTrans_Gen_Docno_Check_Product_Location (DOCNO) VALUES ('" + sResult + "')";
            devdb.ExecuteNoneQuery(sql_update, ref Conn1);
            return sResult;
        }

        #endregion

        public string ConvertDataSetToJSON(DataSet ds)
        {
            string JSONString = string.Empty;
            if (ds != null)
            {
                return JsonConvert.SerializeObject(ds, Formatting.Indented);
            }
            else
            {
                return JSONString;
            }
        }
        public static string LeftString(string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) return value;
            maxLength = Math.Abs(maxLength);

            return (value.Length <= maxLength
                   ? value
                   : value.Substring(0, maxLength)
                   );
        }
        public DataTable JsonToTable(String JsonString)
        {
            DataTable dt = (DataTable)JsonConvert.DeserializeObject(JsonString, typeof(DataTable));
            return dt;
        }
    }
    
}
