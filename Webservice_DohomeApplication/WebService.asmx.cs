using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using dev.lib;
using System.Data;
using SourceCode.SmartObjects.Client;
using Newtonsoft.Json;
using MoreLinq;
using System.Data.SqlClient;
using System.ComponentModel;
using Newtonsoft.Json.Linq;
using System.Globalization;

namespace Webservice_DohomeApplication
{
    /// <summary>
    /// Summary description for WebService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    
    public class WebService : System.Web.Services.WebService
    {
        public string _PRODUCTTYPE;
        public SqlConnection conn;
        public SqlCommand command;
        public string Connect = System.Configuration.ConfigurationManager.ConnectionStrings["DBMASTERConnectionString"].ConnectionString;
        DBMASTERDataContext dBMASTER = new DBMASTERDataContext();
        Class_Configserver configserver = new Class_Configserver();


        #region //--------------------------------------------------------//เช็คสต๊อกสินค้า ATP------------------------------------------------------------------------------
        /// /// <summary>
        /// 
        /// </summary>
        /// <param name="werks">site</param>
        /// <param name="matnr">รหัสสินค้า</param>
        /// <param name="msg">return message</param>
        /// <returns>json format</returns>
        /// 
        [WebMethod]
        public string ZDD_EXPORT_POS_SALEINFO(string werks, string matnr,out string return_msg)
        {
            string ret = "false";
            string sProductCode;
            string sSite;
            //try
            //{
                DataSet ds = new DataSet();
                DataTable dtSaleInfo = new DataTable();
                //set table name
                dtSaleInfo.TableName = "ZDD_EXPORT_POS_SALEINFO";
                //set column name
                dtSaleInfo.Columns.Add("OUT_PRODUCTNAME", typeof(string));
                dtSaleInfo.Columns.Add("OUT_PRODUCTCODE", typeof(string));
                dtSaleInfo.Columns.Add("OUT_STOCKCOST", typeof(string));
                dtSaleInfo.Columns.Add("OUT_SALLERS", typeof(string));
                //---------------
                dtSaleInfo.Columns.Add("OUT_LASTGRDATE", typeof(string));
                dtSaleInfo.Columns.Add("OUT_LASTGRQTY", typeof(string));
                dtSaleInfo.Columns.Add("OUT_LASTSALEDATE", typeof(string));
                dtSaleInfo.Columns.Add("OUT_LASTSALEQTY", typeof(string));
                dtSaleInfo.Columns.Add("OUT_PRODUCTTYPE", typeof(string));
                dtSaleInfo.Columns.Add("OUT_SALEAMOUNT", typeof(string));
                dtSaleInfo.Columns.Add("OUT_SALEBACKOFFICE", typeof(string));
                dtSaleInfo.Columns.Add("OUT_SALEPOS", typeof(string));
                dtSaleInfo.Columns.Add("OUT_SALEQTY", typeof(string));
                dtSaleInfo.Columns.Add("OUT_SALERATE", typeof(string));
                dtSaleInfo.Columns.Add("OUT_SALE_PER_DAY", typeof(string));

                Utilities util = new Utilities();
                //convert matnr to 18 degit   
                sSite = werks;
                sProductCode = matnr;
                matnr = util.AddZero(matnr, 18);
                #region Connect K2 
                SmartObjectClientServer serverName = K2_CONNECT_SERVER();// Connect K2 Server 
                SmartObject sm = serverName.GetSmartObject("ZDD_EXPORT_POS_SALEINFO");//ชื่อ SmartOject//bapi
                sm.MethodToExecute = "ZDD_EXPORT_POSSALEINFO_ZDD_EXPORT_POS_SALEINFO";//ชื่อ Method ที่ใช้สำหรับ Excute

                //ข้อมูลที่ส่งเข้าไปใน BAPI เพื่อใช้ สำหรับ Excute
                sm.Methods["ZDD_EXPORT_POSSALEINFO_ZDD_EXPORT_POS_SALEINFO"].Parameters["p_IN_MATNR"].Value = matnr;
                sm.Methods["ZDD_EXPORT_POSSALEINFO_ZDD_EXPORT_POS_SALEINFO"].Parameters["p_IN_WERKS"].Value = werks;

                //execute data
                SmartObject smSaleInfoOut = serverName.ExecuteScalar(sm);
                //create empty row
                DataRow rowSaleInfo = dtSaleInfo.NewRow();
                //add new val
                if (smSaleInfoOut != null)
                {

                    rowSaleInfo["OUT_PRODUCTNAME"] = getProductName(sProductCode);
                    rowSaleInfo["OUT_PRODUCTCODE"] =  sProductCode;
                    rowSaleInfo["OUT_SALLERS"] =   getSallers(sProductCode,out _PRODUCTTYPE);
                    rowSaleInfo["OUT_STOCKCOST"] =  getStockCost(sProductCode,sSite);
                    rowSaleInfo["OUT_LASTGRDATE"] = smSaleInfoOut.Properties["OUT_LASTGRDATE"].Value.ToString();
                    rowSaleInfo["OUT_LASTGRQTY"] = smSaleInfoOut.Properties["OUT_LASTGRQTY"].Value.ToString();
                    rowSaleInfo["OUT_LASTSALEDATE"] = smSaleInfoOut.Properties["OUT_LASTSALEDATE"].Value.ToString();
                    rowSaleInfo["OUT_LASTSALEQTY"] = smSaleInfoOut.Properties["OUT_LASTSALEQTY"].Value.ToString();
                    rowSaleInfo["OUT_PRODUCTTYPE"] = _PRODUCTTYPE; // smSaleInfoOut.Properties["OUT_PRODUCTTYPE"].Value.ToString();
                    rowSaleInfo["OUT_SALEAMOUNT"] = smSaleInfoOut.Properties["OUT_SALEAMOUNT"].Value.ToString();
                    rowSaleInfo["OUT_SALEBACKOFFICE"] = smSaleInfoOut.Properties["OUT_SALEBACKOFFICE"].Value.ToString();
                    rowSaleInfo["OUT_SALEPOS"] = smSaleInfoOut.Properties["OUT_SALEPOS"].Value.ToString();
                    rowSaleInfo["OUT_SALEQTY"] = smSaleInfoOut.Properties["OUT_SALEQTY"].Value.ToString();
                    rowSaleInfo["OUT_SALERATE"] = smSaleInfoOut.Properties["OUT_SALERATE"].Value.ToString();
                    rowSaleInfo["OUT_SALE_PER_DAY"] = smSaleInfoOut.Properties["OUT_SALE_PER_DAY"].Value.ToString();
                    dtSaleInfo.Rows.Add(rowSaleInfo);
                }
                ds.Tables.Add(dtSaleInfo);
                #endregion Connect K2
                // Json return
                ret = ConvertDataSetToJSON(ds);
                //return success
                return_msg = "success";
                return ret;
            //}
            //catch (Exception ex)
            //{
            //    return_msg = ex.ToString();

            //    return ret;
            //}
        }

