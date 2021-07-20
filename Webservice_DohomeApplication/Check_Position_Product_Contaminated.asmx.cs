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
    /// Summary description for Check_Position_Product_Contaminated
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class Check_Position_Product_Contaminated : System.Web.Services.WebService
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

        #region Create By : Naruenart Matsombut
        // ค้นหาสินค้าตาม ARTICLECODE
        [WebMethod]
        public string GETARTICLE_BY_ARTICLECODE(string ARTICLECODE ,string UNITCODE)
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = Connect;
            SqlCommand command = new SqlCommand();
            command.Connection = conn;
            command.CommandText = "select top 100 tProduct.code as ArticleCode, tProduct.nameth as ArticleName, tBarcode.UNITCODE, tUnit.myname as UNITNAME, tBarcode.Barcode"
                                + " from TBMaster_Product as tProduct inner join "
                                + " TBMaster_Barcode as tBarcode on tProduct.code = tBarcode.productcode inner join"
                                + " TBMaster_Unit as tUnit on tBarcode.UnitCode = tUnit.code"
                                + " where tProduct.code = '" + ARTICLECODE + "' and tBarcode.UNITCODE = '"+ UNITCODE + "'";
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(command);
            da.Fill(dt);
            dt.TableName = "TBMaster_Product";
            ds.Tables.Add(dt);

            return ConvertDataSetToJSON(ds);
        }
        // ค้นหาสินค้าตาม BARCODE
        [WebMethod]
        public string GETARTICLE_BY_BARCODE(string BARCODE ,string UNITCODE)
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = Connect;
            SqlCommand command = new SqlCommand();
            command.Connection = conn;
            command.CommandText = "select top 100 tProduct.code as ArticleCode, tProduct.nameth as ArticleName, tBarcode.UNITCODE, tUnit.myname as UNITNAME, tBarcode.Barcode"
                                + " from TBMaster_Product as tProduct inner join "
                                + " TBMaster_Barcode as tBarcode on tProduct.code = tBarcode.productcode inner join"
                                + " TBMaster_Unit as tUnit on tBarcode.UnitCode = tUnit.code"
                                + " where tBarcode.Barcode = '" + BARCODE + "' and tBarcode.UNITCODE = '" + UNITCODE + "'";
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(command);
            da.Fill(dt);
            dt.TableName = "TBMaster_Product";
            ds.Tables.Add(dt);

            return ConvertDataSetToJSON(ds);
        }
        // บันทึกข้อมูล
        [WebMethod]
        public string InsertToTBTrans_Check_Position_Product_Contaminated(string Jsonstring)
        {
            DataTable objdt = new DataTable();
            
            try
            {
                DateTime oDate = DateTime.Now.Date;
                string sDate = oDate.ToString("yyyy/MM/dd", new CultureInfo("en-US"));

                DataTable dt = new DataTable();
                dt.TableName = "TBTrans_Gen_Docno_Check_Position_Product_Contaminated";
                dt = this.JsonToTable(Jsonstring);
                string docno = this.RunningNewDocNo(dt.Rows[0]["SITE"].ToString());
                string stringcon = configserver.StringConn(dt.Rows[0]["SITE"].ToString());
                string Leftdocno = LeftString(docno, 2);

                SqlConnection conn = new SqlConnection();
                //conn.ConnectionString = Connect;
                conn.ConnectionString = stringcon;
                SqlCommand command = new SqlCommand();
                command.Connection = conn;
                command.CommandText = "SELECT TOP 1 DOCNO  FROM DBTRANS..TBTrans_Gen_Docno_Check_Position_Product_Contaminated where DOCNO like '%" + Leftdocno + "%' order by DOCNO desc";
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
                                String query = "INSERT INTO DBTRANS..TBTrans_Check_Position_Product_Contaminated"
                                             + "(DOCNO,"
                                             + "ROWORDER,"
                                             + "DOCDATE,"
                                             + "BRANCH,"
                                             + "SITE,"
                                             + "SLOC,"
                                             + "BINCODE,"
                                             + "BARCODE,"
                                             + "PRODUCTCODE,"
                                             + "PRODUCTNAME,"
                                             + "UNITCODE,"
                                             + "UNITNAME,"
                                             + "STATUS,"
                                             + "DATECREATE,"
                                             + "EMPLOYEECODE,"
                                             + "EMPLOYEENAME,"
                                             + "CARETAKERCODE,"
                                             + "CARETAKERNAME)"

                                             // Value
                                             + " VALUES (@DOCNO,"
                                             + "@ROWORDER,"
                                             + "@DOCDATE,"
                                             + "@BRANCH,"
                                             + "@SITE,"
                                             + "@SLOC,"
                                             + "@BINCODE,"
                                             + "@BARCODE,"
                                             + "@PRODUCTCODE,"
                                             + "@PRODUCTNAME,"
                                             + "@UNITCODE,"
                                             + "@UNITNAME,"
                                             + "@STATUS,"
                                             + "@DATECREATE,"
                                             + "@EMPLOYEECODE,"
                                             + "@EMPLOYEENAME,"
                                             + "@CARETAKERCODE,"
                                             + "@CARETAKERNAME)";
                                command = new SqlCommand(query, conn, tran);


                                // Parameters.AddWithValue
                                command.Parameters.AddWithValue("@DOCNO", objdt.Rows[0]["DOCNO"].ToString());
                                command.Parameters.AddWithValue("@ROWORDER",  dt.Rows[i]["ROWORDER"].ToString());
                                command.Parameters.AddWithValue("@DOCDATE", sDate); //DateTime.Now.Date);
                                command.Parameters.AddWithValue("@BRANCH", dt.Rows[i]["BRANCH"].ToString());
                                command.Parameters.AddWithValue("@SITE", dt.Rows[i]["SITE"].ToString());
                                command.Parameters.AddWithValue("@SLOC", dt.Rows[i]["SLOC"].ToString());
                                command.Parameters.AddWithValue("@BINCODE", dt.Rows[i]["BINCODE"].ToString());
                                command.Parameters.AddWithValue("@BARCODE", dt.Rows[i]["BARCODE"].ToString());
                                command.Parameters.AddWithValue("@PRODUCTCODE", dt.Rows[i]["PRODUCTCODE"].ToString());
                                command.Parameters.AddWithValue("@PRODUCTNAME", dt.Rows[i]["PRODUCTNAME"].ToString());
                                command.Parameters.AddWithValue("@UNITCODE", dt.Rows[i]["UNITCODE"].ToString());
                                command.Parameters.AddWithValue("@UNITNAME", dt.Rows[i]["UNITNAME"].ToString());
                                command.Parameters.AddWithValue("@STATUS", dt.Rows[i]["STATUS"].ToString());
                                command.Parameters.AddWithValue("@DATECREATE", DateTime.Now);
                                command.Parameters.AddWithValue("@EMPLOYEECODE", dt.Rows[i]["EMPLOYEECODE"].ToString());
                                command.Parameters.AddWithValue("@EMPLOYEENAME", dt.Rows[i]["EMPLOYEENAME"].ToString());
                                command.Parameters.AddWithValue("@CARETAKERCODE", dt.Rows[i]["CARETAKERCODE"].ToString());
                                command.Parameters.AddWithValue("@CARETAKERNAME", dt.Rows[i]["CARETAKERNAME"].ToString());
                                command.ExecuteNonQuery();
                            }
                            tran.Commit();
                            conn.Close();
                        }
                        catch (Exception ex)
                        {
                            tran.Rollback();
                            conn.Close();
                            return ex.StackTrace.ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return ex.StackTrace.ToString();
            }
            return objdt.Rows[0]["DOCNO"].ToString();
        }
        // Get Storage Bin หรือ Bin Code (K2 Connect)
        // BAPI : ZHH_GET_BIN_ASSIGNLOC
        [WebMethod]
        public string GetStorageBinFromSAP(string ProductCode, string BinCode, string SiteCode, string Sloc)
        {
            DataSet ds = new DataSet();
            if (SiteCode != "" && Sloc != "")
            {
                SmartObjectClientServer serverName = K2_CONNECT_SERVER();// Connect K2 Server 
                SmartObject smartObject = serverName.GetSmartObject("ZHH_GET_BIN_ASSIGNLOC");//ชื่อ SmartOject
                smartObject.MethodToExecute = "ZHH_GET_BINASSIGNLOC_ZHH_GET_BIN_ASSIGNLOC";
                //Set parameter
                if (ProductCode != "")
                    smartObject.ListMethods["ZHH_GET_BINASSIGNLOC_ZHH_GET_BIN_ASSIGNLOC"].InputProperties["p_I_MATNR"].Value = ProductCode;
                if (BinCode != "")
                    smartObject.ListMethods["ZHH_GET_BINASSIGNLOC_ZHH_GET_BIN_ASSIGNLOC"].InputProperties["p_I_BINCODE"].Value = BinCode.ToUpper();
                smartObject.ListMethods["ZHH_GET_BINASSIGNLOC_ZHH_GET_BIN_ASSIGNLOC"].InputProperties["p_I_WERKS"].Value = SiteCode;
                smartObject.ListMethods["ZHH_GET_BINASSIGNLOC_ZHH_GET_BIN_ASSIGNLOC"].InputProperties["p_I_LGORT"].Value = Sloc;
                DataTable dt = serverName.ExecuteListDataTable(smartObject);

                dt.Columns.Add("Article_Name");

                foreach (DataRow row in dt.Rows)
                {
                    row["O_ASSIGNLOC_MATERIAL"] = row["O_ASSIGNLOC_MATERIAL"].ToString().TrimStart('0');
                    dev.lib.SQLConnection devdb = new dev.lib.SQLConnection(Connect);
                    SqlConnection cnn = devdb.getSqlConncetion;
                    DataSet objds = new DataSet();
                    string strsql = "select top 1 * " +
                                    "from dbmaster..tbmaster_product " +
                                    "where code = '" + row["O_ASSIGNLOC_MATERIAL"].ToString() + "'";
                    bool ret = devdb.ExecuteCommand(ref objds, strsql, "tbl", ref cnn);
                    cnn.Close();
                    if (ret)
                    {
                        if (objds.Tables["tbl"].Rows.Count > 0)
                        {
                            DataRow dr = objds.Tables["tbl"].Rows[0];
                            row["Article_Name"] = dr["NAMETH"].ToString();
                        }
                        else
                        {
                            row["Article_Name"] = "";
                        }
                    }
                }

                ds.Tables.Add(dt);

                serverName.Connection.Close();
            }
            return ConvertDataSetToJSON(ds); // Retrun DataSet โดยแปลงข้อมูลทั้งหมดเป้น JSON
        }

        // / Get local Bin Code 
        [WebMethod]
        public string GetBinCode_From_TBTrans_Check_Position_Product_Contaminated(string BranchCode, string Site, string Sloc, string BinCode, string EmpCode)
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
            command.CommandText = "select DOCNO, BRANCH, SITE, SLOC, BINCODE, EMPLOYEECODE from DBTRANS..TBTrans_Check_Position_Product_Contaminated"
                                + " where DOCDATE = '" + sDate + "'"
                                + " and BRANCH = '" + BranchCode + "'"
                                + " and site = '" + Site + "'"
                                + " and sloc = '" + Sloc + "'"
                                + " and bincode like '%" + BinCode + "%' "
                                + " and CARETAKERCODE = '" + EmpCode + "'"
                                + " Group by DOCNO, BRANCH, SITE, SLOC, BINCODE, EMPLOYEECODE";
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(command);
            da.Fill(dt);
            dt.TableName = "TB_Contaminated";
            ds.Tables.Add(dt);
            return ConvertDataSetToJSON(ds);
        }
        // Get local Bin Code And Product Detail
        [WebMethod]
        public string GetBinCode_Detail_From_TBTrans_Check_Position_Product_Contaminated(string DOCNO, string BinCode,string site)
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
            command.CommandText = "select DOCNO,ROWORDER, DATECREATE, BRANCH, SITE, SLOC, BINCODE, EMPLOYEECODE, PRODUCTCODE, BARCODE, PRODUCTNAME, UNITCODE, UNITNAME, STATUS from DBTRANS..TBTrans_Check_Position_Product_Contaminated"
                                + " where DOCDATE = '" + sDate + "'"
                                + " and DOCNO = '" + DOCNO + "'"
                                + " and bincode = '" + BinCode + "'"
                                + " order by ROWORDER";
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(command);
            da.Fill(dt);
            dt.TableName = "TB_Contaminated";
            ds.Tables.Add(dt);


            return ConvertDataSetToJSON(ds);
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

        public DataTable JsonToTable(String JsonString)
        {
            DataTable dt = (DataTable)JsonConvert.DeserializeObject(JsonString, typeof(DataTable));
            return dt;
        }

        public string RunningNewDocNo(string SITE)
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
            //string sShortName = Leftsite + "LA";
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
            dbutil.RunningTableName = "dbtrans.dbo.TBTrans_Gen_Docno_Check_Position_Product_Contaminated";
            sResult = dbutil.RunningNewDocNo(devdb);
            string sql_update = "INSERT INTO dbtrans.dbo.TBTrans_Gen_Docno_Check_Position_Product_Contaminated (DOCNO) VALUES ('" + sResult + "')";
            devdb.ExecuteNoneQuery(sql_update, ref Conn1);
            return sResult;
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
    }
}