        private string getStockCost(string sProductCode, string sSite)
        {

            Utilities util = new Utilities();
            //convert matnr to 18 degit   
            sProductCode = util.AddZero(sProductCode, 18);
            SmartObjectClientServer serverName = K2_CONNECT_SERVER();// Connect K2 Server 
            SmartObject sm = serverName.GetSmartObject("ZDD_EXPORT_PO_NOT_REC");//ชื่อ SmartOject//bapi
            sm.MethodToExecute = "ZDD_EXPORTPO_NOT_REC_ZDD_EXPORT_PO_NOT_REC";//ชื่อ Method ที่ใช้สำหรับ Excute

            //ข้อมูลที่ส่งเข้าไปใน BAPI เพื่อใช้ สำหรับ Excute
            sm.Methods["ZDD_EXPORTPO_NOT_REC_ZDD_EXPORT_PO_NOT_REC"].Parameters["p_IN_MATNR"].Value = sProductCode;
            sm.Methods["ZDD_EXPORTPO_NOT_REC_ZDD_EXPORT_PO_NOT_REC"].Parameters["p_IN_WERKS"].Value = sSite;

            //execute data
            SmartObject smZDD_EXPORT_PO_NOT_REC = serverName.ExecuteScalar(sm);
            if (smZDD_EXPORT_PO_NOT_REC != null)
            {
                return smZDD_EXPORT_PO_NOT_REC.Properties["OUT_STOCKCOST"].Value.ToString();
            }
            else {
                return "not found data ZDD_EXPORT_PO_NOT_REC " + sProductCode + "-" + sSite;
            }
        }
        
        private object getSallers(string sProductCode ,out string OUT_PRODUCTTYPE)
        {
            string sSallers="";
            Utilities util = new Utilities();
            //convert matnr to 18 degit   
            sProductCode = util.AddZero(sProductCode, 18);
            SmartObjectClientServer serverName = K2_CONNECT_SERVER();// Connect K2 Server 
            SmartObject sm = serverName.GetSmartObject("ZDD_EXPORT_TABLE_ZMATSELLER");//ชื่อ SmartOject//bapi
            sm.MethodToExecute = "ZDD_EXPORTTABLE_ZMATSELLER_ZDD_EXPORT_TABLE_ZMATSELLER";//ชื่อ Method ที่ใช้สำหรับ Excute

            //ข้อมูลที่ส่งเข้าไปใน BAPI เพื่อใช้ สำหรับ Excute
            sm.ListMethods["ZDD_EXPORTTABLE_ZMATSELLER_ZDD_EXPORT_TABLE_ZMATSELLER"].Parameters["p_IN_MATNR"].Value = sProductCode; 

            //execute data
            DataTable dtZMATSELLER = serverName.ExecuteListDataTable(sm);

            if (dtZMATSELLER != null && dtZMATSELLER.Rows.Count>0)
            {
                OUT_PRODUCTTYPE = dtZMATSELLER.Rows[0]["IN_ZMAT_SELLER_MATKL"].ToString() + "-" + dtZMATSELLER.Rows[0]["IN_ZMAT_SELLER_WGBEZ"].ToString();

                 return   sSallers += (dtZMATSELLER.Rows[0]["IN_ZMAT_SELLER_LOGGR"].ToString() + "-" + dtZMATSELLER.Rows[0]["IN_ZMAT_SELLER_LTEXT"].ToString()); 

            }
            else
            {
                OUT_PRODUCTTYPE = "";
                //return "data not found ZDD_EXPORT_TABLE_ZMATSELLER " + util.RemoveZero(sProductCode);
                return sSallers;
            }
        }

        private object getProductName(string sProductCode)
        {
            SqlConnection conn = new SqlConnection(Connect);
            string sqlQuery = "select NAMETH as ProductName from tbmaster_product where code ='"+sProductCode+"'";
            if (conn.State == ConnectionState.Open) {
                conn.Close();
            }
            conn.Open();
            SqlDataAdapter da = new SqlDataAdapter(sqlQuery, conn);
            DataTable dt = new DataTable();
            da.Fill(dt);
            conn.Close();
            if (dt != null && dt.Rows.Count > 0)
                return dt.Rows[0]["ProductName"].ToString();
            else
                return "data not found master product ->" + sProductCode;

        }

        [WebMethod]
        public string LOGIN_DOHOMEAPP(string Usernam, string password,out string PolicyCode)
        {

            //string Token = "cOqyOKGB1UitBX1gdoo0SLgoPfh7jAQS";   //DD
            string Token = "jzrN9sul1kjaVOLwYpXgTo+uq/JUperG";   //Dohome Application

            WebLogIn.Token token = new WebLogIn.Token();
            token.Value = Token;
            WebLogIn.Login lg = new WebLogIn.Login();
            lg.Username = Usernam;
            lg.Password = password;

            WebLogIn.AuthenticationSoapClient aut = new WebLogIn.AuthenticationSoapClient();
            WebLogIn.ResLogin res = aut.Login(token, lg);

            if (res.Authenticated)
            {
                //เช็คว่าเป็น Cashier หรือไม่
                if (res.Policy != null)
                {
                    PolicyCode = res.Policy.PolicyCode.ToString();
                }
                else {
                    PolicyCode = null;
                }
                return res.Authenticated.ToString();
            }
            else
            {
                PolicyCode = "";
                return res.Message;
            }
        }

        [WebMethod]
        public string GET_USER_DETAIL(string EMP_ID)
        {
            DataSet ds = new DataSet();
            DBMASTERDataContext db = new DBMASTERDataContext();
            DataTable dt = new DataTable();
            dt.Columns.Add("EMP_ID", typeof(string));
            dt.Columns.Add("EMP_NAME", typeof(string));
            dt.Columns.Add("EMP_JOBKEY_CODE", typeof(string));
            dt.Columns.Add("EMP_JOBKEY_NAME", typeof(string));
            dt.Columns.Add("Server", typeof(string));
            dt.Columns.Add("Client", typeof(string));
            DataTable dt_user = (from tb_user in db.TBMaster_Users
                                 join tb_job in db.TBMaster_User_Job_Keys on tb_user.JOBKEY equals tb_job.CODE
                                 into tb_position
                                 from tb_posi in tb_position.DefaultIfEmpty()
                                 where tb_user.CODE.Equals(EMP_ID)
                                 select new { EMP_IDs = tb_user.CODE, EMP_FNAME = tb_user.MYNAME, EMP_JOBKEY_CODE = tb_posi.CODE, EMP_JOBKEY_NAME = tb_posi.MYNAME }).ToDataTable();
            DataTable user_g = (from tb_us in db.TBMaster_DHApp_Users
                                join tb_detail in db.TBMaster_DHApp_Group_Details on tb_us.Group_Code equals tb_detail.Group_Code
                                join tb_branch in db.TBMaster_Branches on tb_detail.Branch equals tb_branch.CODE
                                where tb_us.User_Code.Equals(EMP_ID)
                                group tb_detail by new { tb_detail.Branch, tb_branch.MYNAME } into d
                                select new { BRANCH_CODE = d.Key.Branch, BRANCH_NAME = d.Key.MYNAME }).ToDataTable();

            user_g.TableName = "TB_Branch";
            ds.Tables.Add(user_g);
            DataRow drow = dt.NewRow();
            drow["EMP_ID"] = dt_user.Rows[0]["EMP_IDs"].ToString();
            drow["EMP_NAME"] = dt_user.Rows[0]["EMP_FNAME"].ToString();
            drow["EMP_JOBKEY_CODE"] = dt_user.Rows[0]["EMP_JOBKEY_CODE"].ToString();
            drow["EMP_JOBKEY_NAME"] = dt_user.Rows[0]["EMP_JOBKEY_NAME"].ToString();
            string server = System.Configuration.ConfigurationManager.AppSettings["K2Server"].ToString() ?? "";
            drow["Server"] = server;
            if (server.Equals("192.168.0.159"))
            {
                drow["Client"] = "DEV";
            }
            else if (server.Equals("192.168.0.157"))
            {
                drow["Client"] = "QAS";
            }
            else
            {
                drow["Client"] = "";
            }
            dt.Rows.Add(drow);
            dt.TableName = "TB_EMP";
            ds.Tables.Add(dt);
            return ConvertDataSetToJSON(ds);
        }

        [WebMethod]
        public string GET_SITE(string Group_Code)
        {
            DataTable dt = new DataTable();
            DataTable dt_site = new DataTable();
            DataSet ds = new DataSet();
            DBMASTERDataContext db = new DBMASTERDataContext();
            dt = (from tb_u in db.TBMaster_DHApp_Group_Details
                  join tb_s_name in db.TBMaster_Sites on tb_u.Site equals tb_s_name.CODE
                  where tb_u.Group_Code == Group_Code
                  group tb_u by new { tb_u.Site, tb_s_name.MYNAME } into d

                  select new { Site = d.Key.Site, SiteName = d.Key.MYNAME }).ToDataTable();
            dt.TableName = "TB_SITE";
            ds.Tables.Add(dt);
            return ConvertDataSetToJSON(ds);

        }

        //[WebMethod]
        //public bool USER_CHECKING(string EMP_ID)
        //{
        //    DBMASTERDataContext db = new DBMASTERDataContext();
        //    List<TBMaster_DHApp_User> dt2 = (from tb_u in db.TBMaster_DHApp_Users
        //                                     where tb_u.User_Code == EMP_ID
        //                                     select tb_u).ToList();
        //    if (dt2.Count > 0)
        //    {
        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}

        [WebMethod]
        public string GET_Sloc(string Group_Code, string Site)
        {
            DataTable dt = new DataTable();
            DataTable dt_site = new DataTable();
            DataSet ds = new DataSet();
            DBMASTERDataContext db = new DBMASTERDataContext();
            dt = (from tb_u in db.TBMaster_DHApp_Group_Details
                  join tb_sl_name in db.TBMaster_Slocs on new { Sloc = tb_u.Sloc, Site = tb_u.Site } equals new { Sloc = tb_sl_name.SLOC, Site = tb_sl_name.SITECODE }
                  where tb_u.Group_Code.Equals(Group_Code) && tb_u.Site.Equals(Site)
                  select new { tb_u.Sloc, SlocName = tb_sl_name.SLOC_NAME }).ToDataTable();
            dt.TableName = "TB_Sloc";
            ds.Tables.Add(dt);
            return ConvertDataSetToJSON(ds);
        }

        [WebMethod]
        public string ZGET_ARTICLE_DH_APP_DOHOME_APPLICATION(String p_I_EAN11, String p_I_LGORT, String p_I_MATNR, String p_I_WERKS)
        {
            dev.lib.Utilities utilities = new Utilities(); //xประกาศ Object เพื่อเอาไว้สำหรับเติม 0

            DataSet dataSet = new DataSet(); //สร้าง DataSet เอาไว้เก็บ DataTable

            if (p_I_EAN11 != "")
            {

                #region Connect K2 
                SmartObjectClientServer serverName = K2_CONNECT_SERVER();// Connect K2 Server 
                SmartObject smartObject0 = serverName.GetSmartObject("ZGET_ARTICLE_DH_APP_ARTICLE");//ชื่อ SmartOject
                smartObject0.MethodToExecute = "ZGET_ARTICLE_DH_APP_E_ARTICLE_ZGET_ARTICLE_DH_APP";//ชื่อ Method ที่ใช้สำหรับ Excute
                #endregion Connect K2

                //ข้อมูลที่ส่งเข้าไปใน BAPI เพื่อใช้ สำหรับ Excute
                smartObject0.ListMethods["ZGET_ARTICLE_DH_APP_E_ARTICLE_ZGET_ARTICLE_DH_APP"].InputProperties["p_I_EAN11"].Value = p_I_EAN11;
                smartObject0.ListMethods["ZGET_ARTICLE_DH_APP_E_ARTICLE_ZGET_ARTICLE_DH_APP"].InputProperties["p_I_LANGU"].Value = "EN";
                smartObject0.ListMethods["ZGET_ARTICLE_DH_APP_E_ARTICLE_ZGET_ARTICLE_DH_APP"].InputProperties["p_I_LGORT"].Value = p_I_LGORT;
                //smartObject.ListMethods["ZGET_ARTICLE_DH_APP_E_ARTICLE_ZGET_ARTICLE_DH_APP"].InputProperties["p_I_MATNR"].Value = p_I_MATNR;
                smartObject0.ListMethods["ZGET_ARTICLE_DH_APP_E_ARTICLE_ZGET_ARTICLE_DH_APP"].InputProperties["p_I_WERKS"].Value = p_I_WERKS;

                DataTable dt0 = serverName.ExecuteListDataTable(smartObject0); // สร้าง DataTable มารับ ข้อมูลที่ Execute SmartObject
                //dt0.Rows.Remove(dt0.Rows[dt0.Rows.Count - 1]);//ตัด Rows ที่ซ้ำออก
                dataSet.Tables.Add(dt0);// add Datatable ใส่ DataSet

                #region Connect K2               
                SmartObject smartObject1 = serverName.GetSmartObject("ZGET_ARTICLE_DH_APP_BIN");//ชื่อ SmartOject
                smartObject1.MethodToExecute = "ZGET_ARTICLE_DH_APP_E_BIN_ZGET_ARTICLE_DH_APP";//ชื่อ Method ที่ใช้สำหรับ Excute
                #endregion Connect K2

                //ข้อมูลที่ส่งเข้าไปใน BAPI เพื่อใช้ สำหรับ Excute
                smartObject1.ListMethods["ZGET_ARTICLE_DH_APP_E_BIN_ZGET_ARTICLE_DH_APP"].InputProperties["p_I_EAN11"].Value = p_I_EAN11;
                smartObject1.ListMethods["ZGET_ARTICLE_DH_APP_E_BIN_ZGET_ARTICLE_DH_APP"].InputProperties["p_I_LANGU"].Value = "EN";
                smartObject1.ListMethods["ZGET_ARTICLE_DH_APP_E_BIN_ZGET_ARTICLE_DH_APP"].InputProperties["p_I_LGORT"].Value = p_I_LGORT;
                //smartObject.ListMethods["ZGET_ARTICLE_DH_APP_E_BIN_ZGET_ARTICLE_DH_APP"].InputProperties["p_I_MATNR"].Value = p_I_MATNR;
                smartObject1.ListMethods["ZGET_ARTICLE_DH_APP_E_BIN_ZGET_ARTICLE_DH_APP"].InputProperties["p_I_WERKS"].Value = p_I_WERKS;

                DataTable dt1 = serverName.ExecuteListDataTable(smartObject1); // สร้าง DataTable มารับ ข้อมูลที่ Execute SmartObject
                //dt1.Rows.Remove(dt1.Rows[dt1.Rows.Count - 1]);//ตัด Rows ที่ซ้ำออก
                dataSet.Tables.Add(dt1);// add Datatable ใส่ DataSet

                #region Connect K2  
                //สำหรับ Connect K2 
                SmartObject smartObject2 = serverName.GetSmartObject("ZGET_ARTICLE_DH_APP_UNIT");//ชื่อ SmartOject
                smartObject2.MethodToExecute = "ZGET_ARTICLE_DH_APP_E_UNIT_ZGET_ARTICLE_DH_APP";//ชื่อ Method ที่ใช้สำหรับ Excute
                #endregion Connect K2

                //ข้อมูลที่ส่งเข้าไปใน BAPI เพื่อใช้ สำหรับ Excute             
                smartObject2.ListMethods["ZGET_ARTICLE_DH_APP_E_UNIT_ZGET_ARTICLE_DH_APP"].InputProperties["p_I_EAN11"].Value = p_I_EAN11;
                smartObject2.ListMethods["ZGET_ARTICLE_DH_APP_E_UNIT_ZGET_ARTICLE_DH_APP"].InputProperties["p_I_LANGU"].Value = "EN";
                smartObject2.ListMethods["ZGET_ARTICLE_DH_APP_E_UNIT_ZGET_ARTICLE_DH_APP"].InputProperties["p_I_LGORT"].Value = p_I_LGORT;
                //smartObject2.ListMethods["ZGET_ARTICLE_DH_APP_E_UNIT_ZGET_ARTICLE_DH_APP"].InputProperties["p_I_MATNR"].Value = p_I_MATNR;
                smartObject2.ListMethods["ZGET_ARTICLE_DH_APP_E_UNIT_ZGET_ARTICLE_DH_APP"].InputProperties["p_I_WERKS"].Value = p_I_WERKS;

                DataTable dt2 = serverName.ExecuteListDataTable(smartObject2); // สร้าง DataTable มารับ ข้อมูลที่ Execute SmartObject
                //dt2.Rows.Remove(dt2.Rows[dt2.Rows.Count - 1]);//ตัด Rows ที่ซ้ำออก
                dataSet.Tables.Add(dt2);// add Datatable ใส่ DataSet

                #region Connect K2  
                //สำหรับ Connect K2 
                SmartObject smartObject3 = serverName.GetSmartObject("ZGET_ARTICLE_DH_APP_STOCK");//ชื่อ SmartOject
                smartObject3.MethodToExecute = "ZGET_ARTICLE_DH_APP_E_STOCK_ZGET_ARTICLE_DH_APP";//ชื่อ Method ที่ใช้สำหรับ Excute
                #endregion Connect K2

                //ข้อมูลที่ส่งเข้าไปใน BAPI เพื่อใช้ สำหรับ Excute             
                smartObject3.ListMethods["ZGET_ARTICLE_DH_APP_E_STOCK_ZGET_ARTICLE_DH_APP"].InputProperties["p_I_EAN11"].Value = p_I_EAN11;
                smartObject3.ListMethods["ZGET_ARTICLE_DH_APP_E_STOCK_ZGET_ARTICLE_DH_APP"].InputProperties["p_I_LANGU"].Value = "EN";
                smartObject3.ListMethods["ZGET_ARTICLE_DH_APP_E_STOCK_ZGET_ARTICLE_DH_APP"].InputProperties["p_I_LGORT"].Value = p_I_LGORT;
                //smartObject3.ListMethods["ZGET_ARTICLE_DH_APP_E_STOCK_ZGET_ARTICLE_DH_APP"].InputProperties["p_I_MATNR"].Value = p_I_MATNR;
                smartObject3.ListMethods["ZGET_ARTICLE_DH_APP_E_STOCK_ZGET_ARTICLE_DH_APP"].InputProperties["p_I_WERKS"].Value = p_I_WERKS;

                DataTable dt3 = serverName.ExecuteListDataTable(smartObject3); // สร้าง DataTable มารับ ข้อมูลที่ Execute SmartObject
                //dt3.Rows.Remove(dt3.Rows[dt3.Rows.Count - 1]);//ตัด Rows ที่ซ้ำออก
                dataSet.Tables.Add(dt3);// add Datatable ใส่ DataSet

                return ConvertDataSetToJSON(dataSet); // Retrun DataSet โดยแปลงข้อมูลทั้งหมดเป้น JSON
            }
            else
            {
                #region Connect K2 
                SmartObjectClientServer serverName = K2_CONNECT_SERVER();// Connect K2 Server 
                SmartObject smartObject0 = serverName.GetSmartObject("ZGET_ARTICLE_DH_APP_ARTICLE");//ชื่อ SmartOject
                smartObject0.MethodToExecute = "ZGET_ARTICLE_DH_APP_E_ARTICLE_ZGET_ARTICLE_DH_APP";//ชื่อ Method ที่ใช้สำหรับ Excute
                #endregion Connect K2

                //ข้อมูลที่ส่งเข้าไปใน BAPI เพื่อใช้ สำหรับ Excute
                //smartObject0.ListMethods["ZGET_ARTICLE_DH_APP_E_ARTICLE_ZGET_ARTICLE_DH_APP"].InputProperties["p_I_EAN11"].Value = p_I_EAN11;
                smartObject0.ListMethods["ZGET_ARTICLE_DH_APP_E_ARTICLE_ZGET_ARTICLE_DH_APP"].InputProperties["p_I_LANGU"].Value = "EN";
                smartObject0.ListMethods["ZGET_ARTICLE_DH_APP_E_ARTICLE_ZGET_ARTICLE_DH_APP"].InputProperties["p_I_LGORT"].Value = p_I_LGORT;
                smartObject0.ListMethods["ZGET_ARTICLE_DH_APP_E_ARTICLE_ZGET_ARTICLE_DH_APP"].InputProperties["p_I_MATNR"].Value = utilities.AddZero(p_I_MATNR, 18);
                smartObject0.ListMethods["ZGET_ARTICLE_DH_APP_E_ARTICLE_ZGET_ARTICLE_DH_APP"].InputProperties["p_I_WERKS"].Value = p_I_WERKS;

                DataTable dt0 = serverName.ExecuteListDataTable(smartObject0); // สร้าง DataTable มารับ ข้อมูลที่ Execute SmartObject
                //dt0.Rows.Remove(dt0.Rows[dt0.Rows.Count - 1]);//ตัด Rows ที่ซ้ำออก
                dataSet.Tables.Add(dt0);// add Datatable ใส่ DataSet

                #region Connect K2 
                SmartObject smartObject1 = serverName.GetSmartObject("ZGET_ARTICLE_DH_APP_BIN");//ชื่อ SmartOject
                smartObject1.MethodToExecute = "ZGET_ARTICLE_DH_APP_E_BIN_ZGET_ARTICLE_DH_APP";//ชื่อ Method ที่ใช้สำหรับ Excute
                #endregion Connect K2

                //ข้อมูลที่ส่งเข้าไปใน BAPI เพื่อใช้ สำหรับ Excute
                //smartObject1.ListMethods["ZGET_ARTICLE_DHAPP_E_BIN_ZGET_ARTICLE_DH_APP"].InputProperties["p_I_EAN11"].Value = p_I_EAN11;
                smartObject1.ListMethods["ZGET_ARTICLE_DH_APP_E_BIN_ZGET_ARTICLE_DH_APP"].InputProperties["p_I_LANGU"].Value = "EN";
                smartObject1.ListMethods["ZGET_ARTICLE_DH_APP_E_BIN_ZGET_ARTICLE_DH_APP"].InputProperties["p_I_LGORT"].Value = p_I_LGORT;
                smartObject1.ListMethods["ZGET_ARTICLE_DH_APP_E_BIN_ZGET_ARTICLE_DH_APP"].InputProperties["p_I_MATNR"].Value = utilities.AddZero(p_I_MATNR, 18);
                smartObject1.ListMethods["ZGET_ARTICLE_DH_APP_E_BIN_ZGET_ARTICLE_DH_APP"].InputProperties["p_I_WERKS"].Value = p_I_WERKS;

                DataTable dt = serverName.ExecuteListDataTable(smartObject1); // สร้าง DataTable มารับ ข้อมูลที่ Execute SmartObject
                //dt.Rows.Remove(dt.Rows[dt.Rows.Count - 1]);//ตัด Rows ที่ซ้ำออก
                dataSet.Tables.Add(dt);// add Datatable ใส่ DataSet

                #region Connect K2  
                //สำหรับ Connect K2 
                SmartObject smartObject2 = serverName.GetSmartObject("ZGET_ARTICLE_DH_APP_UNIT");//ชื่อ SmartOject
                smartObject2.MethodToExecute = "ZGET_ARTICLE_DH_APP_E_UNIT_ZGET_ARTICLE_DH_APP";//ชื่อ Method ที่ใช้สำหรับ Excute
                #endregion Connect K2

                //ข้อมูลที่ส่งเข้าไปใน BAPI เพื่อใช้ สำหรับ Excute           
                //smartObject2.ListMethods["ZGET_ARTICLE_DH_APP_E_UNIT_ZGET_ARTICLE_DH_APP"].InputProperties["p_I_EAN11"].Value = p_I_EAN11;
                smartObject2.ListMethods["ZGET_ARTICLE_DH_APP_E_UNIT_ZGET_ARTICLE_DH_APP"].InputProperties["p_I_LANGU"].Value = "EN";
                smartObject2.ListMethods["ZGET_ARTICLE_DH_APP_E_UNIT_ZGET_ARTICLE_DH_APP"].InputProperties["p_I_LGORT"].Value = p_I_LGORT;
                smartObject2.ListMethods["ZGET_ARTICLE_DH_APP_E_UNIT_ZGET_ARTICLE_DH_APP"].InputProperties["p_I_MATNR"].Value = utilities.AddZero(p_I_MATNR, 18);
                smartObject2.ListMethods["ZGET_ARTICLE_DH_APP_E_UNIT_ZGET_ARTICLE_DH_APP"].InputProperties["p_I_WERKS"].Value = p_I_WERKS;

                DataTable dt2 = serverName.ExecuteListDataTable(smartObject2); // สร้าง DataTable มารับ ข้อมูลที่ Execute SmartObject
                //dt2.Rows.Remove(dt2.Rows[dt2.Rows.Count - 1]);//ตัด Rows ที่ซ้ำออก
                dataSet.Tables.Add(dt2);// add Datatable ใส่ DataSet

                #region Connect K2  
                //สำหรับ Connect K2 
                SmartObject smartObject3 = serverName.GetSmartObject("ZGET_ARTICLE_DH_APP_STOCK");//ชื่อ SmartOject
                smartObject3.MethodToExecute = "ZGET_ARTICLE_DH_APP_E_STOCK_ZGET_ARTICLE_DH_APP";//ชื่อ Method ที่ใช้สำหรับ Excute
                #endregion Connect K2

                //ข้อมูลที่ส่งเข้าไปใน BAPI เพื่อใช้ สำหรับ Excute           
                //smartObject2.ListMethods["ZGET_ARTICLE_DH_APP_E_STOCK_ZGET_ARTICLE_DH_APP"].InputProperties["p_I_EAN11"].Value = p_I_EAN11;
                smartObject3.ListMethods["ZGET_ARTICLE_DH_APP_E_STOCK_ZGET_ARTICLE_DH_APP"].InputProperties["p_I_LANGU"].Value = "EN";
                smartObject3.ListMethods["ZGET_ARTICLE_DH_APP_E_STOCK_ZGET_ARTICLE_DH_APP"].InputProperties["p_I_LGORT"].Value = p_I_LGORT;
                smartObject3.ListMethods["ZGET_ARTICLE_DH_APP_E_STOCK_ZGET_ARTICLE_DH_APP"].InputProperties["p_I_MATNR"].Value = utilities.AddZero(p_I_MATNR, 18);
                smartObject3.ListMethods["ZGET_ARTICLE_DH_APP_E_STOCK_ZGET_ARTICLE_DH_APP"].InputProperties["p_I_WERKS"].Value = p_I_WERKS;

                DataTable dt3 = serverName.ExecuteListDataTable(smartObject3); // สร้าง DataTable มารับ ข้อมูลที่ Execute SmartObject
                //dt3.Rows.Remove(dt3.Rows[dt3.Rows.Count - 1]);//ตัด Rows ที่ซ้ำออก
                dataSet.Tables.Add(dt3);// add Datatable ใส่ DataSet

                return ConvertDataSetToJSON(dataSet); // Retrun DataSet โดยแปลงข้อมูลทั้งหมดเป้น JSON
            }
        }
        #endregion
        #region --------------------------------------------------------//ตรวจสอบป้ายราคา------------------------------------------------------------------------------

        [WebMethod]
        public string GETTBMaster_Sale_Price(string PRODUCTCODE, string BARCODE, string SITECODE, string UNITCODE)
        {
            DataSet dataSet = new DataSet();
            if (!PRODUCTCODE.Equals("") && PRODUCTCODE != null && BARCODE.Equals("") || BARCODE == null)//Article
            {
                SqlConnection conn = new SqlConnection();
                conn.ConnectionString = Connect;
                SqlCommand command = new SqlCommand();
                command.Connection = conn;
                command.CommandText = "SELECT top 1 DT.MATNR as CODE ,br.BARCODE ,dt.MATNR_TEXT as NAMETH,dt.PROMO_PRICE as SALEPRICE ,dt.UNIT_CODE as UNITCODE , dt.LAST_UPDATE_TIME as CREATEDATE "
                                    + " FROM [DBMASTER].[dbo].[TBMaster_Promotion_Price_HD] HD"
                                    + " INNER JOIN [DBMASTER].[dbo].TBMaster_Promotion_Price_DT DT ON HD.Docno = DT.docno"
                                    + " left join [DBMASTER].[dbo].TBMaster_Barcode br ON DT.MATNR = br.PRODUCTCODE and br.UNITCODE = '" + UNITCODE + "' "
                                    + " WHERE HD.STATUS = 'APPROVE' AND DT.SITECODE = '" + SITECODE + "' AND DT.MATNR = '" + PRODUCTCODE + "' AND DT.UNIT_CODE = '" + UNITCODE + "' "
                                    + " and GETDATE() between HD.VALID_FROM and HD.VALID_TO "
                                    + " order by dt.LAST_UPDATE_TIME DESC";

                DataTable data = new DataTable();
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                adapter.Fill(data);               
                if (data.Rows.Count > 0) {
                    data.TableName = "TBMaster_Sale_Price";
                    dataSet.Tables.Add(data);
                }

                if (data.Rows.Count == 0)
                {
                    SqlCommand command1 = new SqlCommand();
                    command1.Connection = conn;
                    command1.CommandText = "select top 1 pd.CODE, bc.BARCODE, pd.NAMETH, sp.SALEPRICE, sp.UNITCODE ,sp.CREATEDATE "
                                        + " from DBMASTER..TBMaster_Sale_Price sp"
                                        + " left join DBMASTER..TBMaster_Product pd  on sp.PRODUCTCODE = pd.CODE"
                                        + " left join DBMASTER..TBMaster_Barcode bc on sp.PRODUCTCODE = bc.PRODUCTCODE and sp.UNITCODE = bc.UNITCODE"
                                        + " where sp.PRODUCTCODE = '" + PRODUCTCODE + "' and sp.SITECODE = '" + SITECODE + "' and sp.UNITCODE = '" + UNITCODE + "' and GETDATE() between sp.BEGINDATE and sp.ENDDATE"
                                        + " order by sp.CREATEDATE DESC";

                    DataTable data1 = new DataTable();
                    SqlDataAdapter adapter1 = new SqlDataAdapter(command1);
                    adapter1.Fill(data1);
                    data1.TableName = "TBMaster_Sale_Price";
                    dataSet.Tables.Add(data1);
                }
            }
            if (!BARCODE.Equals("") && BARCODE != null && PRODUCTCODE.Equals("") || PRODUCTCODE == null)//Barcode
            {
                SqlConnection conn = new SqlConnection();
                conn.ConnectionString = Connect;
                string getArticlebybercode = BarcodeGetArticle(BARCODE);
                SqlCommand command = new SqlCommand();
                command.Connection = conn;
                command.CommandText = "SELECT top 1 DT.MATNR as CODE ,br.BARCODE ,dt.MATNR_TEXT as NAMETH,dt.PROMO_PRICE as SALEPRICE ,dt.UNIT_CODE as UNITCODE , dt.LAST_UPDATE_TIME as CREATEDATE "
                                    + " FROM [DBMASTER].[dbo].[TBMaster_Promotion_Price_HD] HD"
                                    + " INNER JOIN [DBMASTER].[dbo].TBMaster_Promotion_Price_DT DT ON HD.Docno = DT.docno"
                                    + " left join [DBMASTER].[dbo].TBMaster_Barcode br ON DT.MATNR = br.PRODUCTCODE and br.UNITCODE = '" + UNITCODE + "' "
                                    + " WHERE HD.STATUS = 'APPROVE' AND DT.SITECODE = '" + SITECODE + "' AND DT.MATNR = '" + getArticlebybercode + "' AND DT.UNIT_CODE = '" + UNITCODE + "'"
                                    + " and GETDATE() between HD.VALID_FROM and HD.VALID_TO "
                                    + " order by dt.LAST_UPDATE_TIME DESC";

                DataTable data = new DataTable();
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                adapter.Fill(data);
                if (data.Rows.Count > 0)
                {
                    data.TableName = "TBMaster_Sale_Price";
                    dataSet.Tables.Add(data);
                }

                if (data.Rows.Count == 0)
                {
                    SqlCommand command1 = new SqlCommand();
                    command1.Connection = conn;
                    command1.CommandText = "select top 1 pd.CODE, bc.BARCODE, pd.NAMETH, sp.SALEPRICE,sp.UNITCODE ,sp.CREATEDATE"
                                        + " from DBMASTER..TBMaster_Sale_Price sp"
                                        + " left join DBMASTER..TBMaster_Product pd on sp.PRODUCTCODE = pd.CODE"
                                        + " left join DBMASTER..TBMaster_Barcode bc on sp.PRODUCTCODE = bc.PRODUCTCODE  and sp.UNITCODE = bc.UNITCODE"
                                        + " where bc.BARCODE = '" + BARCODE + "' and sp.SITECODE = '" + SITECODE + "' and GETDATE() between sp.BEGINDATE and sp.ENDDATE"
                                        + " order by sp.CREATEDATE DESC";

                    DataTable data1 = new DataTable();
                    SqlDataAdapter adapter1 = new SqlDataAdapter(command1);
                    adapter1.Fill(data1);
                    data1.TableName = "TBMaster_Sale_Price";
                    dataSet.Tables.Add(data1);
                }

            }

            return ConvertDataSetToJSON(dataSet);
        }

        [WebMethod]
        public string GETUNIT_BY_ARTICLE(string PRODUCTCODE)
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = Connect;
            SqlCommand command = new SqlCommand();
            command.Connection = conn;
            command.CommandText = "SELECT PRODUCTCODE ,UNITCODE,b.MYNAME  FROM DBMASTER..TBMaster_Product_Unit a left join DBMASTER..TBMaster_Unit b on a.UNITCODE = b.CODE where a.PRODUCTCODE = '" + PRODUCTCODE + "'";

            DataSet dataSet = new DataSet();
            DataTable data = new DataTable();
            SqlDataAdapter adapter = new SqlDataAdapter(command);
            adapter.Fill(data);
            data.TableName = "TBMaster_Unit";
            dataSet.Tables.Add(data);

            return ConvertDataSetToJSON(dataSet);
        }

        [WebMethod]
        public string GETUNIT_BY_BARCODE(string BARCODE)
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = Connect;
            SqlCommand command = new SqlCommand();
            command.Connection = conn;
            command.CommandText = "SELECT PRODUCTCODE,BARCODE ,UNITCODE,b.MYNAME  FROM DBMASTER..TBMaster_Barcode a "
                                + " left join DBMASTER..TBMaster_Unit b on a.UNITCODE = b.CODE"
                                + " where a.STATUS = 1 and BARCODE = '" + BARCODE + "'";

            DataSet dataSet = new DataSet();
            DataTable data = new DataTable();
            SqlDataAdapter adapter = new SqlDataAdapter(command);
            adapter.Fill(data);
            data.TableName = "GETUNIT_BY_BARCODE";
            dataSet.Tables.Add(data);

            return ConvertDataSetToJSON(dataSet);
        }

        [WebMethod]
        public string INSERTJSONSTRING(string Jsonstring )
        {
            DataTable data = new DataTable();
            try
            {
                
                DataTable dt = new DataTable();
                dt.TableName = "TBTrans_Check_Price_Tag";
                dt = JsonToTable(Jsonstring);

                string stringcon = configserver.StringConn(dt.Rows[0]["SITE"].ToString());

                string docno = RunningNewDocNo(dt.Rows[0]["SITE"].ToString());
                string Leftdocno = LeftString(docno, 2);
                //string sDate = DateTime.Now.ToString("yyyy/MM/dd", new CultureInfo("en-US"));

                SqlConnection conn = new SqlConnection();
                //conn.ConnectionString = Connect;
                conn.ConnectionString = stringcon;
                SqlCommand command = new SqlCommand();
                command.Connection = conn;
                command.CommandText = "SELECT TOP 1 DOCNO  FROM DBTRANS..TBTrans_Gen_Docno_Check_Price_Tag where DOCNO like '%" + Leftdocno + "%' order by DOCNO desc";


                SqlDataAdapter adapter = new SqlDataAdapter(command);
                adapter.Fill(data);
                data.TableName = "TBTrans_Gen_Docno";

                if (dt.Rows.Count > 0 && data.Rows.Count > 0)
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


                                String query = "INSERT INTO DBTRANS..TBTrans_Check_Price_Tag (DOCNO,BRANCH,SITE,SLOC,DATEEDIT,PRODUCTCODE,BARCODE,PRODUCTNAME,UNITCODE,UNITNAME,"
                                             + "PRICETAG,PRICESYSTEM,STATUSTAG,EMPLOYEECODE,EMPLOYEENAME,CHECKTAGCODE,CHECKTAGNAME) "
                                             + " VALUES (@DOCNO,@BRANCH,@SITE,@SLOC,@DATEEDIT,@PRODUCTCODE,@BARCODE,@PRODUCTNAME"
                                             + " ,@UNITCODE,@UNITNAME,@PRICETAG,@PRICESYSTEM,@STATUSTAG,@EMPLOYEECODE,@EMPLOYEENAME,@CHECKTAGCODE,@CHECKTAGNAME)";

                                command = new SqlCommand(query, conn, tran);

                                //command.Parameters.Add("@DOCNO", data.Rows[0]["DOCNO"].ToString());
                                command.Parameters.Add("@DOCNO", docno);
                                command.Parameters.Add("@BRANCH", dt.Rows[i]["BRANCH"].ToString());
                                command.Parameters.Add("@SITE", dt.Rows[i]["SITE"].ToString());
                                command.Parameters.Add("@SLOC", dt.Rows[i]["SLOC"].ToString());

                                string iDate = dt.Rows[i]["DATEEDIT"].ToString();
                                //DateTime oDate = DateTime.Parse(iDate);
                                //string ddd = oDate.Year.ToString();

                                //string sDate = oDate.ToString("yyyy-MM-dd", new CultureInfo("en-US"));

                                command.Parameters.Add("@DATEEDIT", iDate);
                                //command.Parameters.Add("@DATEEDIT", "2019-1-28");
                                //command.Parameters.Add("@DATECREATE", DateTime.Now);
                                //command.Parameters.Add("@DATECREATE", sDate);
                                //command.Parameters.Add("@DATECREATE", "2561-10-03 12:12:12");
                                command.Parameters.Add("@PRODUCTCODE", dt.Rows[i]["PRODUCTCODE"].ToString());
                                command.Parameters.Add("@BARCODE", dt.Rows[i]["BARCODE"].ToString());
                                command.Parameters.Add("@PRODUCTNAME", dt.Rows[i]["PRODUCTNAME"].ToString());
                                command.Parameters.Add("@UNITCODE", dt.Rows[i]["UNITCODE"].ToString());
                                command.Parameters.Add("@UNITNAME", dt.Rows[i]["UNITNAME"].ToString());
                                command.Parameters.Add("@PRICETAG", dt.Rows[i]["PRICETAG"].ToString());
                                command.Parameters.Add("@PRICESYSTEM", dt.Rows[i]["PRICESYSTEM"].ToString());
                                command.Parameters.Add("@STATUSTAG", dt.Rows[i]["STATUSTAG"].ToString());
                                command.Parameters.Add("@EMPLOYEECODE", dt.Rows[i]["EMPLOYEECODE"].ToString());
                                command.Parameters.Add("@EMPLOYEENAME", dt.Rows[i]["EMPLOYEENAME"].ToString());
                                command.Parameters.Add("@CHECKTAGCODE", dt.Rows[i]["CHECKTAGCODE"].ToString());
                                command.Parameters.Add("@CHECKTAGNAME", dt.Rows[i]["CHECKTAGNAME"].ToString());

                                command.ExecuteNonQuery();
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
            return data.Rows[0]["DOCNO"].ToString();
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
            dbutil.ConnectionString = Connect;
            dbutil.RunningFieldName = "DOCNO";
            //string sformat = "yyyyMMdd-####";
            string sformat = "-yyMMdd####";
            //string Leftsite = LeftString(SITE, 2);
            //string sShortName = Leftsite + "LP";
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
            dbutil.RunningTableName = "dbtrans.dbo.TBTrans_Gen_Docno_Check_Price_Tag";
            sResult = dbutil.RunningNewDocNo(devdb);
            string sql_update = "INSERT INTO dbtrans.dbo.TBTrans_Gen_Docno_Check_Price_Tag (DOCNO) VALUES ('" + sResult + "')";
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


        #endregion --------------------------------------------------------//ตรวจสอบป้ายราคา------------------------------------------------------------------------------

        private SmartObjectClientServer K2_CONNECT_SERVER()
        {
            #region Connect K2 
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
            #endregion Connect K2
            return serverName;
        }

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

        public string BarcodeGetArticle(string Barcode) {
            TBMaster_Barcode barcode = new TBMaster_Barcode();
            barcode = (from br in dBMASTER.TBMaster_Barcodes
                       where br.BARCODE == Barcode
                       select br).FirstOrDefault();
            if (barcode != null)
            {
                return barcode.PRODUCTCODE.ToString();
            }
            return "";
        }
    }

}
